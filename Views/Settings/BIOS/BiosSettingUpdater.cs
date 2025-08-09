using System.Text.RegularExpressions;

namespace AutoOS.Views.Settings.BIOS;

public static class BiosSettingUpdater
{
    public static bool IsBatchUpdating { get; set; }

    public static void SaveSingleSetting(BiosSettingModel setting)
    {
        // get lines from nvram
        var lines = setting.OriginalLines;

        // update settings
        if (setting.HasValueField)
        {
            UpdateValue(setting, lines);
        }
        else if (setting.HasOptions)
        {
            UpdateOption(setting, lines);
        }

        // write changes
        File.WriteAllLines(Path.Combine(PathHelper.GetAppDataFolderPath(), "SCEWIN", "nvram.txt"), lines);
    }

    public static void SaveAllSettings(IEnumerable<BiosSettingModel> modifiedSettings)
    {
        // get lines from nvram
        var lines = modifiedSettings.First().OriginalLines;

        // update settings
        foreach (var setting in modifiedSettings)
        {
            if (setting.HasValueField)
            {
                UpdateValue(setting, lines);
            }
            else if (setting.HasOptions)
            {
                UpdateOption(setting, lines);
            }
        }

        // write changes
        File.WriteAllLines(Path.Combine(PathHelper.GetAppDataFolderPath(), "SCEWIN", "nvram.txt"), lines);

        // reset is modified
        foreach (var setting in modifiedSettings)
            setting.IsModified = false;
    }

    public static void UpdateValue(BiosSettingModel setting, List<string> lines = null)
    {
        int startLineIndex = setting.Line;

        if (startLineIndex < 0 || startLineIndex >= lines.Count)
            return;

        int valueLineIndex = -1;
        for (int i = startLineIndex; i < lines.Count; i++)
        {
            if (lines[i].TrimStart().StartsWith("Value", StringComparison.OrdinalIgnoreCase))
            {
                valueLineIndex = i;
                break;
            }
        }
        if (valueLineIndex == -1)
            return;

        string line = lines[valueLineIndex];
        int commentIndex = line.IndexOf("//");

        string commentPart = commentIndex >= 0
            ? line[(commentIndex - (line[commentIndex - 1] == ' ' ? 1 : 0))..]
            : "";

        string valuePart = commentIndex >= 0
            ? line[..commentIndex].TrimEnd()
            : line.TrimEnd();

        int equalsIndex = valuePart.IndexOf('=');

        string prefix = valuePart[..(equalsIndex + 1)].TrimEnd();
        string originalValueText = valuePart[(equalsIndex + 1)..].Trim();

        string newValue = setting.Value ?? "";

        if ((originalValueText.StartsWith('\"') && originalValueText.EndsWith('\"')) ||
            (originalValueText.StartsWith('<') && originalValueText.EndsWith('>')) ||
            (originalValueText.StartsWith('{') && originalValueText.EndsWith('}')))
        {
            string inner = originalValueText[1..^1];
            int trailingSpacesCount = inner.Length - inner.TrimEnd().Length;
            string trailingSpaces = new(' ', trailingSpacesCount);

            newValue = $"{originalValueText[0]}{newValue}{trailingSpaces}{originalValueText[^1]}";
        }

        lines[valueLineIndex] = $"{prefix}{newValue}{commentPart}".TrimEnd();
    }

    public static void UpdateOption(BiosSettingModel setting, List<string> lines = null)
    {
        int start = setting.Line;
        if (start < 0 || start >= lines.Count) return;

        int optionsIdx = -1;
        for (int i = start; i < lines.Count; i++)
            if (lines[i].TrimStart().StartsWith("Options", StringComparison.OrdinalIgnoreCase))
            {
                optionsIdx = i;
                break;
            }

        string optLine = lines[optionsIdx];
        int cIdx = optLine.IndexOf("//");
        string comment = "";
        string optionsPart = optLine;
        if (cIdx >= 0)
        {
            int startComment = cIdx;
            while (startComment > 0 && char.IsWhiteSpace(optLine[startComment - 1])) startComment--;
            comment = optLine[startComment..];
            optionsPart = optLine[..startComment];
        }

        int eq = optionsPart.IndexOf('=');
        if (eq < 0) return;
        string prefix = optionsPart[..(eq + 1)];
        string optionsText = optionsPart[(eq + 1)..];

        var matches = Regex.Matches(optionsText, @"(\*?\[\w+\][^\[\]\n\r\t\f\v]*)");
        var newParts = new List<string>(matches.Count);

        foreach (Match m in matches)
        {
            string opt = m.Value;
            var idm = Regex.Match(opt, @"\*?\[(\w+)\]");
            string idx = idm.Success ? idm.Groups[1].Value : null;
            string withoutStar = opt.TrimStart('*');

            if (setting.SelectedOption == null)
            {
                newParts.Add(opt);
                continue;
            }

            if (idx == setting.SelectedOption.Index)
            {
                if (!opt.StartsWith('*'))
                    opt = "*" + withoutStar;
            }
            else if (opt.StartsWith('*'))
            {
                opt = withoutStar;
            }

            newParts.Add(opt);
        }

        lines[optionsIdx] = prefix + string.Join(" ", newParts) + comment;

        int ptr = optionsIdx + 1;
        while (ptr < lines.Count)
        {
            string original = lines[ptr];
            string trimmed = original.TrimStart();

            if (trimmed.StartsWith('[') || trimmed.StartsWith("*["))
            {
                var idxM = Regex.Match(trimmed, @"^\*?\[(\w+)\]");
                string idx = idxM.Success ? idxM.Groups[1].Value : null;
                string indent = original[..(original.Length - trimmed.Length)];
                string withoutStar = trimmed.StartsWith('*') ? trimmed[1..] : trimmed;

                lines[ptr] = (setting.SelectedOption != null && idx == setting.SelectedOption.Index)
                             ? indent + "*" + withoutStar
                             : indent + withoutStar;
                ptr++;
                continue;
            }

            break;
        }
    }
}