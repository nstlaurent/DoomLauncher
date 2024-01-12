using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace DoomLauncher
{
    public class StatFileScanner
    {
        protected List<string> m_errors = new List<string>();

        public class ParseItem
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
            StatFile = statFile;
        }

        public string StatFile { get; }

        protected void SetStatProperty(StatsData stats, ParseItem item, string value)
        {
            foreach (char c in item.Replace)
                value = value.Replace(c.ToString(), string.Empty);

            PropertyInfo pi = stats.GetType().GetProperty(item.DataSourceProperty);

            if (item.DataSourceProperty == "LevelTime")
            {
                string[] time = value.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (time.Length == 2)
                    pi.SetValue(stats, (Convert.ToSingle(time[0], CultureInfo.InvariantCulture) * 60) + Convert.ToSingle(time[1], CultureInfo.InvariantCulture));
                // This is to support hours being written by ports like woof
                else if (TimeSpan.TryParse(value, AppConfiguration.Culture, out var ts))
                    pi.SetValue(stats, (float)ts.TotalSeconds);
                else
                    m_errors.Add($"Failed to parse [{value}] for [LevelTime]");
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
