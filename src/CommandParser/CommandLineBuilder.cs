using System.Text;

namespace CmlLib.Core.CommandParser;

public class CommandLineBuilder
{
    private readonly StringBuilder _sb = new();
    private List<KeyValueArgument> _arguments = new();

    public IEnumerable<KeyValueArgument> Arguments => _arguments;

    public void AddCommandLine(string cmd)
    {
        if (string.IsNullOrEmpty(cmd))
            return;

        var parsedArgs = Parser.ParseCommandLineToArguments(cmd);
        var parsedKv = Parser.ParseArgumentsToKeyValue(parsedArgs);
        foreach (var kv in parsedKv)
        {
            _arguments.Add(kv);
        }

        _sb.Append(cmd);
        _sb.Append(" ");
    }

    public void AddArguments(IEnumerable<string> args)
    {
        args = args.Select(arg => 
        {
            _sb.Append(KeyValueArgument.Escape(arg));
            _sb.Append(" ");
            return arg;
        });

        var parsedKv = Parser.ParseArgumentsToKeyValue(args);
        foreach (var kv in parsedKv)
        {
            _arguments.Add(kv);
        }
    }

    public void AddKeyValueArgument(KeyValueArgument arg)
    {
        _arguments.Add(arg);

        var argStr = arg.ToString();
        if (!string.IsNullOrEmpty(argStr))
        {
            _sb.Append(arg.ToString());
            _sb.Append(" ");
        }
    }

    public bool ContainsKey(string key)
    {
        return _arguments.Any(arg => arg.Key == key);
    }

    public IEnumerable<KeyValueArgument> Find(string key)
    {
        return _arguments.Where(arg => arg.Key == key);
    }

    public string Build()
    {
        if (_sb.Length > 0)
        {
            // Add 메서드는 항상 _sb 끝에 공백 문자를 추가하기 때문에,
            // 마지막으로 추가된 공백 문자를 지우고 문자열을 만든다.
            // 예를 들어 Add("a"); Add("b"); Add("c") 를 하면 _sb 안에는
            // "a b c " 가 들어있기에 마지막 공백을 지우고 "a b c" 를 반환한다.

            return _sb.ToString(0, _sb.Length - 1);
        }
        else
        {
            return "";
        }
    }
}