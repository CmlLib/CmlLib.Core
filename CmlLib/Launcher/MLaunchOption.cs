using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Launcher
{
    public class MLaunchOption
    {
        public string JavaPath { get; set; } = "";
        public int MaximumRamMb { get; set; } = 1024;
        public MProfile StartProfile { get; set; } = null;
        public MProfile BaseProfile { get; set; } = null;
        public MSession Session { get; set; } = null;
        public string LauncherName { get; set; } = "";
        public string ServerIp { get; set; } = "";
        public string CustomJavaParameter { get; set; } = "";

        public int ScreenWidth { get; set; } = 0;
        public int ScreenHeight { get; set; } = 0;

        internal void CheckValid()
        {
            var exMsg = ""; // error message

            if (MaximumRamMb < 1)
                exMsg = "MaximumRamMb is too small.";

            if (StartProfile == null)
                exMsg = "StartProfile is null";

            if (Session == null)
                exMsg = "Session is null";

            if (LauncherName == null)
                LauncherName = "";
            else if (LauncherName.Contains(" "))
                exMsg = "Launcher Name must not contains Space.";

            if (ServerIp == null)
                ServerIp = "";

            if (CustomJavaParameter == null)
                CustomJavaParameter = "";

            if (ScreenWidth < 0 || ScreenHeight < 0)
                exMsg = "Screen Size must be greater than or equal to zero.";

            if (exMsg != "") // if launch option is invaild, throw exception
                throw new ArgumentException(exMsg);
        }
    }
}
