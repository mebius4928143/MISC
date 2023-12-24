using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace vb6callgraph
{
    public class GraphMaker
    {
        public string CommentOut = @"(^[\s]*('.*)$)|(^[^""']*(""[^""]*(("""")[^""]*)*""[^""']*)*('.*)$)";
        public string StringLiteral = @"""(""""|[^""]+)*";
        public string SubFuncDef = @"^\s*(?'ispublic'Private|Public)\s+(Sub|Function)\s+(?'name'\w[\w\d]+)\(";
        public string SubFuncCall = @"(?'name'\w[\w\d]+)";
        public string stmtBlock = @"(^[\s]*)(End (If|While|Loop|Next( [A-z0-9_]+)?|Sub|Function|With))([\s]*$)";
        public void MakerMain(string[] files)
        {
            var commentOut = new Regex(CommentOut);
            var stringLiteral = new Regex(StringLiteral);
            var subFuncDef = new Regex(SubFuncDef);
            var subFuncCall = new Regex(SubFuncCall);
            var anz = new List<Analyzer.VBMethod>();
            foreach (string file in files)
            {
                var lines = File.ReadAllLines(file, Encoding.Default);
                var lineno = 0;
                var mdlnm = Path.GetFileName(file);
                foreach (string line in lines)
                {
                    lines[lineno] = commentOut.Replace(lines[lineno], string.Empty);
                    lines[lineno] = stringLiteral.Replace(lines[lineno], string.Empty);
                    var matches = subFuncDef.Matches(lines[lineno]);
                    if (matches.Count > 0)
                    {
                        if (anz.Count > 0)
                        {
                            if (anz[anz.Count - 1].ModuleName == mdlnm)
                            {
                                anz[anz.Count - 1].EndLine = lineno;
                            }
                        }
                        anz.Add(new Analyzer.VBMethod()
                        {
                            Name = matches[0].Groups["name"].Value,
                            ModuleName = mdlnm,
                            IsPublic = matches[0].Groups["ispublic"].Value == "Public",
                            StartLine = lineno + 1,
                            EndLine = lineno + 1,
                        });
                    }
                    else
                    {
                        var matchesCall = subFuncCall.Matches(lines[lineno]);
                    }
                    lineno++;
                }
            }
            foreach (string file in files)
            {
                var lines = File.ReadAllLines(file, Encoding.Default);
                var lineno = 0;
                foreach (string line in lines)
                {
                    lines[lineno] = commentOut.Replace(lines[lineno], string.Empty);
                    lines[lineno] = stringLiteral.Replace(lines[lineno], string.Empty);
                    var matches = subFuncDef.Matches(lines[lineno]);
                    if (matches.Count > 0)
                    {
                        anz.Add(new Analyzer.VBMethod()
                        {
                            Name = matches[0].Groups["name"].Value,
                            ModuleName = Path.GetFileName(file),
                            IsPublic = matches[0].Groups["ispublic"].Value == "Public",
                            StartLine = lineno,
                            EndLine = lineno,
                        });
                    }
                    lineno++;
                }
            }
        }
    }
}
