using FlareMark.Readers;

namespace FlareMark.Nodes;

public class Error(Exception e) : Node {
    public string exeption = e.ToString();
    public string message = e.Message;
    public override string ToHtml(HtmlGenerator htmlGenerator) {
        return $"<p class=\"error\" style=\"color: red;\">{message}</p>";
    }
}