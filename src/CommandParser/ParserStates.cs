namespace CmlLib.Core.CommandParser;

interface IParserStateMachine
{
    IParserStateMachine Put(KeyValueArgumentBuilder current, string next);
    void End(KeyValueArgumentBuilder current);
}

static class ParserStates
{
    public readonly static IParserStateMachine Init = new InitState();
    public readonly static IParserStateMachine Key = new KeyState();

    public static IParserStateMachine PutKey(KeyValueArgumentBuilder current, string next)
    {
        var separatorIndex = next.IndexOf("=");
        if (separatorIndex >= 0) // key=value
        {
            var key = next.Substring(0, separatorIndex);
            var value = (separatorIndex < next.Length - 1)
                ? next.Substring(separatorIndex + 1, next.Length - separatorIndex - 1)
                : "";

            current.Key = key;
            current.Value = value;
            current.Complete();
            return ParserStates.Init;
        }
        else // key
        {
            current.Key = next;
            return ParserStates.Key;
        }
    }

    class InitState : IParserStateMachine
    {
        public void End(KeyValueArgumentBuilder current)
        {

        }

        public IParserStateMachine Put(KeyValueArgumentBuilder current, string next)
        {
            if (next.StartsWith("-")) // key
            {
                return PutKey(current, next);
            }
            else // value
            {
                current.Key = "";
                current.Value = next;
                current.Complete();
                return ParserStates.Init;
            }
        }
    }

    class KeyState : IParserStateMachine
    {
        public void End(KeyValueArgumentBuilder current)
        {
            current.Complete();
        }

        public IParserStateMachine Put(KeyValueArgumentBuilder current, string next)
        {
            if (next.StartsWith("-")) // key
            {
                current.Complete();
                return PutKey(current, next);
            }
            else // value
            {
                current.Value = next;
                current.Complete();
                return ParserStates.Init;
            }
        }
    }
}