using ES.EvPointer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ES
{
    [TypeRegistryItem("针播放器_老输入系统的Key", "针播放器")]
    public class PointerPlayerByInputGetKey : PointerPlayer,IPointerForBoolCaster
    {
        [LabelText("应用的键")]public KeyCode keyCode;
        [Space(10)]
        [FoldoutGroup("启用的输入"), LabelText("启用按下时")] public bool useWasPress = true;
        [Space(10)]
        [FoldoutGroup("启用的输入"), LabelText("启用按住时")] public bool useIsPress = false;
        [Space(10)]
        [FoldoutGroup("启用的输入"), LabelText("启用松开时")] public bool useWasRelease = false;
        [LabelText("读取的布尔值")] public bool readBool = default;
        public override IPointer Pointer => pointerNoneForWasPress;

        [LabelText("按下时触发"), SerializeReference, FoldoutGroup("输入事件")] public IPointerNone pointerNoneForWasPress = new PointerPickerEveryThing();
        [Space(10)][LabelText("按住时触发"), SerializeReference, FoldoutGroup("输入事件")] public IPointerNone pointerNoneForIsPress = new PointerPickerEveryThing();
        [Space(10)][LabelText("松开时触发"), SerializeReference, FoldoutGroup("输入事件")] public IPointerNone pointerNoneForWasRelease = new PointerPickerEveryThing();

        [DetailedInfoBox("", "此处需要引用一个PointerPlayerCaster,把自己的值投射给他它", Message = @"@ ""【绑定投射目标备注："" + (playerCaster != null ? playerCaster.des : ""！未绑定"") ", VisibleIf = "@usePlayerCaster")]
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")] public PointerPlayerSystemObjectCaster playerCaster_;
        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (useWasPress && Input.GetKeyDown(keyCode))
            {
                pointerNoneForWasPress?.Pick();
            }
            if (useIsPress)
            {

                if (Input.GetKey(keyCode))
                {
                    pointerNoneForIsPress?.Pick();
                    readBool = true;
                }
                else
                {
                    readBool = false;
                }
            }
            else
            {
                readBool = Input.GetKey(keyCode);
            }
            if (useWasRelease && Input.GetKeyUp(keyCode))
            {
                pointerNoneForWasRelease?.Pick();
            }
            if (usePlayerCaster && playerCaster_ != null)
            {
                playerCaster_.Recieve(readBool);
            }

        }

        public bool Cast()
        {
           return  readBool;
        }
    }
}
