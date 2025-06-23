using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.Converters
{
    public class IntArrayToStringConverter : ValueConverter<int[], string>
    {
        public IntArrayToStringConverter()
            : base(
                v => string.Join(";", v),
                v => v.Split(";", StringSplitOptions.RemoveEmptyEntries)
                .Select(val => int.Parse(val))
                .ToArray())
        {
        }
    }
}
