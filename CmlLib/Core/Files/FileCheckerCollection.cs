using System;
using System.Collections;
using System.Collections.Generic;

namespace CmlLib.Core.Files;

public class FileCheckerCollection : IEnumerable<IFileChecker>
{
    private readonly List<IFileChecker> checkers;

    private AssetChecker? asset;

    private ClientChecker? client;

    private JavaChecker? java;

    private LibraryChecker? library;

    private LogChecker? log;

    public FileCheckerCollection()
    {
        checkers = new List<IFileChecker>(4);

        library = new LibraryChecker();
        asset = new AssetChecker();
        client = new ClientChecker();
        java = new JavaChecker();
        log = new LogChecker();

        checkers.Add(library);
        checkers.Add(asset);
        checkers.Add(client);
        checkers.Add(log);
        checkers.Add(java);
    }

    public IFileChecker this[int index] => checkers[index];

    public AssetChecker? AssetFileChecker
    {
        get => asset;
        set
        {
            if (asset != null)
                checkers.Remove(asset);

            asset = value;

            if (asset != null)
                checkers.Add(asset);
        }
    }

    public ClientChecker? ClientFileChecker
    {
        get => client;
        set
        {
            if (client != null)
                checkers.Remove(client);

            client = value;

            if (client != null)
                checkers.Add(client);
        }
    }

    public LibraryChecker? LibraryFileChecker
    {
        get => library;
        set
        {
            if (library != null)
                checkers.Remove(library);

            library = value;

            if (library != null)
                checkers.Add(library);
        }
    }

    public JavaChecker? JavaFileChecker
    {
        get => java;
        set
        {
            if (java != null)
                checkers.Remove(java);

            java = value;

            if (java != null)
                checkers.Add(java);
        }
    }

    public LogChecker? LogFileChecker
    {
        get => log;
        set
        {
            if (log != null)
                checkers.Remove(log);

            log = value;

            if (log != null)
                checkers.Add(log);
        }
    }

    public IEnumerator<IFileChecker> GetEnumerator()
    {
        return checkers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return checkers.GetEnumerator();
    }

    public void Add(IFileChecker item)
    {
        CheckArgument(item);
        checkers.Add(item);
    }

    public void AddRange(IEnumerable<IFileChecker?> items)
    {
        foreach (var item in items)
            if (item != null)
                Add(item);
    }

    public void Remove(IFileChecker item)
    {
        CheckArgument(item);
        checkers.Remove(item);
    }

    public void RemoveAt(int index)
    {
        var item = checkers[index];
        Remove(item);
    }

    public void Insert(int index, IFileChecker item)
    {
        CheckArgument(item);
        checkers.Insert(index, item);
    }

    private void CheckArgument(IFileChecker item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (item is LibraryChecker)
            throw new ArgumentException($"Set {nameof(LibraryFileChecker)} property.");
        if (item is AssetChecker)
            throw new ArgumentException($"Set {nameof(AssetFileChecker)} property.");
        if (item is ClientChecker)
            throw new ArgumentException($"Set {nameof(ClientFileChecker)} property.");
        if (item is JavaChecker)
            throw new ArgumentException($"Set {nameof(JavaFileChecker)} property.");
    }
}
