using System.Collections.Generic;
using System.IO;
using System.Text;
using CherrySeed.EntityDataProvider;
using CsvHelper;

namespace CherrySeed.DataProviders.Csv
{
    class CsvFile
    {
        private readonly string _filePath;
        private readonly string _delimiter;
        private readonly Encoding _encoding;

        public CsvFile(string filePath, string delimiter, Encoding encoding)
        {
            _filePath = filePath;
            _delimiter = delimiter;
            _encoding = encoding;
        }

        public string FileName => Path.GetFileNameWithoutExtension(_filePath);

        public EntityData ReadFile()
        {
            return new EntityData
            {
                EntityName = FileName,
                Objects = ReadDataFromFile()
            };
        }

        private List<Dictionary<string, string>> ReadDataFromFile()
        {
            var textReader = File.OpenText(_filePath);
            var csvReader = new CsvReader(textReader);

            var entityDicts = new List<Dictionary<string, string>>();
            csvReader.Read();
            csvReader.ReadHeader();
            var fieldNames = csvReader.Context.HeaderRecord;
            while (csvReader.Read())
            {

                // read one row
                var entityDict = new Dictionary<string, string>();

                foreach (var fieldName in fieldNames)
                {
                    var fieldValue = csvReader.GetField<string>(fieldName);
                    entityDict.Add(fieldName, fieldValue);
                }

                entityDicts.Add(entityDict);
            }

            return entityDicts;
        }
    }
}