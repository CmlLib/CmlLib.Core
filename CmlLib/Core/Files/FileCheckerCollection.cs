using System;
using System.Collections;
using System.Collections.Generic;

namespace CmlLib.Core.Files
{
    public class FileCheckerCollection : IEnumerable<IFileChecker>
    {
        public IFileChecker this[int index] => checkers[index];

        private readonly List<IFileChecker> checkers;

        private AssetChecker asset = new AssetChecker();
        public AssetChecker AssetFileChecker
        {
            get => asset;
            set
            {
                checkers.Remove(asset);
                asset = value;

                if (asset != null)
                    checkers.Add(asset);
            }
        }

        private ClientChecker client = new ClientChecker();
        public ClientChecker ClientFileChecker
        {
            get => client;
            set
            {
                checkers.Remove(client);
                client = value;

                if (client != null)
                    checkers.Add(client);
            }
        }

        private LibraryChecker library = new LibraryChecker();
        public LibraryChecker LibraryFileChecker
        {
            get => library;
            set
            {
                checkers.Remove(library);
                library = value;

                if (library != null)
                    checkers.Add(library);
            }
        }

        public FileCheckerCollection()
        {
            checkers = new List<IFileChecker>(3);

            checkers.AddRange(new IFileChecker[]
            {
                LibraryFileChecker, AssetFileChecker, ClientFileChecker
            });
        }

        public void Add(IFileChecker item)
        {
            CheckArgument(item);
            checkers.Add(item);
        }

        public void AddRange(IEnumerable<IFileChecker> items)
        {
            foreach (IFileChecker item in items)
            {
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
