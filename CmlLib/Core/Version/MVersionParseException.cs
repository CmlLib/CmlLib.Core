using System;

namespace CmlLib.Core.Version
{
    public class MVersionParseException : Exception
    {
        public MVersionParseException(string message) : base(message)
        {

        }

        public MVersionParseException(Exception inner) : base("Failed to parse version", inner)
        {

        }

        public string VersionName { get; internal set; }
    }
}
