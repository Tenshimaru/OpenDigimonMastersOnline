using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;

public class IntListToStringConverter : ValueConverter<List<int>, string>
{
    public IntListToStringConverter(ConverterMappingHints? mappingHints = null)
        : base(
              v => ConvertToString(v),
              v => ConvertToIntList(v),
              mappingHints)
    {
    }

    private static string ConvertToString(List<int> value)
    {
        return string.Join(',', value);
    }

    private static List<int> ConvertToIntList(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new List<int>();
        }

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();
    }
}
