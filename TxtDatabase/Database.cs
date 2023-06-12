﻿using Newtonsoft.Json;

namespace TxtDatabase
{
    public class DatabaseService : DatabaseService<Attribute>, IDatabaseService //Change Attribute class to decide which attribute to put on members for the key of database lines
    {
    }

    public class DatabaseService<TAttribute> where TAttribute : Attribute
    {
        public string DatabaseFolder { get; set; }

        public DatabaseService()
        {
            DatabaseFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\DatabaseService";
            System.IO.Directory.CreateDirectory($"{DatabaseFolder}");
        }

        public void Add<T>(T obj)
        {
            var filePath = GetFilePath<T>();

            string[] lines = File.ReadAllLines(filePath);
            var serializedObject = JsonConvert.SerializeObject(obj);
            var line = $"{Accessor<T, TAttribute>.ReadIDValue};{serializedObject}";
            var linesList = lines.ToList();
            linesList.Add(line);
            lines = linesList.ToArray();

            File.WriteAllLines(filePath, lines);
        }

        public void Update<T>(T obj)
        {
            var filePath = GetFilePath<T>();

            string[] lines = File.ReadAllLines(filePath);

            var serializedObject = JsonConvert.SerializeObject(obj);
            var id = Accessor<T, TAttribute>.ReadIDValue(obj);
            int lineIndex = 0;
            var linesList = lines.ToList();
            foreach (var line in linesList)
            {
                if (line.Split(';')[0] == id)
                {
                    lines[lineIndex] = $"{id};{serializedObject}";
                }
                lineIndex++;
            }

            File.WriteAllLines(filePath, lines);
        }

        public List<T> Query<T>()
        {
            var filePath = GetFilePath<T>();

            string[] lines = File.ReadAllLines(filePath);

            List<T> table = new List<T>();

            foreach (var line in lines)
            {
                var deserializedObject = JsonConvert.DeserializeObject<T>(line);
                if (deserializedObject != null)
                {
                    table.Add(deserializedObject);
                }
            }

            return table.ToList();
        }

        public void Delete<T>(T obj)
        {
            var filePath = GetFilePath<T>();

            string[] lines = File.ReadAllLines(filePath);
            List<string> linesList = lines.ToList();

            var serializedObject = JsonConvert.SerializeObject(obj);
            foreach (string line in lines)
            {
                if (line.Split(';')[1] == serializedObject)
                {
                    linesList.Remove(line);
                    break;
                }
            }

            File.WriteAllLines(filePath, linesList.ToArray());
        }

        private string GetFilePath<T>()
        {
            var filePath = $"{DatabaseFolder}\\{DatabaseMapper<T>.TableName}";

            Console.WriteLine(filePath);
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            return filePath;
        }
    }

}