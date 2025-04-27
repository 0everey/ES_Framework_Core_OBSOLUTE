using ES.EvPointer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [TypeRegistryItem("针播放器_触发针", "针播放器")]
    public class PointerPlayerNone : PointerPlayer, IPointerNone
    {
        [LabelText("使用的针")] public IPointerNone pointer = new PointerPackOnlyAction_LoopOnce();

        public override IPointer Pointer => pointer;
        [Button("Pick测试")]
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            return pointer.Pick();
        }
    }
    [TypeRegistryItem("针播放器_抽象父类", "针播放器")]
    public abstract class PointerPlayer : SerializedMonoBehaviour,IPointer
    {
        public abstract IPointer Pointer { get; }
        [LabelText("备注信息", SdfIconType.At),PropertyOrder(-1), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")] public string des = "备注";
        public virtual object Pick(object by = null, object yarn = null, object on = null)
        {
            return Pointer?.Pick();
        }
        public void Pick_Invoke()
        {
            Pick();
        }
    }
}
