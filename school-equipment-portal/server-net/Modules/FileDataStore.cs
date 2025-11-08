using System.Text.Json;
using SchoolEquipmentApi.DTOs;

namespace SchoolEquipmentApi.Modules
{
    /// <summary>
    /// File-based implementation of IDataStore
    /// Uses JSON file for data persistence - suitable for development and small deployments
    /// </summary>
    public class FileDataStore : IDataStore
    {
        private readonly string _filePath;
        private readonly object _lock = new();
        private readonly JsonSerializerOptions _writeOptions;
        private readonly JsonSerializerOptions _readOptions;

        public FileDataStore(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            
            _writeOptions = new JsonSerializerOptions 
            { 
                WriteIndented = true, 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };
            
            _readOptions = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            };
        }

        public Data Read()
        {
            lock (_lock)
            {
                try
                {
                    if (!File.Exists(_filePath))
                    {
                        var initialData = CreateInitialData();
                        WriteToFile(initialData);
                        return initialData;
                    }

                    var jsonContent = File.ReadAllText(_filePath);
                    if (string.IsNullOrWhiteSpace(jsonContent))
                    {
                        var initialData = CreateInitialData();
                        WriteToFile(initialData);
                        return initialData;
                    }

                    var data = JsonSerializer.Deserialize<Data>(jsonContent, _readOptions);
                    return data ?? CreateInitialData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading data file: {ex.Message}");
                    return CreateInitialData();
                }
            }
        }

        public void Write(Data data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            lock (_lock)
            {
                try
                {
                    WriteToFile(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error writing data file: {ex.Message}");
                    throw;
                }
            }
        }

        private void WriteToFile(Data data)
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write to temporary file first, then move to avoid corruption
            var tempFilePath = _filePath + ".tmp";
            var jsonContent = JsonSerializer.Serialize(data, _writeOptions);
            
            File.WriteAllText(tempFilePath, jsonContent);
            
            // Atomic move operation
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
            File.Move(tempFilePath, _filePath);
        }

        private static Data CreateInitialData()
        {
            return new Data
            {
                Users = new List<User>(),
                Sessions = new Dictionary<string, Session>(),
                Equipment = new List<Equipment>(),
                Requests = new List<BorrowRequest>()
            };
        }

        /// <summary>
        /// Get file-based storage information
        /// </summary>
        public FileInfo GetFileInfo()
        {
            return new FileInfo(_filePath);
        }

        /// <summary>
        /// Create a backup of the current data file
        /// </summary>
        public string CreateBackup()
        {
            lock (_lock)
            {
                if (!File.Exists(_filePath))
                    throw new FileNotFoundException("Data file does not exist");

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var backupPath = Path.ChangeExtension(_filePath, $".backup_{timestamp}.json");
                
                File.Copy(_filePath, backupPath);
                return backupPath;
            }
        }

        /// <summary>
        /// Restore data from a backup file
        /// </summary>
        public void RestoreFromBackup(string backupFilePath)
        {
            if (!File.Exists(backupFilePath))
                throw new FileNotFoundException("Backup file does not exist");

            lock (_lock)
            {
                File.Copy(backupFilePath, _filePath, overwrite: true);
            }
        }
    }
}