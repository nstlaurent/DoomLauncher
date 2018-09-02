using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoomLauncher.Interfaces;

namespace DoomLauncher
{
    public partial class GameFileEdit : UserControl
    {
        private bool m_showCheckBoxes;

        public GameFileEdit()
        {
            InitializeComponent();
        }

        public void SetShowCheckBoxes(bool set)
        {
            chkAuthor.Visible = chkDescription.Visible = chkRating.Visible =
                chkReleaseDate.Visible = chkTitle.Visible = chkComments.Visible = set;
            m_showCheckBoxes = true;
        }

        public void SetCheckBoxesChecked(bool set)
        {
            chkAuthor.Checked = chkDescription.Checked = chkRating.Checked =
                chkReleaseDate.Checked = chkTitle.Checked = chkComments.Checked = set;
        }

        public bool AuthorChecked
        {
            get { return chkAuthor.Checked; }
            set { chkAuthor.Checked = value; }
        }

        public bool DescriptionChecked
        {
            get { return chkDescription.Checked; }
            set { chkDescription.Checked = value; }
        }

        public bool RatingChecked
        {
            get { return chkRating.Checked; }
            set { chkRating.Checked = value; }
        }

        public bool ReleaseDateChecked
        {
            get { return chkReleaseDate.Checked; }
            set { chkReleaseDate.Checked = value; }
        }

        public bool TitleChecked
        {
            get { return chkTitle.Checked; }
            set { chkTitle.Checked = value; }
        }

        public bool CommentsChecked
        {
            get { return chkComments.Checked; }
            set { chkComments.Checked = value; }
        }

        public void SetDataSource(IGameFile gameFile, IEnumerable<ITagData> tags)
        {
            if (!string.IsNullOrEmpty(gameFile.Title)) txtTitle.Text = gameFile.Title;
            else txtTitle.Text = string.Empty;
            if (!string.IsNullOrEmpty(gameFile.Author)) txtAuthor.Text = gameFile.Author;
            else txtAuthor.Text = string.Empty;
            if (!string.IsNullOrEmpty(gameFile.Description)) txtDescription.Text = Util.CleanDescription(gameFile.Description);
            else txtDescription.Text = string.Empty;
            if (!string.IsNullOrEmpty(gameFile.Comments)) txtComments.Text = gameFile.Comments;
            else txtComments.Text = string.Empty;
            if (gameFile.ReleaseDate.HasValue) dtRelease.Value = gameFile.ReleaseDate.Value;
            else dtRelease.Value = DateTime.Now;
            if (gameFile.Rating.HasValue) ctrlStarRating.SelectedRating = Convert.ToInt32(gameFile.Rating.Value);
            else ctrlStarRating.SelectedRating = 0;

            dtRelease.Checked = gameFile.ReleaseDate.HasValue;

            lblTags.Text = string.Join(", ", tags.Select(x => x.Name).ToArray());
        }

        public List<GameFileFieldType> UpdateDataSource(IGameFile gameFile)
        {
            List<GameFileFieldType> fields = new List<GameFileFieldType>();

            if (AssertSet(chkTitle, fields, GameFileFieldType.Title)) gameFile.Title = txtTitle.Text;
            if (AssertSet(chkAuthor, fields, GameFileFieldType.Author)) gameFile.Author = txtAuthor.Text;
            if (AssertSet(chkDescription, fields, GameFileFieldType.Description)) gameFile.Description = txtDescription.Text;
            if (AssertSet(chkComments, fields, GameFileFieldType.Comments)) gameFile.Comments = txtComments.Text;
            if (AssertSet(chkReleaseDate, fields, GameFileFieldType.ReleaseDate))
            {
                if (dtRelease.Checked) gameFile.ReleaseDate = new DateTime(dtRelease.Value.Year, dtRelease.Value.Month, dtRelease.Value.Day);
                else gameFile.ReleaseDate = null;
            }
            if (AssertSet(chkRating, fields, GameFileFieldType.Rating))
            {
                if (ctrlStarRating.SelectedRating == 0)
                    gameFile.Rating = null;
                else
                    gameFile.Rating = ctrlStarRating.SelectedRating;
            }

            return fields;
        }

        private bool AssertSet(CheckBox chk, List<GameFileFieldType> fields, GameFileFieldType field)
        {
            if ((m_showCheckBoxes && chk.Checked) || !m_showCheckBoxes)
            {
                fields.Add(field);
                return true;
            }

            return false;
        }
    }
}
