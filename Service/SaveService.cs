using System;
using System.IO;
using System.Text.Json;
using WpfApp1.GameCore;

namespace WpfApp1.Service
{
    public static class SaveService
    {
        private static string ProjectRoot =>
            Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."));

        private static string SaveFolder =>
            Path.Combine(ProjectRoot, "Saves");

        private static string SavePath =>
            Path.Combine(SaveFolder, "save.json");

        public static bool SaveExists()
        {
            return File.Exists(SavePath);
        }

        public static void Save(GameSave save)
        {
            Directory.CreateDirectory(SaveFolder);

            var json = JsonSerializer.Serialize(save, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(SavePath, json);
        }

        public static GameSave Load()
        {
            if (!SaveExists())
                throw new FileNotFoundException("Файл сохранения не найден");

            var json = File.ReadAllText(SavePath);
            return JsonSerializer.Deserialize<GameSave>(json)
                   ?? throw new InvalidOperationException("Сохранение повреждено");
        }
    }
}
