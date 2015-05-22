using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace XmlDocTableCli
{
    /// <summary>The command line interface.</summary>
    class Program
    {
        /// <summary>The main method.</summary>
        static void Main(string[] args)
        {
            var walker = new TexTableWalker();
            foreach (var src in args.SelectMany(arg => Directory.EnumerateFiles(arg, "*.cs", SearchOption.AllDirectories).Select(File.ReadAllText)))
                walker.Visit(CSharpSyntaxTree.ParseText(src).GetRoot());
            var sb = new StringBuilder();
            sb.AppendLine(@"% AUTOGENERATED");
            sb.AppendLine(@"\begin{tabular}{ l | l }");
            sb.AppendLine(@"\multicolumn{2}{ l }{Classes}");
            sb.AppendLine(@"\hline");
            sb.Append(walker.ClassTable);
            sb.AppendLine(@"\end{tabular}");
            foreach (var table in walker.MemberTables)
            {
                sb.AppendLine($"\n{table.Key}");
                if (table.Value.FieldsCount > 0)
                {
                    sb.AppendLine(@"\begin{tabular}{ l | l | l | l }");
                    sb.AppendLine(@"\multicolumn{4}{ l }{Fields}");
                    sb.AppendLine(@"\hline");
                    sb.Append(table.Value.FieldsTable);
                    sb.AppendLine(@"\end{tabular}");
                }
                if (table.Value.PropertiesCount > 0)
                {
                    sb.AppendLine(@"\begin{tabular}{ l | l | l | l | l }");
                    sb.AppendLine(@"\multicolumn{5}{ l }{Properties}");
                    sb.AppendLine(@"\hline");
                    sb.Append(table.Value.PropertiesTable);
                    sb.AppendLine(@"\end{tabular}");
                }
                if (table.Value.MethodsCount > 0)
                {
                    sb.AppendLine(@"\begin{tabular}{ l | l | l | l | l }");
                    sb.AppendLine(@"\multicolumn{5}{ l }{Methods}");
                    sb.AppendLine(@"\hline");
                    sb.Append(table.Value.MethodsTable);
                    sb.AppendLine(@"\end{tabular}");
                }
            }
            sb.AppendLine(@"% /AUTOGENERATED");
            Console.Write(sb);
        }
    }
}
