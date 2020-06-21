using System;
using System.Collections.Generic;

namespace Equin.ApplicationFramework
{
    public class CompositeItemFilter<T> : IItemFilter<T>
    {
        private List<IItemFilter<T>> _filters;

        public CompositeItemFilter()
        {
            _filters = new List<IItemFilter<T>>();
        }

        public void AddFilter(IItemFilter<T> filter)
        {
            _filters.Add(filter);
        }

        public void RemoveFilter(IItemFilter<T> filter)
        {
            _filters.Remove(filter);
        }

        public bool Include(T item)
        {
            foreach (IItemFilter<T> filter in _filters)
            {
                if (!filter.Include(item))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
