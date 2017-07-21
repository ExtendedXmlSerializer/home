using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

// ReSharper disable StringIndexOfIsCultureSpecific.1

namespace ExtendedXmlSerializer.DocGenerator
{
    public enum CodeFormat
    {
        Cs,
        Xml
    }
    public class ReStructuredText
    {
        StringBuilder _builder = new StringBuilder();

        public void AddHeader(string header)
        {
            _builder.AppendLine(header);
            for (int i = 0; i < header.Length; i++)
            {
                _builder.Append("=");
            }
            _builder.AppendLine();
            _builder.AppendLine();
        }

        public void Add(string line)
        {
            _builder.AppendLine(line);
        }

        public void AddList(params string[] list)
        {
            _builder.AppendLine();
            foreach (var item in list)
            {
                _builder.AppendLine("* "+item);
            }
            _builder.AppendLine();
        }

        public void AddCode(string file, string section, CodeFormat format = CodeFormat.Cs)
        {
            string fileContend = File.ReadAllText(file);
            var code = ExtractFromString(fileContend, "// " + section, "// End" + section).SelectMany(p=>p.Split('\n')).ToList();

            _builder.AppendLine();
            _builder.AppendLine(format == CodeFormat.Cs ? ".. sourcecode:: csharp" : ".. sourcecode:: xml");
            _builder.AppendLine();

            var minIndent = code.Min(GetIndent);

            foreach (var item in code)
            {
                NormalizeIndent(item, minIndent);
            }
            _builder.AppendLine();
        }

        public void AddCode(string file, CodeFormat format = CodeFormat.Cs)
        {
            string fileContend = File.ReadAllText(file);
            var code = fileContend.Split('\n').ToList();

            _builder.AppendLine();
            _builder.AppendLine(format == CodeFormat.Cs ? ".. sourcecode:: csharp" : ".. sourcecode:: xml");
            _builder.AppendLine();

            var minIndent = code.Min(GetIndent);

            foreach (var item in code)
            {
                NormalizeIndent(item, minIndent);
            }
            _builder.AppendLine();
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        void NormalizeIndent(string line, int minIndent)
        {
            var currentIndent = GetIndent(line) - minIndent + 4;

            for (int i = 0; i < currentIndent; i++)
            {
                _builder.Append(' ');
            }
            _builder.AppendLine(line.Trim());
        }


        static int GetIndent(string line)
        {
            var result = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\t')
                {
                    result += 4;
                    continue;
                }
                if (line[i] == ' ')
                {
                    result += 1;
                    continue;
                }
                break;
            }
            return result;
        }

        static List<string> ExtractFromString(
            string text, string startString, string endString)
        {
            List<string> matched = new List<string>();
            int indexStart, indexEnd;
            bool exit = false;
            while (!exit)
            {
                indexStart = text.IndexOf(startString) + 1 + 1;
                indexEnd = text.IndexOf(endString) - 1;
                if (indexStart >= 0 && indexEnd >= 0)
                {
                    matched.Add(text.Substring(indexStart + startString.Length,
                        indexEnd - indexStart - startString.Length));
                    text = text.Substring(indexEnd + endString.Length);
                }
                else
                    exit = true;
            }
            return matched;
        }
    }
}
