using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class TxtGenerator : Form
    {
        private IDataSourceAdapter m_adapter;

        public TxtGenerator()
        {
            InitializeComponent();

            PopulatePlayInfoCombo(cmbSingle);
            PopulatePlayInfoCombo(cmbCoop);
            PopulatePlayInfoCombo(cmbDeathmatch);
            cmbSingle.SelectedIndex = cmbCoop.SelectedIndex = cmbDeathmatch.SelectedIndex = cmbBase.SelectedIndex = cmbPermission.SelectedIndex =
                 cmbPrimaryPurpose.SelectedIndex = 0;

            dtRelease.Format = DateTimePickerFormat.Custom;
            dtRelease.CustomFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        public void SetData(IDataSourceAdapter adapter)
        {
            SetData(adapter, null);
        }

        public void SetData(IDataSourceAdapter adapter, IGameFile gameFile)
        {
            m_adapter = adapter;

            cmbEngine.ValueMember = "SourcePortID";
            cmbEngine.DisplayMember = "Name";
            List<ISourcePortData> sourcePorts = adapter.GetSourcePorts().ToList();
            SourcePortData noPort = new SourcePortData();
            noPort.SourcePortID = 0;
            noPort.Name = "N/A";
            sourcePorts.Insert(0, noPort);
            cmbEngine.DataSource = sourcePorts;

            cmbGame.ValueMember = "IWadID";
            cmbGame.DisplayMember = "Name";
            cmbGame.DataSource = GetIwads(adapter);
            cmbGame.SelectedIndex = 0;

            if (gameFile != null)
            {
                if (gameFile.SourcePortID.HasValue) cmbEngine.SelectedValue = gameFile.SourcePortID.Value;
                txtTile.Text = gameFile.Title;
                txtFilename.Text = gameFile.FileName;
                if (gameFile.ReleaseDate.HasValue) dtRelease.Value = gameFile.ReleaseDate.Value;
                txtAuthor.Text = gameFile.Author;
                txtDescription.Text = gameFile.Description;
                if (gameFile.MapCount.HasValue) numLevels.Value = gameFile.MapCount.Value;
                if (gameFile.Map != null) txtMaps.Text = GetMapString(gameFile.Map);
                if (gameFile.IWadID.HasValue) cmbGame.SelectedValue = gameFile.IWadID.Value;
            }
        }

        private static IEnumerable<IIWadData> GetIwads(IDataSourceAdapter adapter)
        {
            List<IIWadData> iwads = new List<IIWadData>();

            IWadData data = new IWadData();
            data.IWadID = -1;
            data.Name = "N/A";
            iwads.Add(data);
            iwads.AddRange(adapter.GetIWads());

            return iwads;
        }

        private string CreateTextFile()
        {
            string textFile = @"===========================================================================
Advanced engine needed  : {34}
Primary purpose         : {35}
===========================================================================
Title                   : {0}
Filename                : {1}
Release date            : {2}
Author                  : {3}
Email Address           : {4}
Other Files By Author   : {5}
Misc. Author Info       : {6}

Description             : {7}

Additional Credits to   : {8}
===========================================================================
* What is included *

New levels              : {9}
Sounds                  : {10}
Music                   : {11}
Graphics                : {12}
Dehacked/BEX Patch      : {13}
Demos                   : {14}
Other                   : {15}
Other files required    : {16}


* Play Information *

Game                    : {17}
Map #                   : {18}
Single Player           : {19}
Cooperative 2-4 Player  : {20}
Deathmatch 2-4 Player   : {21}
Other game styles       : {22}
Difficulty Settings     : {23}


* Construction *

Base                    : {24}
Build Time              : {25}
Editor(s) used          : {26}
Known Bugs              : {27}
May Not Run With        : {28}
Tested With             : {29}



* Copyright / Permissions *

Authors {30} use the contents of this file as a base for
modification or reuse.  Permissions have been obtained from original 
authors for any of their resources modified or included in this file.

{31}

* Where to get the file that this text file describes *

The Usual: ftp://archives.3dgamers.com/pub/idgames/ and mirrors
Web sites: {32}
FTP sites: {33}";

            string may = @"You MAY not distribute this file in any format.";
            string maynot = @"You MAY distribute this file, provided you include this text file, with
no modifications.  You may distribute this file in any electronic
format (BBS, Diskette, CD, etc) as long as you include this file 
intact.  I have received permission from the original authors of any
modified or included content in this file to allow further distribution.";

            return string.Format(textFile, txtTile.Text, txtFilename.Text, dtRelease.Value.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern),
                txtAuthor.Text, txtEmail.Text, txtOtherFiles.Text, txtMiscAuthor.Text, txtDescription.Text, txtAdditionalCredits.Text, Convert.ToInt32(numLevels.Value),
                GetIncluded(chkSounds), GetIncluded(chkMusic), GetIncluded(chkGraphics), GetIncluded(chkDehacked), GetIncluded(chkDemos), GetIncluded(chkOther),
                txtOtherFilesRequired.Text, GetGameName(), txtMaps.Text, cmbSingle.SelectedItem.ToString(), cmbCoop.SelectedItem.ToString(),
                cmbDeathmatch.SelectedItem.ToString(), txtOtherGameStyles.Text, GetIncluded(chkDifficulty), GetBase(cmbBase), txtBuildTime.Text, txtEditorsUsed.Text, txtKnownBugs.Text,
                txtMayNotRun.Text, txtTestedWith.Text, cmbPermission.SelectedIndex == 0 ? "MAY" : "may NOT",
                cmbPermission.SelectedIndex == 0 ? may : maynot, txtWebSites.Text, txtFtpSites.Text, 
                ((ISourcePortData)cmbEngine.SelectedItem).Name, cmbPrimaryPurpose.SelectedItem.ToString());
        }

        private string GetGameName()
        {
            IIWadData iwad = cmbGame.SelectedItem as IIWadData;

            string ext = Path.GetExtension(iwad.Name);
            if (!string.IsNullOrEmpty(ext))
                return iwad.Name.Replace(ext, string.Empty);
            else
                return iwad.Name;
        }

        private static string GetMapString(string mapString)
        {
            string[] maps = mapString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (maps.Length == 1)
            {
                return maps.First();
            }
            else if (maps.Length > 1)
            {
                List<Tuple<int, string>> mapLookup = new List<Tuple<int, string>>();

                foreach (string map in maps)
                {
                    int mapDigit = 0;
                    string numberString = new string(map.Where(x => Char.IsDigit(x)).ToArray());
                    if (int.TryParse(numberString, out mapDigit))
                        mapLookup.Add(new Tuple<int, string>(mapDigit, map));
                }

                if (mapLookup.Count > 0)
                {
                    int min = mapLookup.Min(x => x.Item1);
                    int max = mapLookup.Max(x => x.Item1);

                    return string.Format("{0}-{1}", mapLookup.First(x => x.Item1 == min).Item2,
                        mapLookup.First(x => x.Item1 == max).Item2);
                }
            }

            return string.Empty;
        }

        private static string GetBase(ComboBox cmbBase)
        {
            if (cmbBase.SelectedIndex == 0)
                return cmbBase.SelectedItem.ToString();

            return string.Format("{0} (-)", cmbBase.SelectedItem.ToString());
        }

        private static string GetIncluded(CheckBox chk)
        {
            return chk.Checked ? "Yes" : "No";
        }

        private static void PopulatePlayInfoCombo(ComboBox cmb)
        {
            cmb.Items.Add("Designed for");
            cmb.Items.Add("Player starts only");
            cmb.Items.Add("No");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cmbGame.SelectedItem == null)
            {
                MessageBox.Show(this, "A game must be selected.", "Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string textFile = CreateTextFile();
            string filename = "outgen.txt";
            
            try
            {
                File.WriteAllText(filename, textFile);
                Process.Start(filename);
            }
            catch(Exception ex)
            {
                Util.DisplayUnexpectedException(this, ex);
            }
        }
    }
}
