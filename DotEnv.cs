using System;
using System.IO;
using System.Linq;

namespace CodeConverterTool;

public static class DotEnv
{
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split(
                '=',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
                continue;

            Environment.SetEnvironmentVariable(parts[0], String.Join("=", parts.Skip(1)));
        }
    }
}
