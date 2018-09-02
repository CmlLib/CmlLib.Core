using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Launcher
{
    public delegate void ExceptionEventHandler(Exception excep, string msg);

    public class WrapLauncher
    {
        public event MChangeDownloadProgress ChangeDownloadFileEvent;
        public event System.Net.DownloadProgressChangedEventHandler ChangeProgressEvent;
        public event ExceptionEventHandler ExceptionEvent;

        public WrapLauncher()
        {
            ChangeDownloadFileEvent += delegate { };
            ChangeProgressEvent += delegate { };
            ExceptionEvent += delegate { };
        }

        public MLaunch Patch(MProfileInfo[] list, string profilename, bool resource = true)
        {
            try
            {
                var profile = download(list,profilename, resource);
                MProfile baseprofile = null;

                if (profile.IsForge) //만약 포지 프로파일이라면
                {
                    baseprofile = download(list,profile.InnerJarId, resource);
                    profile.NativePath = baseprofile.NativePath;
                }

                var launch = new MLaunch();
                launch.StartProfile = profile;
                launch.BaseProfile = baseprofile;
                return launch;
            }
            catch (Exception ex)
            {
                exception(ex);
                throw ex;
            }
        }

        MProfile download(MProfileInfo[] versions, string name, bool resource)
        {
            var list = new List<MProfileInfo>(versions);

            d("Find Profile");
            MProfileInfo info = list.Find(x => x.Name == name);

            if (info == null) //프로파일 찾을 수 없을때
                throw new Exception("Can't Find Profile");

            d("Parse Profile");
            MProfile profile = MProfile.Parse(info);

            d("Download Start");
            var downloader = new MDownloader(profile);
            downloader.ChangeFileProgressEvent += Downloader_ChangeFileProgressEvent;
            downloader.ChangeProgressEvent += Downloader_ChangeProgressEvent;
            downloader.DownloadLibraries();
            downloader.DownloadMinecraft();

            if (resource && !profile.IsForge)
            {
                downloader.DownloadIndex();
                downloader.DownloadResource();
            }
            downloader.ExtractNatives();

            return profile;
        }

        private void Downloader_ChangeProgressEvent(ChangeProgressEventArgs e)
        {
            ChangeDownloadFileEvent(e);
        }

        private void Downloader_ChangeFileProgressEvent(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            ChangeProgressEvent(sender, e);
        }

        string debugText = "";
        void d(string str, bool isset = true)
        {
#if DEBUG
            Console.WriteLine("[DEBUG] " + str);
#endif
            if (isset)
                debugText = str;
        }

        void exception(Exception ex)
        {
            string error = "ERROR : \n" +
    debugText + "\n" +
    ex.Message + "\n" +
    ex.Source + "\n" +
    ex.StackTrace + "\n" +
    ex.TargetSite;
            ExceptionEvent(ex,error);
        }
    }
}
