using System.Text;
using FlareMark.Readers;

namespace FlareMark.Nodes;

public class Tag(string tag_, List<Node> nodes_) : Node {
    public string tag => tag_;
    public List<Node> nodes => nodes_;
    public override string ToHtml(HtmlGenerator htmlGenerator) {
        var sb = new StringBuilder();
        sb.Append('<').Append(tag).Append('>');
        foreach (var n in nodes) {
            sb.Append(n.ToHtml(htmlGenerator));
        }
        sb.Append("</").Append(tag).Append('>');
        
        return sb.ToString();
    }
}