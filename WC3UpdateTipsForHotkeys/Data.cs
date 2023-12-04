using System.Text.RegularExpressions;

namespace WC3UpdateTipsForHotkeys;

public class Data
{
    private string? _hotkey;
    private string? _unHotkey;
    private string? _researchHotkey;

    public string? Comment { get; set; }

    public string? Hotkey
    {
        get => _hotkey;
        set
        {
            if (_hotkey != null && value != null)
            {
                if (Tip != null)
                    Tip = ReplaceHotkeys(Tip, _hotkey, value);
                if (ReviveTip != null)
                    ReviveTip = ReplaceHotkeys(ReviveTip, _hotkey, value);
                if (AwakenTip != null)
                    AwakenTip = ReplaceHotkeys(AwakenTip, _hotkey, value);
            }
            if (_hotkey == _unHotkey)
                UnHotkey = value;
            if (_researchHotkey == _unHotkey)
                ResearchHotkey = value;
            _hotkey = value;
        }
    }

    public string? UnHotkey
    {
        get => _unHotkey;
        set
        {
            if (UnTip != null && _unHotkey != null && value != null)
                UnTip = ReplaceHotkeys(UnTip, _unHotkey, value);
            _unHotkey = value;
        }
    }

    public string? ResearchHotkey
    {
        get => _researchHotkey;
        set
        {
            if (ResearchTip != null && _researchHotkey != null && value != null)
                ResearchTip = ReplaceHotkeys(ResearchTip, _researchHotkey, value);
            _researchHotkey = value;
        }
    }

    public string? ButtonPos { get; set; }
    public string? UnButtonPos { get; set; }
    public string? ResearchButtonPos { get; set; }
    
    public string? Tip { get; set; }
    public string? ReviveTip { get; set; }
    public string? AwakenTip { get; set; }
    public string? UnTip { get; set; }
    public string? ResearchTip { get; set; }

    public void UpdateFromOtherData(Data oth)
    {
        Tip = oth.Tip ?? Tip;
        UnTip = oth.UnTip ?? UnTip;
        ReviveTip = oth.ReviveTip ?? ReviveTip;
        AwakenTip = oth.AwakenTip ?? AwakenTip;
        ResearchTip = oth.ResearchTip ?? ResearchTip;
        
        Comment = oth.Comment ?? Comment;
        Hotkey = oth.Hotkey ?? Hotkey;
        UnHotkey = oth.UnHotkey ?? UnHotkey;
        ButtonPos = oth.ButtonPos ?? ButtonPos;
        UnButtonPos = oth.UnButtonPos ?? UnButtonPos;
        ResearchHotkey = oth.ResearchHotkey ?? ResearchHotkey;
        ResearchButtonPos = oth.ResearchButtonPos ?? ResearchButtonPos;
    }

    private string ReplaceHotkeys(string str, string oldHotkey, string newHotkey)
    {
        string[] oldHotkeys = oldHotkey.Split(",").Select(s => s.Trim()).ToArray();
        string[] newHotkeys = newHotkey.Split(",").Select(s => s.Trim()).ToArray();
        if (oldHotkeys.Length > newHotkeys.Length)
            throw new ArgumentException("OldHotkeys len is greater than newHotkeys");
        int startIndex = 0;
        const string patternStart = "|cffffcc00";
        for (int i = 0; i < oldHotkeys.Length; i++)
        {
            string from = $"|cffffcc00{GetHotkeyValue(oldHotkeys[i])}|r";
            string to = $"|cffffcc00{GetHotkeyValue(newHotkeys[i])}|r";
            int searchStart = str.IndexOf(patternStart + GetHotkeyValue(oldHotkeys[i]), startIndex,
                StringComparison.Ordinal);
            if(searchStart == -1)
                continue;
            var regex = new Regex(Regex.Escape(from));
            str = string.Concat(str.AsSpan(0, searchStart), regex.Replace(str[searchStart..], to, 1));
            startIndex = searchStart + 1;
        }

        return str;
    }

    private string GetHotkeyValue(string hotkey)
    {
        if (hotkey == "512")
            return "ESC";
        return hotkey[0].ToString().ToUpper();
    }
}