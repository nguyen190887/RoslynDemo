using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynDemo
{
    /// <summary>
    /// https://github.com/dotnet/roslyn/wiki/Getting-Started-C%23-Syntax-Analysis
    /// </summary>
    class SyntaxWalkerSample
    {
        const string snippet =
@"
using System;
using System.Threading.Tasks;

namespace RoslynDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var message = ""Hello Techcon Audience !!!"";
            Console.WriteLine(message);
        }
    }

    abstract class TheAbstract
    {
        public abstract Task<string> DoAsync(int x, int y);
    }

    [AttributeA]
    class TheConcrete : TheAbstract
    {
        public override Task<string> DoAsync(int x, int y) { throw new NotImplementedException(); }
    }

    class AttributeA: Attribute { }
}
";
        internal static void Do()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(snippet);
            var root = tree.GetRoot();

            var walker = new AttributeUsageFinder("AttributeA");
            walker.Visit(root);
        }
    }

    class AttributeUsageFinder : CSharpSyntaxWalker
    {
        public string AttributeName { get; set; }

        public AttributeUsageFinder(string attributeName)
        {
            AttributeName = attributeName;
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            if ((node.Name as IdentifierNameSyntax)?.Identifier.Text == AttributeName)
            {
                SyntaxNode classNode = node;
                do
                {
                    classNode = classNode.Parent;
                }
                while (!(classNode is ClassDeclarationSyntax));

                Console.WriteLine($"Class using AttributeA: {((ClassDeclarationSyntax)classNode).Identifier.Text}");
            }

            base.VisitAttribute(node);
        }
    }
}
