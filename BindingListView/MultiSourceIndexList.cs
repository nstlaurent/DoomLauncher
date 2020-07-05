using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace Equin.ApplicationFramework
{
    internal class MultiSourceIndexList<T> : List<KeyValuePair<ListItemPair<T>, int>>
    {
        public void Add(IList sourceList, ObjectView<T> item, int index)
        {
            Add(new KeyValuePair<ListItemPair<T>, int>(new ListItemPair<T>(sourceList, item), index));
        }

        /// <summary>
        /// Searches for a given source index value, returning the list index of the value.
        /// </summary>
        /// <param name="sourceIndex">The source index to find.</param>
        /// <returns>Returns the index in this list of the source index, or -1 if not found.</returns>
        public int IndexOfSourceIndex(IList sourceList, int sourceIndex)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Key.List == sourceList && this[i].Value == sourceIndex)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Searches for a given item, returning the index of the value in this list.
        /// </summary>
        /// <param name="item">The <typeparamref name="T"/> item to search for.</param>
        /// <returns>Returns the index in this list of the item, or -1 if not found.</returns>
        public int IndexOfItem(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Key.Item.Object.Equals(item) && this[i].Value > -1)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Searches for a given item's <see cref="ObjectView&lt;T&gt;"/> wrapper, returning the index of the value in this list.
        /// </summary>
        /// <param name="item">The <see cref="ObjectView&lt;T&gt;"/> to search for.</param>
        /// <returns>Returns the index in this list of the item, or -1 if not found.</returns>
        public int IndexOfKey(ObjectView<T> item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Key.Item.Equals(item) && this[i].Value > -1)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Checks if the list contains a given item.
        /// </summary>
        /// <param name="item">The <typeparamref name="T"/> item to check for.</param>
        /// <returns>True if the item is contained in the list, otherwise false.</returns>
        public bool ContainsItem(T item)
        {
            return (IndexOfItem(item) != -1);
        }

        /// <summary>
        /// Checks if the list contains a given <see cref="ObjectView&lt;T&gt;"/> key.
        /// </summary>
        /// <param name="item">The key to search for.</param>
        /// <returns>True if the key is contained in the list, otherwise false.</returns>
        public bool ContainsKey(ObjectView<T> key)
        {
            return (IndexOfKey(key) != -1);
        }

        /// <summary>
        /// Returns an array of all the <see cref="ObjectView&lt;T&gt;"/> keys in the list.
        /// </summary>
        public ObjectView<T>[] Keys
        {
            get
            {
                return ConvertAll<ObjectView<T>>(new Converter<KeyValuePair<ListItemPair<T>, int>, ObjectView<T>>(
                    delegate(KeyValuePair<ListItemPair<T>, int> kvp)
                    { return kvp.Key.Item; }
                )).ToArray();
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator&lt;T&gt;"/> to iterate over all the keys in this list.
        /// </summary>
        /// <returns>The <see cref="IEnumerator&lt;T&gt;"/> to use.</returns>
        public IEnumerator<ObjectView<T>> GetKeyEnumerator()
        {
            foreach (KeyValuePair<ListItemPair<T>, int> kvp in this)
            {
                yield return kvp.Key.Item;
            }
        }
    }

    internal class ListItemPair<T>
    {
        private IList _list;
        private ObjectView<T> _item;

        public ListItemPair(IList list, ObjectView<T> item)
        {
            _list = list;
            _item = item;
        }

        public IList List
        {
            get
            {
                return _list;
            }
        }

        public ObjectView<T> Item
        {
            get
            {
                return _item;
            }
        }
    }
}
