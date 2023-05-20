using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.Xml;
using SpaceBattle.Lib;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddServiceModelWebServices(o =>
        {
            o.Title = "Test API";
            o.Version = "1";
            o.Description = "API Description";
            o.TermsOfService = new("http://example.com/terms");
            o.ContactName = "Contact";
            o.ContactEmail = "support@example.com";
            o.ContactUrl = new("http://example.com/contact");
            o.ExternalDocumentUrl = new("http://example.com/doc.pdf");
            o.ExternalDocumentDescription = "Documentation";
        });

        services.AddSingleton(new SwaggerOptions());
    }
}