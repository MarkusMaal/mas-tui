using System;
using System.Collections.Generic;
using System.Text;

namespace MasTUICommon
{
    public abstract class Utils
    {
        public static string WrapLines(string content, int constraint)
        {
            StringBuilder sb = new();
            foreach (var l in content.Split('\n'))
            {
                var debt = 0;
                var words = new List<string>();
                foreach (var w in l.Split(' '))
                {
                    if (debt + w.Length > constraint)
                    {
                        debt = 0;
                        sb.AppendLine(string.Join(' ', words));
                        words.Clear();
                    }
                    words.Add(w);
                    debt += w.Length + 1;
                }
                sb.AppendLine(string.Join(' ', words));
                debt = 0;
            }
            return sb.ToString();
        }
    }
}
