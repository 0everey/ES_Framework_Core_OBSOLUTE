using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ES
{
    /* [CreateAssetMenu(fileName = "BuffSoInfo", menuName = "EvData/BuffSoInfo")]*/
    [ESDisplayNameKeyToType("数据单元", "Buff数据单元")]
    public class BuffSoInfo :SoDataInfo , IWithSharedAndVariableData<ESBuffSharedData, ESBuffVariableData>
    {
        [LabelText("绑定状态基")]
        public StateDataInfo bindingState;
        [LabelText("实体共享数据")]
        public ESBuffSharedData buffSharedData;

        [LabelText("实体变量数据")]
        public ESBuffVariableData defaultVariableData;

        public ESBuffSharedData SharedData { get => buffSharedData; set => buffSharedData = value; }
        public ESBuffVariableData VariableData { get => defaultVariableData; set => defaultVariableData = value; }
    }
    /*//[SerializeReference]
    //[LabelText("仅测试阶段")]public IKey TestKey;
    //public KeyString_BuffUse key => BuffKey;
    // [LabelText("Buff的键",SdfIconType.KeyFill)]
    //public KeyString_BuffUse BuffKey=new KeyString_BuffUse();

    [TypeSelectorSettings(FilterTypesFunction = nameof(TypeFilterBool)), LabelText("绑定业务逻辑", SdfIconType.Link45deg), GUIColor("@new Color(0.95f,0.9f,0.7f)"), NonSerialized, OdinSerialize]
    //[TypeFilter("TypeFilter",DrawValueNormally =true),NonSerialized,OdinSerialize,LabelText("绑定业务逻辑"),GUIColor("@Color.magenta")]
    public Type BindingLogic;



    public virtual bool TypeFilterBool(Type type)
    {
        return type.IsSubclassOf(typeof(BuffRunTimeLogic)) && !type.IsAbstract && !type.IsInterface;
    }


    public virtual GameKeyType[] TypeFilter()
    {
        List<GameKeyType> types = typeof(BaseESModule).Assembly.GetTypes().ToList();
        return types.Where(n => n.IsSubclassOf(typeof(BaseESModule)) && !n.IsAbstract && !n.IsInterface).ToArray();
    }*/
}

