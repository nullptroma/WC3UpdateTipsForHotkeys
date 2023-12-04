using System.Text;

namespace WC3UpdateTipsForHotkeys;

internal static class Program
{
    private static Dictionary<string, Data> ReadFile(string path)
    {
        Dictionary<string, Data> res = new Dictionary<string, Data>();
        var sr = File.OpenText(path);
        string? id = null;
        Data cur = new Data();
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()?.Trim();
            if (line == null)
                continue;
            if (id != null && (line.StartsWith("//") || line.StartsWith('['))) // end read
            {
                res.Add(id, cur);
                cur = new Data();
                id = null;
            }

            if (line.StartsWith('['))
                id = line.Substring(1, line.IndexOf(']') - 1).ToLower();

            if (line.StartsWith("//")) cur.Comment = line.Substring(2).Trim();

            if (!line.Contains('=')) continue;
            var key = line.Substring(0, line.IndexOf('=')).ToLower();
            var value = line.Substring(line.IndexOf('=') + 1).Trim();
            if (value.Length == 0) value = null;
            if (key == "hotkey") cur.Hotkey = value?.ToUpper();
            if (key == "unhotkey") cur.UnHotkey = value?.ToUpper();
            if (key == "buttonpos") cur.ButtonPos = value;
            if (key == "unbuttonpos") cur.UnButtonPos = value;
            if (key == "researchhotkey") cur.ResearchHotkey = value?.ToUpper();
            if (key == "researchbuttonpos") cur.ResearchButtonPos = value;
            if (key == "tip") cur.Tip = value;
            if (key == "untip") cur.UnTip = value;
            if (key == "revivetip") cur.ReviveTip = value;
            if (key == "awakentip") cur.AwakenTip = value;
            if (key == "researchtip") cur.ResearchTip = value;
        }

        if (id != null) // end read
            res.Add(id, cur);
        return res;
    }

    private static void PrintIfNotNull(this StreamWriter sw, string name, string? value)
    {
        if (value == null)
            return;
        sw.WriteLine($"{name}={value}");
    }

    public static int Main(string[] args)
    {
        Console.Write("Tips file: ");
        string? tipsFile = Console.ReadLine();
        Console.Write("Hotkeys file: ");
        string? hotkeysFile = Console.ReadLine();
        if (tipsFile == null || hotkeysFile == null)
            return -1;
        Dictionary<string, Data> withTips = ReadFile(tipsFile);
        Dictionary<string, Data> withHotkeys = ReadFile(hotkeysFile);
        foreach (var over in withHotkeys)
        {
            if (withTips.ContainsKey(over.Key))
            {
                withTips[over.Key].ButtonPos = null;
                withTips[over.Key].UnButtonPos = null;
                withTips[over.Key].ResearchButtonPos = null;
                withTips[over.Key].UpdateFromOtherData(over.Value);
            }
            else
                withTips.Add(over.Key, over.Value);
        }

        string outPath = Path.Combine(Path.GetDirectoryName(hotkeysFile) ?? Environment.CurrentDirectory,
            "NewCustomKeys.txt");
        var output = File.CreateText(outPath);
        foreach (var res in withTips)
        {
            var val = res.Value;
            if (val.Comment != null)
                output.WriteLine($"// {val.Comment}");
            output.WriteLine($"[{res.Key}]");
            output.PrintIfNotNull(nameof(val.Hotkey), val.Hotkey);
            output.PrintIfNotNull(nameof(val.UnHotkey), val.UnHotkey);
            output.PrintIfNotNull(nameof(val.ResearchHotkey), val.ResearchHotkey);
            output.PrintIfNotNull(nameof(val.ButtonPos), val.ButtonPos);
            output.PrintIfNotNull(nameof(val.UnButtonPos), val.UnButtonPos);
            output.PrintIfNotNull(nameof(val.ResearchButtonPos), val.ResearchButtonPos);
            output.PrintIfNotNull(nameof(val.Tip), val.Tip);
            output.PrintIfNotNull(nameof(val.ReviveTip), val.ReviveTip);
            output.PrintIfNotNull(nameof(val.AwakenTip), val.AwakenTip);
            output.PrintIfNotNull(nameof(val.UnTip), val.UnTip);
            output.PrintIfNotNull(nameof(val.ResearchTip), val.ResearchTip);
            output.WriteLine();
        }

        output.Close();
        Console.WriteLine($"File saved in {outPath}");
        return 0;
    }
}