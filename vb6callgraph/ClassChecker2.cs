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
    using System.Reflection;

    public class ClassChecker2
    {
        public static void runProc(string asm,string[] args)
        {
            // アセンブリのパスを指定
            string assemblyPath = asm;

            // クラス名のリストを指定
            List<string> classNames = new List<string> { "Contact", "ClassName2" };

            // アセンブリを一度だけ読み込む
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            // プロパティ名を取得
            foreach (string className in classNames)
            {
                List<string> propertyNames = GetPropertyNames(assembly, className);
                if (propertyNames != null)
                {
                    Console.WriteLine($"Class: {className}");
                    Console.WriteLine("Properties:");
                    foreach (string propertyName in propertyNames)
                    {
                        Console.WriteLine(propertyName);
                    }
                }
                else
                {
                    Console.WriteLine($"Class {className} not found in the assembly.");
                }
            }
        }

        static List<string> GetPropertyNames(Assembly assembly, string className)
        {
            // クラスを探す
            Type type = assembly.GetType(className);
            if (type != null)
            {
                // プロパティを列挙してリストに追加
                List<string> propertyNames = new List<string>();
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    propertyNames.Add(property.Name);
                }
                return propertyNames;
            }
            else
            {
                return null;
            }
        }
    }
}
