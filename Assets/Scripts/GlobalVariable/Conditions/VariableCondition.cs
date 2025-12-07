using System;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GlobalVariable.Conditions
{
    public class VariableCondition : MonoBehaviour
    {
        [field: SerializeField, TreeDropdown(nameof(Variables))] public string VariableID { get; private set; }
        
        private IEnumerable<string> Variables()
        {
            return GlobalVariables.instance.Variables.Select(var => var.variableName);
        }
        
        [ShowIf(nameof(ShowString)), SerializeReference, SubclassSelector] public IStringCondition stringConditions;
        [ShowIf(nameof(ShowBool)), SerializeReference, SubclassSelector] public IBoolCondition boolConditions;
        [ShowIf(nameof(ShowInt)), SerializeReference, SubclassSelector] public IIntCondition intConditions;
        [ShowIf(nameof(ShowFloat)), SerializeReference, SubclassSelector] public IFloatCondition floatConditions;
        
        private Type GetTypeFromName()
        {
            var type = GlobalVariables.instance.Variables.Find(x => x.variableName == VariableID).variableType.GetType();
            return type;
        }

        private bool ShowString() => GetTypeFromName() == typeof(StringGlobalVariable);
        private bool ShowBool() => GetTypeFromName() == typeof(BoolGlobalVariable);
        private bool ShowInt() => GetTypeFromName() == typeof(IntGlobalVariable);
        private bool ShowFloat() => GetTypeFromName() == typeof(FloatGlobalVariable);

        
        public bool Check()
        {
            var variable = VariablesManager.Instance.Variables[VariableID];

            var varType = variable.VariableType;
            
            if(varType == typeof(bool))
                return boolConditions.CheckCondition(variable.GetValue<bool>());
            if(varType == typeof(int))
                return intConditions.CheckCondition(variable.GetValue<int>());
            if(varType == typeof(float))
                return floatConditions.CheckCondition(variable.GetValue<float>());
            if(varType == typeof(string))
                return stringConditions.CheckCondition(variable.GetValue<string>());
            return false;
        }
    }
}