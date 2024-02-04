using FlareMark.Readers;

namespace FlareMark.Nodes;

public abstract class Node {
    public abstract string ToHtml(HtmlGenerator htmlGenerator);

    public string ToHtml() {
        var gen = new HtmlGenerator();
        return ToHtml(gen);
    }
    
    public static Node Empty = new Text("");
}