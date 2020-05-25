using DoomLauncher.Interfaces;
using System.Drawing;
using System.Linq;

namespace DoomLauncher
{
    public class LocalTabViewCtrl : BasicTabViewCtrl
    {
        private readonly ITagMapLookup m_tagLookup;

        public LocalTabViewCtrl(object key, string title, IGameFileDataSourceAdapter adapter, GameFileFieldType[] selectFields, ITagMapLookup lookup, GameFileViewFactory factory)
            : base(key, title, adapter, selectFields, factory)
        {
            GameFileViewControl.DoomLauncherParent = this;
            m_tagLookup = lookup;

            if (m_tagLookup != null && GameFileViewControl is IGameFileColumnView columnView)
            {
                columnView.CustomRowColorPaint = true;
                columnView.CustomRowPaint += GameFileViewControl_CustomRowPaint;
            }
        }

        void GameFileViewControl_CustomRowPaint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (GameFileViewControl is IGameFileColumnView columnView)
            {
                e.Cancel = false;
                IGameFile gameFile = columnView.CustomRowPaintDataBoundItem;

                if (gameFile != null)
                {
                    ITagData tag = m_tagLookup.GetTags(gameFile).FirstOrDefault(x => x.HasColor && x.Color.HasValue);

                    if (tag != null)
                        columnView.CustomRowPaintForeColor = Color.FromArgb(tag.Color.Value);
                    else
                        columnView.CustomRowPaintForeColor = CDataGridView.DefaultForeColor;
                }
            }
        }
    }
}
