using DoomLauncher.Interfaces;
using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace DoomLauncher
{
    enum ToolTipState
    {
        Waiting,
        BeforeShow,
        Showing
    }

    class ToolTipDisplayHandler
    {
        private readonly System.Timers.Timer m_toolTipTimer;
        private readonly ToolTip m_toolTip;
        private readonly Form m_form;
        private IGameFile m_gameFile;
        private Point m_showPoint = new Point();
        private Point m_lastPollPoint = new Point();
        private ToolTipState m_state = ToolTipState.Waiting;

        private const int TimerDelay = 500;
        private const int Range = 16;

        public ToolTipDisplayHandler(Form form)
        {
            m_form = form;
            m_toolTip = new ToolTip();

            m_toolTipTimer = new System.Timers.Timer(TimerDelay);
            m_toolTipTimer.Elapsed += ToolTipTimer_Elapsed;
        }

        public void RegisterView(IGameFileView view)
        {
            view.GameFileEnter += View_GameFileEnter;
            view.GameFileLeave += View_GameFileLeave;
            view.SelectionChange += View_SelectionChange;
            view.ItemClick += View_ItemClick;
        }

        private void View_ItemClick(object sender, EventArgs e)
        {
            ResetToolTipAndClear();
        }

        private void View_SelectionChange(object sender, EventArgs e)
        {
            if (m_state == ToolTipState.Showing)
                ResetToolTipAndClear();
        }

        private void ToolTipTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_lastPollPoint = GetMouseLocation();

            if (m_state == ToolTipState.Showing)
            {
                if (!MouseMoveInRange(m_showPoint))
                    ResetToolTipAndClear();
            }
            else if (m_state == ToolTipState.Waiting)
            {
                if (MouseMoveInRange(m_lastPollPoint) && m_form.InvokeRequired)
                    m_form.Invoke(new Action(ShowToolTip));
            }
        }

        private void ShowToolTip()
        {
            if (m_gameFile == null)
                return;

            ToolTipHandler toolTipHandler = new ToolTipHandler();
            m_showPoint = GetMouseLocation();
            m_toolTip.Show(toolTipHandler.GetToolTipText(m_form.Font, m_gameFile), m_form, m_showPoint.X, m_showPoint.Y, 32767);
            m_state = ToolTipState.Showing;
        }

        private Point GetMouseLocation()
        {
            return new Point(Cursor.Position.X - m_form.Location.X + 1, Cursor.Position.Y - m_form.Location.Y + 16);
        }

        private void View_GameFileEnter(object sender, GameFileEventArgs e)
        {
            if (m_gameFile != e.GameFile)
            {
                m_toolTip.Hide(m_form);
                m_gameFile = e.GameFile;
                m_toolTipTimer.Interval = TimerDelay;
                m_toolTipTimer.Start();
                m_state = ToolTipState.Waiting;
            }
        }

        private void View_GameFileLeave(object sender, GameFileEventArgs e)
        {
            ResetToolTipAndClear();
            m_gameFile = null;
        }

        private void ResetToolTipAndClear()
        {
            if (m_form.InvokeRequired)
            {
                m_form.Invoke(new Action(ResetToolTipAndClear));
            }
            else
            {
                m_toolTip.Hide(m_form);
                m_state = ToolTipState.BeforeShow;
                m_gameFile = null;
                m_showPoint = new Point();
            }
        }

        private bool MouseMoveInRange(in Point pt)
        {
            Rectangle rect = new Rectangle(pt, new Size(Range, Range));
            rect.Offset(-Range / 2, -Range / 2);

            return rect.Contains(GetMouseLocation());
        }
    }
}
