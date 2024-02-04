using FlareMark.Readers;

namespace FlareMark.Nodes;

public class Text(string value_) : Node {
    public string value => value_;
    public override string ToHtml(HtmlGenerator htmlGenerator) => value;
}

public class Character(char value_) : Node {
    public char value => value_;

    public override string ToHtml(HtmlGenerator htmlGenerator) {
        throw new Exception("Character should not be in the final document.");
    }
}
