using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    class TabHandler
    {
        private readonly Dictionary<GameFileViewControl, Tuple<ITabView, TabPage>> m_tabLookup = new Dictionary<GameFileViewControl, Tuple<ITabView, TabPage>>();
        private readonly List<ITabView> m_tabs = new List<ITabView>();

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
            TabControl.TabPages.Add(page);

            m_tabLookup.Add(tab.GameFileViewControl, new Tuple<ITabView, TabPage>(tab, page));
            m_tabs.Add(tab);
        }

        public void InsertTab(int index, ITabView tab)
        {
            TabPage page = CreateTabPage(tab);
            TabControl.TabPages.Insert(index, page);

            m_tabLookup.Add(tab.GameFileViewControl, new Tuple<ITabView, TabPage>(tab, page));
            m_tabs.Insert(index, tab);
        }

        public void RemoveTab(ITabView tab)
        {
            if (m_tabLookup.ContainsKey(tab.GameFileViewControl))
            {
                Tuple<ITabView, TabPage> item = m_tabLookup[tab.GameFileViewControl];

                m_tabLookup.Remove(tab.GameFileViewControl);
                m_tabs.Remove(item.Item1);

                TabControl.TabPages.Remove(item.Item2);
            }
        }

        public void UpdateTabTitle(ITabView tab, string text)
        {
            if (m_tabLookup.ContainsKey(tab.GameFileViewControl))
            {
                Tuple<ITabView, TabPage> item = m_tabLookup[tab.GameFileViewControl];
                item.Item2.Text = text;
            }
        }

        public ITabView TabViewForControl(GameFileViewControl ctrl)
        {
            if (m_tabLookup.ContainsKey(ctrl))
                return m_tabLookup[ctrl].Item1;

            return null;
        }

        public TabControl TabControl { get; private set; }
        public ITabView[] TabViews
        {
            get { return m_tabs.ToArray(); }
        }
    }
}
