using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GlobalVariable
{
    [CreateAssetMenu(fileName = "GlobalVariables", menuName = "GlobalVariables")]
    [FilePath("Assets/Scripts/GlobalVariable/GlobalVariables.asset", FilePathAttribute.Location.ProjectFolder)]
    public class GlobalVariables : ScriptableSingleton<GlobalVariables>
    {
        [Serializable]
        public class Variable
        {
            public string variableName;
            [SerializeReference] public IGlobalVariableType variableType;

            public Variable(string variableName, IGlobalVariableType variableType)
            {
                this.variableName = variableName;
                this.variableType = variableType;
            }
        }
        
        [field: SerializeField] public List<Variable> Variables { get; private set; }
        
        public IEnumerable<string> GetAllVariablesNames()
        {
            return Variables.Select(var => var.variableName);
        }
        
        public IEnumerable<string> GetVariablesNamesOfType(Type type)
        {
            return Variables.Where(x =>x.variableType.GetType() == type).Select(var => var.variableName);
        }
        
        public Type GetTypeFromName(string variableName)
        {
            var type = Variables.Find(x => x.variableName == variableName).variableType.GetType();
            return type;
        }
    }
}

