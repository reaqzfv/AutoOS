using System.Text.RegularExpressions;

namespace AutoOS.Views.Settings.BIOS;

public class BiosSettingParser
{
    public static IEnumerable<BiosSettingModel> ParseFromStream(Stream stream)
    {
        using var reader = new StreamReader(stream);
        string line;
        BiosSettingModel current = null;
        bool readingOptions = false;
        int lineNumber = 0;

        while ((line = reader.ReadLine()) != null)
        {
            lineNumber++;
            line = line.Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (line.StartsWith("Setup Question", StringComparison.OrdinalIgnoreCase))
            {
                if (current != null)
                {
                    yield return current;
                }
                current = new BiosSettingModel
                {
                    Line = lineNumber
                };
                readingOptions = false;

                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                    current.SetupQuestion = parts[1].Trim().Replace('�', '™');
                continue;
            }

            if (current == null) continue;

            if (line.StartsWith("Help String", StringComparison.OrdinalIgnoreCase))
            {
                current.HelpString = FormatHelpString(line.Split('=', 2)[1].Trim());
                continue;
            }

            if (line.StartsWith("Token", StringComparison.OrdinalIgnoreCase))
            {
                current.Token = line.Split('=', 2)[1].Split("//")[0].Trim();
                continue;
            }

            if (line.StartsWith("Offset", StringComparison.OrdinalIgnoreCase))
            {
                current.Offset = line.Split('=', 2)[1].Trim();
                continue;
            }

            if (line.StartsWith("Width", StringComparison.OrdinalIgnoreCase))
            {
                current.Width = line.Split('=', 2)[1].Trim();
                continue;
            }

            if (line.StartsWith("BIOS Default", StringComparison.OrdinalIgnoreCase))
            {
                var part = line.Split('=', 2)[1].Split("//")[0].Trim();
                var match = Regex.Match(part, @"\[[^\]]+\](.+)");
                current.BiosDefault = match.Success ? match.Groups[1].Value.Trim() : ExtractValue(part);
                continue;
            }

            if (line.StartsWith("Value", StringComparison.OrdinalIgnoreCase))
            {
                var valuePart = line.Split('=', 2)[1].Split("//")[0].Trim();
                current.Value = ExtractValue(valuePart);
                continue;
            }

            if (line.StartsWith("Options", StringComparison.OrdinalIgnoreCase))
            {
                current.Options = [];
                readingOptions = true;
                var inline = line.Substring(line.IndexOf('=') + 1).Trim();
                if (!string.IsNullOrWhiteSpace(inline))
                    ParseOptionLine(inline, current.Options, current.Token);
                continue;
            }

            if (readingOptions)
            {
                if (line.StartsWith("[") || line.StartsWith("*["))
                {
                    ParseOptionLine(line, current.Options, current.Token);
                }
                else
                {
                    readingOptions = false;
                }
            }
        }

        if (current != null)
        {
            yield return current;
        }

        static string ExtractValue(string raw)
        {
            raw = raw.Trim();
            return raw.StartsWith("\"") && raw.EndsWith("\"") ? raw[1..^1]
                 : raw.StartsWith("<") && raw.EndsWith(">") ? raw[1..^1]
                 : raw;
        }

        static void ParseOptionLine(string line, List<Option> options, string groupName)
        {
            var match = Regex.Match(line, @"^\*?\[(\w+)\](.*)$");
            if (!match.Success) return;

            var isSelected = line.StartsWith("*");
            var index = match.Groups[1].Value.Trim();
            var label = match.Groups[2].Value.Trim();
            var commentIndex = label.IndexOf("//");
            if (commentIndex >= 0) label = label.Substring(0, commentIndex).Trim();

            options.Add(new Option
            {
                Index = index,
                Label = label,
                IsSelected = isSelected
            });
        }

        static string FormatHelpString(string help)
        {
            if (string.IsNullOrWhiteSpace(help))
                return "No help string";

            help = help.Trim();

            // replace � with °
            help = help.Replace('�', '°');

            //  new lines before bracketed markers like [Auto]:
            help = Regex.Replace(
                help,
                @"(?<!^)(?=\[\w.*?\]:)",
                "\n"
            );

            // insert newline before technical markers like Min.: Max.: etc.
            string[] markers = { "Min.:", "Max.:", "Standard:", "Increment:" };
            foreach (var marker in markers)
            {
                int index = help.IndexOf(marker);
                if (index > 0)
                {
                    var before = help.Substring(0, index).TrimEnd();
                    var after = help.Substring(index).TrimStart();
                    help = $"{before}\n{after}";
                    break;
                }
            }

            // new lines before lines starting with * or * followed by space
            help = Regex.Replace(help, @"(?<!^)(?=\*\s?)", "\n");

            // new line before NOTE: or Note:
            help = Regex.Replace(help, @"(?<!^)(?=NOTE:|Note:)", "\n");

            return help;
        }
    }
}