using DoomLauncher.Interfaces;
using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace DoomLauncher
{
    class ToolTipDisplayHandler
    {
        private readonly IGameFileView m_view;
        private readonly System.Timers.Timer m_toolTipTimer;
        private readonly ToolTip m_toolTip;
        private IGameFile m_gameFile;

        public ToolTipDisplayHandler(IGameFileView view, ToolTip toolTip)
        {
            m_view = view;
            m_toolTip = toolTip;

            m_toolTipTimer = new System.Timers.Timer(500);
            m_toolTipTimer.Elapsed += ToolTipTimer_Elapsed;

            view.GameFileEnter += View_GameFileEnter;
            view.GameFileLeave += View_GameFileLeave;
        }

        private void ToolTipTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_toolTipTimer.Enabled && m_view.ToolTipControl.InvokeRequired)
            {
                m_view.ToolTipControl.Invoke(new Action(SetToolTipText));
                m_toolTipTimer.Stop();
            }
        }

        private void SetToolTipText()
        {
            ToolTipHandler toolTipHandler = new ToolTipHandler();
            Point pt = new Point(Cursor.Position.X - MainForm.Instance.Location.X, Cursor.Position.Y - MainForm.Instance.Location.Y);
            m_toolTip.Show(toolTipHandler.GetToolTipText(MainForm.Instance.Font, m_gameFile), MainForm.Instance, pt.X, pt.Y, 32767);
        }

        private void View_GameFileEnter(object sender, GameFileEventArgs e)
        {
            m_toolTipTimer.Stop();

            if (m_gameFile != e.GameFile)
            {
                m_toolTip.Hide(MainForm.Instance);
                m_gameFile = e.GameFile;
                m_toolTipTimer.Interval = 500;
                m_toolTipTimer.Start();
            }
        }

        private void View_GameFileLeave(object sender, GameFileEventArgs e)
        {
            m_toolTipTimer.Stop();
            m_toolTip.Hide(MainForm.Instance);
            m_gameFile = null;
        }
    }
}
