using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vb6callgraph
{
    public class Analyzer
    {
        public class VBMethod
        {
            public string Name { get; set; }
            public List<VBMethod> Children { get; set; }
            public int StartLine { get; set; }
            public int EndLine { get; set; }
            public string ModuleName { get; set; }
            public bool IsPublic { get; set; }  // true:publicメソッド
            public List<VBMethod> Parents { get; set; }    // 呼び出し元関数
            public override string ToString()
            {
                return Name;
            }
        }
    }
}
