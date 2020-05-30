using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class RatingControl : UserControl
    {
        private readonly int ColumnCount = 5;
        private int m_selectedRating;

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

            for (int i = 0; i < ColumnCount; i++)
            {
                PictureBox pb;

                if (i <= m_selectedRating)
                    pb = CreatePictureBox(DoomLauncher.Properties.Resources.bon2b);
                else
                    pb = CreatePictureBox(DoomLauncher.Properties.Resources.bon2a);

                m_pictures.Add(pb);

                tblMain.Controls.Add(pb, i, 0);
            }
        }

        private PictureBox CreatePictureBox(Image img)
        {
            PictureBox pb = new PictureBox
            {
                Image = img,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Margin = new Padding(2, 2, 2, 2),
                Dock = DockStyle.Fill
            };
            pb.MouseDown += pb_MouseDown;
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
            get { return ColumnCount; }
        }

        public int SelectedRating
        {
            get { return m_selectedRating + 1; }
            set { m_selectedRating = value - 1; InitPictures(); }
        }
    }
}
