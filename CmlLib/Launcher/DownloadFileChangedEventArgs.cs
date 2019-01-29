using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Launcher
{
    public enum MFile { Library, Resource, Minecraft };

    public class DownloadFileChangedEventArgs : EventArgs
    {
        public MFile FileKind;
        public string FileName;
        public int MaxValue;
        public int CurrentValue;
    }
}
