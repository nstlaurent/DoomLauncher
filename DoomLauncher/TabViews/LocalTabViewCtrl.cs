using DoomLauncher.Interfaces;
using System.Drawing;
using System.Linq;

namespace DoomLauncher
{
    public class LocalTabViewCtrl : BasicTabViewCtrl
    {
        private readonly ITagMapLookup m_tagLookup;

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
                ITagData tag = m_tagLookup.GetTags(gameFile).FirstOrDefault(x => x.HasColor && x.Color.HasValue);

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
