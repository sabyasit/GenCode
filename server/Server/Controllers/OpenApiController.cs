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

namespace Server.Controllers
{
    public class OpenApiController : ApiController
    {
        [HttpPost]
        [Route("api/openapi")]
        public OpenApiPath GetOpenApi([FromBody] OpenApi data)
        {
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
                            Verb = operation.Key.ToString()
                        };

                        foreach (var param in operation.Value.Parameters)
                        {
                            var objparam = new OpenApiOperationParam()
                            {
                                Name = param.Name,
                                Type = param.Schema.Type,
                                IsRequired = param.Required,
                                Description = param.Description
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
                                objparam.Type = "string"; // hard code
                                foreach (var val in param.Schema.Items.Enum)
                                {
                                    objparam.Values.Add((val as Microsoft.OpenApi.Any.OpenApiString).Value);
                                }
                            }

                            objOperation.ParamTree.Add(new ParameterTree()
                            {
                                Name = objparam.Name,
                                Type = objparam.Type,
                                Node = 1,
                                Position = "query",
                                Values = objparam.Values
                            });

                            objOperation.Params.Add(objparam);
                        }

                        if (operation.Value.RequestBody != null && operation.Value.RequestBody.Content.Count > 0)
                        {
                            var content = operation.Value.RequestBody.Content.FirstOrDefault();
                            objOperation.BodyParams = GetBodyParam(content.Value.Schema, null, objOperation.ParamTree, 0)[0].Property;
                        }

                        foreach (var server in openApiDocument.Servers)
                        {
                            objOperation.Server.Add(server.Url);
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
        [Route("api/preview")]
        public HttpResponseMessage Preview([FromBody] OpenApi previewData)
        {
            Request request = Newtonsoft.Json.JsonConvert.DeserializeObject<Request>(previewData.Data);
            var response = new HttpResponseMessage();
            response.Content = new StringContent(PreviewUtil.PreviewHTML(request));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private List<OpenApiOperationParam> GetBodyParam(Microsoft.OpenApi.Models.OpenApiSchema schema, string key, List<ParameterTree> tree, int node)
        {
            var paramLst = new List<OpenApiOperationParam>();
            var param = new OpenApiOperationParam()
            {
                Name = key,
                Type = schema.Type,
                Description = schema.Description
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
                    Node = node,
                    Position = "body",
                    Values = param.Values
                });
            }

            if (schema.Properties != null)
            {
                foreach (var prop in schema.Properties)
                {
                    param.Property.AddRange(GetBodyParam(prop.Value, prop.Key, tree, node + 1));
                }
            }

            if (schema.Type == "array" && schema.Items != null)
            {
                param.Type = schema.Items.Properties.Count == 0 ? "string" : "object"; // hard code

                foreach (var prop in schema.Items.Properties)
                {
                    param.Property.AddRange(GetBodyParam(prop.Value, prop.Key, tree, node + 1));
                }
            }

            paramLst.Add(param);

            return paramLst;
        }
    }
}
