using Server.Models;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Ionic.Zip;

namespace Server.Util
{
    public class GenerateUtil
    {
        private static Dictionary<int, int[]> gridRow = new Dictionary<int, int[]>()
        {
            {1, new int[] {12} },
            {2, new int[] {6,6} },
            {3, new int[] {4,4,4} },
            {4, new int[] {3,3,3,3} }
        };

        public static string ExportReactCode(Request request, string page, string path)
        {
            string code = GenerateReactCode(request, page);

            //File.WriteAllText(@"C:\Users\462676\Desktop\codeGen\my-app\src\App.js", code);

            string guid = Guid.NewGuid().ToString();
            using (var zip = ZipFile.Read(path + "\\dummy.zip"))
            {
                zip.ExtractAll(path + "\\" + guid);
            }
            File.WriteAllText(path + "\\" + guid + "\\dummy\\src\\app.js", code);
            using (var zip = new ZipFile())
            {
                zip.AddDirectory(path + "\\" + guid + "\\dummy");
                zip.Save(path + "\\" + guid + "\\" + request.Project + ".zip");
            }

            return guid;
        }

        public static string GenerateReactCode(Request request, string page)
        {
            List<ReactCode> codeGens = new List<ReactCode>();

            foreach (var design in request.Design)
            {
                var rows = gridRow[design.Count];
                for (int i = 0; i < design.Count; i++)
                {
                    if (design[i].Type != "array")
                    {
                        switch (design[i].Control)
                        {
                            case "1":
                                {
                                    codeGens.Add(DrawTextBox(design[i], "text", rows[i]));
                                    break;
                                }
                            case "2":
                                {
                                    codeGens.Add(DrawTextBox(design[i], "number", rows[i]));
                                    break;
                                }
                            case "3":
                                {
                                    codeGens.Add(DrawTextBox(design[i], "email", rows[i]));
                                    break;
                                }
                            case "4":
                                {
                                    codeGens.Add(DrawTextBox(design[i], "date", rows[i]));
                                    break;
                                }
                            case "5":
                                {
                                    //TODO File
                                    break;
                                }
                            case "6":
                                {
                                    codeGens.Add(DrawSelect(design[i], rows[i]));
                                    break;
                                }
                        }
                    }
                    else
                    {
                        if (design[i].Items.Count == 0)
                        {
                            if (design[i].Values.Count == 0)
                                codeGens.Add(DrawArray(design[i], rows[i]));
                            else
                                codeGens.Add(DrawSelect(design[i], rows[i]));

                        }
                        else
                        {
                            codeGens.Add(DrawArrayObject(design[i], rows[i]));
                        }
                    }
                }
            }

            codeGens.Add(DrawSubmitModel(request, codeGens));

            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("import logo from './logo.svg';");
            sb.AppendLine("import React, {useCallback, useState} from 'react'");
            sb.AppendLine("import Grid from '@material-ui/core/Grid';");
            sb.AppendLine("import { makeStyles } from '@material-ui/core/styles';");
            sb.AppendLine("import TextField from '@material-ui/core/TextField';");
            sb.AppendLine("import Select from '@material-ui/core/Select';");
            sb.AppendLine("import MenuItem from '@material-ui/core/MenuItem';");
            sb.AppendLine("import InputLabel from '@material-ui/core/InputLabel';");
            sb.AppendLine("import FormHelperText from '@material-ui/core/FormHelperText';");
            sb.AppendLine("import Button from '@material-ui/core/Button';");
            sb.AppendLine("import Table from '@material-ui/core/Table';");
            sb.AppendLine("import TableBody from '@material-ui/core/TableBody';");
            sb.AppendLine("import TableCell from '@material-ui/core/TableCell';");
            sb.AppendLine("import TableContainer from '@material-ui/core/TableContainer';");
            sb.AppendLine("import TableHead from '@material-ui/core/TableHead';");
            sb.AppendLine("import TableRow from '@material-ui/core/TableRow';");
            sb.AppendLine("import Paper from '@material-ui/core/Paper';");
            //sb.AppendLine("import './App.css';");

            sb.AppendLine("const useStyles = makeStyles((theme) => ({ root: { flexGrow: 1, padding: 10 }, rightMargin: { marginRight: 100 }, rightFloat: { float: 'right', top: -40 }, rightMarginPos: { paddingRight: 100, position: 'relative' }, rightFloatPos: { position: 'absolute', right: 0, top: 24 }, container: {  maxHeight: 440 } }));");

            sb.AppendLine("function " + page + "() {");
            sb.AppendLine("const classes = useStyles();");

            foreach (var gen in codeGens)
            {
                foreach (var state in gen.States)
                {
                    sb.AppendLine(state.Value);
                }
            }

            foreach (var gen in codeGens)
            {
                foreach(var func in gen.Function)
                {
                    sb.AppendLine(func);
                }
            }

            sb.AppendLine("return (");
            sb.AppendLine("<div>");
            sb.AppendLine("<form className={classes.root} autoComplete='off' onSubmit={onSubmit}>");
            sb.AppendLine("<h1>" + request.Header + "</h1>");
            sb.AppendLine("<Grid container spacing={3}>");

            foreach(var gen in codeGens)
            {
                sb.AppendLine(gen.Html);
            }

            sb.AppendLine("");

            sb.AppendLine("</Grid>");
            sb.AppendLine("</form>");
            // add response
            sb.AppendLine("<div style={{ padding: '10px' }}>");
            sb.AppendLine(GenerateResponse(request));
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine(");");
            sb.AppendLine("}");

            sb.AppendLineFormat("export default {0};", page);

            return sb.ToString();
        }

