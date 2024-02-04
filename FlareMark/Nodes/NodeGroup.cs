using System.Text;
using FlareMark.Readers;

namespace FlareMark.Nodes;

public class NodeGroup(List<Node> nodes_) : Node {
    public List<Node> nodes => nodes_;
    public override string ToHtml(HtmlGenerator htmlGenerator) {
        var sb = new StringBuilder();
        foreach (var node in nodes) {
            sb.Append(node.ToHtml(htmlGenerator));
        }

        return sb.ToString();
    }
}