namespace CmlLib.Core.Tasks;

public interface ITaskFactory
{
    LinkedTask CheckFile(TaskFile file, LinkedTask onSuccess, LinkedTask onFail);
    LinkedTask Download(TaskFile file);
    LinkedTask CheckAndDownload(TaskFile file);
    LinkedTask ReportDone(TaskFile file);
}