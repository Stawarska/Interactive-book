using System;
using UnityEngine;

namespace GlobalVariable
{
    [Serializable]
    public class VariableOperation<T>
    {
        [field: SerializeField] public string VariableID { get; private set; }
        [field: SerializeReference] public T Operation { get; private set; }
    }
}