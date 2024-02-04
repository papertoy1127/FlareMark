// ReSharper disable InconsistentNaming
using System.Text;
using FlareMark.Nodes;

namespace FlareMark.Readers;

public enum MatchType {
    None,
    Document,
    Paragraph,
    Text,
}

public interface IReaderState {
    public bool Match(ReadOnlyIndexString source);
    public Node Process(ref IndexString source);
}

[AttributeUsage(AttributeTargets.Class)]
public class MatchTypeAttribute : Attribute {
    public MatchTypeAttribute() {
        type = [];
    }

    public MatchTypeAttribute(params MatchType[] type) {
        this.type = type;
    }
    public MatchTypeAttribute(int priority, params MatchType[] type) {
        this.type = type;
        this.priority = priority;
    }
    public MatchType[] type { get; }
    public int priority { get; } = 0;
}

[MatchType(MatchType.Document)] public class StateParagraph : IReaderState {
    public bool Match(ReadOnlyIndexString source) {
        return true;
    }

    public Node Process(ref IndexString source) {
        var chr = source[0];
        var nodes = new List<Node>();
        var sb = new StringBuilder();
        var indent = 0;
        while (chr == ' ') {
            indent++;
            source.MoveNext();
            chr = source[0];
        }

        do {
            chr = source[0];
            if (chr == '\n') break;
            var p = Reader.Parse<StateText>(ref source, true);
            if (p is Character c) {
                sb.Append(c.value);
            } else {
                if (sb.Length > 0) {
                    nodes.Add(new Text(sb.ToString()));
                    sb.Clear();
                }
                nodes.Add(p);
            }
        } while (chr != '\0');
        
        if (sb.Length > 0) {
            nodes.Add(new Text(sb.ToString()));
        }

        return new Paragraph(nodes, indent);
    }
}

public class StateText : IReaderState {
    public bool Match(ReadOnlyIndexString source) {
        return true;
    }

    public Node Process(ref IndexString source) {
        var chr = source[0];
        switch (chr) {
            case '\0':
                return Node.Empty;
            
            case '\\':
                source.MoveNext();
                return Reader.Parse<StateEscape>(ref source, true);

            case '\n':
                source.MoveNext();
                return new Text("<br>");

            default:
                if (Reader.ParseChecks(MatchType.Text, ref source, out var node))
                    return node;
                
                source.MoveNext();
                return new Character(chr);
        }
    }
}

[MatchType] public class StateEscape : IReaderState {
    public bool Match(ReadOnlyIndexString source) {
        return false;
    }

    public Node Process(ref IndexString source) {
        var chr = source[0];
        if (chr != '\\') throw new Exception("Invalid escape sequence");
        source.MoveNext();
        chr = source[0];

        switch (chr) {
            case 'n':
                return new Text("<br>");
            
            case 'u':
                var t4 = source[1..5];
                if (!int.TryParse(t4, System.Globalization.NumberStyles.HexNumber, null, out var hex))
                    throw new Exception("Invalid escape sequence");
                
                source.Move(4);
                return new Text(((char)hex).ToString());
            
            case 'U':
                var t8 = source[1..9];
                if (!int.TryParse(t8, System.Globalization.NumberStyles.HexNumber, null, out hex))
                    throw new Exception("Invalid escape sequence");
                
                source.Move(8);
                return new Text(((char)hex).ToString());
            
            case 'x':
                var sb = new StringBuilder();
                while (source.MoveNext()) {
                    chr = source[0];
                    if (chr is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F') {
                        sb.Append(chr);
                    } else break;
                }

                source.MovePrevious();
                
                if (sb.Length == 0) throw new Exception("Invalid escape sequence");
                hex = int.Parse(sb.ToString(), System.Globalization.NumberStyles.HexNumber);
                return new Text(((char)hex).ToString());
            
            default:    
                return new Text(chr.ToString());
        }
    }
}

[MatchType(MatchType.Document)] public class StateHeader : IReaderState {
    public bool Match(ReadOnlyIndexString source) {
        if (source[0] != '#') return false;
        for (int i = 1; i <= 5; i++) { // Max header length is 5
            if (source[i] != '#') return true;
        }

        return false;
    }

    public Node Process(ref IndexString source) {
        var level = 1;
        while (source.MoveNext()) {
            if (source[0] != '#') break;
            level++;
        }

        var chr = source[0];
        while (chr == ' ') {
            source.MoveNext();
            chr = source[0];
        }

        var nodes = new List<Node>();
        var sb = new StringBuilder();
        
        source.MovePrevious();
        while (source.MoveNext()) {
            chr = source[0];
            switch (chr) {
                case '\n':
                    PushText();
                    goto end;
                
                default:
                    sb.Append(chr);
                    break;
            }
        }
        
        end:
        if (nodes.Count == 0) return new Text(new string('#', level));
        return new Title(level, nodes);

        void PushText() {
            if (sb.Length == 0) return;
            var str = sb.ToString();
            nodes.Add(new Text(str));
            sb.Clear();
        }
    }
}

