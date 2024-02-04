using System.Text;
using FlareMark.Readers;

namespace FlareMark.Nodes;

public class Paragraph(List<Node> children_, int indent = 0) : Node {
    public List<Node> children => children_;
    public override string ToHtml(HtmlGenerator htmlGenerator) {
        if (children.Count == 0) {
            return "<br>\n";
        }
        var sb = new StringBuilder();
        sb.Append("<p ").Append(htmlGenerator.ClassIndented(indent)).Append(">\n");
        foreach (var n in children) {
            sb.AppendIndented(n.ToHtml(htmlGenerator), htmlGenerator.indent);
        }

        sb.Append("\n</p>\n");
        return sb.ToString();
    }
}