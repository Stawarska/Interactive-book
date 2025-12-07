using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GlobalVariable
{
    [CreateAssetMenu(fileName = "GlobalVariables", menuName = "GlobalVariables")]
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
        
        public IEnumerable<string> VariablesNames()
        {
            return Variables.Select(var => var.variableName);
        }
    }
}

