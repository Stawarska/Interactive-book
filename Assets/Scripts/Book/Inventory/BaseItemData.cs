using System;
using System.Collections.Generic;
using GlobalVariable;
using SaintsField;
using UnityEngine;

namespace Book.Inventory
{
    [Serializable]
    public struct BaseItemData
    {
        [field: SerializeField, AdvancedDropdown(nameof(IntVariables))] public string VariableNameInt { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; }
        
        private IEnumerable<string> IntVariables()
        {
#if !UNITY_EDITOR
            return null;
#else
            return GlobalVariables.instance.GetVariablesNamesOfType(typeof(IntVariable));
#endif
        }
    }
}