using CsvHelper;
using System.Globalization;
using System.IO;
using TransactionTestCase.Contracts;
using TransactionTestCase.Entity;

namespace TransactionTestCase.Services
{
    public class CSVService : ICSVService
    {
        public IEnumerable<Transaction> ReadCSV(Stream file)
        {
            using (var reader = new StreamReader(file))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csvReader.GetRecords<Transaction>().ToList();
            }
        }

        public void  WriteToCSV(IEnumerable<Transaction> transactions,string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(transactions);
            }
        }
    }
}
