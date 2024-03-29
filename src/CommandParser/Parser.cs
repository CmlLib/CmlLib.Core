namespace CmlLib.Core.CommandParser;

public static class Parser
{
    public static IEnumerable<KeyValueArgument> ParseArgumentsToKeyValue(IEnumerable<string> args)
    {
        IParserStateMachine state = ParserStates.Init;
        KeyValueArgumentBuilder current = new();

        foreach (var arg in args)
        {
            state = state.Put(current, arg);

            foreach (var parsed in current.PopArguments())
                yield return parsed;
        }

        state.End(current);
        foreach (var parsed in current.PopArguments())
            yield return parsed;
    }

    public static IEnumerable<string> ParseCommandLineToArguments(string input)
    {
        ICommandLineStateMachine state = CommandLineStates.Init;
        ArgumentBuilder current = new();

        foreach (char c in input)
        {
            state = state.Put(current, c);

            foreach (var parsed in current.PopArguments())
                yield return parsed;
        }

        state.End(current);
        foreach (var parsed in current.PopArguments())
            yield return parsed;
    }
}