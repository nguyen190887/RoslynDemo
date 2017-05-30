using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDemo
{
    class ReplSample
    {
        /* Copy below snippets and paste to C# Interactive windows of Visual Studio

using System.Text.RegularExpressions;
string url = "www.kms-technology.com?user=test-user&city=hochiminh";
var re = new Regex("user=(?<user>[a-z-]+)");
var m = re.Match(url);
var userName = m.Success ? $"User name is: {m.Groups["user"].Value}" : "(user not found)";
userName

        */

        public object Execute(string code)
        {
            throw new NotImplementedException();
        }
    }
}
