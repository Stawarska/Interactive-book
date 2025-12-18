using System;
using System.Collections.Generic;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GlobalVariable.Conditions
{
    [Serializable]
    public class VariableCondition 
    {
        [field: SerializeField, TreeDropdown(nameof(Variables))] public string VariableID { get; private set; }
        
        private IEnumerable<string> Variables() => GlobalVariables.instance.GetAllVariablesNames();
        
        [ShowIf(nameof(ShowString)), SerializeReference, SubclassSelector] public IStringCondition stringConditions;
        [ShowIf(nameof(ShowBool)), SerializeReference, SubclassSelector] public IBoolCondition boolConditions;
        [ShowIf(nameof(ShowInt)), SerializeReference, SubclassSelector] public IIntCondition intConditions;
        [ShowIf(nameof(ShowFloat)), SerializeReference, SubclassSelector] public IFloatCondition floatConditions;

        private bool ShowString() => GlobalVariables.instance.GetTypeFromName(VariableID) == typeof(StringGlobalVariable);
        private bool ShowBool() => GlobalVariables.instance.GetTypeFromName(VariableID) == typeof(BoolGlobalVariable);
        private bool ShowInt() => GlobalVariables.instance.GetTypeFromName(VariableID) == typeof(IntGlobalVariable);
        private bool ShowFloat() => GlobalVariables.instance.GetTypeFromName(VariableID) == typeof(FloatGlobalVariable);
        
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