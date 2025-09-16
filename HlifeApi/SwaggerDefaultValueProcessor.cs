using NJsonSchema.Generation;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Diagnostics;

namespace HlifeApi
{
    public class SwaggerDefaultValueProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext ctx)
        {
            try
            {
                if (ctx.OperationDescription.Path.StartsWith("/query"))
                {
                    ctx.OperationDescription.Operation.RequestBody.Content["application/json"].Example =
                        Newtonsoft.Json.Linq.JToken.FromObject(new Query.Request() { Key = "王" });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            try
            {
                if (ctx.OperationDescription.Path.StartsWith("/upload"))
                {
                    ctx.OperationDescription.Operation.RequestBody.Content["application/json"].Example =
                        Newtonsoft.Json.Linq.JToken.FromObject(Upload.Request.Mock());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return true;
        }
    }
}