using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace GlobalVariable
{
    [CreateAssetMenu(fileName = "GlobalVariables", menuName = "GlobalVariables")]
    [FilePath("Assets/Scripts/GlobalVariable/GlobalVariables.asset", FilePathAttribute.Location.ProjectFolder)]
    public class GlobalVariables : ScriptableSingleton<GlobalVariables>
    {
        [Serializable]
        public class VariableWrapper
        {
            public string variableName;
            [FormerlySerializedAs("variableType")] [SerializeReference] public IVariable variable;

            public VariableWrapper(string variableName, IVariable variable)
            {
                this.variableName = variableName;
                this.variable = variable;
            }
        }
        
        [field: SerializeField] public List<VariableWrapper> Variables { get; private set; }
        
        public IEnumerable<string> GetAllVariablesNames()
        {
            return Variables.Select(var => var.variableName);
        }
        
        public IEnumerable<string> GetVariablesNamesOfType(Type type)
        {
            return Variables.Where(x =>x.variable.GetType() == type).Select(var => var.variableName);
        }
        
        public Type GetTypeFromName(string variableName)
        {
            return Variables.Find(x => x.variableName == variableName)?.variable.GetType();
        }

        public VariableWrapper GetVariableFromName(string variableName)
        {
            return Variables.FirstOrDefault(x => x.variableName == variableName);
        }
    }
}

