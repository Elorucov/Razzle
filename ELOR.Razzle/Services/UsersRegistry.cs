using System.Text.Json;

namespace ELOR.Razzle.Services
{
    public sealed class UsersRegistry
    {
        private readonly string _path;
        private readonly object _lock = new();
        private Dictionary<string, string> _users = new(StringComparer.OrdinalIgnoreCase);

        public UsersRegistry(string dataDir)
        {
            _path = Path.Combine(dataDir, "_users.json");
            Load();
        }

        private void Load()
        {
            if (!File.Exists(_path)) return;

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(_path));
            if (data is null) return;

            _users = data;
        }

        public bool Exists(string login)
        {
            lock (_lock)
            {
                return _users.ContainsKey(login);
            }
        }

        public bool TryGetId(string login, out string storageName)
        {
            lock (_lock)
            {
                return _users.TryGetValue(login, out storageName);
            }
        }

        public string Add(string username)
        {
            lock (_lock)
            {
                if (_users.ContainsKey(username)) throw new InvalidOperationException("Login already registered.");

                var storageName = Guid.CreateVersion7().ToString("N");
                _users[username] = storageName;
                Save();
                return storageName;
            }
        }

        // Returns the existing id, or assigns and persists a new one.
        //public uint GetOrAdd(string login)
        //{
        //    lock (_lock)
        //    {
        //        if (_users.TryGetValue(login, out var existing))
        //        {
        //            return existing;
        //        }

        //        var id = _nextId++;
        //        _users[login] = id;
        //        Save();
        //        return id;
        //    }
        //}

        private void Save()
        {
            File.WriteAllText(_path, JsonSerializer.Serialize(_users));
        }
    }
}
