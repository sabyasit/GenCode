using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Models
{
    public class OpenApi
    {
        public string Data { get; set; }
    }

    public class OpenApiPath
    {
        public OpenApiPath()
        {
            Tags = new List<OpenApiTag>();
        }
        public List<OpenApiTag> Tags { get; set; }
    }

    public class OpenApiTag
    {
        public OpenApiTag()
        {
            Operations = new List<OpenApiOperation>();
        }
        public string Tag { get; set; }
        public string Description { get; set; }
        public List<OpenApiOperation> Operations { get; set; }
    }

    public class OpenApiOperation
    {
        public OpenApiOperation()
        {
            Params = new List<OpenApiOperationParam>();
            BodyParams = new List<OpenApiOperationParam>();
            ParamTree = new List<ParameterTree>();
            Server = new List<string>();
        }
        public string Name { get; set; }
        public string Verb { get; set; }
        public string Id { get; set; }
        public List<OpenApiOperationParam> Params { get; set; }
        public List<OpenApiOperationParam> BodyParams { get; set; }
        public List<ParameterTree> ParamTree { get; set; }
        public List<string> Server { get; set; }
    }

    public class OpenApiOperationParam
    {
        public OpenApiOperationParam()
        {
            Values = new List<string>();
            Property = new List<OpenApiOperationParam>();
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public List<string> Values { get; set; }
        public bool IsRequired { get; set; }
        public string Description { get; set; }
        public List<OpenApiOperationParam> Property { get; set; }
    }

    public class ParameterTree
    {
        public ParameterTree()
        {
            Values = new List<string>();
            Items = new List<ParameterTree>();
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Node { get; set; }
        public string Position { get; set; }
        public List<string> Values { get; set; }
        public List<ParameterTree> Items { get; set; }
    }

    public class Request
    {
        public string Header { get; set; }
        public OpenApiOperation Operation { get; set; }
        public List<List<RequestDesign>> Design { get; set; }
    }

    public class RequestDesign
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public int Node { get; set; }
        public int Id { get; set; }
        public string Control { get; set; }
        public string Value { get; set; }
        public bool Required { get; set; }
        public string Position { get; set; }
        public List<string> Values { get; set; }
        public string Description { get; set; }
        public string Error { get; set; }
        public List<RequestDesign> Items { get; set; }
    }
}