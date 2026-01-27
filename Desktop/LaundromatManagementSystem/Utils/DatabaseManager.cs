using LaundromatManagementSystem.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LaundromatManagementSystem.Utils
{
    public static class DatabaseManager
    {
        public static string GetDatabasePath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, "laundromat.db3");
        }

        public static async Task<bool> DatabaseExistsAsync()
        {
            var databasePath = GetDatabasePath();
            return File.Exists(databasePath);
        }

        public static async Task BackupDatabaseAsync(string backupPath)
        {
            var databasePath = GetDatabasePath();
            if (File.Exists(databasePath))
            {
                File.Copy(databasePath, backupPath, true);
            }
        }

        public static async Task RestoreDatabaseAsync(string backupPath)
        {
            var databasePath = GetDatabasePath();
            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, databasePath, true);
            }
        }

        public static async Task<long> GetDatabaseSizeAsync()
        {
            var databasePath = GetDatabasePath();
            if (File.Exists(databasePath))
            {
                var fileInfo = new FileInfo(databasePath);
                return fileInfo.Length;
            }
            return 0;
        }

        public static async Task<List<string>> GetTableNamesAsync()
        {
            var tableNames = new List<string>();
            var databasePath = GetDatabasePath();

            if (File.Exists(databasePath))
            {
                var connectionString = $"Data Source={databasePath}";
                
                using var connection = new SqliteConnection(connectionString);
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT name FROM sqlite_master 
                    WHERE type='table' AND name NOT LIKE 'sqlite_%'
                    ORDER BY name";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    tableNames.Add(reader.GetString(0));
                }
            }

            return tableNames;
        }
    }
}