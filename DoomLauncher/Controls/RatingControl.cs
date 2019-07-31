using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class RatingControl : UserControl
    {
        private int m_columnCount = 5, m_selectedRating;

        private List<PictureBox> m_pictures = new List<PictureBox>();

        public RatingControl()
        {
            InitializeComponent();
            InitPictures();
        }

        private void InitPictures()
        {
            foreach(PictureBox pb in m_pictures)
            {
                tblMain.Controls.Remove(pb);
                pb.Dispose();
            }

            m_pictures.Clear();

            for (int i = 0; i < m_columnCount; i++)
            {
                PictureBox pb = null;

                if (i <= m_selectedRating)
                    pb = CreatePictureBox(DoomLauncher.Properties.Resources.bon2b);
                else
                    pb = CreatePictureBox(DoomLauncher.Properties.Resources.bon2a);

                m_pictures.Add(pb);

                tblMain.Controls.Add(pb, i, 0);
            }
        }

        void pb_MouseEnter(object sender, EventArgs e)
        {
            //PictureBox pb = sender as PictureBox;
            //Image imgSet = DoomLauncher.Properties.Resources.bon2b;

            //for(int i=0; i < m_pictures.Count; i++)
            //{
            //    tblMain.Controls.Remove(m_pictures[i]);
            //    m_pictures[i].Dispose();

            //    PictureBox pbNew = CreatePictureBox(imgSet);
            //    if (m_pictures[i].Equals(pb))
            //        imgSet = DoomLauncher.Properties.Resources.bon2a;
            //    m_pictures[i] = pbNew;
            //    tblMain.Controls.Add(pbNew, i, 0);
            //}
        }

        private void tblMain_MouseLeave(object sender, EventArgs e)
        {
            //InitPictures();
        }

        private PictureBox CreatePictureBox(Image img)
        {
            PictureBox pb = new PictureBox();
            pb.Image = img;
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.MouseHover += pb_MouseEnter;
            pb.MouseDown += pb_MouseDown;
            pb.Margin = new Padding(2, 2, 2, 2);
            pb.Dock = DockStyle.Fill;
            return pb;
        }

        void pb_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;

            for (int i = 0; i < m_pictures.Count; i++)
            {
                if (m_pictures[i].Equals(pb))
                {
                    m_selectedRating = i;
                    break;
                }
            }

            InitPictures();
        }

        public int RatingCount
        {
            get { return m_columnCount; }
        }

        public int SelectedRating
        {
            get { return m_selectedRating + 1; }
            set { m_selectedRating = value - 1; InitPictures(); }
        }
    }
}
