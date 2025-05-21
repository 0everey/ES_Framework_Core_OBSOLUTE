using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HellishBattle
{
    public class ESReferValue : MonoBehaviour
    {
    
        void Start()
        {
        
        }

    
        void Update()
        {
        
        }
    }
    [Serializable]
    public abstract class ESReferCheckout<T>
    {
        protected T _value;
        [ShowInInspector,LabelText("值")]public T Value { get => _value; set => _value = value; }
    }
    [Serializable,TypeRegistryItem("结算值-浮点数")]
    public class ESReferCheckout_Float : ESReferCheckout<float>
    {
        
    }
}
