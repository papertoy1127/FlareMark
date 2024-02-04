using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using FlareMark.Nodes;

namespace FlareMark.Readers;

public class Reader {
    private static readonly Dictionary<MatchType, List<Type>> Checks = new();
    private static readonly Dictionary<Type, IReaderState> States = new();

    public static void RegisterState(IReaderState state) {
        var type = state.GetType();
        States.TryAdd(type, state);

        var match = state.GetType().GetCustomAttribute<MatchTypeAttribute>();
        if (match == null) return;

        foreach (var t in match.type) {
            Console.WriteLine(t);
            if (!Checks.TryGetValue(t, out var list)) {
                list = new List<Type>();
                Checks.Add(t, list);
            }
            
            list.Add(type);
        }
    }
    
    public static bool MatchChecks(MatchType type, ref IndexString source) {
        if (!Checks.TryGetValue(type, out var states)) return false;

        foreach (var state in states) {
            var s = States[state];
            if (s.Match(source)) return true;
        }

        return false;
    }

    public static bool ParseChecks(MatchType type, ref IndexString source, [MaybeNullWhen(false)] out Node output) {
        if (!Checks.TryGetValue(type, out var states)) {
            output = null;
            return false;
        }

        foreach (var state in states) {
            var s = States[state];
            if (!s.Match(source)) continue;

            output = Parse(ref source, state, true);
            return true;
        }

        output = null;
        return false;
    }

    public static Node Parse<T>(ref IndexString source, bool suppressError) where T : IReaderState {
        return Parse(ref source, typeof(T), suppressError);
    }
    
    public static Node Parse(ref IndexString source, Type type, bool suppressError) {
        if (suppressError) {
            try {
                return States[type].Process(ref source);
            } catch (Exception e) {
                return new Error(e);
            }
        }
        
        return States[type].Process(ref source);
    }

    public static Node Parse(string text, bool suppressError = true) {
        // CRLF to LF
        text = text.Replace("\r\n", "\n");
        var source = new IndexString(text);
        source.MoveNext();
        return Parse<StateDocument>(ref source, suppressError);
    }
}

public class HtmlGenerator {
    public int indent = 2;
    public readonly Dictionary<string, Dictionary<string, string>> classes = new();
    public bool inline = true;

    public string ClassIndented(int indentLevel) {
        if (!inline && classes.ContainsKey($"indent{indentLevel}")) return $"class=\"indent{indentLevel}\"";
        var dict = new Dictionary<string, string> {
            {"margin-left", $"{indentLevel * 5}pt"},
        };
        if (inline) return $"style=\"{GenerateCss(dict)}\"";
        classes.Add($"indent{indentLevel}", dict);
        
        return $"class=\"indent{indentLevel}\"";
    }

    public string GenerateCss(Dictionary<string, string> css) {
        var sb = new StringBuilder();
        foreach (var (k, v) in css) {
            sb.Append(k).Append(": ").Append(v).Append("; ");
        }
        sb.Length--;
        
        return sb.ToString();
    }
}
