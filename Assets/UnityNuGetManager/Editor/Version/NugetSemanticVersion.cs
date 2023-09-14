using System;
using System.Text.RegularExpressions;

namespace UnityNuGetManager.Version
{
    public readonly struct NugetSemanticVersion : IComparable<NugetSemanticVersion>
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public int Revision { get; }
        public string Prerelease { get; }

        public bool IsStrict { get; }
        
        public static NugetSemanticVersion Zero => new NugetSemanticVersion(0);
        public static NugetSemanticVersion Max => new NugetSemanticVersion(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
        public static NugetSemanticVersion Invalid => new NugetSemanticVersion(-1, -1, -1, -1);

        public bool IsValid => Major >= 0 && Minor >= 0 && Patch >= 0 && Revision >= 0 && 
                               (!IsStrict || Major+Minor+Patch+Revision > 0 || !string.IsNullOrWhiteSpace(Prerelease));
        
        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}{(Revision > 0 ? "." + Revision : string.Empty)}";
        }

        
        private const string VersionPattern =
            @"^(0|[1-9][0-9]*|[1-9]+)(?:.(0|[1-9][0-9]*|[1-9]+)(?:.(0|[1-9][0-9]*|[1-9]+)(?:.(0|[1-9][0-9]*|[1-9]+))?)?)?(?:-(.*))?(?:\+(.*))?$";
        
        public static bool TryParse(string version, out NugetSemanticVersion result)
        {
            Match match = Regex.Match(version, VersionPattern);
            if (!match.Success)
            {
                result = Invalid;
                return false;
            }
            result = new NugetSemanticVersion(match);
            return result.IsValid;
        }
        public static NugetSemanticVersion Parse(string version)
        {
            return TryParse(version, out NugetSemanticVersion result)
                ? result
                : throw new ArgumentException($"Invalid version string format: {version}");
        }

        private NugetSemanticVersion(Match match, bool isStrict = false)
        {
            if (!match.Success) throw new ArgumentException($"Match.Success was false");
            IsStrict = isStrict;
            Major = int.TryParse(match.Groups[1].ToString(), out int parsedMajor) ? parsedMajor : 0;
            Minor = int.TryParse(match.Groups[2].ToString(), out int parsedMinor) ? parsedMinor : 0;
            Patch = int.TryParse(match.Groups[3].ToString(), out int parsedPatch) ? parsedPatch : 0;
            Revision = int.TryParse(match.Groups[4].ToString(), out int parsedRevision) ? parsedRevision : 0;
            Prerelease = match.Groups[5].ToString();
        }
        public NugetSemanticVersion(string version) : this(Regex.Match(version, VersionPattern))
        {

        }

        public NugetSemanticVersion(string major, string minor = "0", string patch = "0", string revision = "0", 
            string prerelease = "", bool isStrict = false)
        {
            IsStrict = isStrict;
            Major = int.Parse(major);
            Minor = int.Parse(minor);
            Patch = int.Parse(patch);
            Revision = int.Parse(revision);
            Prerelease = prerelease;
        }
        
        public NugetSemanticVersion(int major, int minor = 0, int patch = 0, int revision = 0, string prerelease = "",
            bool isStrict = false)
        {
            IsStrict = isStrict;
            Major = major;
            Minor = minor;
            Patch = patch;
            Revision = revision;
            Prerelease = prerelease;
        }

        public int CompareTo(NugetSemanticVersion other)
        {
            int majorComparison = Major.CompareTo(other.Major);
            if (majorComparison != 0) return majorComparison;
            int minorComparison = Minor.CompareTo(other.Minor);
            if (minorComparison != 0) return minorComparison;
            int patchComparison = Patch.CompareTo(other.Patch);
            if (patchComparison != 0) return patchComparison;
            int revisionComparison = Revision.CompareTo(other.Revision);
            if (revisionComparison != 0) return revisionComparison;
            return string.Compare(Prerelease, other.Prerelease, StringComparison.Ordinal);
        }
        
        public static bool operator >(NugetSemanticVersion a, NugetSemanticVersion b) => a.CompareTo(b) > 0;

        public static bool operator <(NugetSemanticVersion a, NugetSemanticVersion b) => a.CompareTo(b) < 0;

        public static bool operator ==(NugetSemanticVersion a, NugetSemanticVersion b) => a.CompareTo(b) == 0;

        public static bool operator !=(NugetSemanticVersion a, NugetSemanticVersion b) => a.CompareTo(b) != 0;

        public static bool operator >=(NugetSemanticVersion a, NugetSemanticVersion b) => a.CompareTo(b) >= 0;

        public static bool operator <=(NugetSemanticVersion a, NugetSemanticVersion b) => a.CompareTo(b) <= 0;
        
        public bool Equals(NugetSemanticVersion other)
        {
            return 
                Major == other.Major && 
                Minor == other.Minor && 
                Patch == other.Patch &&
                Revision == other.Revision &&
                Prerelease == other.Prerelease;
        }

        public override bool Equals(object obj)
        {
            return obj is NugetSemanticVersion other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch, Revision, Prerelease);
        }

        public static NugetSemanticVersion ParseRange(string range)
        {
            NugetSemanticVersion version = TryParseExact(range);
            if (version.IsValid) return version;
            version = TryParseRange(range);
            return version.IsValid ? version : throw new ArgumentException($"Invalid version string: {range}");
        }
        
        private static NugetSemanticVersion TryParseExact(string range)
        {
            TryParse(range, out NugetSemanticVersion version);
            return version;
        }

        // TODO: Properly parse version ranges
        // https://learn.microsoft.com/en-us/nuget/concepts/package-versioning#version-ranges
        private static NugetSemanticVersion TryParseRange(string range)
        {
            return TryParseExact(range.Trim(new[] { '[', ']', '(', ')', ',', ' ' }));
        }
    }
}