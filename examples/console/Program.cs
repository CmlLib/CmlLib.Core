using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using CmlLib.Core.Executors;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Java;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.VersionLoader;
using System.Diagnostics;

namespace CmlLibCoreSample;

class Program
{
    public static readonly HttpClient HttpClient = new();

    public static async Task Main()
    {
        //var t = new TPL();
        //await t.Test();
        //return;

        var p = new Program();

        // Login
        MSession session;
        session = p.OfflineLogin(); // Login by username

        // log login session information
        Console.WriteLine("Success to login : {0} / {1} / {2}", session.Username, session.UUID, session.AccessToken);

        // Launch
        await p.Start(session);
    }

    MSession OfflineLogin()
    {
        // Create fake session by username
        return MSession.GetOfflineSession("tester123");
    }

    async Task Start(MSession session)
    {
        var minecraftPath = new MinecraftPath();
        var httpClient = new HttpClient();
        var rulesEvaluator = new RulesEvaluator();
        var javaPathResolver = new MinecraftJavaPathResolver(minecraftPath);
        var rulesContext = new RulesEvaluatorContext(LauncherOSRule.CreateCurrent());

        var versionLoader = new VersionLoaderCollection()
        {
            new LocalVersionLoader(minecraftPath),
            new MojangVersionLoader(httpClient),
        };

        var versions = await versionLoader.GetVersionMetadatasAsync();
        foreach (var v in versions)
        {
            Console.WriteLine($"{v.Name}, {v.Type}, {v.ReleaseTime}");
        }

        var version = await versions.GetAndSaveVersionAsync(
            "1.16.5", minecraftPath);

        var extractors = FileExtractorCollection.CreateDefault(
            httpClient, javaPathResolver, rulesEvaluator, rulesContext);

        //foreach (var ex in extractors)
        //{
        //    var tasks = await ex.Extract(minecraftPath, version);
        //    foreach (var task in tasks)
        //    {
        //        printTask(task);
        //    }
        //}

        var installer = new TPLTaskExecutor(6);

        var sw = new Stopwatch();
        sw.Start();
        await installer.Install(extractors, minecraftPath, version);
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
        
        while (true)
        {
            Console.ReadLine();
            installer.PrintStatus();
        }
    }

    private void printTask(LinkedTask? task)
    {
        while (task != null)
        {
            Console.WriteLine(task.GetType().Name);
            if (task is FileCheckTask fct)
            {
                Console.WriteLine(fct.Path);
                Console.WriteLine(fct.Hash);
                printTask(fct.OnTrue);
                printTask(fct.OnFalse);
            }
            else if (task is DownloadTask dt)
            {
                Console.WriteLine(dt.Path);
                Console.WriteLine(dt.Url);
            }
            Console.WriteLine();
            task = task.NextTask;
        }
    }

    // Event Handling

    // The code below has some tricks to display logs prettier.
    // You can also use a simpler event handler

    #region Pretty event handler

    private void Downloader_ChangeProgress(object sender, System.ComponentModel.ProgressChangedEventArgs e)
    {
        var top = Console.CursorTop;
        Console.SetCursorPosition(0, top);
        // e.ProgressPercentage: 0~100
        Console.Write($"{e.ProgressPercentage}%  ");
        Console.SetCursorPosition(0, top);
    }

    private void Downloader_ChangeFile(DownloadFileChangedEventArgs e)
    {
        // More information about DownloadFileChangedEventArgs
        // https://github.com/AlphaBs/CmlLib.Core/wiki/Handling-Events#downloadfilechangedeventargs

        Console.WriteLine("[{0}] ({2}/{3}) {1}   ", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
    }

    #endregion

    #region Simple event handler
    //private void Downloader_ChangeProgress(object sender, System.ComponentModel.ProgressChangedEventArgs e)
    //{
    //    Console.WriteLine("{0}%", e.ProgressPercentage);
    //}

    //private void Downloader_ChangeFile(DownloadFileChangedEventArgs e)
    //{
    //    Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
    //}
    #endregion
}

