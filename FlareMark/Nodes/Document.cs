using System.Text;
using FlareMark.Readers;

namespace FlareMark.Nodes;

public class Document(List<Node> children_) : Node {
    public List<Node> children => children_;
    
    public override string ToHtml(HtmlGenerator htmlGenerator) {
        var sb = new StringBuilder();
        
        foreach (var child in children) {
            sb.Append(child.ToHtml(htmlGenerator));
        }
        
        return sb.ToString();
    }
}