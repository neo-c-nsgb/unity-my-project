using System;
using System.Collections.Generic;
using UnityEngine;

public static class CSVLoader
{
    /// <summary>
    /// Parses a CSV-formatted text and returns each non-empty line as an array of fields.
    /// Handles quoted values ("" → one literal "), custom delimiters, and both CRLF/LF line breaks.
    /// </summary>
    public static List<string[]> LoadCSV(string csvText, char delimiter = ',')
    {
        var rows = new List<string[]>();
        var lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            rows.Add(ParseLine(line, delimiter));
        }

        return rows;
    }

    private static string[] ParseLine(string line, char delimiter)
    {
        var result = new List<string>();
        bool inQuotes = false;
        var value = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                // escaped quote
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    value += '"';
                    i++; // skip next
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == delimiter && !inQuotes)
            {
                result.Add(value);
                value = "";
            }
            else
            {
                value += c;
            }
        }
        result.Add(value);
        return result.ToArray();
    }
}
