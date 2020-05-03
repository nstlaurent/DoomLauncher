using DoomLauncher.Interfaces;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DoomLauncher
{
    class ToolTipHandler
    {
        public ToolTipHandler() {}

        public string GetToolTipText(Font font, IGameFile item)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(item.FileName))
            {
                sb.Append("File: ");
                sb.Append(item.FileName);

                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
            }

            sb.Append("Title: ");
            if (item.Title != null)
                sb.Append(item.Title);

            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("Author: ");
            if (item.Author != null)
                sb.Append(item.Author);

            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("Release Date: ");
            if (item.ReleaseDate.HasValue)
                sb.Append(item.ReleaseDate.Value.ToShortDateString());
            else
                sb.Append("N/A");

            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            int maxWidth = 800, maxLines = 26, lineCount = 0;

            try
            {
                sb.Append("Description: ");
                sb.Append(FormatDescritionToWidth(font, item.Description, maxWidth, maxLines, out lineCount));
                maxLines -= lineCount;
            }
            catch
            {
                sb.Append(item.Description);
            }

            if (!string.IsNullOrEmpty(item.Comments) && maxLines > 0)
            {
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);

                try
                {
                    sb.Append("Comments: ");
                    sb.Append(FormatDescritionToWidth(font, item.Comments, maxWidth, maxLines, out lineCount));
                }
                catch
                {
                    sb.Append(item.Comments);
                }
            }

            return sb.ToString();
        }

        private string FormatDescritionToWidth(Font font, string description, int maxWidth, int maxLines, out int lineCount)
        {
            string[] lines = description.Split(new char[] { '\n' });
            StringBuilder sb = new StringBuilder();
            lineCount = 0;

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    lineCount++;
                    sb.Append(Environment.NewLine);
                    continue;
                }

                string formatLine = line;
                int width = 0;
                
                if (!string.IsNullOrEmpty(formatLine))
                    width = TextRenderer.MeasureText(formatLine, font).Width;

                while (width > maxWidth)
                {
                    string sub = TruncateLine(font, formatLine, maxWidth);
                    sb.Append(sub.TrimStart());
                    sb.Append(Environment.NewLine);
                    if (++lineCount > maxLines) break;
                    formatLine = formatLine.Replace(sub, string.Empty);
                    width = TextRenderer.MeasureText(formatLine, font).Width;
                }

                if (lineCount < maxLines)
                {
                    sb.Append(formatLine.TrimStart());
                    sb.Append(Environment.NewLine);
                }

                if (++lineCount > maxLines) break;
            }

            string ret = sb.Replace("\r", string.Empty).ToString();

            if (ret.Length > 1)
            {
                while (ret.Contains("\n\n\n"))
                    ret = ret.Replace("\n\n\n", "\n\n");
                while (ret.Length > 1 && ret[ret.Length - 1] == '\n')
                    ret = ret.Substring(0, ret.Length - 1);
            }
            
            return ret;
        }

        private string TruncateLine(Font font, string line, int maxWidth)
        {
            for(int i = 1; i < line.Length; i++)
            {
                int test = TextRenderer.MeasureText(line.Substring(0, i), font).Width;

                if (test > maxWidth)
                {
                    int start = i;
                    while (i > 0 && line[i] != ' ') i--;
                    if (i == 0) return line.Substring(0, i);
                    return line.Substring(0, i);
                }
            }

            return line;
        }
    }
}
