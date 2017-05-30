using System;
using System.Threading.Tasks;

namespace RoslynDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Hello();

            SyntaxWalkerSample.Do();
        }

        static void Hello()
        {
            var message = "Hello Techcon Audience !!!";
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

    class AttributeA : Attribute { }
}
