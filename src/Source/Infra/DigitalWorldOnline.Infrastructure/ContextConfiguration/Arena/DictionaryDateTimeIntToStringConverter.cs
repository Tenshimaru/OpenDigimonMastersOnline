using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Arena
{
    internal class DictionaryDateTimeIntToStringConverter : ValueConverter<Dictionary<DateTime, int>, string>
    {
        public DictionaryDateTimeIntToStringConverter(ConverterMappingHints? mappingHints = null)
            : base(
                v => ConvertToString(v),
                v => ConvertToDictionary(v),
                mappingHints)
        {
        }

        private static string ConvertToString(Dictionary<DateTime, int> dictionary)
        {
            return string.Join(";", dictionary.Select(kv => $"{kv.Key:O}:{kv.Value}"));
        }

        private static Dictionary<DateTime, int> ConvertToDictionary(string stringValue)
        {
            var keyValuePairs = stringValue.Split(';')
                .Select(pair => pair.Split(':'))
                .Where(parts => parts.Length == 2)
                .Select(parts => new KeyValuePair<DateTime, int>(DateTime.Parse(parts[0]), int.Parse(parts[1])))
                .ToList();

            return new Dictionary<DateTime, int>(keyValuePairs);
        }
    }
}
