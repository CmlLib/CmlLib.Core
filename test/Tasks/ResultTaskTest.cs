using CmlLib.Core.Tasks;
using NUnit.Framework;

namespace CmlLib.Core.Test.Tasks;

public class ResultTaskTest
{
    public void TestTrue()
    {
        var (task, onTrue, onFalse, next) = createTasks();

        task.Result = true;
        task.OnTrue = onTrue;
        task.OnFalse = onFalse;
        task.InsertNextTask(next);

        Assert.True(onTrue.IsExecuted);
        Assert.False(onFalse.IsExecuted);
        Assert.True(next.IsExecuted);
    }

    public void TestFalse()
    {
        var (task, onTrue, onFalse, next) = createTasks();

        task.Result = false;
        task.OnTrue = onTrue;
        task.OnFalse = onFalse;
        task.InsertNextTask(next);

        Assert.False(onTrue.IsExecuted);
        Assert.True(onFalse.IsExecuted);
        Assert.True(next.IsExecuted);
    }

    public void TestNullTrue()
    {
        var (task, onTrue, onFalse, next) = createTasks();

        task.Result = true;
        task.OnTrue = null;
        task.OnFalse = onFalse;
        task.InsertNextTask(next);

        Assert.False(onFalse.IsExecuted);
        Assert.True(next.IsExecuted);
    }

    public void TestNullFalse()
    {
        var (task, onTrue, onFalse, next) = createTasks();

        task.Result = false;
        task.OnTrue = onTrue;
        task.OnFalse = null;
        task.InsertNextTask(next);

        Assert.False(onTrue.IsExecuted);
        Assert.True(next.IsExecuted);
    }

    public void TestNull()
    {
        var (task, onTrue, onFalse, next) = createTasks();
        task.Result = true;
        task.OnTrue = null;
        task.OnFalse = null;
        task.InsertNextTask(next);

        Assert.True(next.IsExecuted);
    }

    private (MockResultTask, MockTask, MockTask, MockTask) createTasks()
    {
        var task = new MockResultTask("task");
        var onTrue = new MockTask(new TaskFile("OnTrue"));
        var onFalse = new MockTask(new TaskFile("OnFalse"));
        var next = new MockTask(new TaskFile("Next"));

        return (task, onTrue, onFalse, next);
    }
}