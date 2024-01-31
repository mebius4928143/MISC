using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace vb6callgraph
{
    public class GraphMaker
    {
        public string CommentOut = @"(^[\s]*('.*)$)|(^[^""']*(""[^""]*(("""")[^""]*)*""[^""']*)*('.*)$)";
        public string StringLiteral = @"""(""""|[^""]+)""*";
        public string SubFuncDef = @"^\s*(?'ispublic'Private|Public)\s+(Sub|Function)\s+(?'name'\w[\w\d]+)\(";
        public string SubFuncCall = @"(?'name'\w[\w\d]+)";
        public string StmtBlock = @"(^[\s]*)(End (If|While|Loop|Next( [A-z0-9_]+)?|Sub|Function|With))([\s]*$)";
        public void MakerMain(string[] files)
        {
            var commentOut = new Regex(CommentOut);
            var stringLiteral = new Regex(StringLiteral);
            var subFuncDef = new Regex(SubFuncDef);
            var subFuncCall = new Regex(SubFuncCall);
            var stmtBlock = new Regex(StmtBlock);
            var anz = new Dictionary<string, VBMethod>();
            var children = new Dictionary<string, List<string>>();
            var matrix = new GraphMatrix();
            foreach (string file in files)
            {
                var lines = File.ReadAllLines(file, Encoding.Default);
                var lineno = 0;
                var mdlnm = Path.GetFileName(file);
                var methodName = string.Empty;
                var anzkey = string.Empty;
                foreach (string line in lines)
                {
                    lines[lineno] = commentOut.Replace(lines[lineno], string.Empty);
                    if (!lines[lineno].Trim().EndsWith(" _"))    // 継続行なし
                    {
                        lines[lineno] = stringLiteral.Replace(lines[lineno], string.Empty);
                        var matches = subFuncDef.Matches(lines[lineno]);
                        if (matches.Count > 0)
                        {
#if true
                            methodName = matches[0].Groups["name"].Value;
                            var vbm = Analyzer.AddVbMethod(methodName, mdlnm);
                            anzkey = vbm.GeyKey();
                            anz.Add(anzkey, vbm);
                            vbm.IsPublic = matches[0].Groups["ispublic"].Value == "Public";
                            vbm.StartLine = lineno + 1;
                            children.Add(VBMethod.GetKey(methodName, mdlnm), new List<string>());
#else
#endif
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(anzkey))
                            {
                                var matchesCall = subFuncCall.Matches(lines[lineno]);
                                var nowlist = children[anzkey];
                                for (int i = 0; i < matchesCall.Count; i++)
                                {
                                    nowlist.Add(matchesCall[i].Value);
                                }
                                nowlist = nowlist.Distinct().ToList();
                                children[anzkey] = nowlist;
                                var stmtMatches = stmtBlock.Matches(lines[lineno]);
                                if (stmtMatches.Count > 0)
                                {
                                    var end = stmtMatches[0].Groups[0].Value;
                                    if (!string.IsNullOrEmpty(anzkey) && anz.ContainsKey(anzkey) && anz[anzkey].ModuleName == mdlnm)
                                    {
                                        if (end == "End Sub" || end == "End Function")
                                        {
                                            anz[anzkey].EndLine = lineno + 1;
                                            anzkey = null;
                                        }
                                    }
                                    lines[lineno] = stmtBlock.Replace(lines[lineno], string.Empty);
                                }
                            }
                        }
                    }
                    else
                    {
                        // 次の行と結合して保管
                        if (lines.Length - 1 > lineno)
                        {
                            lines[lineno + 1] = lines[lineno].Trim().Replace(" _", "") + lines[lineno + 1];
                            lines[lineno] = string.Empty;
                        }
                    }
                    lineno++;
                }
            }
            var keys = children.Keys.ToList();
            foreach (string _anzkey in keys)
            {
                children[_anzkey] = children[_anzkey].Intersect(keys).Except(new string[] { _anzkey }).ToList();
                anz[_anzkey].Children = children[_anzkey].Select(c => anz[c]).ToList();
                children[_anzkey].ForEach(c => anz[c].Parents.Add(anz[_anzkey]));
            }
            matrix.Cells = new List<VBMethod>(anz.Count);
            var heads = anz.Values.ToList();
            heads.Sort(cmp);
        }

        private int cmp(VBMethod x, VBMethod y)
        {
            return x.Parents.Count - y.Parents.Count;
        }
    }
}
