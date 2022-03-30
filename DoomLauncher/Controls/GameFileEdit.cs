using DoomLauncher.Controls;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class GameFileEdit : UserControl
    {
        public IGameFile DataSource { get; private set; }
        public ITagData[] TagData { get; private set; }
        public bool TagsChanged { get; private set; }

        private bool m_showCheckBoxes;
        private string m_maps;

        public GameFileEdit()
        {
            InitializeComponent();
            chkTags.Visible = false;
            txtComments.WarnLinkClick = false;
            lblFile.IsPath = true;

            Stylizer.StylizeControl(this, DesignMode);
        }

        public void SetShowCheckBoxes(bool set)
        {
            chkAuthor.Visible = chkDescription.Visible = chkRating.Visible =
                chkReleaseDate.Visible = chkTitle.Visible = chkComments.Visible = 
                chkMaps.Visible = set;
            m_showCheckBoxes = set;
        }

        public void SetShowTagCheckBox(bool set)
        {
            chkTags.Visible = set;
        }

        public void SetShowMaps(bool set)
        {
            lnkMapsEdit.Visible = set;
            chkMaps.Visible = set;
        }

        public void SetCheckBoxesChecked(bool set)
        {
            chkAuthor.Checked = chkDescription.Checked = chkRating.Checked =
                chkReleaseDate.Checked = chkTitle.Checked = chkComments.Checked = 
                chkMaps.Checked = set;
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

        public bool TagsChecked
        {
            get { return chkTags.Checked; }
            set { chkTags.Checked = value; }
        }

        public bool MapsChecked
        {
            get { return chkMaps.Checked; }
            set { chkMaps.Checked = value; }
        }

        public void SetDataSource(IGameFile gameFile, IEnumerable<ITagData> tags)
        {
            DataSource = gameFile;
            TagData = tags.ToArray();

            lblFile.Text = gameFile.FileName;

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
            SetTagsLabel();
            m_maps = DataSource.Map;
        }

        private void SetTagsLabel()
        {
            if (TagData.Length > 0)
                lblTags.Text = string.Join(", ", TagData.Select(x => x.Name).ToArray());
            else
                lblTags.Text = "Select...";
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
            if (AssertSet(chkMaps, fields, GameFileFieldType.Map))
            {
                fields.Add(GameFileFieldType.MapCount);
                gameFile.Map = m_maps;
                gameFile.MapCount = DataSources.GameFile.GetMaps(gameFile).Length;
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

        private void lnkMapsEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TextBoxForm textBoxForm = new TextBoxForm(true, MessageBoxButtons.OKCancel)
            {
                Text = "Maps",
                HeaderText = "Enter map names, separated by commas.",
                DisplayText = m_maps,
                StartPosition = FormStartPosition.CenterScreen
            };

            if (textBoxForm.ShowDialog(this) == DialogResult.OK)
                m_maps = textBoxForm.DisplayText;
        }

        private void lblTags_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TagSelectForm form = new TagSelectForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.TagSelectControl.Init(new TagSelectOptions() { ShowCheckBoxes = true });
            form.TagSelectControl.SetCheckedTags(TagData);

            if (form.ShowDialog() == DialogResult.OK)
            {
                TagsChanged = true;
                TagData = form.TagSelectControl.GetCheckedTags().ToArray();
                SetTagsLabel();
            }
        }
    }
}
