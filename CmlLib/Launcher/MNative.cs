using System;
using System.IO;
using CmlLib.Launcher;
using Ionic.Zip;

namespace CmlLib.Launcher
{
    public class MNative
    {
        public MNative(MLaunchOption launchOption)
        {
            this.LaunchOption = launchOption;
        }

        public MLaunchOption LaunchOption { get; private set; }

        public void CreateNatives()
        {
            var path = ExtractNatives(LaunchOption.StartProfile);
            LaunchOption.StartProfile.NativePath = path;

            if (LaunchOption.BaseProfile != null)
                ExtractNatives(LaunchOption.BaseProfile, path);
        }

        /// <summary>
        /// 네이티브 라이브러리들의 압축을 해제해 랜덤 폴더에 저장합니다.
        /// </summary>
        private string ExtractNatives(MProfile profile)
        {
            var ran = new Random();
            int random = ran.Next(10000, 99999); //랜덤숫자 생성
            string path = Minecraft.Versions + profile.Id + "\\natives-" + random.ToString(); //랜덤 숫자를 만들어 경로생성
            ExtractNatives(profile, path);
            return path;
        }

        /// <summary>
        /// 네이티브 라이브러리들을 설정한 경로에 압축을 해제해 저장합니다.
        /// </summary>
        /// <param name="_path">압축 풀 폴더의 경로</param>
        private void ExtractNatives(MProfile profile, string path)
        {
            Directory.CreateDirectory(path); //폴더생성

            foreach (var item in profile.Libraries) //네이티브 라이브러리 리스트를 foreach 로 하나씩 돌림
            {
                try
                {
                    if (item.IsNative)
                    {
                        using (var zip = ZipFile.Read(item.Path))
                        {
                            zip.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                        }
                    }
                }
                catch { }
            }

            profile.NativePath = path;
        }

        /// <summary>
        /// 저장된 네이티브 라이브러리들을 모두 제거합니다.
        /// </summary>
        public void CleanNatives()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(Minecraft.Versions + LaunchOption.StartProfile.Id);
                foreach (var item in di.GetDirectories("native*")) //native 라는 이름이 포함된 폴더를 모두 가져옴
                {
                    DeleteDirectory(item.FullName);
                }
            }
            catch { }
        }

        private void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, true);
        }
    }
}