        private static ReactCode DrawTextBox(RequestDesign design, string type, int grid)
        {
            ReactCode codeGen = new ReactCode();

            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendLine("<Grid item xs={" + grid.ToString() + "}>");
            sbHtml.AppendLineFormat("<TextField label='{0}' helperText='{1}' type='{2}' {3} {4} fullWidth />",
                design.Level, design.Description, type, design.Required ? "required" : "",
                "onInput={ e=>Set_" + design.ObjectName + "(e.target.value)}");
            sbHtml.AppendLine("</Grid>");
            codeGen.Html = sbHtml.ToString();

            codeGen.States.Add(design.ObjectName, string.Format("const [{0}, {1}] = useState('{2}')", design.ObjectName, "Set_" + design.ObjectName, design.Value));

            codeGen.Imports.Add("import TextField from '@material-ui/core/TextField';");

            return codeGen;
        }

        private static ReactCode DrawArrayObject(RequestDesign design, int grid)
        {
            ReactCode codeGen = new ReactCode();

            var row = gridRow[design.Items.Count];
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendLine("<Grid item xs={" + grid.ToString() + "}>");
            sbHtml.AppendLine("<div className={classes.rightMarginPos}>");
            sbHtml.AppendLine("<Grid container spacing={3}>");
            for (int i = 0; i < design.Items.Count; i++)
            {
                sbHtml.AppendLineFormat("<Grid item xs={0}>", "{" + row[i] + "}");
                sbHtml.AppendLineFormat("<TextField label='{0}' helperText='{1}' type='text' {2} {3} fullWidth />",
                    design.Items[i].Level, design.Items[i].Description, design.Items[i].Required ? "required" : "",
                    "onInput={ e=>Set_" + design.ObjectName + "(e.target.value, '" + design.Items[i].Name + "', 0)}");
                sbHtml.AppendLine("</Grid>");
            }
            sbHtml.AppendLine("</Grid>");
            sbHtml.AppendLineFormat("<Button variant='contained' className={0} onClick={1}>Add</Button>", "{classes.rightFloatPos}", "{Add_" + design.ObjectName + "}");
            sbHtml.AppendLine("</div>");

            sbHtml.AppendLine("{" + design.ObjectName + ".map((value, index) => {");
            sbHtml.AppendLine("if (index > 0) {");
            sbHtml.AppendLine("return (");
            sbHtml.AppendLine("<div key={`tag-${index}`} className={classes.rightMarginPos}>");
            sbHtml.AppendLine("<Grid container spacing={3}>");
            for (int i = 0; i < design.Items.Count; i++)
            {
                sbHtml.AppendLineFormat("<Grid item xs={0}>", "{" + row[i] + "}");
                sbHtml.AppendLineFormat("<TextField label='{0}' helperText='{1}' type='text' {2} {3} fullWidth />",
                    design.Items[i].Level, design.Items[i].Description, design.Items[i].Required ? "required" : "",
                    "onInput={ e=>Set_" + design.ObjectName + "(e.target.value, '" + design.Items[i].Name + "', index)}");
                sbHtml.AppendLine("</Grid>");
            }
            sbHtml.AppendLine("</Grid>");
            sbHtml.AppendLineFormat("<Button variant='contained' className={0} onClick={1}>Remove</Button>", "{classes.rightFloatPos}", "{e=> Remove_" + design.ObjectName + "(index)}");
            sbHtml.AppendLine("</div>");
            sbHtml.AppendLine(")");
            sbHtml.AppendLine("}");
            sbHtml.AppendLine("})}");

            sbHtml.AppendLine("</Grid>");
            codeGen.Html = sbHtml.ToString();

            codeGen.States.Add(design.ObjectName, string.Format("const [{0}, {1}] = useState([{2}])", design.ObjectName, "Set_Array_" + design.ObjectName, "{}"));

            StringBuilder sbFunc = new StringBuilder();
            //Set Array value function
            sbFunc.AppendLineFormat("function Set_{0}(val, prop, index)", design.ObjectName);
            sbFunc.AppendLine("{");
            sbFunc.AppendLineFormat("{0}[index][prop]=val;", design.ObjectName);
            sbFunc.AppendLineFormat("Set_Array_{0}([...{0}]);", design.ObjectName);
            sbFunc.AppendLine("}");

            // Add Array
            sbFunc.AppendLineFormat("function Add_{0}()", design.ObjectName);
            sbFunc.AppendLine("{");
            sbFunc.AppendLineFormat("Set_Array_{0}([...{0}, {1}]);", design.ObjectName, "{}");
            sbFunc.AppendLine("}");

            //Remove Array
            sbFunc.AppendLineFormat("function Remove_{0}(index)", design.ObjectName);
            sbFunc.AppendLine("{");
            sbFunc.AppendLineFormat("{0}.splice(index,1);", design.ObjectName);
            sbFunc.AppendLineFormat("Set_Array_{0}([...{0}]);", design.ObjectName);
            sbFunc.AppendLine("}");
            codeGen.Function.Add(sbFunc.ToString());


            return codeGen;
        }

