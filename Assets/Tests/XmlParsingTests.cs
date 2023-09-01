using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
using UnityNuGetManager.Nuspec;

namespace Tests
{
    public class XmlParsingTests
    {
        public string NuspecString = @"<?xml version=""1.0"" encoding=""utf-8""?>
        <package xmlns=""http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd"">
            <metadata>
        <id>CelesteMarina.DependencyInjection</id>
        <version>1.1.0</version>
        <title>CelesteMarina.DependencyInjection</title>
        <authors>Celeste Marina Soueid</authors>
        <description>A light-weight dependency injection container inspired by the Microsoft DI.</description>
        <releaseNotes>- Add ServiceDescriptor factory methods
        - Add ApplicationBuilder
        - Add IServiceCollection extensions for adding services</releaseNotes>
        <tags>Dependency Injection</tags>
        <repository url=""https://github.com/BlackxSnow/DependencyInjection"" />
            <dependencies>
        <group targetFramework="".NETFramework4.8"">
            <dependency id=""Microsoft.Extensions.Logging"" version=""7.0.0"" exclude=""Build,Analyzers"" />
        <dependency id=""Microsoft.Extensions.Logging.Console"" version=""7.0.0"" exclude=""Build,Analyzers"" />
        </group>
        </dependencies>
        <frameworkAssemblies>
        <frameworkAssembly assemblyName=""System.ComponentModel.DataAnnotations"" targetFramework="".NETFramework4.8"" />
        <frameworkAssembly assemblyName=""System.Core"" targetFramework="".NETFramework4.8"" />
        <frameworkAssembly assemblyName=""System.Data"" targetFramework="".NETFramework4.8"" />
        <frameworkAssembly assemblyName=""System"" targetFramework="".NETFramework4.8"" />
        <frameworkAssembly assemblyName=""System.Numerics"" targetFramework="".NETFramework4.8"" />
        <frameworkAssembly assemblyName=""System.Runtime"" targetFramework="".NETFramework4.8"" />
        <frameworkAssembly assemblyName=""System.Runtime.InteropServices.RuntimeInformation"" targetFramework="".NETFramework4.8"" />
        <frameworkAssembly assemblyName=""System.Xml"" targetFramework="".NETFramework4.8"" />
        </frameworkAssemblies>
        </metadata>
        </package>";
        
        
        
        [Test]
        public void ParseNuspec()
        {
            using var xmlStream = new MemoryStream();
            var writer = new StreamWriter(xmlStream);
            writer.Write(NuspecString);
            writer.Flush();
            xmlStream.Position = 0;

            var serialiser = new XmlSerializer(typeof(NuspecEntry));
            var entry = (NuspecEntry)serialiser.Deserialize(xmlStream);
            
            Assert.NotNull(entry);
            Assert.NotNull(entry.Metadata);
            Assert.NotNull(entry.Metadata.Dependencies);
            Assert.IsNotEmpty(entry.Metadata.Dependencies[0].TargetFramework);
            Assert.IsNotEmpty(entry.Metadata.Dependencies[0].Dependencies);
            Assert.IsNotEmpty(entry.Metadata.Dependencies[0].Dependencies[0].Id);
            Assert.IsNotEmpty(entry.Metadata.FrameworkAssemblies[0].TargetFramework);
        }
    }
}