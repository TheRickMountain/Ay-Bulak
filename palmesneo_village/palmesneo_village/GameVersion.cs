using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class GameVersion : IEquatable<GameVersion>, IComparable<GameVersion>, IComparable
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }

        public GameVersion(int major, int minor, int patch)
        {
            if (major < 0 || minor < 0 || patch < 0)
                throw new ArgumentOutOfRangeException("Version components cannot be negative");

            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public static GameVersion Parse(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentNullException(nameof(version));

            var parts = version.Split('.');
            if (parts.Length != 3)
                throw new FormatException("Version must be in format 'major.minor.patch'");

            if (!int.TryParse(parts[0], out int major) ||
                !int.TryParse(parts[1], out int minor) ||
                !int.TryParse(parts[2], out int patch))
                throw new FormatException("All version components must be integers");

            return new GameVersion(major, minor, patch);
        }

        public static bool TryParse(string version, out GameVersion result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(version))
                return false;

            var parts = version.Split('.');
            if (parts.Length != 3)
                return false;

            if (!int.TryParse(parts[0], out int major) ||
                !int.TryParse(parts[1], out int minor) ||
                !int.TryParse(parts[2], out int patch) ||
                major < 0 || minor < 0 || patch < 0)
                return false;

            result = new GameVersion(major, minor, patch);
            return true;
        }

        #region Operators
        public static bool operator <(GameVersion a, GameVersion b)
        {
            return (a is null) ? !(b is null) : a.CompareTo(b) < 0;
        }

        public static bool operator >(GameVersion a, GameVersion b)
        {
            return (b is null) ? false : a.CompareTo(b) > 0;
        }

        public static bool operator <=(GameVersion a, GameVersion b)
        {
            return (a is null) ? true : a.CompareTo(b) <= 0;
        }

        public static bool operator >=(GameVersion a, GameVersion b)
        {
            return (a is null) ? (b is null) : a.CompareTo(b) >= 0;
        }

        public static bool operator ==(GameVersion a, GameVersion b)
        {
            return EqualityComparer<GameVersion>.Default.Equals(a, b);
        }

        public static bool operator !=(GameVersion a, GameVersion b)
        {
            return !(a == b);
        }
        #endregion

        #region IComparable
        public int CompareTo(GameVersion other)
        {
            if (other is null)
                return 1;

            var majorComparison = Major.CompareTo(other.Major);
            if (majorComparison != 0)
                return majorComparison;

            var minorComparison = Minor.CompareTo(other.Minor);
            if (minorComparison != 0)
                return minorComparison;

            return Patch.CompareTo(other.Patch);
        }

        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;

            if (!(obj is GameVersion other))
                throw new ArgumentException($"Object must be of type {nameof(GameVersion)}");

            return CompareTo(other);
        }
        #endregion

        #region Equality
        public bool Equals(GameVersion other)
        {
            if (other is null)
                return false;

            return Major == other.Major && Minor == other.Minor && Patch == other.Patch;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (!(obj is GameVersion version))
                return false;

            return Equals(version);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Major.GetHashCode();
                hash = hash * 23 + Minor.GetHashCode();
                hash = hash * 23 + Patch.GetHashCode();
                return hash;
            }
        }
        #endregion

        public override string ToString() => $"{Major}.{Minor}.{Patch}";
    }
}
