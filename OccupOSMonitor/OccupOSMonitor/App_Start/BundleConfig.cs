// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BundleConfig.cs" company="UCL">
//   (c) 2013
// </copyright>
// <summary>
//   Defines the BundleConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOSMonitor.App_Start {
    using System.Web.Optimization;

    /// <summary>
    /// The bundle config.
    /// </summary>
    public class BundleConfig {
        /// <summary>
        /// The register bundles.
        /// </summary>
        /// <param name="bundles">
        /// The bundles.
        /// </param>
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new Bundle("~/bundles/base", new JsMinify()).Include(
                        "~/Scripts/Vendor/JQuery/jquery-{version}.js",
                        "~/Scripts/Vendor/Twitter/Dev-3.0/bootstrap.js",
                //"~/Scripts/Vendor/D3/d3.v3.js",
                        "~/Scripts/Vendor/Ember/handlebars.runtime.js",
                        "~/Scripts/Vendor/Ember/ember.js",
                        "~/Scripts/Vendor/Ember/ember-data.js",
                        "~/Scripts/Vendor/Ember/webapi_serializer.js",
                        "~/Scripts/Vendor/Ember/webapi_adapter.js"));

            bundles.Add(new Bundle("~/bundles/templates", new OccupOSMonitor.App_Start.EmberHandlebarsBundleTransform())
                        .Include("~/Scripts/App/Templates/*.hbs"));

            // possibly have to include store.js before the models (not sure, just as a future hint)
            bundles.Add(new Bundle("~/bundles/app", new JsMinify()).Include(
                "~/Scripts/App/*.js",
                "~/Scripts/App/Helpers/*.js",
                "~/Scripts/App/Models/*.js",
                "~/Scripts/App/Views/*.js",
                "~/Scripts/App/Controllers/*.js",
                "~/Scripts/App/Routes/*.js"));

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