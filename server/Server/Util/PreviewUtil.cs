using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Server.Models;
using System.IO;
using System.Diagnostics;

namespace Server.Util
{
    public class PreviewUtil
    {
        public static void Preview(Request request, string page, string path)
        {
            string code = GenerateUtil.GenerateReactCode(request, page);
            File.WriteAllText(path + "\\pages\\" + page + ".js", code);
        }

        public static string GeneratePreview(string path)
        {
            string page = "APP" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
            using (FileStream fs = new FileStream(path + "\\pages\\" + page + ".js", FileMode.CreateNew))
            {
                using(StreamWriter sr = new StreamWriter(fs))
                {
                    sr.Write("function " + page + "() { return <h1>No preview</h1>; } export default " + page + ";");
                }
            }
            string appJs = File.ReadAllText(path + "\\App.js");
            appJs = appJs.Replace("//import", string.Format("import {0} from './pages/{0}'\n //import", page));
            appJs = appJs.Replace("{/* Route */}", "<Route path='/" + page + "' component={" + page + "} />\n {/* Route */}");

            File.WriteAllText(path + "\\App.js", appJs); 
            return page;
        }
    }
}