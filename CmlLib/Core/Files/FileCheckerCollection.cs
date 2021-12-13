using System;
using System.Collections;
using System.Collections.Generic;

namespace CmlLib.Core.Files
{
    public class FileCheckerCollection : IEnumerable<IFileChecker>
    {
        public IFileChecker this[int index] => checkers[index];

        private readonly List<IFileChecker> checkers;

        private AssetChecker? asset;
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

        private ClientChecker? client;
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

        private LibraryChecker? library;
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

        private JavaChecker? java;

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

        private LogChecker? log;
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

        public void Add(IFileChecker item)
        {
            CheckArgument(item);
            checkers.Add(item);
        }

        public void AddRange(IEnumerable<IFileChecker?> items)
        {
            foreach (IFileChecker? item in items)
            {
                if (item != null)
                    Add(item);
            }
        }

        public void Remove(IFileChecker item)
        {
            CheckArgument(item);
            checkers.Remove(item);
        }

        public void RemoveAt(int index)
        {
            IFileChecker item = checkers[index];
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

        public IEnumerator<IFileChecker> GetEnumerator()
        {
            return checkers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return checkers.GetEnumerator();
        }
    }
}
