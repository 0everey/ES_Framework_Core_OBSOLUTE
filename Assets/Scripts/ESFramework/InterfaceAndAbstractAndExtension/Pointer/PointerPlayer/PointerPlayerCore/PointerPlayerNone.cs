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
        [LabelText("备注信息", SdfIconType.At),PropertyOrder(-1), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForDes")] 
        public string des = "备注";


        #region 注释
        [FoldoutGroup("<编写注释>")]
        [LabelText("---开始编辑---")]
        public bool edit = true;
        [FoldoutGroup("<编写注释>")]
        [DisplayAsString(fontSize: 20, EnableRichText = true), ShowIf("@!edit"), GUIColor("showColor"), ShowInInspector, HideLabel]
        public string ReadMe = "编写提示文件";
        [TextArea(3, 10), ShowIf("edit"), OnValueChanged("SetString")]
        [FoldoutGroup("<编写注释>")] public string readMe = "编写提示文件";
        [ColorPalette, ShowIf("edit")]
        [FoldoutGroup("<编写注释>")] public Color showColor = KeyValueMatchingUtility.ColorSelector.ColorForPlayerReadMe;
        private void SetString(string edit)
        {
            ReadMe = edit;
        }
        #endregion
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
