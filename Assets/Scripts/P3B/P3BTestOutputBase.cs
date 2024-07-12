using System.IO;

namespace Cocone.P3B.Test
{
    public abstract class P3BTestOutputBase : TestOutputBase
    {
        public MarkdownCreator markdownCreator { get; private set; }

        public P3BTestOutputBase()
        {
            markdownCreator = new MarkdownCreator();
        }

        public override void Save(string path)
        {
            File.WriteAllText(path + "/index.md", markdownCreator.BuildString());
        }
    }
}