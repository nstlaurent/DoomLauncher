using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Equin.ApplicationFramework
{
    /// <summary>
    /// A searchable, sortable, filterable, data bindable view of a list of objects.
    /// </summary>
    /// <typeparam name="T">The type of object in the list.</typeparam>
    public class BindingListView<T> : AggregateBindingListView<T>
    {
        /// <summary>
        /// Creates a new <see cref="BindingListView&lt;T&gt;"/> of a given IBindingList.
        /// All items in the list must be of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="list">The list of objects to base the view on.</param>
        public BindingListView(IList list)
            : base()
        {
            DataSource = list;
        }

        public BindingListView(IContainer container)
            : base(container)
        {
            DataSource = null;
        }

        [DefaultValue(null)]
        [AttributeProvider(typeof(IListSource))]
        public IList DataSource
        {
            get
            {
                IEnumerator<IList> e = GetSourceLists().GetEnumerator();
                e.MoveNext();
                return e.Current;
            }
            set
            {
                if (value == null)
                {
                    // Clear all current data
                    SourceLists = new BindingList<IList<T>>();
                    NewItemsList = null;
                    FilterAndSort();
                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                    return;
                }

                if (!(value is ICollection<T>))
                {
                    // list is not a strongy-type collection.
                    // Check that items in list are all of type T
                    foreach (object item in value)
                    {
                        if (!(item is T))
                        {
                            throw new ArgumentException(string.Format(Properties.Resources.InvalidListItemType, typeof(T).FullName), "DataSource");
                        }
                    }
                }

                SourceLists = new object[] { value };
                NewItemsList = value;
            }
        }

        private bool ShouldSerializeDataSource()
        {
            return (SourceLists.Count > 0);
        }

        protected override void SourceListsChanged(object sender, ListChangedEventArgs e)
        {
            if ((SourceLists.Count > 1 && e.ListChangedType == ListChangedType.ItemAdded) || e.ListChangedType == ListChangedType.ItemDeleted)
            {
                throw new Exception("BindingListView allows strictly one source list.");
            }
            else
            {
                base.SourceListsChanged(sender, e);
            }
        }
    }
}
