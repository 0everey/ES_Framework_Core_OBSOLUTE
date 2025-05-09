using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class testCopyTo : MonoBehaviour
{
    [Serializable]
    public class WithDoubleData001 : IWithSharedAndVariableData<SharedData001, VariableData001>
    {
        [ShowInInspector]public SharedData001 SharedData { get; set; }
        [ShowInInspector] public VariableData001 VariableData { get; set; }
    }
    [Serializable]
    public class WithDoubleData002 : IWithSharedAndVariableData<SharedData001, VariableData002>
    {
        [ShowInInspector] public SharedData001 SharedData { get; set; }
        [ShowInInspector] public VariableData002 VariableData { get; set; }
    }
    [Serializable]
    public class SharedData001 : ISharedData
    {

    }
    
    [Serializable]
    public class VariableData001 : IVariableData,ICopyToClass<VariableData001>
    {
        public float f1;
        public float f2;

        public void CopyTo(VariableData001 other)
        {
            other.f1 = f1+0.114514f;
            other.f2 = f2+0.114514f;
        }

        public void Init(params object[] ps)
        {
            
        }
    }
    [Serializable]
    public class VariableData002 : VariableData001
    {
        public float ff3;
    }

    public WithDoubleData002 from;
    public WithDoubleData002 to_;
    [Button("CopyTo")]
    public void Test()
    {
        KeyValueMatchingUtility.DataApply.CopyToClassDynamic_WithSharedAndVariableDataCopyTo(from,to_);
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
