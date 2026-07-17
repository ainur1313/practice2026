using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13
{
    public static class StudentService
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

        public static string SerializeStudent(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            student.Valid();

            return JsonSerializer.Serialize(student, Options);
        }

        public static Student DeserializeStudent(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON строка не может быть пустой.", nameof(json));

            Student? student;
            try
            {
                student = JsonSerializer.Deserialize<Student>(json, Options);
            }
            catch (JsonException ex)
            {
                throw new JsonException("Ошибка десериализации JSON.", ex);
            }

            if (student == null)
                throw new JsonException("Ошибка: десериализованный объект равен null.");

            student.Valid();

            return student;
        }

        public static Student LoadFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден.", filePath);

            string jsonContent;
            try
            {
                jsonContent = File.ReadAllText(filePath);
            }
            catch (IOException ex)
            {
                throw new IOException($"Ошибка чтения файла: {filePath}", ex);
            }

            if (string.IsNullOrWhiteSpace(jsonContent))
                throw new InvalidDataException("Файл пуст или содержит только пробельные символы.");

            return DeserializeStudent(jsonContent);
        }

        public static void SaveFile(string filePath, Student student)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));

            if (student == null)
                throw new ArgumentNullException(nameof(student));

            student.Valid();

            string jsonContent = SerializeStudent(student);

            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, jsonContent);
            }
            catch (IOException ex)
            {
                throw new IOException($"Ошибка сохранения файла: {filePath}", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"Нет прав для записи в файл: {filePath}", ex);
            }
        }
    }
}