        private static ReactCode DrawArray(RequestDesign design, int grid)
        {
            ReactCode codeGen = new ReactCode();

            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendLine("<Grid item xs={" + grid.ToString() + "}>");
            sbHtml.AppendLine("<Grid item xs={12}>");
            sbHtml.AppendLine("<Grid item xs={12} className={classes.rightMargin}>");
            sbHtml.AppendLineFormat("<TextField label='{0}' helperText='{1}' type='text' {2} {3} fullWidth />",
                design.Level, design.Description, design.Required ? "required" : "",
                "onInput={ e=>Set_" + design.ObjectName + "(e.target.value, 0)}");
            sbHtml.AppendLine("</Grid>");
            sbHtml.AppendLineFormat("<Button variant='contained' className={0} onClick={1}>Add</Button>", "{classes.rightFloat}", "{Add_" + design.ObjectName + "}");
            sbHtml.AppendLine("</Grid>");

            sbHtml.AppendLine("{"+ design.ObjectName + ".map((value, index) => {");
            sbHtml.AppendLine("if (index > 0) {");
            sbHtml.AppendLine("return (");
            sbHtml.AppendLine("<Grid key={`tag-${index}`} item xs={12}>");
            sbHtml.AppendLine("<Grid item xs={12} className={classes.rightMargin}>");
            sbHtml.AppendLineFormat("<TextField label='{0}' type='text' {1} fullWidth />", design.Level, "onInput={ e=>Set_" + design.ObjectName + "(e.target.value, index)}");
            sbHtml.AppendLine("</Grid>");
            sbHtml.AppendLineFormat("<Button variant='contained' className={0} onClick={1}>Remove</Button>", "{classes.rightFloat}", "{e=> Remove_" + design.ObjectName + "(index)}");
            sbHtml.AppendLine("</Grid>");
            sbHtml.AppendLine(")");
            sbHtml.AppendLine("}");
            sbHtml.AppendLine("})}");

            sbHtml.AppendLine("</Grid>");
            codeGen.Html = sbHtml.ToString();

            codeGen.States.Add(design.ObjectName, string.Format("const [{0}, {1}] = useState(['{2}'])", design.ObjectName, "Set_Array_" + design.ObjectName, design.Value));

            StringBuilder sbFunc = new StringBuilder();
            //Set Array value function
            sbFunc.AppendLineFormat("function Set_{0}(val, index)", design.ObjectName);
            sbFunc.AppendLine("{");
            sbFunc.AppendLineFormat("{0}[index]=val;", design.ObjectName);
            sbFunc.AppendLineFormat("Set_Array_{0}([...{0}]);", design.ObjectName);
            sbFunc.AppendLine("}");

            // Add Array
            sbFunc.AppendLineFormat("function Add_{0}()", design.ObjectName);
            sbFunc.AppendLine("{");
            sbFunc.AppendLineFormat("Set_Array_{0}([...{0}, '']);", design.ObjectName);
            sbFunc.AppendLine("}");

            //Remove Array
            sbFunc.AppendLineFormat("function Remove_{0}(index)", design.ObjectName);
            sbFunc.AppendLine("{");
            sbFunc.AppendLineFormat("{0}.splice(index,1);", design.ObjectName);
            sbFunc.AppendLineFormat("Set_Array_{0}([...{0}]);", design.ObjectName);
            sbFunc.AppendLine("}");
            codeGen.Function.Add(sbFunc.ToString());

            codeGen.Imports.Add("import TextField from '@material-ui/core/TextField';");
            codeGen.Imports.Add("import Button from '@material-ui/core/Button';");

            return codeGen;
        }

