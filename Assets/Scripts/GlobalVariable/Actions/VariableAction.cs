using System;
using System.Collections.Generic;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GlobalVariable.Actions
{
    [Serializable]
    public class VariableAction 
    {
        [field: SerializeField, TreeDropdown(nameof(Variables))] public string VariableID { get; private set; }
        private IEnumerable<string> Variables() => GlobalVariables.instance.GetAllVariablesNames();
        
        [ShowIf(nameof(ShowString)), SerializeReference, SubclassSelector] public IStringAction stringActions;
        [ShowIf(nameof(ShowBool)), SerializeReference, SubclassSelector] public IBoolAction boolActions;
        [ShowIf(nameof(ShowInt)), SerializeReference, SubclassSelector] public IIntAction intActions;
        [ShowIf(nameof(ShowFloat)), SerializeReference, SubclassSelector] public IFloatAction floatActions;
        
        private bool ShowString() => GlobalVariables.instance.GetTypeFromName(VariableID) == typeof(StringGlobalVariable);
        private bool ShowBool() => GlobalVariables.instance.GetTypeFromName(VariableID) == typeof(BoolGlobalVariable);
        private bool ShowInt() => GlobalVariables.instance.GetTypeFromName(VariableID) == typeof(IntGlobalVariable);
        private bool ShowFloat() => GlobalVariables.instance.GetTypeFromName(VariableID) == typeof(FloatGlobalVariable);
        
        public void Perform()
        {
            var variable = VariablesManager.Instance.Variables[VariableID];

            var varType = variable.VariableType;

            if (varType == typeof(bool))
            {
                variable.SetValue<bool>(boolActions.Perform(variable.GetValue<bool>()));
                PerformAction();
                return;
            }

            if (varType == typeof(int))
            {
                variable.SetValue<int>(intActions.Perform(variable.GetValue<int>()));
                PerformAction();
                return;
            }
            
            if (varType == typeof(float))
            {
                variable.SetValue<float>(floatActions.Perform(variable.GetValue<float>()));
                PerformAction();
                return;
            }
            
            variable.SetValue<string>(stringActions.Perform(variable.GetValue<string>()));
            PerformAction();
        }
        
        
        private void PerformAction()
        {
            var variable = GlobalVariables.instance.GetVariableFromName(VariableID);
            VariablesManager.Instance.OnVariableChanged.Invoke(variable);
        }
    }
}