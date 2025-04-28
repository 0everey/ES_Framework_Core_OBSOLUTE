using ES.EvPointer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [TypeRegistryItem("针播放器_老输入系统的轴", "针播放器")]
    public class PointerPlayerByGetAxis : PointerPlayer, IPointerForFloatCaster
    {
        [LabelText("应用的按钮名")] public string axisName;
        [Space(10)]
        [LabelText("使用Raw")]public bool isRaw = false;
        public override IPointer Pointer => null;
        [LabelText("获取的值-可投射")]public float readFloat;
        [DetailedInfoBox("", "此处需要引用一个PointerPlayerCaster,把自己的值投射给他它", Message = @"@ ""【绑定投射目标备注："" + (playerCaster != null ? playerCaster.des : ""！未绑定"") ", VisibleIf = "@usePlayerCaster")]
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public PointerPlayerSystemObjectCaster playerCaster_;
        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
           if(isRaw) readFloat = Input.GetAxisRaw(axisName);
            else readFloat = Input.GetAxis(axisName);

            if (usePlayerCaster && playerCaster_ != null)
            {
                playerCaster_.Recieve(readFloat);
            }
        }

        
        float ICaster<float>.Cast()
        {
            return readFloat;
        }
    }
}