        private static ReactCode DrawSelect(RequestDesign design, int grid)
        {
            ReactCode codeGen = new ReactCode();

            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendLine("<Grid item xs={" + grid.ToString() + "}>");
            sbHtml.AppendLineFormat("<InputLabel id='{0}'>{1}</InputLabel>", design.ObjectName, design.Level);
            sbHtml.AppendLineFormat("<Select labelId = '{0}' {1} fullWidth {2}>", design.ObjectName, design.Required ? "required" : "",
                "onChange={ e=>Set_" + design.ObjectName + "(e.target.value)}");
            foreach (var value in design.Values)
            {
                sbHtml.AppendLineFormat("<MenuItem value={0}>{1}</MenuItem>", "{'" + value + "'}", value);
            }
            sbHtml.AppendLine("</Select>");
            sbHtml.AppendLineFormat("<FormHelperText>{0}</FormHelperText>", design.Description);
            sbHtml.AppendLine("</Grid>");
            codeGen.Html = sbHtml.ToString();

            codeGen.States.Add(design.ObjectName, string.Format("const [{0}, {1}] = useState('{2}')", design.ObjectName, "Set_" + design.ObjectName, design.Value));

            codeGen.Imports.Add("import Select from '@material-ui/core/Select';");
            codeGen.Imports.Add("import MenuItem from '@material-ui/core/MenuItem';");
            codeGen.Imports.Add("import InputLabel from '@material-ui/core/InputLabel';");
            codeGen.Imports.Add("import FormHelperText from '@material-ui/core/FormHelperText';");

            return codeGen;
        }

