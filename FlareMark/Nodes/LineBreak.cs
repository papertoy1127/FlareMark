using FlareMark.Readers;

namespace FlareMark.Nodes;

public class LineBreak : Node {
    public override string ToHtml(HtmlGenerator htmlGenerator) {
        return "<br>";
    }
}