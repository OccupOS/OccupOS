using System.IO;
using System.Web;
using System.Globalization;
using System.Web.Optimization;
using Ember.Handlebars;
using OccupOSMonitor.Helpers;

namespace OccupOSMonitor.App_Start {
    public class EmberHandlebarsBundleTransform : IBundleTransform {
        public void Process(BundleContext context, BundleResponse response) {
            var builder = new TemplateBuilder();
            var usTextInfo = new CultureInfo("en-US", false).TextInfo;

            foreach (var assetFile in response.Files) {
                var path = context.HttpContext.Server.MapPath(assetFile.VirtualPath);
                var template = File.ReadAllText(path);
                var templateName = Path.GetFileNameWithoutExtension(path).ToCamelCase();
                builder.Register(templateName, template);
            }

            response.Content = builder.ToString();
            response.ContentType = "text/javascript";
            response.Cacheability = HttpCacheability.Public;
        }
    }
}