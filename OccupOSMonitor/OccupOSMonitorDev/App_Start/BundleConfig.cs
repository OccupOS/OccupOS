using System.Web.Optimization;

namespace OccupOSMonitorDev.App_Start {
    public class BundleConfig {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new Bundle("~/bundles/base", new JsMinify()).Include(
                        "~/Scripts/Vendor/jquery-{version}.js",
                        "~/Scripts/Vendor/Twitter/Dev-3.0/bootstrap.js",
                        "~/Scripts/Vendor/handlebars.runtime.js",
                        "~/Scripts/Vendor/ember.js"));

            bundles.Add(new Bundle("~/bundles/templates", new EmberHandlebarsBundleTransform())
                        .Include("~/scripts/app/templates/*.hbs"));

            bundles.Add(new Bundle("~/bundles/app", new JsMinify()).Include(
                        "~/scripts/app/app.js",
                        "~/scripts/app/models/*.js",
                        "~/scripts/app/views/*.js",
                        "~/scripts/app/controllers/*.js",
                        "~/scripts/app/routes/*.js"));

            bundles.Add(new Bundle("~/content/css", new CssMinify()).Include(
                "~/Content/Vendor/Twitter/Dev-3.0/bootstrap.css",
                "~/Content/main.css"));

        }

        /*public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new Bundle("~/bundles/base", new JsMinify()).Include(
                        "~/Scripts/Vendor/JQuery/jquery-{version}.js",
                        "~/Scripts/Vendor/Twitter/Dev-3.0/bootstrap.js",
                        "~/Scripts/Vendor/Ember/handlebars.runtime.js",
                        "~/Scripts/Vendor/Ember/ember.js"));

            bundles.Add(new Bundle("~/bundles/templates", new EmberHandlebarsBundleTransform())
                        .Include("~/Scripts/App/Templates/*.hbs"));

            bundles.Add(new Bundle("~/bundles/app", new JsMinify()).Include(
                        "~/Scripts/App/app.js",
                        "~/Scripts/App/Models/intermediateHwMetadata.js",
                        "~/Scripts/App/Models/sensorData.js",
                        "~/Scripts/App/Models/sensorMetadata.js",
                        "~/Scripts/App/Models/user.js",
                        "~/Scripts/App/Views/*.js",
                        "~/Scripts/App/Controllers/*.js",
                        "~/Scripts/App/Routes/*.js"));

            bundles.Add(new Bundle("~/content/css", new CssMinify()).Include(
                "~/Content/Vendor/Twitter/Dev-3.0/bootstrap.css",
                "~/Content/main.css"));

        }*/
    }
}