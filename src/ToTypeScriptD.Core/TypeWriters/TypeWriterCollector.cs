using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToTypeScriptD.Core.TypeWriters
{
    public interface ITypeNotFoundErrorHandler
    {
        void Handle(Type typeReference);
    }

    public class TypeWriterCollector
    {
        private ITypeNotFoundErrorHandler typeNotFoundErrorHandler;
        private ITypeWriterTypeSelector typeSelector;

        public TypeWriterCollector(ITypeNotFoundErrorHandler typeNotFoundErrorHandler, ITypeWriterTypeSelector typeSelector)
        {
            this.typeNotFoundErrorHandler = typeNotFoundErrorHandler;
            this.typeSelector = typeSelector;
        }
        public void Collect(IEnumerable<Type> tds, TypeCollection typeCollection, ConfigBase config)
        {
            foreach (var item in tds)
            {
                Collect(item, typeCollection, config);
            }
        }
        public void Collect(Type td, TypeCollection typeCollection, ConfigBase config)
        {
            var typeName = td.Name;

            if (td.ShouldIgnoreType())
            {
                return;
            }

            // don't duplicate types
            if (typeCollection.Contains(td.FullName))
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            var indentCount = 0;
            ITypeWriter typeWriter = typeSelector.PickTypeWriter(td, indentCount, typeCollection, config);

            Type[] baseTypes;
            if(td.BaseType == null)
                baseTypes = new[] { td.BaseType};
            else
                baseTypes = new Type[] {};
            
            /*
            Pat threw Di in the tornado.

            And then what?

            Well Pat didn't really mind.
            */

            td.GetInterfaces().Union(baseTypes).Each(item =>
            {
                if(item == null) return;

                var foundType = typeCollection.LookupType(item);

                if (foundType == null)
                {
                    //TODO: This reporting a missing type is too early in the process.
                    // typeNotFoundErrorHandler.Handle(item);
                    return;
                }

                var itemWriter = typeSelector.PickTypeWriter(foundType, indentCount, typeCollection, config);
                typeCollection.Add(foundType.Namespace, foundType.Name, itemWriter);

            });

            typeCollection.Add(td.Namespace, td.Name, typeWriter);
        }
    }
}
