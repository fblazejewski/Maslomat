using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Maslomat.Models
{
    class PasswordDatabase
    {
        public CloudHandler CloudHandler { get; }

        readonly List<PasswordEntry> _items;
        readonly string _localRepoPath;

        public PasswordDatabase()
        {
            _items = new List<PasswordEntry>();
            _localRepoPath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "passwordsDatabase.json");

            if (!File.Exists(_localRepoPath))
                File.Create(_localRepoPath);

            CloudHandler = new CloudHandler(_items);
        }

        public async Task InitializeDBAsync()
        {
            _items.Clear();
            await LoadDatabaseAsync();
        }

        public async Task SyncPasswords()
        {
            if (await CloudHandler.SyncPasswords())
            {
                await SaveDatabaseLocallyAsync();
            }
        }

        private void FillRepoDummy()
        {
            for (int i = 0; i < 4; i++)
                _items.Add(new PasswordEntry($"Page {i + 1}", $"admin{i + 1}"));
        }

        private async Task SaveDatabaseLocallyAsync()
        {
            using (var file = File.Create(_localRepoPath))
            {
                await JsonSerializer.SerializeAsync(file, _items);
            }
        }

        private async Task LoadDatabaseAsync()
        {
            using (var reader = File.OpenRead(_localRepoPath))
            {
                try
                {
                    var items = await JsonSerializer.DeserializeAsync<List<PasswordEntry>>(reader);
                    _items.AddRange(items);
                }
                catch (Exception e)
                {
                    Debug.Print($"Error accessing the database file: {e}");
                }
            }
        }


        public async Task<IEnumerable<PasswordEntry>> GetAllItemsAsync()
        {
            return await Task.FromResult(_items);
        }

        public async Task<PasswordEntry> GetItemAsync(int id)
        {
            return await Task.FromResult(_items.Find(x => x.PassID == id));
        }

        public async Task<PasswordEntry> AddItemAsync(PasswordEntry newEntry)
        {
            if (_items.Exists(x => x.Designation == newEntry.Designation))
                return null;

            _items.Add(newEntry);

            await SaveDatabaseLocallyAsync();
            await CloudHandler.UploadPasswords(new List<PasswordRequest> { new PasswordRequest(newEntry) });

            return await Task.FromResult(newEntry);
        }

        public async Task<PasswordEntry> UpdateItemAsync(PasswordEntry newEntry)
        {
            var entry = _items.Find(x => x.PassID == newEntry.PassID);
            if (entry != null)
            {
                _items.Remove(entry);
                _items.Add(newEntry);
            }
            else
            {
                _items.Add(newEntry);
            }

            await SaveDatabaseLocallyAsync();
            await CloudHandler.UploadPasswords(new List<PasswordRequest> { new PasswordRequest(newEntry) });

            return await Task.FromResult(newEntry);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var entry = _items.Find(x => x.PassID == id);
            _items.Remove(entry);

            await SaveDatabaseLocallyAsync();
            await CloudHandler.DeletePassword(new PasswordRequest(entry));

            return await Task.FromResult(true);
        }

    }
}
