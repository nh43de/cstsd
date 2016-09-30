using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace cstsd.Tests
{
    public class TestAssembly
    {
        private Assembly _nativeAssemblyDefinition;
        private string path;

        public TestAssembly(string path)
        {
            this.path = path;
        }
        public string ComponentPath
        {
            get { return path; }
        }

        public Assembly AssemblyDefinition => _nativeAssemblyDefinition ?? (_nativeAssemblyDefinition = Assembly.LoadFile(ComponentPath));


        public Module ModuleDefinition => Assembly.GetExecutingAssembly().ManifestModule;


        public Type GetNativeType(string name)
        {
            return ModuleDefinition.GetTypes().Single(s => s.FullName == name);
        }

        protected IEnumerable<Type> GetAllTypesThatStartsWith(string name)
        {
            return ModuleDefinition.GetTypes().Where(s => s.FullName.StartsWith(name));
        }
    }
}
