using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LPCloudCore.Helpers
{
    public static class LPLinq
    {
        public static bool Contains(this string str, IEnumerable<string> values)
        {
            return str.Contains(values.ToArray());
        }

        public static bool Contains(this string str, params string[] values)
        {
            foreach (var v in values)
            {
                if (!str.Contains(v))
                    return false;
            }
            return true;
        }
    }
}
