using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vb6callgraph
{
    public class Analyzer
    {
        public static VBMethod AddVbMethod(string methodName, string moduleName)
        {
            var vbMethod = new VBMethod() { MethodName = methodName, ModuleName = moduleName };
            return vbMethod;
        }
    }
    public class VBMethod
    {
        public string MethodName { get; set; }
        public List<VBMethod> Children { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public string ModuleName { get; set; }
        public bool IsPublic { get; set; }  // true:publicメソッド
        public List<VBMethod> Parents { get; set; }    // 呼び出し元関数
        public override string ToString()
        {
            return MethodName;
        }
        public VBMethod()
        {
            Children = new List<VBMethod>();
            Parents = new List<VBMethod>();
        }
        public string GeyKey()
        {
            return GetKey(this.MethodName, this.ModuleName);
        }
        public static string GetKey(string name, string moduleName, string curmdl = "")
        {
            return getKey(name, moduleName, curmdl);
        }
        private static string getKey(string name, string moduleName, string curmdl = "")
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(moduleName))
            {
                throw new Exception("No Name Or Module Name.");
            }
            if (moduleName == "*")
            {
                moduleName = curmdl;
            }
            return $"{name}@{moduleName}";
        }
    }
    public class GraphMatrix
    {
        public List<VBMethod> Cells { get; set; }
    }
    public class Position
    {
        public VBMethod VBMethodObject { get; set; }
    }
}
