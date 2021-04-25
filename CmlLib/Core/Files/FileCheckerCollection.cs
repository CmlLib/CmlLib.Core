using System;
using System.Collections;
using System.Collections.Generic;

namespace CmlLib.Core.Files
{
    public class FileCheckerCollection : IEnumerable<IFileChecker>
    {
        public IFileChecker this[int index] => checkers[index];

        private readonly List<IFileChecker> checkers;

        private AssetChecker _asset = new AssetChecker();
        public AssetChecker AssetFileChecker
        {
            get => _asset;
            set
            {
                checkers.Remove(_asset);
                _asset = value;

                if (_asset != null)
                    checkers.Add(_asset);
            }
        }

        private ClientChecker _client = new ClientChecker();
        public ClientChecker ClientFileChecker
        {
            get => _client;
            set
            {
                checkers.Remove(_client);
                _client = value;

                if (_client != null)
                    checkers.Add(_client);
            }
        }

        private LibraryChecker _library = new LibraryChecker();
        public LibraryChecker LibraryFileChecker
        {
            get => _library;
            set
            {
                checkers.Remove(_library);
                _library = value;

                if (_library != null)
                    checkers.Add(_library);
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
            throw new NotImplementedException();
        }
    }
}
