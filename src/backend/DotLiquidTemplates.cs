using System.Reflection;
using DotLiquid;
using Microsoft.AspNetCore.Mvc;

namespace demoidp
{
    public static class DotLiquidTemplates
    {
        public static readonly Template Login;
        public static readonly Template Home;

        static DotLiquidTemplates()
        {
            var type = typeof(DotLiquidTemplates).GetTypeInfo();
            var rootNamespace = type.Namespace;
            var assembly = type.Assembly;

            string ReadResource(string name)
            {
                var resource = assembly?.GetManifestResourceStream($"{rootNamespace}.{name}.liquid");
                using var reader = new StreamReader(resource ?? Stream.Null);
                return reader.ReadToEnd();
            }

            Login = Template.Parse(ReadResource("Login"));
            Home = Template.Parse(ReadResource("Home"));
        }
    }

    public static class LiquidContentResult
    {
        internal static IActionResult Get(Template template, object model = null)
        {
            var hash = Hash.FromAnonymousObject(model);
            var content = template.Render(hash);
            return new ContentResult()
            {
                Content = content,
                ContentType = "text/html"
            };
        }
    }
}
