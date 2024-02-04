using FlareMark.Readers;

namespace FlareMark;

class Program {
    private static void Main(string[] args) {
        var text = File.ReadAllText(@"D:\Project\FlareMark\FlareMark\ExampleText.txt");
        Register();

        var p = Reader.Parse(text, false);
        File.WriteAllText(@"D:\Project\FlareMark\FlareMark\ExampleText.html", p.ToHtml());
    }

    private static void Register() {
        Reader.RegisterState(new StateHeader());
        Reader.RegisterState(new StateParagraph());
        
        Reader.RegisterState(new StateDocument());
        Reader.RegisterState(new StateEscape());
        Reader.RegisterState(new StateText());

        Reader.RegisterState(new StateBold());
        Reader.RegisterState(new StateUnderline());
        Reader.RegisterState(new StateStrikethrough());
        Reader.RegisterState(new StateItalic());
    }
}