using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DoomLauncher
{
    public class StatFileScanner
    {
        private readonly string m_statFile;

        protected List<string> m_errors = new List<string>();

        protected class ParseItem
        {
            public ParseItem(string regexInput, string replace, string dataSourceProperty)
            {
                RegexInput = regexInput;
                Replace = replace;
                DataSourceProperty = dataSourceProperty;
            }

            public string RegexInput { get; set; }
            public string Replace { get; set; }
            public string DataSourceProperty { get; set; }
        }

        public StatFileScanner(string statFile)
        {
            m_statFile = statFile;
        }

        public string StatFile { get { return m_statFile; } }

        protected void SetStatProperty(StatsData stats, ParseItem item, string value)
        {
            foreach (char c in item.Replace)
                value = value.Replace(c.ToString(), string.Empty);

            PropertyInfo pi = stats.GetType().GetProperty(item.DataSourceProperty);

            if (item.DataSourceProperty == "LevelTime") //speical case, need to split out ':' and calculate time
            {
                string[] time = value.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                pi.SetValue(stats, (Convert.ToSingle(time[0]) * 60) + Convert.ToSingle(time[1]));
            }
            else
            {
                try
                {
                    if (pi.PropertyType == typeof(string))
                        pi.SetValue(stats, value);
                    else if (pi.PropertyType == typeof(int))
                        pi.SetValue(stats, Convert.ToInt32(value));
                    else if (pi.PropertyType == typeof(float))
                        pi.SetValue(stats, Convert.ToSingle(value));
                }
                catch
                {
                    m_errors.Add(string.Format("Failed for parse value[{0}] for [{1}]", value, item.DataSourceProperty));
                }
            }
        }
    }
}