public class StateDocument : IReaderState {
    public bool Match(ReadOnlyIndexString source) {
        return false;
    }

    public Node Process(ref IndexString source) {
        Console.WriteLine(source[0]);
        
        if (source[0] == '\0') return Node.Empty;
        var nodes = new List<Node>();
        do {
            if (Reader.ParseChecks(MatchType.Document, ref source, out var node)) {
                nodes.Add(node);
            } else {
                throw new Exception("Invalid document");
            }
        } while (source.MoveNext());

        return new NodeGroup(nodes);
    }
}

[MatchType(MatchType.Text)] public class StateBold  : IReaderState {
    public bool Match(ReadOnlyIndexString source) {
        return source.Match("**");
    }

    public Node Process(ref IndexString source) {
        source.Move(2);
        var chr = source[0];
        var nodes = new List<Node>();
        var sb = new StringBuilder();
        var indent = 0;
        
        do {
            chr = source[0];
            if (chr == '\n') break;
            if (source.Match("**")) {
                source.Move(2);
                break;
            }
            var p = Reader.Parse<StateText>(ref source, true);
            if (p is Character c) {
                sb.Append(c.value);
            } else {
                if (sb.Length > 0) {
                    nodes.Add(new Text(sb.ToString()));
                    sb.Clear();
                }
                nodes.Add(p);
            }
        } while (chr != '\0');
        
        if (sb.Length > 0) {
            nodes.Add(new Text(sb.ToString()));
        }

        return new Tag("b", nodes);
    }
}

[MatchType(MatchType.Text)] public class StateItalic  : IReaderState {
    public bool Match(ReadOnlyIndexString source) {
        return source.Match("*") || source.Match("_");
    }

    public Node Process(ref IndexString source) {
        var match = source[..1];
        source.MoveNext();
        var chr = source[0];
        var nodes = new List<Node>();
        var sb = new StringBuilder();
        var indent = 0;
        
        do {
            chr = source[0];
            if (chr == '\n') break;
            if (source.Match(match)) {
                source.MoveNext();
                break;
            }
            var p = Reader.Parse<StateText>(ref source, true);
            if (p is Character c) {
                sb.Append(c.value);
            } else {
                if (sb.Length > 0) {
                    nodes.Add(new Text(sb.ToString()));
                    sb.Clear();
                }
                nodes.Add(p);
            }
        } while (chr != '\0');
        
        if (sb.Length > 0) {
            nodes.Add(new Text(sb.ToString()));
        }

        return new Tag("i", nodes);
    }
}

[MatchType(MatchType.Text)] public class StateStrikethrough  : IReaderState {
    public bool Match(ReadOnlyIndexString source) {
        return source.Match("~~");
    }

    public Node Process(ref IndexString source) {
        source.Move(2);
        var chr = source[0];
        var nodes = new List<Node>();
        var sb = new StringBuilder();
        var indent = 0;
        
        do {
            chr = source[0];
            if (chr == '\n') break;
            if (source.Match("~~")) {
                source.Move(2);
                break;
            }
            var p = Reader.Parse<StateText>(ref source, true);
            if (p is Character c) {
                sb.Append(c.value);
            } else {
                if (sb.Length > 0) {
                    nodes.Add(new Text(sb.ToString()));
                    sb.Clear();
                }
                nodes.Add(p);
            }
        } while (chr != '\0');
        
        if (sb.Length > 0) {
            nodes.Add(new Text(sb.ToString()));
        }

        return new Tag("del", nodes);
    }
}

[MatchType(MatchType.Text)] public class StateUnderline : IReaderState {
    public bool Match(ReadOnlyIndexString source) {
        return source.Match("__");
    }

    public Node Process(ref IndexString source) {
        source.Move(2);
        var chr = source[0];
        var nodes = new List<Node>();
        var sb = new StringBuilder();
        var indent = 0;
        
        do {
            chr = source[0];
            if (chr == '\n') break;
            if (source.Match("__")) {
                source.Move(2);
                break;
            }
            var p = Reader.Parse<StateText>(ref source, true);
            if (p is Character c) {
                sb.Append(c.value);
            } else {
                if (sb.Length > 0) {
                    nodes.Add(new Text(sb.ToString()));
                    sb.Clear();
                }
                nodes.Add(p);
            }
        } while (chr != '\0');
        
        if (sb.Length > 0) {
            nodes.Add(new Text(sb.ToString()));
        }

        return new Tag("u", nodes);
    }
}