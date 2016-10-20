using System;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Extensions;
using TLDDesigns.Owin.ContentSecurityPolicy;

[assembly: OwinStartup("OwinCSP", typeof(TLDDesigns.Home.Startup))]

namespace TLDDesigns.Home
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            ContentSecurityPolicyOptions cspOptions = new ContentSecurityPolicyOptions();

            cspOptions.NonceSecret = Environment.GetEnvironmentVariable("NONCE_SECRET");

            cspOptions.Default.Sources.Add("'none'");

            cspOptions.Style.UseNonce = true;
            cspOptions.Style.UseSelf = true;
            cspOptions.Style.UseUnsafeInline = true;
            cspOptions.Style.Sources.Add("https://fonts.googleapis.com");

            cspOptions.Script.UseSelf = true;
            cspOptions.Script.UseNonce = true;
            cspOptions.Script.UseUnsafeInline = true;
            cspOptions.Script.Sources.Add("https://www.google-analytics.com");
            cspOptions.Script.Sources.Add("https://ajax.googleapis.com");

            cspOptions.Script.UseStrictDynamic = true;

            cspOptions.Image.UseSelf = true;
            cspOptions.Image.Sources.Add("https://www.google-analytics.com");

            cspOptions.Font.UseSelf = true;
            cspOptions.Font.Sources.Add("https://fonts.gstatic.com");

            cspOptions.Object.Sources.Add("'none'");

            cspOptions.Connect.UseSelf = true;

            cspOptions.ReportOnly = false;

            cspOptions.ReportOnlyUri = Environment.GetEnvironmentVariable("CSP_REPORT_ONLY_URI"); ;

            cspOptions.EnforceUri = Environment.GetEnvironmentVariable("CSP_ENFORCE_URI");

            cspOptions.EnforceOnLocalhost = false;

            app.UseContentSecurityPolicy(cspOptions);

            app.UseErrorPage();

            app.UseStageMarker(PipelineStage.MapHandler);

        }

    }

}



