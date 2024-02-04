using System.Text;

namespace FlareMark;

public static class Utils {
    public static void AppendIndented(this StringBuilder sb, string str, int indent) {
        if (!str.Contains('\n')) {
            sb.Append(str);
            return;
        }
        for (var i = 0; i < indent; i++) {
            sb.Append(' ');
        }
        foreach (var chr in str) {
            if (chr == '\n') {
                sb.Append('\n');
                for (var i = 0; i < indent; i++) {
                    sb.Append(' ');
                }
            } else {
                sb.Append(chr);
            }
        }
    }
    

    public static string MergeTags(params string[] tags) {
        var strs = new Dictionary<string, StringBuilder>();

        foreach (var t in tags) {
            var s = t.Split("=", 1);
            if (!strs.TryGetValue(s[0], out var sb)) {
                sb = new StringBuilder();
                strs[s[0]] = sb;
            }
            
            sb.Append(s[1]).Append(' ');
        }
        
        var result = new StringBuilder();
        foreach (var (k, v) in strs) {
            v.Length--;
            result.Append(k).Append("=\"").Append(v).Append("\" ");
        }
        
        result.Length--;
        return result.ToString();
    }
}