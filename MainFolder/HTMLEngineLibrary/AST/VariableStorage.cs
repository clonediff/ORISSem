using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLEngineLibrary.AST
{
    internal class VariableStorage
    {
        VariableStorage Relative { get; }

        Dictionary<string, object> VariableValues { get; } = new();

        public VariableStorage(VariableStorage relative)
            => Relative = relative;

        public void SetVariable(string name, object value = default!)
        {
            var storage = GetVariableStorage(name);
            if (storage is not null)
                storage.VariableValues[name] = value;
            else
                VariableValues[name] = value;
        }

        VariableStorage? GetVariableStorage(string name)
        {
            var root = this;
            while (root is not null)
                if (root.VariableValues.ContainsKey(name))
                    return root;
                else
                    root = root.Relative;

            return null;
        }

        public object GetVariable(string name)
        {
            var storage = GetVariableStorage(name);
            if (storage is not null)
                return storage.VariableValues[name];

            throw new ArgumentException($"Переменная {name} не существует или не доступна");
        }
    }
}
