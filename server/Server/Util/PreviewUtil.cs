using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Server.Models;

namespace Server.Util
{
    public class PreviewUtil
    {
        public static string PreviewHTML(Request request)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='en'>");
            sb.AppendLine("<head>");
            sb.AppendLine("<title>Preview</title>");
            sb.AppendLine("<meta charset='utf-8'>");
            sb.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1'>");
            sb.AppendLine("<link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css'>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body style='background-color: #f0f8ff;'>");

            sb.AppendLine("<div class='container'>");
            sb.AppendLine("<form>");
            sb.AppendLine("<h2>Preview (UI Only)</h2>");
            foreach (var design in request.Design)
            {
                sb.AppendLine("<div style='display: flex;'>");
                foreach (var item in design)
                {
                    sb.AppendLine("<div style='flex: 1; padding: 10px'>");
                    if (item.Required)
                    {
                        sb.AppendLine("<span style='font-size: 12px; color: red;' class='glyphicon glyphicon-certificate'></span>");
                    }
                    sb.AppendFormat("<level style='font-weight: 700;'>{0}</level>", item.Level);
                    switch (item.Control)
                    {
                        case "1":
                            {
                                sb.AppendFormat("<input type='text' value='{0}' class='form-control'>", item.Value);
                                break;
                            }
                        case "2":
                            {
                                sb.AppendFormat("<input type='number' value='{0}' class='form-control'>", item.Value);
                                break;
                            }
                        case "3":
                            {
                                sb.AppendFormat("<input type='email' value='{0}' class='form-control'>", item.Value);
                                break;
                            }
                        case "4":
                            {
                                sb.AppendFormat("<input type='data' value='{0}' class='form-control'>", item.Value);
                                break;
                            }
                        case "5":
                            {
                                sb.AppendLine("<input type='file' class='form-control'>");
                                break;
                            }
                        case "6":
                            {
                                sb.AppendLine("<select class='form-control'>");
                                sb.AppendLine("<option selected='selected'>Select</option>");
                                foreach (var value in item.Values)
                                {
                                    sb.AppendFormat("<option>{0}</option>", value);
                                }
                                sb.AppendLine("</select>");
                                break;
                            }
                    }
                    if (!string.IsNullOrEmpty(item.Description))
                    {
                        sb.AppendFormat("<span style='font-size: 12px;'><i>{0}</i></span>", item.Description);
                    }
                    sb.AppendLine("</div>");
                }
                sb.AppendLine("</div>");
            }
            sb.AppendLine("<div style='padding: 10px;'><button class='btn btn-primary'>Submit</button></div>");
            sb.AppendLine("</form>");
            sb.AppendLine("</div>");

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
    }
}