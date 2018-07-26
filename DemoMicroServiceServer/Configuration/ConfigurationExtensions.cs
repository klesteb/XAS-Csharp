using System.IO;

using XAS.Core.Configuration;

namespace DemoMicroServiceServer.Configuration {

    public static class ConfigurationExtensions {

        public static void LoadDemoMicroServiceServer(this IConfiguration config) {

            var key = config.Key;
            var section = config.Section;
            string libDir = config.GetValue(section.Environment(), key.LibDir());

            config.CreateSection(section.Web());
            config.AddKey(section.Web(), key.Address(), "http://localhost:8080");
            config.AddKey(section.Web(), key.WebRootPath(), Path.Combine(libDir, "web"));
            config.AddKey(section.Web(), key.EnableClientCertificates(), false.ToString());

        }

    }

}
