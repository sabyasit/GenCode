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
        public static string PreviewHTML(Request request)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<int, int[]> gridRow = new Dictionary<int, int[]>()
            {
                {1, new int[] {12} },
                {2, new int[] {6,6} },
                {3, new int[] {4,4,4} },
                {4, new int[] {3,3,3,3} }
            };

            sb.AppendLine("import logo from './logo.svg';");
            sb.AppendLine("import Grid from '@material-ui/core/Grid';");
            sb.AppendLine("import { makeStyles } from '@material-ui/core/styles';");
            sb.AppendLine("import TextField from '@material-ui/core/TextField';");
            sb.AppendLine("import Select from '@material-ui/core/Select';");
            sb.AppendLine("import MenuItem from '@material-ui/core/MenuItem';");
            sb.AppendLine("import InputLabel from '@material-ui/core/InputLabel';");
            sb.AppendLine("import FormHelperText from '@material-ui/core/FormHelperText';");
            sb.AppendLine("import './App.css';");

            sb.AppendLine("const useStyles = makeStyles((theme) => ({ root: { flexGrow: 1, padding: 10 }}));");

            sb.AppendLine("function App() {");
            sb.AppendLine("const classes = useStyles();");
            sb.AppendLine("return (");
            sb.AppendLine("<form className={classes.root} noValidate autoComplete='off'>");
            sb.AppendLine("<Grid container spacing={3}>");

            foreach (var design in request.Design)
            {
                var rows = gridRow[design.Count];
                for (int i = 0; i < design.Count; i++)
                {
                    sb.AppendLine("<Grid item xs={"+ rows[i].ToString() + "}>");
                    switch (design[i].Control)
                    {
                        case "1":
                            {
                                sb.AppendFormat("<TextField label='{0}' helperText='{1}' {2} fullWidth />", design[i].Level, design[i].Description, design[i].Required ? "required" : "");
                                break;
                            }
                        case "2":
                            {
                                sb.AppendFormat("<TextField label='{0}' type='number' helperText='{1}' {2} fullWidth />", design[i].Level, design[i].Description, design[i].Required ? "required" : "");
                                break;
                            }
                        case "3":
                            {
                                sb.AppendFormat("<TextField label='{0}' type='email' helperText='{1}' {2} fullWidth />", design[i].Level, design[i].Description, design[i].Required ? "required" : "");
                                break;
                            }
                        case "4":
                            {
                                sb.AppendFormat("<TextField label='{0}' type='date' helperText='{1}' {2} fullWidth />", design[i].Level, design[i].Description, design[i].Required ? "required" : "");
                                break;
                            }
                        case "5":
                            {
                                sb.AppendFormat("<TextField label='{0}' type='file' helperText='{1}' {2} fullWidth />", design[i].Level, design[i].Description, design[i].Required ? "required" : "");
                                break;
                            }
                        case "6":
                            {
                                sb.AppendFormat("<InputLabel id='demo-select'>{0}</InputLabel>", design[i].Level);
                                sb.AppendFormat("<Select labelId = 'demo-select' {0} fullWidth>", design[i].Required ? "required" : "");
                                foreach (var value in design[i].Values)
                                {
                                    sb.AppendFormat("<MenuItem value={{0}}>{0}</MenuItem>", value);
                                }
                                sb.AppendLine("</Select>");
                                sb.AppendFormat("<FormHelperText>{1}</FormHelperText>", design[i].Description);
                                break;
                            }
                    }
                    sb.AppendLine("</Grid>");
                }
            }
            sb.AppendLine("</Grid>");
            sb.AppendLine("</form>");
            sb.AppendLine(");");
            sb.AppendLine("}");

            sb.AppendLine("export default App;");

            string html = sb.ToString();

            File.WriteAllText(@"C:\Users\462676\Desktop\codeGen\my-app\src\App.js", html);

            var initResult = RunCommand("npm run build", @"C:\Users\462676\Desktop\codeGen\my-app");

            var dir = new DirectoryInfo(@"C:\Users\462676\Desktop\codeGen\my-app\build\static\js\");
            FileInfo[] files = dir.GetFiles();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(@"C:\Users\462676\Desktop\codeGen\my-app\build\index.html");
            var scriptsTag = doc.DocumentNode.Descendants().Where(x => x.Name == "script").ToList();
            foreach (var script in scriptsTag)
            {
                if (script.Attributes["src"] != null)
                {
                    var scriptName = script.Attributes["src"].Value.Split('/').Last();
                    foreach(var file in files)
                    {
                        if (file.Name == scriptName)
                        {
                            string data = File.ReadAllText(file.FullName);
                            script.Attributes.RemoveAll();
                            script.AppendChild(doc.CreateComment("\n" + data + "\n"));
                        }
                    }
                }
            }

            //dir = new DirectoryInfo(@"C:\Users\462676\Desktop\codeGen\my-app\build\static\css\");
            //files = dir.GetFiles();

            //var styleTag = doc.DocumentNode.Descendants().Where(x => x.Name == "link").ToList();
            //foreach(var style in styleTag)
            //{
            //    if (style.Attributes["rel"] != null)
            //    {
            //        var styleName = style.Attributes["href"].Value.Split('/').Last();
            //    }
            //}
            return doc.DocumentNode.OuterHtml;
        }

        private static string RunCommand(string commandToRun, string workingDirectory)
        {
            var processStartInfo = new ProcessStartInfo()
            {
                FileName = "cmd",
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false
            };

            var process = Process.Start(processStartInfo);

            if (process == null)
            {
                throw new Exception("Process should not be null.");
            }

            process.StandardInput.WriteLine($"{commandToRun} & exit");
            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();
            return output;
        }
    }
}