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
    public class BuffSoInfo : SoDataInfo
    {
        [LabelText("注册与注销"), SerializeReference] public List<OutputOpeationBuffDelegate> dele = new List<OutputOpeationBuffDelegate>();
        [LabelText("更新的"),SerializeReference] public OutputOpeationBuffDo upd;
        public StateDataInfo binding;
        [LabelText("结算的"), SerializeReference] public List<OutputOpeationBuffSettle> settle = new List<OutputOpeationBuffSettle>();
        [LabelText("缓冲的"), SerializeReference] public OutputOperationBuffBuffer buffer;
        //[SerializeReference]
        //[LabelText("仅测试阶段")]public IKey TestKey;
        //public KeyString_BuffUse key => BuffKey;
        // [LabelText("Buff的键",SdfIconType.KeyFill)]
        //public KeyString_BuffUse BuffKey=new KeyString_BuffUse();
        [LabelText("Buff默认状态", SdfIconType.InfoCircleFill)]
        public BuffStatusTest defaultStatus;
        [TypeSelectorSettings(FilterTypesFunction = nameof(TypeFilterBool)), LabelText("绑定业务逻辑", SdfIconType.Link45deg), GUIColor("@new Color(0.95f,0.9f,0.7f)"),NonSerialized,OdinSerialize]
        //[TypeFilter("TypeFilter",DrawValueNormally =true),NonSerialized,OdinSerialize,LabelText("绑定业务逻辑"),GUIColor("@Color.magenta")]
        public Type BindingLogic;

        [LabelText("Buff图标")] public Sprite icon;
        [LabelText("Buff好坏")]public EnumCollect.BuffTagForGoodOrBad buffGoodOrBad;

        public virtual bool TypeFilterBool(Type type)
        {
            return type.IsSubclassOf(typeof(BuffRunTimeLogic)) && !type.IsAbstract && !type.IsInterface;
        }

#if UNITY_EDITOR
        [ContextMenu("删除自己")]
        public void DeleteThis()
        {
            Undo.DestroyObjectImmediate(this);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
#endif
        /*  public virtual GameKeyType[] TypeFilter()
 {        
     List<GameKeyType> types = typeof(BaseESModule).Assembly.GetTypes().ToList();
     return types.Where(n => n.IsSubclassOf(typeof(BaseESModule))&&!n.IsAbstract&&!n.IsInterface).ToArray();
 }*/
    }
}
