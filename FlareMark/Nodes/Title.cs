using System.Text;
using FlareMark.Readers;

namespace FlareMark.Nodes;

public class Title(int level_, List<Node> title_) : Node {
    public int level => level_;
    public List<Node> title => title_;
    public override string ToHtml(HtmlGenerator htmlGenerator) {
        var sb = new StringBuilder();
        sb.Append("<h").Append(level).Append('>');
        foreach (var n in title) {
            var t = n.ToHtml(htmlGenerator);
            sb.Append(t);
            Console.WriteLine(t);
        }
        sb.Append("</h").Append(level).Append(">\n");
        Console.WriteLine(sb.ToString());
        Console.WriteLine();
        return sb.ToString();
    }
}