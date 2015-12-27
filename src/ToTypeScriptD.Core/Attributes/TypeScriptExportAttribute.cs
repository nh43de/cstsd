using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToTypeScriptD.Core.Attributes
{
    public struct CodeGen
    {
        public Func<Type, string> Invoke;
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class TSGenAttribute : Attribute
    {
        private CodeGen CodeGenerator { get; }

        public TSGenAttribute(CodeGen codeGen)
        {
            CodeGenerator = codeGen;
        }

        public string GetTypeScript(Type type) 
            => CodeGenerator.Invoke(type);
    }


    public class TSInterfaceAttribute : Attribute
    {

    }

    public class TSControllerAttribute : Attribute
    {
        
    }

}