        private static ReactCode DrawSubmitModel(Request request, List<ReactCode> gens)
        {
            ReactCode codeGen = new ReactCode();

            Dictionary<string, string> states = new Dictionary<string, string>();
            string postModel = GeneratePostModel(request.Operation, ref states);

            string url = "'" + request.Server + request.Operation.Name + "'";
            foreach (var param in request.Operation.Params)
            {
                url = url.Replace("{" + param.Name + "}", "'+" + param.In + "_" + param.Name + "+'");
                states.Add(param.In + "_" + param.Name, param.Type);
            }
            url = url.Replace("+''", "");
            foreach (var param in request.Operation.Params)
            {
                if (param.In == "query")
                {
                    if (!url.Contains("?"))
                    {
                        url = url + string.Format("+'?{0}='+query_{0}", param.Name);
                    }
                    else
                    {
                        url = url + string.Format("+'&{0}='+query_{0}", param.Name);
                    }
                }
            }

            var usedState = new List<string>();
            foreach (var gen in gens)
            {
                foreach (var state in gen.States)
                {
                    usedState.Add(state.Key);
                }
            }

            foreach (var state in states)
            {
                if (!usedState.Contains(state.Key))
                {
                    codeGen.States.Add(state.Key, string.Format("const [{0}, {1}] = useState({2})", state.Key, "Set_" + state.Key,
                        state.Value == "integer" ? "0" : (state.Value == "array" ? "[]" : "''")));
                }
            }

            string responseStr;
            if (request.ResponseType == "array")
            {
                codeGen.States.Add("response", "const [response, Set_response] = useState([])");
                responseStr = ".then(data => Set_response(data))";
            }
            else if (request.GridView)
            {
                codeGen.States.Add("response", "const [response, Set_response] = useState([])");
                responseStr = ".then(data => Set_response([data]))";
            }
            else if (request.ResponseType == "object")
            {
                codeGen.States.Add("response", "const [response, Set_response] = useState({})");
                responseStr = ".then(data => Set_response(data))";
            }
            else
            {
                codeGen.States.Add("response", "const [response, Set_response] = useState('')");
                responseStr = ".then(data => Set_response(data))";
            }

            StringBuilder sbFunc = new StringBuilder();
            sbFunc.AppendLine("const onSubmit = useCallback((event) => {");
            sbFunc.AppendLine("event.preventDefault();");

            sbFunc.AppendLine("const requestOptions = {");
            sbFunc.AppendLineFormat("method: '{0}',", request.Operation.Verb.ToUpper());
            sbFunc.AppendLine("headers: { 'Content-Type': 'application/json' },");
            if (request.Operation.Verb == "Post" || request.Operation.Verb == "Put")
            {
                sbFunc.AppendLineFormat("body: JSON.stringify({0})", postModel);
            }
            sbFunc.AppendLine("};");

            sbFunc.AppendLineFormat("fetch({0}, requestOptions)", url);
            sbFunc.AppendLine(".then(response => response.json())");
            sbFunc.AppendLine(responseStr);
            sbFunc.AppendLine(".catch (err => console.log(err));");

            sbFunc.AppendLine("})");

            codeGen.Function.Add(sbFunc.ToString());

            codeGen.Html = "<Grid item xs={12}><Button variant='contained' color='primary' type='submit'>Submit</Button></Grid>";

            codeGen.Imports.Add("import Button from '@material-ui/core/Button';");

            return codeGen;
        }

        private static string GeneratePostModel(OpenApiOperation operation, ref Dictionary<string, string> states)
        {
            if (operation.Verb == "Post" || operation.Verb == "Put")
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                foreach (var param in operation.BodyParams)
                {
                    if (param.Property.Count == 0 || param.Type == "array")
                    {
                        sb.AppendFormat("{0}: body_{0},", param.Name);
                        states.Add("body_" + param.Name, param.Type);
                    }
                    else
                    {
                        sb.AppendFormat("{0}:", param.Name);
                        GeneratePostModelRecursive(ref sb, "body_" + param.Name, param.Property, ref states);
                    }
                }
                sb.Append("}");

                return sb.ToString();
            }
            return null;
        }

        private static void GeneratePostModelRecursive(ref StringBuilder sb, string key, List<OpenApiOperationParam> param, ref Dictionary<string, string> states)
        {
            sb.Append("{");
            foreach (var p in param)
            {
                if (p.Property.Count == 0 || p.Type == "array")
                {
                    sb.AppendFormat("{0}: {1}_{0},", p.Name, key);
                    states.Add(key + "_" + p.Name, p.Type);
                }
                else
                {
                    sb.AppendFormat("{0}:", p.Name);
                    GeneratePostModelRecursive(ref sb, key + "_" + p.Name, p.Property, ref states);
                }
            }
            sb.Append("},");
        }

