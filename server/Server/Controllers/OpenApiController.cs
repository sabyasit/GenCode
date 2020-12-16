using Microsoft.OpenApi.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Text;
using Server.Models;
using Server.Util;
using System.Net.Http.Headers;
using System.Web.Hosting;

namespace Server.Controllers
{
    public class OpenApiController : ApiController
    {
        [HttpPost]
        [Route("api/openapi")]
        public OpenApiPath GetOpenApi([FromBody] OpenApi data)
        {
            string preview = PreviewUtil.GeneratePreview(@"C:\Users\462676\Desktop\codeGen\my-app\src\");

            var openApi = new OpenApiPath();

            OpenApiDiagnostic diagnostic = new OpenApiDiagnostic();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data.Data)))
            {
                var openApiDocument = new OpenApiStreamReader().Read(ms, out diagnostic);
                foreach (var tag in openApiDocument.Tags)
                {
                    openApi.Tags.Add(new OpenApiTag()
                    {
                        Tag = tag.Name,
                        Description = tag.Description
                    });
                }
                if (openApi.Tags.Count == 0)
                {
                    openApi.Tags.Add(new OpenApiTag()
                    {
                        Tag = "Default"
                    });
                }

                foreach (var path in openApiDocument.Paths)
                {
                    foreach (var operation in path.Value.Operations)
                    {
                        var objOperation = new OpenApiOperation()
                        {
                            Id = operation.Value.OperationId,
                            Name = path.Key,
                            Verb = operation.Key.ToString(),
                            Preview = preview,
                            Summary = operation.Value.Summary,
                        };

                        foreach (var param in operation.Value.Parameters)
                        {
                            var objparam = new OpenApiOperationParam()
                            {
                                Name = param.Name,
                                Type = param.Schema.Type,
                                IsRequired = param.Required,
                                Description = param.Description,
                                In = param.In.HasValue ? param.In.Value.ToString().ToLower() : "query"
                            };

                            if (param.Schema.Enum != null && param.Schema.Enum.Count > 0)
                            {
                                foreach (var val in param.Schema.Enum)
                                {
                                    objparam.Values.Add((val as Microsoft.OpenApi.Any.OpenApiString).Value);
                                }
                            }

                            if (param.Schema.Type == "array")
                            {
                                foreach (var val in param.Schema.Items.Enum)
                                {
                                    objparam.Values.Add((val as Microsoft.OpenApi.Any.OpenApiString).Value);
                                }
                            }

                            objOperation.ParamTree.Add(new ParameterTree()
                            {
                                Name = objparam.Name,
                                Type = objparam.Type,
                                ObjectName = objparam.Name,
                                Node = 1,
                                Position = param.In.HasValue? param.In.Value.ToString().ToLower(): "query",
                                Values = objparam.Values,
                                IsRequired = objparam.IsRequired
                            });

                            objOperation.Params.Add(objparam);
                        }

                        if (operation.Value.RequestBody != null && operation.Value.RequestBody.Content.Count > 0)
                        {
                            var content = operation.Value.RequestBody.Content.FirstOrDefault();
                            objOperation.BodyParams = GetBodyParam(content.Value.Schema, null, null, objOperation.ParamTree, 0, new List<string>())[0].Property;
                        }

                        foreach (var server in openApiDocument.Servers)
                        {
                            objOperation.Server.Add(server.Url);
                        }

                        if (objOperation.ParamTree.Count > 0)
                        {
                            var paramTree = objOperation.ParamTree.ToList();
                            objOperation.ParamTree.Clear();
                            foreach (var item in paramTree)
                            {
                                if (objOperation.ParamTree.Count > 0
                                    && objOperation.ParamTree.Last().Type == "array"
                                    && objOperation.ParamTree.Last().Node < item.Node)
                                {
                                    objOperation.ParamTree.Last().Items.Add(item);
                                }
                                else
                                    objOperation.ParamTree.Add(item);
                            }
                        }

                        var response = operation.Value.Responses.Where(x => x.Key == "200").FirstOrDefault();
                        if (response.Key != null)
                        {
                            if (response.Value != null && response.Value.Content.Count > 0)
                            {
                                var resContent = response.Value.Content.FirstOrDefault();
                                objOperation.ResponseType = resContent.Value.Schema.Type;
                                if (resContent.Value.Schema.Type == "object" || resContent.Value.Schema.Type == "array")
                                {
                                    objOperation.ResponseParams = GetBodyParam(resContent.Value.Schema, null, null, objOperation.ResponseTree, 0, new List<string>())[0].Property;
                                }
                                else
                                {
                                    objOperation.ResponseParams.Add(new OpenApiOperationParam() { Type = resContent.Value.Schema.Type, Name = "Output" });
                                    objOperation.ResponseTree.Add(new ParameterTree() { Type = resContent.Value.Schema.Type, Node = 1, Name = "Output", ObjectName = "Output" });
                                }
                            }
                        }

                        if (objOperation.ResponseTree.Count > 0)
                        {
                            var paramTree = objOperation.ResponseTree.ToList();
                            objOperation.ResponseTree.Clear();
                            foreach (var item in paramTree)
                            {
                                if (objOperation.ResponseTree.Count > 0
                                    && objOperation.ResponseTree.Last().Type == "array"
                                    && objOperation.ResponseTree.Last().Node < item.Node)
                                {
                                    objOperation.ResponseTree.Last().Items.Add(item);
                                }
                                else
                                    objOperation.ResponseTree.Add(item);
                            }
                        }

                        if (operation.Value.Tags != null && operation.Value.Tags.Count > 0)
                        {
                            openApi.Tags.Where(x => x.Tag == operation.Value.Tags[0].Name).FirstOrDefault().Operations.Add(objOperation);
                        }
                        else
                        {
                            openApi.Tags.Where(x => x.Tag == "Default").FirstOrDefault().Operations.Add(objOperation);
                        }

                        
                    }
                }
            }

