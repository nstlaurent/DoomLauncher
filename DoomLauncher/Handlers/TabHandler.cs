using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DoomLauncher
{
    class TabHandler
    {
        private readonly Dictionary<IGameFileView, TabItem> m_tabLookup = new Dictionary<IGameFileView, TabItem>();
        private readonly List<ITabView> m_tabs = new List<ITabView>();

        private class TabItem
        {
            public ITabView TabView { get; set; }
            public TabPage TabPage { get; set; }
        }

        public TabHandler(TabControl tabControl)
        {            
            TabControl = tabControl;
        }

        public void SetTabs(IEnumerable<ITabView> tabs)
        {
            m_tabLookup.Clear();

            foreach (ITabView tab in tabs)
                AddTab(tab);
        }

        private TabPage CreateTabPage(ITabView tab)
        {
            TabPage page = new TabPage(tab.Title);
            page.Name = tab.Key.ToString();

            if (tab is Control ctrl)
            {
                page.Controls.Add(ctrl);
                ctrl.Dock = DockStyle.Fill;
            }

            return page;
        }

        public void AddTab(ITabView tab)
        {
            TabPage page = CreateTabPage(tab);

            m_tabLookup.Add(tab.GameFileViewControl, new TabItem { TabView = tab, TabPage = page });
            m_tabs.Add(tab);

            TabControl.TabPages.Add(page);
        }

        public void InsertTab(int index, ITabView tab)
        {
            TabPage page = CreateTabPage(tab);
            TabControl.TabPages.Insert(index, page);

            m_tabLookup.Add(tab.GameFileViewControl, new TabItem { TabView = tab, TabPage = page });
            m_tabs.Insert(index, tab);
        }

        public void RemoveTab(ITabView tab)
        {
            if (m_tabLookup.ContainsKey(tab.GameFileViewControl))
            {
                TabItem item = m_tabLookup[tab.GameFileViewControl];

                m_tabLookup.Remove(tab.GameFileViewControl);
                m_tabs.Remove(item.TabView);

                TabControl.TabPages.Remove(item.TabPage);
            }
        }

        public int GetTabIndex(ITabView tab)
        {
            int index = 0;
            foreach (var item in m_tabLookup.Values)
            {
                if (item.TabView == tab)
                    return index;
                index++;
            }

            return -1;
        }

        public void SetTabIndex(int index, ITabView tab)
        {
            if (m_tabLookup.ContainsKey(tab.GameFileViewControl))
            {
                TabItem item = m_tabLookup[tab.GameFileViewControl];
                TabControl.TabPages.Remove(item.TabPage);

                if (index < TabControl.TabPages.Count)
                    TabControl.TabPages.Insert(index, item.TabPage);
                else
                    TabControl.TabPages.Add(item.TabPage);
            }
        }

        public void UpdateTabTitle(ITabView tab, string text)
        {
            tab.Title = text;

            if (m_tabLookup.ContainsKey(tab.GameFileViewControl))
            {
                TabItem item = m_tabLookup[tab.GameFileViewControl];
                item.TabPage.Text = text;
            }
        }

        public ITabView TabViewForControl(IGameFileView ctrl)
        {
            if (m_tabLookup.ContainsKey(ctrl))
                return m_tabLookup[ctrl].TabView;

            return null;
        }

        public ITabView TabViewForTag(ITagData tag)
        {
            foreach (var item in m_tabLookup)
            {
                if (item.Value.TabView is TagTabView tagTabView && tagTabView.TagDataSource.TagID == tag.TagID)
                    return item.Value.TabView;
            }

            return null;
        }

        public bool SelectTabView(ITabView view)
        {
            foreach (var item in m_tabLookup)
            {
                if (item.Value.TabView == view)
                {
                    TabControl.SelectedTab = item.Value.TabPage;
                    return true;
                }
            }

            return false;
        }

        public void SelectTabFromKey(string key)
        {
            foreach (var item in m_tabLookup)
            {
                if (item.Value.TabView.Key.Equals(key))
                {
                    TabControl.SelectedTab = item.Value.TabPage;
                    break;
                }
            }
        }

        public TabControl TabControl { get; private set; }
        public ITabView[] TabViews
        {
            get { return m_tabs.ToArray(); }
        }
    }
}
