using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Diagnostics;

namespace Equin.ApplicationFramework
{
    public class AggregateBindingListView<T> : Component, IBindingListView, IList, IRaiseItemChangedEvents, ICancelAddNew, ITypedList, IEnumerable<T>
    {
        #region Constructors

        public AggregateBindingListView()
        {
            _sourceLists = new BindingList<IList>();
            (_sourceLists as IBindingList).ListChanged += new ListChangedEventHandler(SourceListsChanged);
            _savedSourceLists = new List<IList>();
            _sourceIndices = new MultiSourceIndexList<T>();
            // Start with a filter that includes all items.
            _filter = IncludeAllItemFilter<T>.Instance;
            // Start with no sorts applied.
            _sorts = new ListSortDescriptionCollection();
            _objectViewCache = new Dictionary<T, ObjectView<T>>();
        }

        public AggregateBindingListView(IContainer container)
            : this()
        {
            container.Add(this);

            if (Site is ISynchronizeInvoke)
            {
                SynchronizingObject = Site as ISynchronizeInvoke;
            }
        }

        #endregion

        #region Private Member Fields

        /// <summary>
        /// The list of underlying list of items on which this view is based.
        /// </summary>
        private IList _sourceLists;
        /// <summary>
        /// The sorted, filtered list of item indices in _sourceList.
        /// </summary>
        private MultiSourceIndexList<T> _sourceIndices;
        /// <summary>
        /// The current filter applied to the view.
        /// </summary>
        private IItemFilter<T> _filter;
        /// <summary>
        /// The current sorts applied to the view.
        /// </summary>
        private ListSortDescriptionCollection _sorts;
        /// <summary>
        /// The <see cref="System.Collection.Generic.IComparer">IComparer</see> used to compare items when sorting.
        /// </summary>
        private IComparer<KeyValuePair<ListItemPair<T>, int>> _comparer;
        /// <summary>
        /// The item in the process of being added to the view.
        /// </summary>
        private ObjectView<T> _newItem;
        /// <summary>
        /// The IList we will add new items to.
        /// </summary>
        private IList _newItemsList;
        /// <summary>
        /// The object used to marshal event-handler calls that are invoked on a non-UI thread.
        /// </summary>
        private ISynchronizeInvoke _synchronizingObject;
        /// <summary>
        /// A copy of the source lists so when a list is removed from SourceLists
        /// we still have a reference to use for unhooking events, etc.
        /// </summary>
        private List<IList> _savedSourceLists;
        /// <summary>
        /// The property on a source list item that contains the actual list to view.
        /// If null or empty then the source list item is used instead.
        /// </summary>
        private string _dataMember;
        /// <summary>
        /// ObjectView cache used to prevent re-creation of existing object wrappers when 
        /// in FilterAndSort().
        /// </summary>
        private Dictionary<T, ObjectView<T>> _objectViewCache;
        /// <summary>
        /// Controls whether or not the view is automatically re-filtered and re-sorted when
        /// source lists change.
        /// </summary>
        private bool _autoFilterAndSortSuspended;

        #endregion

        /// <summary>
        /// Gets or sets the list of source lists used by this view.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList SourceLists
        {
            get
            {
                return _sourceLists;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("SourceLists", Properties.Resources.SourceListsNull);
                }

                // Check that every item in each list is of type T.
                foreach (object obj in value)
                {
                    if (obj == null)
                    {
                        throw new InvalidSourceListException();
                    }

                    IList list = null;
                    if (!string.IsNullOrEmpty(DataMember))
                    {
                        foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(obj))
                        {
                            if (pd.Name == DataMember)
                            {
                                list = pd.GetValue(obj) as IList;
                                break;
                            }
                        }
                    }
                    else if (obj is IListSource)
                    {
                        IListSource src = obj as IListSource;
                        if (src.ContainsListCollection)
                        {
                            list = src.GetList()[0] as IList;
                        }
                        else
                        {
                            list = (obj as IListSource).GetList();
                        }
                    }
                    else if (!(obj is ICollection<T>))
                    {
                        list = obj as IList;
                    }
                    else
                    {
                        // We have a typed collection, so can skip the item-by-item check.
                        continue;
                    }

                    if (list == null)
                    {
                        throw new InvalidSourceListException();
                    }

                    foreach (object item in list)
                    {
                        if (!(item is T))
                        {
                            throw new InvalidSourceListException(string.Format(Properties.Resources.InvalidListItemType, typeof(T).FullName));
                        }
                    }
                }

                IBindingList bindingList = _sourceLists as IBindingList;

                // Un-hook old list changed event.
                if (bindingList != null && bindingList.SupportsChangeNotification)
                {
                    bindingList.ListChanged -= new ListChangedEventHandler(SourceListsChanged);
                }

                foreach (object list in _sourceLists)
                {
                    IBindingList bl = list as IBindingList;
                    if (bl != null && bl.SupportsChangeNotification)
                    {
                        bl.ListChanged -= new ListChangedEventHandler(SourceListChanged);
                    }
                }

                _sourceLists = value;

                bindingList = _sourceLists as IBindingList;
                // Hook new list changed event
                if (bindingList != null && bindingList.SupportsChangeNotification)
                {
                    bindingList.ListChanged += new ListChangedEventHandler(SourceListsChanged);
                }
                foreach (object list in _sourceLists)
                {
                    IBindingList bl = list as IBindingList;
                    if (bl != null && bl.SupportsChangeNotification)
                    {
                        bl.ListChanged += new ListChangedEventHandler(SourceListChanged);
                    }
                }

                // save new lists
                BuildSavedList();

