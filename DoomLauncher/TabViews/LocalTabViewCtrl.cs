using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    public class LocalTabViewCtrl : BasicTabViewCtrl
    {
        private ITagMapLookup m_tagLookup;

        public LocalTabViewCtrl(object key, string title, IGameFileDataSourceAdapter adapter, GameFileFieldType[] selectFields, ITagMapLookup lookup)
            : base(key, title, adapter, selectFields)
        {
            GameFileViewControl.DoomLauncherParent = this;
            m_tagLookup = lookup;

            if (m_tagLookup != null)
            {
                GameFileViewControl.CustomRowColorPaint = true;
                GameFileViewControl.CustomRowPaint += GameFileViewControl_CustomRowPaint;
            }
        }

        void GameFileViewControl_CustomRowPaint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            IGameFile gameFile = FromDataBoundItem(GameFileViewControl.CustomRowPaintDataBoundItem);

            if (gameFile != null)
            {
                ITagData tag = m_tagLookup.GetTags(gameFile).Where(x => x.HasColor && x.Color.HasValue).FirstOrDefault();

                if (tag != null)
                {
                    GameFileViewControl.CustomRowPaintForeColor = Color.FromArgb(tag.Color.Value);
                }
                else
                {
                    GameFileViewControl.CustomRowPaintForeColor = CDataGridView.DefaultForeColor;
                }
            }
        }
    }
}
