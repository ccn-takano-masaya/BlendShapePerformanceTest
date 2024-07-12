using System;
using System.Collections.Generic;
using System.Text;

namespace Cocone.P3B.Test
{
    public sealed class HTMLCreator
    {
        private Head head;
        private Body body;

        public HTMLCreator()
        {
            head = new Head();
            body = new Body();
        }

        public void AddToHead(HTMLElement element)
        {
            head.AddChild(element);
        }

        public void AddToBody(HTMLElement element)
        {
            body.AddChild(element);
        }

        public string BuildString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            head.BuildString(sb);
            body.BuildString(sb);
            sb.AppendLine("</html>");
            return sb.ToString();
        }
    }

    public abstract class HTMLElement
    {
        public string guid { get; private set; }
        protected abstract string tag { get; }
        private List<HTMLElement> children;

        public HTMLElement(string text) : this()
        {
            if (!string.IsNullOrEmpty(text))
            {
                children.Add(new PlainText(text));
            }
        }

        public HTMLElement(HTMLElement child) : this()
        {
            children.Add(child);
        }

        public HTMLElement()
        {
            children = new List<HTMLElement>();
            guid = Guid.NewGuid().ToString();
        }

        protected virtual void Begin(StringBuilder sb)
        {
            sb.AppendLine("<" + tag + ">");
        }

        protected virtual void End(StringBuilder sb)
        {
            sb.AppendLine("</" + tag + ">");
        }

        public void AddChild(HTMLElement element)
        {
            children.Add(element);
        }

        public void RemoveChild(string guid)
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].guid.Equals(guid))
                {
                    children.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveAll()
        {
            children.Clear();
        }

        public virtual void BuildString(StringBuilder sb, int indent = 0)
        {
            if (indent > 0) sb.Append(new string('\t', indent));
            Begin(sb);
            foreach (var child in children)
            {
                child.BuildString(sb, indent + 1);
            }
            if (indent > 0) sb.Append(new string('\t', indent));
            End(sb);
        }
    }

    public class PlainText : HTMLElement
    {
        private string text;
        public PlainText(string text)
        {
            this.text = text.Replace(Environment.NewLine, "<br>");
        }
        protected override string tag => string.Empty;
        public override void BuildString(StringBuilder sb, int indent = 0)
        {
            if (indent > 0) sb.Append(new string('\t', indent));
            sb.AppendLine(text);
        }
    }

    public class Head : HTMLElement
    {
        protected override string tag => "head";
    }

    public class Title : HTMLElement
    {
        public Title(string text) : base(text) { }
        protected override string tag => "title";
    }

    public class Script : HTMLElement
    {
        string src;
        string text;
        public Script(string text, string src = null) {
            this.src = src;
            this.text = text;
        }
        protected override string tag => "script";
        public override void BuildString(StringBuilder sb, int indent = 0)
        {
            if (indent > 0) sb.Append(new string('\t', indent));
            if (src != null)
            {
                sb.AppendLine($"<{tag} src=\"{src}\">");
            }
            else {
                sb.AppendLine("<" + tag + ">");
            }
            if (!string.IsNullOrEmpty(text))
            {
                sb.AppendLine(text);
            }
            sb.AppendLine("</" + tag + ">");
        }
    }

    public class Link : HTMLElement
    {
        string rel, href;
        public Link(string rel, string href) : base()
        {
            this.rel = rel;
            this.href = href;
        }
        protected override string tag => "link";
        public override void BuildString(StringBuilder sb, int indent = 0)
        {
            if (indent > 0) sb.Append(new string('\t', indent));
            sb.AppendLine($"<{tag} rel=\"{rel}\" href=\"{href}\">");
        }
    }

    public class Body : HTMLElement
    {
        protected override string tag => "body";
    }

    public class Header : HTMLElement
    {
        private int size;
        public Header(int size, string text) : base(text)
        {
            this.size = size;
        }
        public Header(int size, HTMLElement element) : base(element)
        {
            this.size = size;
        }
        protected override string tag => "h" + size;
    }

    public class Table : HTMLElement
    {
        protected override string tag => "table";

        public void CreateRow(string header, object cell)
        {
            var row = new TableRow();
            row.AddChild(new TableHeader(header));
            row.AddChild(new TableCell(cell.ToString()));
            AddChild(row);
        }

        public void CreateHeader(string header, int colspan = 2)
        {
            var row = new TableRow();
            row.AddChild(new TableHeader(header, colspan));
            AddChild(row);
        }
    }

    public class TableRow : HTMLElement
    {
        protected override string tag => "tr";
    }

    public class TableHeader : HTMLElement
    {
        private int colspan, rowspan;
        public TableHeader(int colspan = 1, int rowspan = 1) : base()
        {
            this.colspan = colspan;
            this.rowspan = rowspan;
        }
        public TableHeader(string text, int colspan = 1, int rowspan = 1) : base(text)
        {
            this.colspan = colspan;
            this.rowspan = rowspan;
        }
        public TableHeader(HTMLElement child, int colspan = 1, int rowspan = 1) : base(child)
        {
            this.colspan = colspan;
            this.rowspan = rowspan;
        }
        protected override string tag => "th";
        protected override void Begin(StringBuilder sb)
        {
            sb.AppendLine($"<{tag} colspan=\"{colspan}\" rowspan=\"{rowspan}\">");
        }
    }

    public class TableCell : HTMLElement
    {
        private int colspan, rowspan;
        public TableCell(int colspan = 1, int rowspan = 1) : base()
        {
            this.colspan = colspan;
            this.rowspan = rowspan;
        }
        public TableCell(string text = null, int colspan = 1, int rowspan = 1) : base(text)
        {
            this.colspan = colspan;
            this.rowspan = rowspan;
        }
        public TableCell(HTMLElement child = null, int colspan = 1, int rowspan = 1) : base(child)
        {
            this.colspan = colspan;
            this.rowspan = rowspan;
        }
        protected override string tag => "td";
        protected override void Begin(StringBuilder sb)
        {
            sb.AppendLine($"<{tag} colspan=\"{colspan}\" rowspan=\"{rowspan}\">");
        }
    }

    public class Image : HTMLElement
    {
        private string src;
        private int width, height;
        public Image(string src, int width = 0, int height = 0)
        {
            this.src = src;
            this.width = width;
            this.height = height;
        }
        protected override string tag => "img";
        public override void BuildString(StringBuilder sb, int indent = 0)
        {
            if (indent > 0) sb.Append(new string('\t', indent));
            sb.Append($"<{tag} src=\"{src}\"");
            if (width > 0) sb.Append($" width=\"{width}\"");
            if (height > 0) sb.Append($" height=\"{height}\"");
            sb.AppendLine(">");
        }
    }

    public class Hyperlink : HTMLElement
    {
        private string url;
        public Hyperlink(string url, string text = null) : base(text)
        {
            this.url = url;
        }
        protected override string tag => "a";
        protected override void Begin(StringBuilder sb)
        {
            sb.AppendLine($"<{tag} href=\"{url}\">");
        }
    }

    public class Canvas : HTMLElement
    {
        private string id;
        public Canvas(string id)
        {
            this.id = id;
        }
        protected override string tag => "canvas";
        protected override void Begin(StringBuilder sb)
        {
            sb.AppendLine($"<{tag} id=\"{id}\">");
        }
    }
}