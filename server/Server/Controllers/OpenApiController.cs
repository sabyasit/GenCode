using Microsoft.OpenApi.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Text;

namespace Server.Controllers
{
    public class OpenApiController : ApiController
    {
        [HttpPost]
        [Route("api/openapi")]
        public List<OpenApi> GetOpenApi([FromBody] OpenApiData data)
        {
            var openApi = new List<OpenApi>();
            OpenApiDiagnostic diagnostic = new OpenApiDiagnostic();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data.Data)))
            {
                var openApiDocument = new OpenApiStreamReader().Read(ms, out diagnostic);
                foreach(var path in openApiDocument.Paths)
                {
                    foreach(var operations in path.Value.Operations)
                    {
                        var operation = new OpenApi()
                        {
                            Operation = path.Key,
                            Verb = operations.Key.ToString(),
                            ParamTree = new List<ParameterTree>()
                        };

                        foreach(var param in operations.Value.Parameters)
                        {
                            var p = new Parameter()
                            {
                                Name = param.Name,
                                Type = param.Schema.Type
                            };

                            operation.ParamTree.Add(new ParameterTree()
                            {
                                Name = param.Name,
                                Type= param.Schema.Type,
                                Node = 0
                            });

                            if (p.Type == "object")
                            {
                                p.properties = GetSchema(param.Schema, null, operation.ParamTree, 0)[0].properties;
                            }

                            operation.Param.Add(p);
                        }
                        openApi.Add(operation);
                        //operation.Param = GetParams(operations.Value.Parameters.ToList());
                    }
                }
            }

            return openApi;
        }

        [HttpPost]
        [Route("api/gencode")]
        public string GenCode(Request request)
        {
            return "";
        }
        private List<Parameter> GetSchema(Microsoft.OpenApi.Models.OpenApiSchema schemas, string key, List<ParameterTree> tree, int node)
        {
            var allparames = new List<Parameter>();
            var p = new Parameter()
            {
                Name = key,
                Type = schemas.Type
            };

            if (!string.IsNullOrEmpty(key))
            {
                tree.Add(new ParameterTree()
                {
                    Name = key,
                    Type = schemas.Type,
                    Node = node
                });
            }

            if (schemas.Type == "object")
            {
                foreach(var prop in schemas.Properties)
                {
                    p.properties.AddRange(GetSchema(prop.Value, prop.Key, tree, node + 1));
                }    
            }
            allparames.Add(p);

            return allparames;
        }
    }

    public class OpenApiData
    {
        public string Data { get; set; }
    }

    public class OpenApi
    {
        public string Operation { get; set; }
        public string Verb { get; set; }
        public List<Parameter> Param { get; set; }
        public List<ParameterTree> ParamTree { get; set;}
        public OpenApi()
        {
            Param = new List<Parameter>();
        }
    }

    public class Parameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<string> Values { get; set; }
        public bool IsRequired { get; set; }
        public List<Parameter> properties { get; set; }

        public Parameter()
        {
            properties = new List<Parameter>();
        }
    }

    public class ParameterTree
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Node { get; set; }
    }

    public class Request
    {
        public string Operation { get; set; }
        public string Verb { get; set; }
        public List<ParameterTree> ParamTree { get; set; }
        public string Project { get; set; }
        public List<List<Design>> Design { get; set; }
    }

    public class Design
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public string Type { get; set; }
        public int Node { get; set; }
        public int Id { get; set; }
        public string Control { get; set; }
        public string Value { get; set; }
        public bool Required { get; set; }
        public string Error { get; set; }
    }
}
