namespace CmlLib.Core.Tasks;

public class TaskFactory : ITaskFactory
{
    private readonly HttpClient _httpClient;

    public TaskFactory(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public LinkedTask CheckAndDownload(TaskFile file)
    {
        return CheckFile(file,
            onSuccess: ReportDone(file),
            onFail: Download(file));
    }

    public LinkedTask CheckFile(TaskFile file, LinkedTask onSuccess, LinkedTask onFail)
    {
        var task = new FileCheckTask(file);
        task.OnTrue = onSuccess;
        task.OnFalse = onFail;
        return task;
    }

    public LinkedTask Download(TaskFile file)
    {
        return new DownloadTask(file, _httpClient);
    }

    public LinkedTask ReportDone(TaskFile file)
    {
        return ProgressTask.CreateDoneTask(file);
    }
}