            return openApi;
        }

        [HttpPost]
        [Route("api/preview/{page}")]
        public string Preview([FromUri]string page, [FromBody]Request request)
        {
            PreviewUtil.Preview(request, page, @"C:\Users\462676\Desktop\codeGen\my-app\src\");
            return "1";
        }

        [HttpPost]
        [Route("api/generate")]
        public string Generate(Request request)
        {
            return GenerateUtil.ExportReactCode(request, "App", HostingEnvironment.MapPath("~/App_Data"));
        }

        [HttpGet]
        [Route("api/export/{file}")]
        public HttpResponseMessage Generate(string file)
        {
            var fileInfo = new FileInfo(Directory.GetFiles(HostingEnvironment.MapPath("~/App_Data/") + file)[0]);
            
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileInfo.Name;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
            return response;
        }

        private List<OpenApiOperationParam> GetBodyParam(Microsoft.OpenApi.Models.OpenApiSchema schema, 
            string key, string parentName, List<ParameterTree> tree, int node, List<string> required)
        {
            var paramLst = new List<OpenApiOperationParam>();
            var param = new OpenApiOperationParam()
            {
                Name = key,
                Type = schema.Type,
                Description = schema.Description,
                IsRequired = required.Contains(key)
            };

            if (schema.Enum != null)
            {
                foreach (var val in schema.Enum)
                {
                    param.Values.Add((val as Microsoft.OpenApi.Any.OpenApiString).Value);
                }
            }

            if (!string.IsNullOrEmpty(key))
            {
                tree.Add(new ParameterTree()
                {
                    Name = param.Name,
                    Type = param.Type,
                    ObjectName = parentName[0]=='_'? parentName.Substring(1) : parentName,
                    Node = node,
                    Position = "body",
                    Values = param.Values,
                    IsRequired = param.IsRequired
                });
            }

            if (schema.Properties != null)
            {
                foreach (var prop in schema.Properties)
                {
                    param.Property.AddRange(GetBodyParam(prop.Value, prop.Key, string.Format("{0}_{1}", key, prop.Key), tree, node + 1, schema.Required.ToList()));
                }
            }

            if (schema.Type == "array" && schema.Items != null)
            {
                foreach (var prop in schema.Items.Properties)
                {
                    param.Property.AddRange(GetBodyParam(prop.Value, prop.Key, string.Format("{0}_{1}", key, prop.Key), tree, node + 1, schema.Required.ToList()));
                }
            }

            paramLst.Add(param);

            return paramLst;
        }
    }
}
