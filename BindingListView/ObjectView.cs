using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Equin.ApplicationFramework
{
    /// <summary>
    /// Serves a wrapper for items being viewed in a <see cref="BindingListView&lt;T&gt;"/>.
    /// This class implements <see cref="INotifyEditableObject"/> so will raise the necessary events during 
    /// the item edit life-cycle.
    /// </summary>
    /// <remarks>
    /// If <typeparamref name="T"/> implements <see cref="System.ComponentModel.IEditableObject"/> this class will call BeginEdit/CancelEdit/EndEdit on the <typeparamref name="T"/> object as well.
    /// If <typeparamref name="T"/> implements <see cref="System.ComponentModel.IDataErrorInfo"/> this class will use that implementation as its own.
    /// </remarks>
    /// <typeparam name="T">The type of object being viewed.</typeparam>
    [Serializable]
    public class ObjectView<T> : INotifyingEditableObject, IDataErrorInfo, INotifyPropertyChanged, ICustomTypeDescriptor, IProvideViews
    {
        /// <summary>
        /// Creates a new <see cref="ObjectView&ltT&gt;"/> wrapper for a <typeparamref name="T"/> object.
        /// </summary>
        /// <param name="object">The <typeparamref name="T"/> object being wrapped.</param>
        public ObjectView(T @object, AggregateBindingListView<T> parent)
        {
            _parent = parent;

            Object = @object;
            if (Object is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)Object).PropertyChanged += new PropertyChangedEventHandler(ObjectPropertyChanged);
            }

            if (typeof(ICustomTypeDescriptor).IsAssignableFrom(typeof(T)))
            {
                _isCustomTypeDescriptor = true;
                _customTypeDescriptor = Object as ICustomTypeDescriptor;
                Debug.Assert(_customTypeDescriptor != null);
            }

            _providedViews = new Dictionary<string, object>();
            CreateProvidedViews();
        }

        /// <summary>
        /// The view containing this ObjectView.
        /// </summary>
        private AggregateBindingListView<T> _parent;
        /// <summary>
        /// Flag that signals if we are currently editing the object.
        /// </summary>
        private bool _editing;
        /// <summary>
        /// The actual object being edited.
        /// </summary>
        private T _object;
        /// <summary>
        /// Flag set to true if type of T implements ICustomTypeDescriptor
        /// </summary>
        private bool _isCustomTypeDescriptor;
        /// <summary>
        /// Holds the Object pre-casted ICustomTypeDescriptor (if supported).
        /// </summary>
        private ICustomTypeDescriptor _customTypeDescriptor;
        /// <summary>
        /// A collection of BindingListView objects, indexed by name, for views auto-provided for any generic IList members.
        /// </summary>
        private Dictionary<string, object> _providedViews;

        /// <summary>
        /// Gets the object being edited.
        /// </summary>
        public T Object
        {
            get { return _object; }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Object", Properties.Resources.ObjectCannotBeNull);
                }
                _object = value;
            }
        }

        public object GetProvidedView(string name)
        {
            return _providedViews[name];
        }

        /// <summary>
        /// Casts an ObjectView&lt;T&gt; to a T by getting the wrapped T object.
        /// </summary>
        /// <param name="eo">The ObjectView&lt;T&gt; to cast to a T</param>
        /// <returns>The object that is wrapped.</returns>
        public static explicit operator T(ObjectView<T> eo)
        {
            return eo.Object;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is T)
            {
                return Object.Equals(obj);
            }
            else if (obj is ObjectView<T>)
            {
                return Object.Equals((obj as ObjectView<T>).Object);
            }
            else
            {
                return Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return Object.GetHashCode();
        }

        public override string ToString()
        {
            return Object.ToString();
        }
        
        private void ObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Raise our own event
            OnPropertyChanged(sender, new PropertyChangedEventArgs(e.PropertyName));
        }

        private bool ShouldProvideViewOf(PropertyDescriptor listProp)
        {
            return _parent.ShouldProvideView(listProp);
        }

        private string GetProvidedViewName(PropertyDescriptor listProp)
        {
            return _parent.GetProvidedViewName(listProp);
        }

        private void CreateProvidedViews()
        {
            foreach (PropertyDescriptor prop in (this as ICustomTypeDescriptor).GetProperties())
            {
                if (ShouldProvideViewOf(prop))
                {
                    object view = _parent.CreateProvidedView(this, prop);
                    string viewName = GetProvidedViewName(prop);
                    _providedViews.Add(viewName, view);
                }
            }
        }

        #region INotifyEditableObject Members

        /// <summary>
        /// Indicates an edit has just begun.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler EditBegun;

        /// <summary>
        /// Indicates the edit was cancelled.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler EditCancelled;

        /// <summary>
        /// Indicated the edit was ended.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler EditEnded;

        protected virtual void OnEditBegun()
        {
            if (EditBegun != null)
            {
                EditBegun(this, EventArgs.Empty);
            }
        }

        protected virtual void OnEditCancelled()
        {
            if (EditCancelled != null)
            {
                EditCancelled(this, EventArgs.Empty);
            }
        }

        protected virtual void OnEditEnded()
        {
            if (EditEnded != null)
            {
                EditEnded(this, EventArgs.Empty);
            }
        }

        #endregion

        #region IEditableObject Members

        public void BeginEdit()
        {
            // As per documentation, this method may get called multiple times for a single edit.
            // So we set a flag to only honor the first call.
            if (!_editing)
            {
                _editing = true;

                // If possible call the object's BeginEdit() method
                // to let it do what ever it needs e.g. save state
                if (Object is IEditableObject)
                {
                    ((IEditableObject)Object).BeginEdit();
                }
                // Raise the EditBegun event.                
                OnEditBegun();
            }
        }

        public void CancelEdit()
        {
            // We can only cancel if currently editing
            if (_editing)
            {
                // If possible call the object's CancelEdit() method
                // to let it do what ever it needs e.g. rollback state
                if (Object is IEditableObject)
                {
                    ((IEditableObject)Object).CancelEdit();
                }
                // Raise the EditCancelled event.
                OnEditCancelled();
                // No longer editing now.
                _editing = false;
            }
        }

        public void EndEdit()
        {
            // We can only end if currently editing
            if (_editing)
            {
                // If possible call the object's EndEdit() method
                // to let it do what ever it needs e.g. commit state
                if (Object is IEditableObject)
                {
                    ((IEditableObject)Object).EndEdit();
                }
                // Raise the EditEnded event.
                OnEditEnded();
                // No longer editing now.
                _editing = false;
            }
        }

        #endregion

        #region IDataErrorInfo Members

        // If the wrapped Object support IDataErrorInfo we forward calls to it.
        // Otherwise, we just return empty strings that signal "no error".

        string IDataErrorInfo.Error
        {
            get
            {
                if (Object is IDataErrorInfo)
                {
                    return ((IDataErrorInfo)Object).Error;
                }
                return string.Empty;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if (Object is IDataErrorInfo)
                {
                    return ((IDataErrorInfo)Object)[columnName];
                }
                return string.Empty;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, args);
            }
        }

        #endregion

        #region ICustomTypeDescriptor Members

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            if (_isCustomTypeDescriptor)
            {
                return _customTypeDescriptor.GetAttributes();
            }
            else
            {
                return TypeDescriptor.GetAttributes(Object);
            }
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            if (_isCustomTypeDescriptor)
            {
                return _customTypeDescriptor.GetClassName();
            }
            else
            {
                return typeof(T).FullName;
            }
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            if (_isCustomTypeDescriptor)
            {
                return _customTypeDescriptor.GetComponentName();
            }
            else
            {
                return TypeDescriptor.GetFullComponentName(Object);
            }
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            if (_isCustomTypeDescriptor)
            {
                return _customTypeDescriptor.GetConverter();
            }
            else
            {
                return TypeDescriptor.GetConverter(Object);
            }
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            if (_isCustomTypeDescriptor)
            {
                return _customTypeDescriptor.GetDefaultEvent();
            }
            else
            {
                return TypeDescriptor.GetDefaultEvent(Object);
            }
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            if (_isCustomTypeDescriptor)
            {
                return _customTypeDescriptor.GetDefaultProperty();
            }
            else
            {
                return TypeDescriptor.GetDefaultProperty(Object);
            }
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            if (_isCustomTypeDescriptor)
            {
                return _customTypeDescriptor.GetEditor(editorBaseType);
            }
            else
            {
                return null;
            }
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            if (_isCustomTypeDescriptor)
            {
                return _customTypeDescriptor.GetEvents();
            }
            else
            {
                return TypeDescriptor.GetEvents(Object, attributes);
            }
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return (this as ICustomTypeDescriptor).GetEvents(null);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            List<PropertyDescriptor> props;
            if (_isCustomTypeDescriptor)
            {
                props = new List<PropertyDescriptor>(_parent.AddProvidedViews(_customTypeDescriptor.GetProperties(attributes)));
            }
            else
            {
                props = new List<PropertyDescriptor>(_parent.AddProvidedViews(TypeDescriptor.GetProperties(Object, attributes)));
            }
            return new PropertyDescriptorCollection(props.ToArray());
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return (this as ICustomTypeDescriptor).GetProperties(null);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            if (_isCustomTypeDescriptor)
            {
                return _customTypeDescriptor.GetPropertyOwner(pd);
            }
            else
            {
                return Object;
            }
        }

        #endregion
    }

    public interface IProvideViews
    {
        object GetProvidedView(string name);
    }

}
