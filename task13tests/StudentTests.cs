using System;
using System.Collections.Generic;
using System.Text;
using task13;

namespace task13tests
{
    public class StudentTests
    {
        [Fact]
        public void Deserialize_EmptyJsonString_ShouldThrowArgumentException()
        {
            string emptyJson = "";

            var thrownException = Assert.Throws<ArgumentException>(() => StudentService.DeserializeStudent(emptyJson));
            Assert.Contains("JSON строка не может быть пустой", thrownException.Message);
        }

        [Fact]
        public void SaveFile_NullStudent_ShouldThrowArgumentNullException()
        {
            string tempFile = Path.GetTempFileName();

            Assert.Throws<ArgumentNullException>(() => StudentService.SaveFile(tempFile, null));

            File.Delete(tempFile);
        }

        [Fact]
        public void LoadFile_EmptyFile_ShouldThrowInvalidDataException()
        {
            string tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "");

            var thrownException = Assert.Throws<InvalidDataException>(() => StudentService.LoadFile(tempFile));
            Assert.Contains("Файл пуст", thrownException.Message);

            File.Delete(tempFile);
        }
    }
}
