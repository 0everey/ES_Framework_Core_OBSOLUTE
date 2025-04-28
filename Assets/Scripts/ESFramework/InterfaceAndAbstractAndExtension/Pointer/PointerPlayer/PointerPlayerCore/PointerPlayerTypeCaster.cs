using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [TypeRegistryItem("针播放器_类型投射", "针播放器")]
    public class PointerPlayerTypeCaster : PointerPlayer, IPointerForType_Only, IPointerForTypeCaster
    {
        [LabelText("投射类型"), TypeSelectorSettings(FilterTypesFunction = nameof(TypeFilterBool))]
        public Type type;
        [LabelText("Pick一个类型"), SerializeReference] public IPointerForType_Only pointerForType;
        public override IPointer Pointer => throw new NotImplementedException();
        [DetailedInfoBox("", "此处需要引用一个PointerPlayerCaster,把自己的值投射给他它", Message = @"@ ""【绑定投射目标备注："" + (playerCaster != null ? playerCaster.des : ""！未绑定"") ", VisibleIf = "@usePlayerCaster")]
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public PointerPlayerSystemObjectCaster playerCaster_;
        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;

        public virtual bool TypeFilterBool(Type type)
        {
            return (type.IsSubclassOf(typeof(object)) && !type.IsAbstract && !type.IsInterface);
        }
        public Type Cast()
        {
            return type;
        }

        Type IPointer<Type, object, object, object>.Pick(object by, object yarn, object on)
        {
            if (pointerForType == (IPointerForType_Only)this)
            {
                pointerForType = null;
                Debug.LogError("不允许把自己当做自己的投射器");
            }
            Type t = pointerForType?.Pick();
            if (t != null)
            {
                type = t;
            }
            if (usePlayerCaster && playerCaster_ != null)
            {
                playerCaster_.Recieve(type);
            }
            return type;
        }
    }
}
