using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace RoslynDemo
{
    /// <summary>
    /// https://github.com/dotnet/roslyn/wiki/Getting-Started-C%23-Syntax-Transformation
    /// </summary>
    class SyntaxTransformationSample
    {
        const string SampleCode =
@"using System;
using System.Collections;
using System.Linq;
using System.Text;
 
namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
            string sampleString = ""Sample"";
        }
    }
}";

        /// <summary>
        /// Replaces namespace using With* and ReplaceNode syntax.
        /// </summary>
        /// <returns></returns>
        public static string ReplaceNamespace()
        {
            NameSyntax name = IdentifierName("System");
            name = QualifiedName(name, IdentifierName("Collections"));
            name = QualifiedName(name, IdentifierName("Generic"));

            SyntaxTree tree = CSharpSyntaxTree.ParseText(SampleCode);
            var root = tree.GetRoot() as CompilationUnitSyntax;
            var oldUsing = root.Usings[1];
            var newUsing = oldUsing.WithName(name);

            root = root.ReplaceNode(oldUsing, newUsing);

            return root.GetText().ToString();
        }

        /// <summary>
        /// Replaces Type declaration with 'var' keyword.
        /// </summary>
        /// <returns></returns>
        public static string ReplaceTypeWithVarKeyword()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(SampleCode);
            MetadataReference mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            CSharpCompilation compilation = CSharpCompilation.Create("TransformationDemo", new [] { tree }, new[] { mscorlib });
            SemanticModel semanticModel = compilation.GetSemanticModel(tree);

            var syntaxRewriter = new DemoSyntaxRewriter(semanticModel);
            SyntaxNode newRoot = syntaxRewriter.Visit(tree.GetRoot());

            var replacedSourceCode = newRoot.GetText();
            return replacedSourceCode.ToString();
        }
    }

    class DemoSyntaxRewriter : CSharpSyntaxRewriter
    {
        public SemanticModel SemanticModel { get; set; }

        public DemoSyntaxRewriter(SemanticModel semanticModel)
        {
            SemanticModel = semanticModel;
        }

        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            if (node.Declaration.Variables.Count > 1)
            {
                return node;
            }
            if (node.Declaration.Variables[0].Initializer == null)
            {
                return null;
            }

            VariableDeclaratorSyntax declarator = node.Declaration.Variables.First();
            TypeSyntax variableTypeName = node.Declaration.Type;

            ITypeSymbol variableType = (ITypeSymbol)SemanticModel.GetSymbolInfo(variableTypeName).Symbol;

            TypeInfo initializerInfo = SemanticModel.GetTypeInfo(declarator.Initializer.Value);

            if (variableType == initializerInfo.Type)
            {
                TypeSyntax varTypeName = IdentifierName("var")
                    .WithLeadingTrivia(variableTypeName.GetLeadingTrivia())
                    .WithTrailingTrivia(variableTypeName.GetTrailingTrivia());

                return node.ReplaceNode(variableTypeName, varTypeName);
            }

            return node;
        }
    }
}