        public static string GenerateResponse(Request request)
        {
            StringBuilder sb = new StringBuilder();
            if (request.GridView)
            {
                sb.AppendLine("<TableContainer component={Paper} className={classes.container}>");
                sb.AppendLine("<Table stickyHeader aria-label='simple table'>");
                sb.AppendLine("<TableHead>");
                sb.AppendLine("<TableRow>");
                foreach (var design in request.ResDesign)
                {
                    foreach (var param in design)
                    {
                        sb.AppendLineFormat("<TableCell>{0}</TableCell>", param.Level);
                    }
                }
                sb.AppendLine("</TableRow>");
                sb.AppendLine("</TableHead>");
                sb.AppendLine("<TableBody>");
                sb.AppendLine("{response.map((obj, index) => (");
                sb.AppendLine("<TableRow key={index}>");

                foreach (var design in request.ResDesign)
                {
                    foreach (var param in design)
                    {
                        if (param.Type == "array")
                        {
                            if (param.Items.Count == 0)
                            {
                                sb.AppendLine("<TableCell>");
                                sb.AppendLine("<ul style={{ padding: '0px 0px 0px 16px', margin: '0px' }}>");
                                sb.AppendLine("{(obj." + param.ObjectName.Replace("_", "?.") + " || []).map((sub, i) => (");
                                sb.AppendLine("<li key={i}>{sub}</li>");
                                sb.AppendLine("))}");
                                sb.AppendLine("</ul>");
                                sb.AppendLine("</TableCell>");
                            }
                            else
                            {
                                sb.AppendLine("<TableCell>");
                                sb.AppendLine("{(obj." + param.ObjectName.Replace("_", "?.") + " || []).map((sub, i) => (");
                                sb.AppendLine("<ul key={i} style={{ padding: '0px 0px 0px 16px', margin: '0px' }}>");
                                foreach (var item in param.Items)
                                {
                                    sb.AppendLine("<li>" + item.Level + ":- {" + item.ObjectName.Replace(param.Name, "sub").Replace("_", "?.") + "}</li>");
                                }
                                sb.AppendLine("</ul>");
                                sb.AppendLine("))}");
                                sb.AppendLine("</TableCell>");
                            }
                        }
                        else
                        {
                            sb.AppendLine("<TableCell>{obj." + param.ObjectName.Replace("_", "?.") + "}</TableCell>");
                        }
                    }
                }

                sb.AppendLine("</TableRow>");
                sb.AppendLine("))}");
                sb.AppendLine("</TableBody>");
                sb.AppendLine("</Table>");
                sb.AppendLine("</TableContainer>");
            }
            else
            {
                sb.AppendLine("<Grid container spacing={3}>");
                foreach (var design in request.ResDesign)
                {
                    var rows = gridRow[design.Count];
                    for (int i = 0; i < design.Count; i++)
                    {
                        sb.AppendLine("<Grid item xs={" + rows[i].ToString() + "}>");
                        if (design[i].Type == "array")
                        {
                            if (design[i].Items.Count == 0)
                            {
                                sb.AppendLineFormat("<label><b>{0} : </b></label>", design[i].Level);
                                sb.AppendLine("<ul style={{ margin: '0px' }}>");
                                sb.AppendLine("{(response." + design[i].ObjectName.Replace("_", "?.") + " || []).map((sub, i) => (");
                                sb.AppendLine("<li key={i}>{sub}</li>");
                                sb.AppendLine("))}");
                                sb.AppendLine("</ul>");
                            }
                            else
                            {
                                sb.AppendLineFormat("<label><b>{0} : </b></label>", design[i].Level);
                                sb.AppendLine("{(response." + design[i].ObjectName.Replace("_", "?.") + " || []).map((sub, i) => (");
                                sb.AppendLine("<ul key={i} style={{ margin: '0px' }}>");
                                foreach (var item in design[i].Items)
                                {
                                    sb.AppendLine("<li><b>" + item.Level + " : </b> {" + item.ObjectName.Replace(design[i].Name, "sub").Replace("_", "?.") + "}</li>");
                                }
                                sb.AppendLine("</ul>");
                                sb.AppendLine("))}");
                            }
                        }
                        else
                        {
                            sb.AppendLineFormat("<label><b>{0} : </b></label>", design[i].Level);
                            sb.AppendLine("<span>{response?." + design[i].ObjectName.Replace("_", "?.") + "}</span>");
                        }
                        sb.AppendLine("</Grid>");
                    }
                }
                sb.AppendLine("</Grid>");
            }
            return sb.ToString();
        }
    }
}