                FilterAndSort();
                OnListChanged(ListChangedType.Reset, -1);
            }
        }

        /// <summary>
        /// Gets the ObjectView&lt;T&gt; of the item at the given index in the view.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <returns>The ObjectView&lt;T&gt; of the item.</returns>
        public ObjectView<T> this[int index]
        {
            get
            {
                return _sourceIndices[index].Key.Item;
            }
        }

        [Browsable(false)]
        public string DataMember
        {
            get
            {
                return _dataMember;
            }
            set
            {
                _dataMember = value;
                FilterAndSort();
                OnListChanged(ListChangedType.Reset, -1);
            }
        }

        private bool ShouldSerializeListMember()
        {
            return !string.IsNullOrEmpty(DataMember);
        }

        #region Adding New Items

        /// <summary>
        /// Occurs before an item is added to the list.
        /// Assign the event argument's NewObject property to provide the object to add.
        /// </summary>
        public event AddingNewEventHandler AddingNew;

        /// <summary>
        /// Attempts to get a new <typeparamref name="T"/> object to add to the list, first by raising the 
        /// AddingNew event and then (if no new object was assigned) by using the 
        /// default public constructor.
        /// </summary>
        /// <returns>The new object to add to the list.</returns>
        /// <exception cref="System.InvalidOperationException">No new object provided by the AddingNew event handler and <typeparamref name="T"/> has no default public constructor.</exception>
        protected virtual T OnAddingNew()
        {
            // We allow users of this class to provide the object to add
            // by raising the AddingNew event.
            if (AddingNew != null)
            {
                AddingNewEventArgs args = new AddingNewEventArgs();
                AddingNew(this, args);
                // Check if we were given an object (and it's the correct type)
                if ((args.NewObject != null) && (args.NewObject is T))
                {
                    return (T)args.NewObject;
                }
            }
            // Otherwise, try the default public constructor instead.
            // Use reflection to find it. Note: We're not using the generic new() constraint since
            // we do not want to force the need for a public default constructor when the user
            // can simply handle the AddingNew event called above.
            System.Reflection.ConstructorInfo ci = typeof(T).GetConstructor(System.Type.EmptyTypes);
            if (ci != null)
            {
                // Invoke the constructor to create the object.
                return (T)ci.Invoke(null);
            }
            else
            {
                throw new InvalidOperationException(Properties.Resources.CannotAddNewItem);
            }
        }

        /// <summary>
        /// Adds a new item to the view. Note that EndNew must be called to commit
        /// the item to the to the source list.
        /// </summary>
        /// <returns>The new item, wrapped in an ObjectView<typeparamref name="T"/>.</returns>
        public ObjectView<T> AddNew()
        {
            // Are we currently adding another item?
            if (_newItem != null)
            {
                // Need to commit previous new item before adding another.
                EndNew(_sourceIndices.Count - 1);
            }

            // Get the new item to add.
            T item = OnAddingNew();

            // Create the ObjectView<T> wrapper for the item.
            ObjectView<T> objectView = new ObjectView<T>(item, this);

            _objectViewCache[item] = objectView;

            HookPropertyChangedEvent(objectView);

            // Set the _newItem reference so we know what to use when ending/cancelling this add operation.
            _newItem = objectView;

            // Add to indicies list, but index of -1 means it's not in the source list yet.
            _sourceIndices.Add(_newItemsList, objectView, -1);
            // Tell any data binders that we've added an item to the view.
            // Put it at the end of the list.
            OnListChanged(ListChangedType.ItemAdded, _sourceIndices.Count - 1);

            return objectView;
        }

        /// <summary>
        /// Cancels the pending addition of a new item to the source list
        /// and remove the item from the view.
        /// </summary>
        /// <param name="itemIndex">The index of the new item.</param>
        public void CancelNew(int itemIndex)
        {
            // We must take special care that the item index does refer to the new item.
            if (itemIndex > -1 && itemIndex < _sourceIndices.Count &&
                _newItem != null && _sourceIndices[itemIndex].Key.Item == _newItem)
            {
                // We no longer need to listen to any events from the object.
                UnHookPropertyChangedEvent(_newItem);
                // Remove the item from the view.
                _sourceIndices.RemoveAt(itemIndex);
                // Data binders need to know the item has gone from the view.
                OnListChanged(ListChangedType.ItemDeleted, itemIndex);
                // Done with this adding operation, so clear the _newItem reference.
                _newItem = null;
            }
        }

        /// <summary>
        /// Commits the pending addition of a new item to the source list.
        /// </summary>
        /// <param name="itemIndex">The index of the new item.</param>
        public void EndNew(int itemIndex)
        {
            // The binding infrastructure tends to call the method
            // more times than needed and often with itemIndex not even pointing to the 
            // new object! So we have to take special care to check.
            if (itemIndex > -1 && itemIndex < _sourceIndices.Count &&
                _newItem != null && _sourceIndices[itemIndex].Key.Item == _newItem)
            {
                // In order to reuse the SourceListChanged code for adding a new item
                // we have to first remove all knowledge of the item, then add it 
                // to the source list.

                // We no longer need to listen to any events from the object.
                UnHookPropertyChangedEvent(_newItem);
                // Remove the item from the view.
                _sourceIndices.RemoveAt(itemIndex);

                // Add the actual data object to the source list.
                // The SourceListChanged event handler will take care of correctly inserting this
                // object into the view (if newItemsList is a IBindingList).
                _newItemsList.Add(_newItem.Object);

                // If it is not an IBindingList (or not SupportsChangeNotification) 
                // then we must force the update ourselves.
                if (!(_newItemsList is IBindingList) || !(_newItemsList as IBindingList).SupportsChangeNotification)
                {
                    if (!_autoFilterAndSortSuspended)
                    {
                        FilterAndSort();
                    }
                    OnListChanged(ListChangedType.Reset, -1);
                }

                // Done with this adding operation, so clear the _newItem reference.
                _newItem = null;
            }
        }

        /// <summary>
        /// Gets or sets the source list to which new items are added.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList NewItemsList
        {
            get
            {
                return _newItemsList;
            }
            set
            {
                if (value != null && !_sourceLists.Contains(value))
                {
                    throw new ArgumentException(Properties.Resources.SourceListNotFound);
                }
                _newItemsList = value;
            }
        }

        #endregion

        /// <summary>
        /// Re-applies any current filter and sorts to refresh the current view.
        /// </summary>
        public void Refresh()
        {
            FilterAndSort();
            // Get any bound objects to refresh everything as well.
            OnListChanged(ListChangedType.Reset, -1);
        }

        /// <summary>
        /// Gets or sets the object used to marshal event-handler calls that are invoked on a non-UI thread.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                return _synchronizingObject;
            }
            set
            {
                _synchronizingObject = value;
            }
        }

        public void SuspendAutoFilterAndSort()
        {
            _autoFilterAndSortSuspended = true;
        }

        public void ResumeAutoFilterAndSort()
        {
            _autoFilterAndSortSuspended = false;
        }

        /// <summary>
        /// Updates the _sourceIndices list to contain the items that are current viewed
        /// according to applied filter and sorts.
        /// </summary>
        protected void FilterAndSort()
        {
            // The view contains items from the source list
            // and possibly a new items that are not yet committed.
            // Therefore we can't just clear the list and start over
            // as we would lose the new items. So we have to to insert
            // filtered source list items into a new list first.
            // New items can then be pulled out of the current view
            // and appended to the new list.
            MultiSourceIndexList<T> newList = new MultiSourceIndexList<T>();

            // Get items from the source list that are included by the current filter.
            foreach (IList sourceList in GetSourceLists())
            {
                for (int i = 0; i < sourceList.Count; i++)
                {
                    T item = (T)sourceList[i];
                    ObjectView<T> editableObject;
                    if (_filter.Include(item))
                    {
                        if (_objectViewCache.ContainsKey(item))
                        {
                            editableObject = _objectViewCache[item];
                        }
                        else
                        {
                            editableObject = new ObjectView<T>(item, this);
                            _objectViewCache.Add(item, editableObject);
                            // Listen to the editing notification and property changed events.
                            HookEditableObjectEvents(editableObject);
                            HookPropertyChangedEvent(editableObject);
                        }

                        // Add the editable object along with the index of the item in the source list.
                        newList.Add(sourceList, editableObject, i);
                    }
                    else
                    {
                        if (_objectViewCache.ContainsKey(item))
                        {
                            editableObject = _objectViewCache[item];
                            UnHookEditableObjectEvents(editableObject);
                            UnHookPropertyChangedEvent(editableObject);
                            _objectViewCache.Remove(item);
                        }
                    }
                }
            }

            // If we have sorts to apply, do them now
            if (_comparer != null)
            {
                newList.Sort(_comparer);
            }

            // Now we can append any new items to the end of the view.
            foreach (KeyValuePair<ListItemPair<T>, int> kvp in _sourceIndices)
            {
                // New items have a source list index of -1 since they are not
                // yet in the source list.
                if (kvp.Value == -1)
                {
                    newList.Add(kvp);
                }
            }

            // Set our view now
            _sourceIndices = newList;

            // Note: We do not raise the ListChanged event with ListChangeType.Reset
            // since the view may not have changed that much. It is better to let
            // the calling code decide what has happened and raise events accordingly.
        }

        #region Editing Items Event Handlers

        /// <remarks>
        /// Currently unused. Here in case we want to perform actions when
        /// an item edit begins.
        /// </remarks>
        protected virtual void BegunItemEdit(object sender, EventArgs e)
        {

        }

        /// <remarks>
        /// Currently unused. Here in case we want to perform actions when
        /// an item edit is cancelled.
        /// </remarks>
        protected virtual void CancelledItemEdit(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the <see cref="ObjectView&lt;T&gt;"/> EndedEdit event.
        /// </summary>
        /// <param name="sender">The <see cref="ObjectView&lt;T&gt;"/> that raised the event.</param>
        protected virtual void EndedItemEdit(object sender, EventArgs e)
        {
            if (_autoFilterAndSortSuspended)
            {
                return;
            }

            ObjectView<T> editableObject = (ObjectView<T>)sender;

            // Check if filtering removed the item from view
            // by getting the index before and after
            int oldIndex = _sourceIndices.IndexOfItem(editableObject.Object);
            FilterAndSort();
            int newIndex = _sourceIndices.IndexOfItem(editableObject.Object);
            // if item was filtered out then the newIndex == -1
            if (newIndex > -1)
            {
                if (oldIndex == newIndex)
                {
                    OnListChanged(ListChangedType.ItemChanged, newIndex);
                }
                else
                {
                    OnListChanged(ListChangedType.ItemMoved, newIndex, oldIndex);
                }
            }
            else
            {
                OnListChanged(ListChangedType.ItemDeleted, oldIndex);
            }
        }

        #endregion

        /// <summary>
        /// Event handler for when SourceLists is changed.
        /// </summary>
        protected virtual void SourceListsChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                IList list = SourceLists[e.NewIndex] as IList;
                if (list == null)
                {
                    SourceLists.RemoveAt(e.NewIndex);
                    throw new InvalidSourceListException();
                }

                if (list is IBindingList)
                {
                    // We need to know when the source list changes
                    (list as IBindingList).ListChanged += new ListChangedEventHandler(SourceListChanged);
                }
                _savedSourceLists.Add(list);
                if (!_autoFilterAndSortSuspended)
                {
                    FilterAndSort();
                    OnListChanged(ListChangedType.Reset, -1);
                }
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                IList list = _savedSourceLists[e.NewIndex] as IList;
                if (list != null)
                {
                    if (list is IBindingList)
                    {
                        (list as IBindingList).ListChanged -= new ListChangedEventHandler(SourceListChanged);
                    }
                    _savedSourceLists.RemoveAt(e.NewIndex);
                    if (!_autoFilterAndSortSuspended)
                    {
                        FilterAndSort();
                        OnListChanged(ListChangedType.Reset, -1);
                    }
                }
            }
            else if (e.ListChangedType == ListChangedType.Reset)
            {
                BuildSavedList();
                if (!_autoFilterAndSortSuspended)
                {
                    FilterAndSort();
                    OnListChanged(ListChangedType.Reset, -1);
                }
            }
        }

        /// <summary>
        /// Event handler for when a source list changes.
        /// </summary>
        private void SourceListChanged(object sender, ListChangedEventArgs e)
        {
            if (_autoFilterAndSortSuspended)
            {
                return;
            }

            int oldIndex;
            int newIndex;
            IBindingList sourceList = sender as IBindingList;
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    FilterAndSort();
                    // Get the index of the newly sorted item
                    newIndex = _sourceIndices.IndexOfSourceIndex(sourceList, e.NewIndex);
                    if (newIndex > -1)
                    {
                        OnListChanged(ListChangedType.ItemAdded, newIndex);
                        // Other items have moved down the list
                        for (int i = newIndex + 1; i < Count; i++)
                        {
                            OnListChanged(ListChangedType.ItemMoved, i - 1, i);
                        }
                    }
                    else
                    {
                        // The item was excluded by the filter,
                        // so to the viewer the item has been "deleted".
                        // The new item will have been added at the end of the view
                        OnListChanged(ListChangedType.ItemDeleted, Math.Max(Count - 1, 0));
                    }
                    break;

                case ListChangedType.ItemChanged:
                    // Check if filtering will remove the item from view
                    // by getting the index before and after
                    oldIndex = _sourceIndices.IndexOfSourceIndex(sourceList, e.NewIndex);

                    // Is the object in our view?
                    if (oldIndex < 0)
                    {
                        return;
                    }

                    FilterAndSort();
                    newIndex = _sourceIndices.IndexOfSourceIndex(sourceList, e.NewIndex);
                    // if item was filtered out then the newIndex == -1
                    // otherwise we can say that the item was changed.
                    if (newIndex > -1)
                    {
                        if (newIndex == oldIndex)
                        {
                            OnListChanged(ListChangedType.ItemChanged, newIndex);
                        }
                        else
                        {
                            // Two items will have changed places
                            OnListChanged(ListChangedType.ItemMoved, newIndex, oldIndex);
                        }
                    }
                    else
                    {
                        OnListChanged(ListChangedType.ItemDeleted, oldIndex);
                    }
                    break;

                case ListChangedType.ItemDeleted:
                    // Find the deleted index
                    newIndex = _sourceIndices.IndexOfSourceIndex(sourceList, e.NewIndex);

                    // Did we have the object in our view?
                    if (newIndex < 0)
                    {
                        return;
                    }

                    // Stop listening to it's events
                    UnHookEditableObjectEvents(_sourceIndices[newIndex].Key.Item);
                    UnHookPropertyChangedEvent(_sourceIndices[newIndex].Key.Item);
                    // Remove its index
                    _sourceIndices.RemoveAt(newIndex);
                    // Move up indices after removed item
                    for (int i = 0; i < _sourceIndices.Count; i++)
                    {
                        if (_sourceIndices[i].Value > e.NewIndex)
                        {
                            _sourceIndices[i] = new KeyValuePair<ListItemPair<T>, int>(_sourceIndices[i].Key, _sourceIndices[i].Value - 1);
                        }
                    }
                    // Inform listeners that an item has been deleted from this view
                    OnListChanged(ListChangedType.ItemDeleted, newIndex);
                    break;

                case ListChangedType.ItemMoved:
                    if (!IsSorted && (Filter is IncludeAllItemFilter<T>))
                    {
                        // We can move the item in the view
                        // note indicies match those in _sourceList
                        OnListChanged(ListChangedType.ItemMoved, e.NewIndex, e.OldIndex);
                    }
                    // Otherwise it makes no sense to move due to sort and/or filter
                    break;

                case ListChangedType.Reset:
                    // Most of the source list has changed
                    // so re-sort and filter
                    FilterAndSort();
                    // The view is most likely to have changed lots as well
                    OnListChanged(ListChangedType.Reset, -1);
                    break;
            }
        }

        /// <summary>
        /// Event handler for when an item in the view changes.
        /// </summary>
        /// <param name="sender">The item that changed.</param>
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // The changed item may not actually be present in the view
            int index = _sourceIndices.IndexOfItem((T)sender);
            // Test the returned index, -1 => not in the view.
            if (index > -1)
            {
                // Tell listeners that an item has changed.
                // This is inline with the IRaiseItemChangedEvents implementation.
                OnListChanged(ListChangedType.ItemChanged, index);
            }
        }

        #region ListChanged Event

        /// <summary>
        /// Occurs when the list changes or an item in the list changes.
        /// </summary>
        public event ListChangedEventHandler ListChanged;

        /// <summary>
        /// Raises the ListChanged event with the given event arguments.
        /// </summary>
        /// <param name="e">The ListChangedEventArgs to raise the event with.</param>
        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            if (ListChanged != null)
            {
                // Check if we need to invoke on the UI thread or not
                if (SynchronizingObject != null && SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.Invoke(ListChanged, new object[] { this, e });
                }
                else
                {
                    ListChanged(this, e);
                }
            }
        }

        /// <summary>
        /// Helper method to build the ListChangedEventArgs needed for the ListChanged event.
        /// </summary>
        /// <param name="listChangedType">The type of change that  occured.</param>
        /// <param name="newIndex">The index of the changed item.</param>
        private void OnListChanged(ListChangedType listChangedType, int newIndex)
        {
            OnListChanged(new ListChangedEventArgs(listChangedType, newIndex));
        }

        /// <summary>
        /// Helper method to build the ListChangedEventArgs needed for the ListChanged event.
        /// </summary>
        /// <param name="listChangedType">The type of change that  occured.</param>
        /// <param name="newIndex">The index of the item after the change.</param>
        /// <param name="oldIndex">The index of the iem before the change.</param>
        private void OnListChanged(ListChangedType listChangedType, int newIndex, int oldIndex)
        {
            OnListChanged(new ListChangedEventArgs(listChangedType, newIndex, oldIndex));
        }

        #endregion

        #region Filtering

        public void ApplyFilter(IItemFilter<T> filter)
        {
            Filter = filter;
        }

        public void ApplyFilter(Predicate<T> includeItem)
        {
            if (includeItem == null)
            {
                throw new ArgumentNullException("includeItem", Properties.Resources.IncludeDelegateCannotBeNull);
            }

            Filter = AggregateBindingListView<T>.CreateItemFilter(includeItem);
        }

        /// <summary>
        /// Gets if this view supports filtering of items. Always returns true.
        /// </summary>
        bool IBindingListView.SupportsFiltering
        {
            get { return true; }
        }

        /// <remarks>Explicitly implemented to expose the stronger Filter property instead.</remarks>
        string IBindingListView.Filter
        {
            get
            {
                return Filter.ToString();
            }
            set
            {
                throw new NotSupportedException("Cannot set filter from string expression.");
                //TODO: Re-instate this line once we have an expression filter
                //Filter = new ExpressionItemFilter<T>(value);
            }
        }

        /// <summary>
        /// Gets or sets the filter currently applied to the view.
        /// </summary>
        public IItemFilter<T> Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                // Do not allow a null filter. Instead, use the "include all items" filter.
                if (value == null) value = IncludeAllItemFilter<T>.Instance;
                if (_filter != value)
                {
                    _filter = value;
                    FilterAndSort();
                    // The list has probably changed a lot, so get bound controls to reset.
                    OnListChanged(ListChangedType.Reset, -1);
                }
            }
        }

        private bool ShouldSerializeFilter()
        {
            return (Filter != IncludeAllItemFilter<T>.Instance);
        }

        public static IItemFilter<T> CreateItemFilter(Predicate<T> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            return new PredicateItemFilter<T>(predicate);
        }

        // Function for LINQ style filtering
        // e.g. SetFilter(i => i.Items.Count < 42)
        /*
        public static void ApplyFilter(Func<T, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            return new FuncItemFilter<T>(predicate);
        }
        
        // Class to wrap a LINQ Func delegate and expose
        // it as an IItemFilter.
        private class FuncItemFilter<T> : IItemFilter<T>
        {
            private Func<T, bool> _func;

            public FuncItemFilter(Func<T, bool> func)
            {
                _func = func;
            }

            public bool Include(T item)
            {
                return _func(item);
            }
        }
        
        */

        /// <summary>
        /// Removes any currently applied filter so that all items are displayed by the view.
        /// </summary>
        public void RemoveFilter()
        {
            // Set filter back to including all items.
            Filter = IncludeAllItemFilter<T>.Instance;
        }

        #endregion

        #region Sorting

        /// <summary>
        /// Used to signal that a sort on a property is to be descending, not ascending.
        /// </summary>
        public readonly string SortDescendingModifier = "DESC";
        /// <summary>
        /// The character used to seperate sorts by multiple properties.
        /// </summary>
        public readonly char SortDelimiter = ',';

        /// <summary>
        /// Gets if this view supports sorting. Always returns true.
        /// </summary>
        bool IBindingList.SupportsSorting
        {
            get { return true; }
        }

        /// <summary>
        /// Gets if this view supports advanced sorting. Always returns true.
        /// </summary>
        bool IBindingListView.SupportsAdvancedSorting
        {
            get { return true; }
        }

        /// <summary>
        /// Sorts the view by a single property in a given direction.
        /// This will remove any existing sort.
        /// </summary>
        /// <param name="property">A property of <typeparamref name="T"/> to sort by.</param>
        /// <param name="direction">The direction to sort in.</param>
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            // Apply sort by setting the current sort descriptions
            // to be a collection containing just one SortDescription.
            SortDescriptions = new ListSortDescriptionCollection(
                new ListSortDescription[] {
                    new ListSortDescription(property, direction)});
        }

        /// <summary>
        /// Sorts the view by the given collection of sort descriptions.
        /// </summary>
        /// <param name="sorts">The sorts to apply.</param>
        public void ApplySort(ListSortDescriptionCollection sorts)
        {
            SortDescriptions = sorts;
        }

        /// <summary>
        /// Sorts the view according to the properties and directions given in the 
        /// SQL style sort parameter.
        /// </summary>
        /// <param name="sort">
        /// The SQL ORDER BY clause style sort.
        /// A comma separated list of properties to sort by.
        /// Use "DESC" after a property name to sort descending.
        /// The default direction is ascending.
        /// </param>
        /// <example><code>view.ApplySort("Surname, FirstName, Age DESC");</code></example>
        public void ApplySort(string sort)
        {
            if (string.IsNullOrEmpty(sort))
            {
                RemoveSort();
                return;
            }

            // Parse string for sort descriptions
            string[] sorts = sort.Split(SortDelimiter);
            ListSortDescription[] col = new ListSortDescription[sorts.Length];
            for (int i = 0; i < sorts.Length; i++)
            {
                // Get the sort description.
                // This will be a name optionally followed by a direction.
                sort = sorts[i].Trim();
                // A space will separate name from direction.
                int pos = sort.IndexOf(' ');
                string name;
                ListSortDirection direction;
                if (pos == -1)
                {
                    // No direction specified, default to ascending.
                    name = sort;
                    direction = ListSortDirection.Ascending;
                }
                else
                {
                    // Name is everything before the space.
                    name = sort.Substring(0, pos);
                    // direction is everything after the space.
                    string dir = sort.Substring(pos + 1).Trim();
                    // Check what kind of direction is specified.
                    // (Ignoring case and culture.)
                    if (string.Compare(dir, SortDescendingModifier, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
                    {
                        direction = ListSortDirection.Descending;
                    }
                    else
                    {
                        // Default to ascending.
                        direction = ListSortDirection.Ascending;
                    }
                }

                // Put the sort description into the collection.
                col[i] = CreateListSortDescription(name, direction);
            }

            ApplySort(new ListSortDescriptionCollection(col));
        }

        public void ApplySort(IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            // Clear any current sorts
            _sorts = new ListSortDescriptionCollection();
            // Sort with this new comparer
            _comparer = new ExternalSortComparer<T>(comparer);
            FilterAndSort();
            OnListChanged(ListChangedType.Reset, -1);
        }

        public void ApplySort(Comparison<T> comparison)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }

            // Clear any current sorts
            _sorts = new ListSortDescriptionCollection();
            // Sort with this new comparer
            _comparer = new ExternalSortComparison<T>(comparison);
            FilterAndSort();
            OnListChanged(ListChangedType.Reset, -1);
        }

        /// <summary>
        /// Removes any sort currently applied to the view, restoring it to the order of the source list.
        /// </summary>
        public void RemoveSort()
        {
            // An empty collection of sorts will achieve what we need.
            SortDescriptions = new ListSortDescriptionCollection();
        }

        /// <summary>
        /// Gets if the view is currently sorted.
        /// </summary>
        [Browsable(false)]
        public bool IsSorted
        {
            get
            {
                // To be sorted there must be some sorts applied.
                return (SortDescriptions.Count > 0);
            }
        }

        /// <summary>
        /// Gets or sets the string representation of the sort currently applied to the view.
        /// </summary>
        public string Sort
        {
            get
            {
                if (IsSorted)
                {
                    // Build a string of the properties being sorted by
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (ListSortDescription sort in SortDescriptions)
                    {
                        sb.Append(sort.PropertyDescriptor.Name);
                        // Need to signal descending sorts
                        if (sort.SortDirection == ListSortDirection.Descending)
                        {
                            sb.Append(' ').Append(SortDescendingModifier);
                        }
                        // Separate by SortDelimiter
                        sb.Append(SortDelimiter);
                    }
                    // Remove trailing SortDelimiter
                    sb.Remove(sb.Length - 1, 1);
                    // Return the string
                    return sb.ToString();
                }
                return string.Empty;
            }
            set
            {
                ApplySort(value);
            }
        }

        private bool ShouldSerializeSort()
        {
            return !String.IsNullOrEmpty(Sort);
        }

        /// <summary>
        /// Gets the direction in which the view is sorted.
        /// If more than one sort is applied, the direction of the first is returned.
        /// </summary>
        [Browsable(false)]
        public ListSortDirection SortDirection
        {
            get
            {
                if (IsSorted)
                {
                    return SortDescriptions[0].SortDirection;
                }
                else
                {
                    // We don't really want to throw exceptions.
                    // Calling code should have checked IsSorted to know the true situation.
                    return ListSortDirection.Ascending;
                }
            }
        }

        /// <summary>
        /// Gets the property the view is currently sorted by.
        /// If more than one sort is applied, the property of the first is returned.
        /// </summary>
        [Browsable(false)]
        public PropertyDescriptor SortProperty
        {
            get
            {
                if (IsSorted)
                {
                    return SortDescriptions[0].PropertyDescriptor;
                }
                else
                {
                    // We don't really want to throw exceptions.
                    // Calling code should have checked IsSorted to know the true situation.
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the sorts currently applied to the view.
        /// </summary>
        [Browsable(false)]
        public ListSortDescriptionCollection SortDescriptions
        {
            get
            {
                return _sorts;
            }
            private set
            {
                _sorts = value;
                _comparer = new SortComparer(value);
                FilterAndSort();
                // Most of the list will have probably changed, so get bound objects to reset.
                OnListChanged(ListChangedType.Reset, -1);
            }
        }

        /// <summary>
        /// Used to compare items in the view when sorting the _sourceIndices list.
        /// It supports mutliple sorts by different properties and directions.
        /// </summary>
        private class SortComparer : IComparer<KeyValuePair<ListItemPair<T>, int>>
        {
            private Dictionary<ListSortDescription, Comparison<T>> _comparisons;

            /// <summary>
            /// Creates a new SortComparer that will use the given sorts.
            /// </summary>
            /// <param name="sorts">The sorts to apply to the view.</param>
            public SortComparer(ListSortDescriptionCollection sorts)
            {
                _sorts = sorts;

                // Build the delegates used to compare properties of objects
                _comparisons = new Dictionary<ListSortDescription, Comparison<T>>();
                foreach (ListSortDescription sort in sorts)
                {
                    _comparisons[sort] = BuildComparison(sort.PropertyDescriptor.Name, sort.SortDirection);
                }
            }

            private ListSortDescriptionCollection _sorts;

            /// <summary>
            /// Compares two items according to the defined sorts.
            /// </summary>
            /// <remarks>
            /// Use of light-weight code generation comparison delegates gives ~10x speed up
            /// compared to the pure reflection based implementation.
            /// </remarks>
            /// <param name="x">The first item to compare.</param>
            /// <param name="y">The second item to compare.</param>
            /// <returns>-1 if x &lt; y, 0 if x = y and 1 if x &gt; y.</returns>
            public int Compare(KeyValuePair<ListItemPair<T>, int> x, KeyValuePair<ListItemPair<T>, int> y)
            {
                foreach (ListSortDescription sort in _sorts)
                {
                    int result = _comparisons[sort](x.Key.Item.Object, y.Key.Item.Object);
                    if (result != 0)
                    {
                        return result;
                    }
                }
                return 0;
            }

            private static Comparison<T> BuildComparison(string propertyName, ListSortDirection direction)
            {
                PropertyInfo pi = typeof(T).GetProperty(propertyName);
                Debug.Assert(pi != null, string.Format("Property '{0}' is not a member of type '{1}'", propertyName, typeof(T).FullName));

                Type pType = pi.PropertyType;
                bool isNullable = pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

                if (isNullable)
                    pType = pi.PropertyType.GetGenericArguments()[0];

                if (typeof(IComparable).IsAssignableFrom(pType))
                {
                    if (pType.IsValueType && !isNullable)
                    {
                        return delegate (T x, T y)
                        {
                            return (pi.GetValue(x, null) as IComparable).CompareTo(pi.GetValue(y, null));
                        };
                    }
                    else
                    {
                        return delegate (T x, T y)
                        {
                            int result;
                            object value1 = pi.GetValue(x, null);
                            object value2 = pi.GetValue(y, null);
                            if (value1 != null && value2 != null)
                                result = (value1 as IComparable).CompareTo(value2);
                            else if (value1 == null && value2 != null)
                                result = -1;
                            else if (value1 != null && value2 == null)
                                result = 1;
                            else
                                result = 0;

                            if (direction == ListSortDirection.Descending)
                                result *= -1;
                            return result;
                        };
                    }
                }
                else
                {
                    return delegate (T o1, T o2)
                    {
                        if (o1.Equals(o2))
                        {
                            return 0;
                        }
                        else
                        {
                            return o1.ToString().CompareTo(o2.ToString());
                        }
                    };
                }
            }
        }

        private class ExternalSortComparer<U> : IComparer<KeyValuePair<ListItemPair<U>, int>>
        {
            public ExternalSortComparer(IComparer<U> comparer)
            {
                _comparer = comparer;
            }

            private IComparer<U> _comparer;

            public int Compare(KeyValuePair<ListItemPair<U>, int> x, KeyValuePair<ListItemPair<U>, int> y)
            {
                return _comparer.Compare(x.Key.Item.Object, y.Key.Item.Object);
            }
        }

        private class ExternalSortComparison<U> : IComparer<KeyValuePair<ListItemPair<U>, int>>
        {
            public ExternalSortComparison(Comparison<U> comparison)
            {
                _comparison = comparison;
            }

            private Comparison<U> _comparison;

            public int Compare(KeyValuePair<ListItemPair<U>, int> x, KeyValuePair<ListItemPair<U>, int> y)
            {
                return _comparison(x.Key.Item.Object, y.Key.Item.Object);
            }
        }

        #endregion

        #region Searching

        /// <summary>
        /// Gets if this view supports searching using the Find method. Always returns true.
        /// </summary>
        bool IBindingList.SupportsSearching
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the index of the first item in the view who's property equals the given value.
        /// -1 is returned if no item is found.
        /// </summary>
        /// <param name="property">The property of each item to check.</param>
        /// <param name="key">The value being sought.</param>
        /// <returns>The index of the item, or -1 if not found.</returns>
        public int Find(PropertyDescriptor property, object key)
        {
            for (int i = 0; i < _sourceIndices.Count; i++)
            {
                if (property.GetValue(_sourceIndices[i].Key.Item.Object).Equals(key))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first item in the view who's property equals the given value.
        /// -1 is returned if no item is found.
        /// </summary>
        /// <param name="property">The property name of each item to check.</param>
        /// <param name="key">The value being sought.</param>
        /// <returns>The index of the item, or -1 if not found.</returns>
        /// <remarks>
        /// It is easier for users of this class to enter a property name
        /// and get the PropertyDescriptor ourselves.
        /// </remarks>
        public int Find(string propertyName, object key)
        {
            PropertyDescriptor pd = GetPropertyDescriptor(propertyName);
            if (pd != null)
            {
                return Find(pd, key);
            }
            else
            {
                throw new ArgumentException(string.Format(Properties.Resources.PropertyNotFound, propertyName, typeof(T).FullName), "propertyName");
            }
        }

        #endregion

        #region IBindingList Members

        /// <summary>
        /// Gets if this view raises the ListChanged event. Always returns true.
        /// </summary>
        bool IBindingList.SupportsChangeNotification
        {
            get { return true; }
        }

        /// <remarks>Explicitly implemented so the type safe AddNew method is exposed instead.</remarks>
        object IBindingList.AddNew()
        {
            return this.AddNew();
        }

        /// <summary>
        /// Gets if this view allows items to be edited.
        /// </summary>
        /// <remarks>Delegates to the source list.</remarks>
        bool IBindingList.AllowEdit
        {
            get
            {
                foreach (object list in SourceLists)
                {
                    if (list is IBindingList)
                    {
                        if (!(list as IBindingList).AllowEdit)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Gets if this view allows new items to be added using AddNew().
        /// </summary>
        /// <remarks>Delegates to the source list.</remarks>
        bool IBindingList.AllowNew
        {
            get
            {
                if (_newItemsList != null)
                {
                    if (_newItemsList is IBindingList)
                    {
                        // Respect what the binding list says.
                        return (_newItemsList as IBindingList).AllowNew;
                    }
                    // _newItemsList is a IList, so we can call Add()
                    // it may fail at runtime - but that is the callee's problem
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets if this view allows items to be removed.
        /// </summary>
        /// <remarks>Delegates to the source list.</remarks>
        bool IBindingList.AllowRemove
        {
            get
            {
                foreach (object list in SourceLists)
                {
                    if (list is IBindingList)
                    {
                        if (!(list as IBindingList).AllowRemove)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Method not implemented.</exception>
        void IBindingList.AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Method not implemented.</exception>
        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRaiseItemChangedEvents Members

        /// <summary>
        /// Gets if this view raises the ListChanged event when an item changes. Always returns true.
        /// </summary>
        [Browsable(false)]
        public bool RaisesItemChangedEvents
        {
            get { return true; }
        }

        #endregion

        #region IList Members

        /// <exception cref="System.ArgumentException">
        /// value is of the wrong type.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="NewItemsList"/> is null, so an item cannot be added.
        /// </exception>
        int IList.Add(object value)
        {
            if (value == null)
            {
                AddNew();
                return Count - 1;
            }

            throw new NotSupportedException(Properties.Resources.CannotAddItem);
        }

        /// <summary>
        /// Cannot clear this view.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Cannot clear this view.
        /// </exception>
        void IList.Clear()
        {
            throw new NotSupportedException(Properties.Resources.CannotClearView);
        }

        /// <summary>
        /// Checks if this view contains the given item.
        /// Note that items excluded by current filter are not searched.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>True if the item is in the view, else false.</returns>
        bool IList.Contains(object item)
        {
            // See if the source indices contain the item
            if (item is ObjectView<T>)
            {
                return _sourceIndices.ContainsKey((ObjectView<T>)item);
            }
            else if (item is T)
            {
                return _sourceIndices.ContainsItem((T)item);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the index in the view of an item.
        /// </summary>
        /// <param name="item">The item to search for</param>
        /// <returns>The index of the item, or -1 if not found.</returns>
        int IList.IndexOf(object item)
        {
            if (item is ObjectView<T>)
            {
                return _sourceIndices.IndexOfKey(item as ObjectView<T>);
            }
            else if (item is T)
            {
                return _sourceIndices.IndexOfItem((T)item);
            }
            return -1;
        }

        /// <summary>
        /// Cannot insert an external item into this collection.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Cannot insert an external item into this collection.
        /// </exception>
        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException(Properties.Resources.CannotInsertItem);
        }

        /// <summary>
        /// Gets a value indicating if this view is read-only.
        /// </summary>
        /// <remarks>Delegates to the source list.</remarks>
        bool IList.IsReadOnly
        {
            get
            {
                foreach (object list in SourceLists)
                {
                    if (list is IBindingList)
                    {
                        if (!(list as IBindingList).IsReadOnly)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Always returns <code>false</code> because the view can change size when
        /// source lists are added.
        /// </summary>
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the given item from the view and underlying source list.
        /// </summary>
        /// <param name="value">Either an ObjectView&lt;T&gt; or T to remove.</param>
        void IList.Remove(object value)
        {
            int index = (this as IList).IndexOf(value);
            (this as IList).RemoveAt(index);
        }

        /// <summary>
        /// Removes the item from the view at the given index.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        void IList.RemoveAt(int index)
        {
            // Get the index in the source list.
            int sourceIndex = _sourceIndices[index].Value;
            IList sourceList = _sourceIndices[index].Key.List;
            if (sourceIndex > -1)
            {
                sourceList.RemoveAt(sourceIndex);
                if (!(sourceList is IBindingList) || !(sourceList as IBindingList).SupportsChangeNotification)
                {
                    FilterAndSort();
                    OnListChanged(ListChangedType.Reset, -1);
                }
            }
            else
            {
                // The item is not in the source list yet as it is new
                // So cancel the new operation instead.
                CancelNew(index);
            }
        }

        /// <summary>
        /// Gets the <see cref="ObjectView&lt;T&gt;"/> at the given index.
        /// </summary>
        /// <param name="index">The index of the item to retrieve.</param>
        /// <returns>An <see cref="ObjectView&lt;T&gt;"/> object.</returns>
        /// <exception cref="System.NotSupportException">
        /// Cannot set an item in the view.
        /// </exception>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                // The interface requires we supply a setter
                // But we don't want external code modifying the view
                // in this manner.
                throw new NotSupportedException(Properties.Resources.CannotSetItem);
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Copies the <see cref="ObjectView&lt;T&gt;"/> objects of the view to an <see cref="System.Array"/>, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the elements copied from view. The System.Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        void ICollection.CopyTo(Array array, int index)
        {
            _sourceIndices.Keys.CopyTo(array, index);
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="System.Collections.ICollection" /> is synchronized (thread safe).
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException(Properties.Resources.SyncAccessNotSupported); }
        }

        /// <summary>
        /// Gets the number of items currently in the view. This does not include those items
        /// excluded by the current filter.
        /// </summary>
        [Browsable(false)]
        public int Count
        {
            get { return _sourceIndices.Count; }
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < _sourceIndices.Count; i++)
                yield return _sourceIndices[i].Key.Item.Object;

        }

        /// <summary>
        /// Returns an enumerator that iterates through all the <see cref="ObjectView&lt;T&gt;"/> items in the view.
        /// This does not include those items excluded by the current filter.
        /// </summary>
        /// <returns>An IEnumerator to iterate with.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sourceIndices.GetKeyEnumerator();
        }

        #endregion

        #region ITypedList Members

        /// <summary>
        /// Returns the <see cref="System.ComponentModel.PropertyDescriptorCollection"/> that represents the properties on each item used to bind data.
        /// </summary>
        /// <param name="listAccessors">Array of property descriptors to navigate object hirerachy to actual item object. It can be null.</param>
        /// <returns>The System.ComponentModel.PropertyDescriptorCollection that represents the properties on each item used to bind data.</returns>
        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            PropertyDescriptorCollection originalProps;

            IEnumerator<IList> lists = GetSourceLists().GetEnumerator();

            if (lists.MoveNext() && lists.Current is ITypedList)
            {
                // Ask the source list for the properties.
                originalProps = (lists.Current as ITypedList).GetItemProperties(listAccessors);
            }
            else
            {
                // Get the properties ourself.
                originalProps = System.Windows.Forms.ListBindingHelper.GetListItemProperties(typeof(T), listAccessors);
            }

            if (listAccessors != null && listAccessors.Length > 0)
            {
                Type type = originalProps[0].ComponentType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ObjectView<>))
                {
                    originalProps = originalProps[0].GetChildProperties();
                }
            }

            List<PropertyDescriptor> newProps = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor pd in AddProvidedViews(originalProps))
            {
                newProps.Add(pd);
            }

            return new PropertyDescriptorCollection(newProps.ToArray());
        }

        protected internal bool ShouldProvideView(PropertyDescriptor property)
        {
            return ProvidedViewPropertyDescriptor.CanProvideViewOf(property);
        }

        protected internal string GetProvidedViewName(PropertyDescriptor sourceListProperty)
        {
            return sourceListProperty.Name + "View";
        }

        protected internal object CreateProvidedView(ObjectView<T> @object, PropertyDescriptor sourceListProperty)
        {
            object list = sourceListProperty.GetValue(@object);
            Type viewType = GetProvidedViewType(sourceListProperty);
            return Activator.CreateInstance(viewType, list);
        }

        private static Type GetProvidedViewType(PropertyDescriptor sourceListProperty)
        {
            // The source list property type implements IList<X>.
            // We want to get the type of X.
            Type typeParam = null;
            foreach (Type interfaceType in sourceListProperty.PropertyType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition().Equals(typeof(IList<>)))
                {
                    typeParam = interfaceType.GetGenericArguments()[0];
                }
            }
            Debug.Assert(typeParam != null, "Did not get the generic argument for type " + sourceListProperty.PropertyType.FullName);

            // Now build the BindingListView<X> type.
            Type viewTypeDef = typeof(BindingListView<>);
            Type viewType = viewTypeDef.MakeGenericType(typeParam);
            return viewType;
        }

        internal IEnumerable<PropertyDescriptor> AddProvidedViews(PropertyDescriptorCollection properties)
        {
            if (properties.Count < 0)
            {
                yield break;
            }
            foreach (PropertyDescriptor prop in properties)
            {
                if (ShouldProvideView(prop))
                {
                    string name = GetProvidedViewName(prop);
                    yield return new ProvidedViewPropertyDescriptor(name, GetProvidedViewType(prop));
                }
                yield return prop;
            }
        }

        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        /// <param name="listAccessors">Unused. Can be null.</param>
        /// <returns>The name of the view.</returns>
        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return GetType().Name;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a new <see cref="System.ComponentModel.ListSortDescription"/> for given property name and sort direction.
        /// </summary>
        /// <param name="propertyName">The name of the property to sort by.</param>
        /// <param name="direction">The direction in which to sort.</param>
        /// <returns>A ListSortDescription.</returns>
        /// <remarks>
        /// Used by external code to simplify sorting the view.
        /// </remarks>
        public ListSortDescription CreateListSortDescription(string propertyName, ListSortDirection direction)
        {
            PropertyDescriptor pd = GetPropertyDescriptor(propertyName);
            if (pd == null)
            {
                throw new ArgumentException(string.Format(Properties.Resources.PropertyNotFound, propertyName, typeof(T).FullName), "propertyName");
            }
            return new ListSortDescription(pd, direction);
        }

        /// <summary>
        /// Gets the property descriptor for a given property name.
        /// </summary>
        /// <param name="propertyName">The name of a property of <typeparamref name="T"/>.</param>
        /// <returns>The <see cref="System.ComponentModel.PropertyDescriptor"/>.</returns>
        private PropertyDescriptor GetPropertyDescriptor(string propertyName)
        {
            return TypeDescriptor.GetProperties(typeof(T)).Find(propertyName, false);
        }

        /// <summary>
        /// Attaches event handlers to the given <see cref="ObjectView&lt;T&gt;"/>'s 
        /// edit life cycle notification events.
        /// </summary>
        /// <param name="objectView">The <see cref="ObjectView&lt;T&gt;"/> to listen to.</param>
        private void HookEditableObjectEvents(ObjectView<T> editableObject)
        {
            editableObject.EditBegun += new EventHandler(BegunItemEdit);
            editableObject.EditCancelled += new EventHandler(CancelledItemEdit);
            editableObject.EditEnded += new EventHandler(EndedItemEdit);
        }

        /// <summary>
        /// Detaches event handlers from the given <see cref="ObjectView&lt;T&gt;"/>'s 
        /// edit life cycle notification events.
        /// </summary>
        /// <param name="objectView">The <see cref="ObjectView&lt;T&gt;"/> to stop listening to.</param>
        private void UnHookEditableObjectEvents(ObjectView<T> editableObject)
        {
            editableObject.EditBegun -= new EventHandler(BegunItemEdit);
            editableObject.EditCancelled -= new EventHandler(CancelledItemEdit);
            editableObject.EditEnded -= new EventHandler(EndedItemEdit);
        }

        /// <summary>
        /// Attaches an event handler to the <see cref="ObjectView&lt;T&gt;"/>'s PropertyChanged event.
        /// </summary>
        /// <param name="objectView">The <see cref="ObjectView&lt;T&gt;"/> to listen to.</param>
        private void HookPropertyChangedEvent(ObjectView<T> editableObject)
        {
            editableObject.PropertyChanged += new PropertyChangedEventHandler(ItemPropertyChanged);
        }

        /// <summary>
        /// Detaches the event handler from the <see cref="ObjectView&lt;T&gt;"/>'s PropertyChanged event.
        /// </summary>
        /// <param name="objectView">The <see cref="ObjectView&lt;T&gt;"/> to stop listening to.</param>
        private void UnHookPropertyChangedEvent(ObjectView<T> editableObject)
        {
            editableObject.PropertyChanged -= new PropertyChangedEventHandler(ItemPropertyChanged);
        }

        private void BuildSavedList()
        {
            _savedSourceLists.Clear();
            foreach (object list in GetSourceLists())
            {
                _savedSourceLists.Add(list as IList);
            }
        }

        protected IEnumerable<IList> GetSourceLists()
        {
            foreach (object obj in _sourceLists)
            {
                if (!string.IsNullOrEmpty(DataMember))
                {
                    bool found = false;
                    foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(obj))
                    {
                        if (pd.Name == DataMember)
                        {
                            found = true;
                            yield return pd.GetValue(obj) as IList;
                            break;
                        }
                    }
                    if (!found)
                    {
                        yield return null;
                    }
                }
                else if (obj is IListSource)
                {
                    IListSource src = obj as IListSource;
                    if (src.ContainsListCollection)
                    {
                        IList list = src.GetList() as IList;
                        if (list != null && list.Count > 0)
                        {
                            list = list[0] as IList;
                            yield return list;
                        }
                        else
                        {
                            yield return null;
                        }
                    }
                    else
                    {
                        yield return src.GetList();
                    }
                }
                else
                {
                    yield return obj as IList;
                }
            }
        }

        #endregion

    }
}
