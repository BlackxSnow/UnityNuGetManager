using System.Linq;
using System.Text.RegularExpressions;
using UnityNuGetManager.Package;

namespace UnityNuGetManager.Version
{
    public enum FrameworkType
    {
        Standard,
        Framework
    }
    
    public class VersionData<T> where T : ITargetFramework
    {
        public string LibFolderName;
        public T SelectedTarget;
        
        public VersionData(string libFolder, T target)
        {
            LibFolderName = libFolder;
            SelectedTarget = target;
        }
    }
    public static class VersionResolver
    {
        private class FrameworkData
        {
            public FrameworkType Type;
            public string FrameworkPattern;
            public string LibPattern;

            private Regex _FrameworkRegex;
            
            public bool TryMatchFramework(string frameworkValue, out Match match)
            {
                match = _FrameworkRegex.Match(frameworkValue);
                return match.Success;
            }
            public string GetLib(string frameworkValue)
            {
                return Regex.Replace(frameworkValue, FrameworkPattern, LibPattern);
            }
            
            public FrameworkData(FrameworkType type, string frameworkPattern, string libPattern)
            {
                Type = type;
                FrameworkPattern = frameworkPattern;
                LibPattern = libPattern;
                _FrameworkRegex = new Regex(frameworkPattern);
            }
        }

        private static FrameworkData[] _Targets =
        {
            new FrameworkData(FrameworkType.Framework, @"^\.NETFramework([0-9])(?:\.([0-9]))?(?:\.([0-9]))?$", "net$1$2$3"),
            new FrameworkData(FrameworkType.Standard, @"^\.NETStandard([0-9])(?:\.([0-9]))?$", "netstandard$1.$2")
        };

        private class FrameworkVersion<T> where T : ITargetFramework
        {
            public FrameworkData Data;
            public NugetSemanticVersion Version;
            public T Target;
        }
        
        public static VersionData<T> GetBestDotNetVersion<T>(params T[] availableTargets) where T : ITargetFramework
        {
            FrameworkVersion<T> currentTarget = null;
            foreach (T target in availableTargets)
            {
                foreach (FrameworkData framework in _Targets)
                {
                    if (!framework.TryMatchFramework(target.TargetFramework, out Match match)) continue;

                    var matchVersion = new NugetSemanticVersion(string.Join('.',
                        match.Groups.Skip(1).Where(g => !string.IsNullOrWhiteSpace(g.Value))));

                    bool targetNull = currentTarget == null;
                    int frameworkTypeComparison = targetNull ? 0 : framework.Type.CompareTo(currentTarget.Data.Type);
                    bool isHigherVersion = frameworkTypeComparison >= 0 && matchVersion > currentTarget?.Version;

                    if (!targetNull && frameworkTypeComparison <= 0 && !isHigherVersion) continue;

                    currentTarget = new FrameworkVersion<T>
                    {
                        Data = framework,
                        Version = matchVersion,
                        Target = target
                    };
                    break;
                }
            }

            if (currentTarget == null) return null;
            return new VersionData<T>(currentTarget.Data.GetLib(currentTarget.Target.TargetFramework),
                currentTarget.Target);
        }
    }
}