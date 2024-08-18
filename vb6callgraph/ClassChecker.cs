using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vb6callgraph
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class ClassChecker
    {
        public static void runProc(string[] args)
        {
            var code = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
            {
                code.Append(File.ReadAllText(args[i]));
            }
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code.ToString());
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            var properties = root.DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Select(p => new { Name = p.Identifier.ValueText, Type = p.Type.ToString() });

            var caseInsensitiveDuplicates = properties
                .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            var typeDifferentDuplicates = properties
                .GroupBy(p => p.Name)
                .Where(g => g.Select(x => x.Type).Distinct().Count() > 1)
                .Select(g => g.Key);

            foreach (var prop in caseInsensitiveDuplicates)
            {
                Console.WriteLine($"Property with case-insensitive duplicate: {prop}");
            }

            foreach (var prop in typeDifferentDuplicates)
            {
                Console.WriteLine($"Property with different types: {prop}");
            }
        }

        public class TestClass
        {
            public string Name { get; set; }
            public string name { get; set; }
            public int Age { get; set; }
            public string age { get; set; }
            public double Weight { get; set; }
            public int weight { get; set; }
        }
    }
}
