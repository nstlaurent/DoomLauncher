using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Equin.ApplicationFramework
{
    class ProvidedViewPropertyDescriptor : PropertyDescriptor
    {
        public ProvidedViewPropertyDescriptor(string name, Type propertyType)
            : base(name, null)
        {
            _propertyType = propertyType;
        }

        private Type _propertyType;

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof(IProvideViews); }
        }

        public override object GetValue(object component)
        {
            if (component is IProvideViews)
            {
                return (component as IProvideViews).GetProvidedView(Name);
            }

            throw new ArgumentException("Type of component is not valid.", "component");
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return _propertyType; }
        }

        public override void ResetValue(object component)
        {
            throw new NotSupportedException();
        }

        public override void SetValue(object component, object value)
        {
            throw new NotSupportedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        /// <summary>
        /// Gets if a BindingListView can be provided for given property. 
        /// The property type must implement IList&lt;&gt; i.e. some generic IList.
        /// </summary>
        public static bool CanProvideViewOf(PropertyDescriptor prop)
        {
            Type propType = prop.PropertyType;
            foreach (Type interfaceType in propType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition().Equals(typeof(IList<>)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
