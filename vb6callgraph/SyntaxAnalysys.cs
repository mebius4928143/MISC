using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class SyntaxAnalysys
{
    public void stxTree(string fs)
    {
        string sourceCode = File.ReadAllText(fs); // ソースコードを読み込む

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = syntaxTree.GetRoot();

        List<ControlStatementInfo> controlStatements = new List<ControlStatementInfo>();

        // 構文木をトラバースして制御文を検出
        foreach (var node in root.DescendantNodes())
        {
            if (node is IfStatementSyntax || node is ForStatementSyntax || node is WhileStatementSyntax || node is Switch || node is SwitchCase)
            {
                // 制御文を検出したらインデントと行番号を記録
                int line = syntaxTree.GetLineSpan(node.Span).StartLinePosition.Line + 1;
                int indent = node.GetLeadingTrivia().ToFullString().Length;

                controlStatements.Add(new ControlStatementInfo
                {
                    Statement = node.ToString(),
                    Indent = indent,
                    LineNumber = line
                });
            }
        }

        // 結果を表示
        foreach (var statementInfo in controlStatements)
        {
            Console.WriteLine($"行番号: {statementInfo.LineNumber}");
            Console.WriteLine($"インデント: {statementInfo.Indent}");
            Console.WriteLine($"制御文: {statementInfo.Statement}");
            Console.WriteLine();
        }
    }
    class ControlStatementInfo
    {
        public string Statement { get; set; }
        public int Indent { get; set; }
        public int LineNumber { get; set; }
    }
}

