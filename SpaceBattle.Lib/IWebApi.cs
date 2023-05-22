using System.Net;
using CoreWCF;
using CoreWCF.OpenApi.Attributes;
using CoreWCF.Web;

namespace SpaceBattle.Lib;

[ServiceContract]
public interface IWebApi
{
    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "/body")]
    [OpenApiTag("Tag")]
    [OpenApiResponse(ContentTypes = new[] { "application/json", "text/xml" }, Description = "Success", StatusCode = HttpStatusCode.OK, Type = typeof(Contract)) ]
    Contract BodyEcho(
        [OpenApiParameter(ContentTypes = new[] { "application/json", "text/xml" }, Description = "param description.")] Contract param);
}
