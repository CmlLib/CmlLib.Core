using System;
using System.Collections.Generic;
using System.Text;

namespace CmlLib.Core.Version
{
    public class MVersionParseException : Exception
    {
        public MVersionParseException() : base()
        {

        }

        public MVersionParseException(Exception inner) : base("Failed to parse version", inner)
        {

        }

        public string VersionName { get; internal set; }
    }
}
