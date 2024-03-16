using System.Text;

namespace CmlLib.Core.CommandParser;

interface ICommandLineStateMachine
{
    void End(ArgumentBuilder current);
    ICommandLineStateMachine Put(ArgumentBuilder current, char next);
}

static class CommandLineStates
{
    public readonly static ICommandLineStateMachine Init = new InitState();
    public readonly static ICommandLineStateMachine Value = new ValueState();
    public readonly static ICommandLineStateMachine Quoted = new QuotedState();
    public readonly static ICommandLineStateMachine EscapedFromValue = new EscapedState(returnTo: Value);
    public readonly static ICommandLineStateMachine EscapedFromQuoted = new EscapedState(returnTo: Quoted);

    class InitState : ICommandLineStateMachine
    {
        public void End(ArgumentBuilder current)
        {
            
        }

        public ICommandLineStateMachine Put(ArgumentBuilder current, char next)
        {
            if (char.IsWhiteSpace(next))
            {
                return CommandLineStates.Init;
            }
            else if (next == '"')
            {
                return CommandLineStates.Quoted;
            }
            else if (next == '\\')
            {
                return CommandLineStates.EscapedFromValue;
            }
            else
            {
                current.Append(next);
                return CommandLineStates.Value;
            }
        }
    }

    class ValueState : ICommandLineStateMachine
    {
        public void End(ArgumentBuilder current) 
        {
            current.Complete();
        }

        public ICommandLineStateMachine Put(ArgumentBuilder current, char next)
        {
            if (char.IsWhiteSpace(next))
            {
                current.Complete();
                return CommandLineStates.Init;
            }
            else if (next == '"')
            {
                return CommandLineStates.Quoted;
            }
            else if (next == '\\')
            {
                return CommandLineStates.EscapedFromValue;
            }
            else
            {
                current.Append(next);
                return CommandLineStates.Value;
            }
        }
    }

    class QuotedState : ICommandLineStateMachine
    {
        public void End(ArgumentBuilder current)
        {
            current.Complete();
        }

        public ICommandLineStateMachine Put(ArgumentBuilder current, char next)
        {
            if (next == '"')
            {
                return CommandLineStates.Value;
            }
            else if (next == '\\')
            {
                return CommandLineStates.EscapedFromQuoted;
            }
            else
            {
                current.Append(next);
                return CommandLineStates.Quoted;
            }
        }
    }

    class EscapedState : ICommandLineStateMachine
    {
        private readonly ICommandLineStateMachine _returnTo;
        public EscapedState(ICommandLineStateMachine returnTo) => _returnTo = returnTo;

        public void End(ArgumentBuilder current)
        {
            current.Complete();
        }

        public ICommandLineStateMachine Put(ArgumentBuilder current, char next)
        {
            current.Append(next);
            return _returnTo;
        }
    }
}

class ArgumentBuilder
{
    private readonly Queue<string> _q = new();
    private readonly StringBuilder _sb = new();

    public void Append(char c) => _sb.Append(c);

    public void Complete()
    {
        _q.Enqueue(_sb.ToString());
        _sb.Clear();
    }

    public IEnumerable<string> PopArguments()
    {
        while (_q.Any())
            yield return _q.Dequeue();
    }
}