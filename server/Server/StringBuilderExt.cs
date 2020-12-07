using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Server
{
    public static class StringBuilderExt
    {
        public static StringBuilder AppendLineFormat(this StringBuilder sb, string format, params object[] args)
        {
            sb.AppendFormat(format, args);
            sb.AppendLine();
            return sb;
        }
    }
}