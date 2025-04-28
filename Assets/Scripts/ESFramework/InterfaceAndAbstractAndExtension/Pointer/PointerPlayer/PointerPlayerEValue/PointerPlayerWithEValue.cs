using ES.EvPointer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ES
{
    public abstract class PointerPlayerWithESValue : PointerPlayer { 
        public override IPointer Pointer => null; 
        [LabelText("值改变时，调用-↓"), SerializeReference, GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForESValue")]
        public IPointer OnValueChange = new PointerPickerEveryThing();
    }
    public abstract class PointerPlayerWithESValue<T> : PointerPlayerWithESValue
    {
        
        [LabelText("",Text = @"@ValueLabelName()"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForESValue")] public T ESValue;
        
        public string ValueLabelName()
        {
            return "当前值<" + typeof(T).Name+">";
        }
        public T Get() { return ESValue; }
        public T Set(T value) {
            if (ESValue.Equals(value)) return ESValue;
            ESValue = value;
            OnValueChange?.Pick();
            return ESValue; }
        [Tooltip("特殊操作，每种类可能都不一样")]
        public T Handle(T value)
        {
            T handle = GetHandleValue(value);
            if (ESValue.Equals(handle)) return ESValue;
            ESValue = handle;
            OnValueChange?.Pick();
            return ESValue;
        }
        public virtual T GetHandleValue(T inValue)
        {
            return inValue;//无操作
        }
    }
}