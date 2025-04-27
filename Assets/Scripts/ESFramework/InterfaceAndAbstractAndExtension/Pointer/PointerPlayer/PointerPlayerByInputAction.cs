using ES.EvPointer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ES
{
    [TypeRegistryItem("针播放器_新输入系统的InputAction", "针播放器")]
    public class PointerPlayerByInputAction : PointerPlayer,IPointerForFloatCaster,IPointerForVector2Caster,IPointerForVector3Caster,IPointerForBoolCaster
    {
        #region 事件相关
        [LabelText("使用的输入键")]public InputAction inputAction;
        [Space(10)]
        [FoldoutGroup("启用的输入"), LabelText("启用按下时")] public bool useWasPress = true;
        [Space(10)]
        [FoldoutGroup("启用的输入"), LabelText("启用按住时")] public bool useIsPress = false;
        [Space(10)]
        [FoldoutGroup("启用的输入"), LabelText("启用松开时")] public bool useWasRelease = false;
        [Space(10)]
        [FoldoutGroup("启用的输入"), LabelText("启用特殊执行")] public bool useWasPerformed = false;
        public override IPointer Pointer => pointerNoneForWasPress;

        [LabelText("按下时触发"),SerializeReference,FoldoutGroup("输入事件")]public IPointerNone pointerNoneForWasPress = new PointerPickerEveryThing();
        [Space(10)][LabelText("按住时触发"), SerializeReference, FoldoutGroup("输入事件")] public IPointerNone pointerNoneForIsPress = new PointerPickerEveryThing();
        [Space(10)][LabelText("松开时触发"), SerializeReference, FoldoutGroup("输入事件")] public IPointerNone pointerNoneForWasRelease = new PointerPickerEveryThing();
        [Space(10)][LabelText("特殊条件达成时触发"), SerializeReference, FoldoutGroup("输入事件")] public IPointerNone pointerNoneForWasPerformed = new PointerPickerEveryThing();

        #endregion
        #region 值相关
        [LabelText("读取值为类型"),EnumToggleButtons,FoldoutGroup("值相关")]public ReadValueType readValueType;
        public enum ReadValueType
        {
            [InspectorName("浮点数")]Float,
            [InspectorName("二维向量")] Vector2,
            [InspectorName("三维向量")] Vector3,
                [InspectorName("布尔值")]Bool
        }
        [DisplayAsString(fontSize: 25), HideLabel, FoldoutGroup("值相关")] public string s = "可以Cast投射这些值";
        [LabelText("读取的浮点数"), FoldoutGroup("值相关")] public float readFloat = 0;
        [LabelText("读取的Vector2"), FoldoutGroup("值相关")] public Vector2 readVector2 = default;
        [LabelText("读取的Vector3"), FoldoutGroup("值相关")] public Vector3 readVector3 = default;
        [LabelText("读取的布尔值"), FoldoutGroup("值相关")] public bool readBool = default;
        #endregion
        [DetailedInfoBox("", "此处需要引用一个PointerPlayerCaster,把自己的值投射给他它", Message = @"@ ""【绑定投射目标备注："" + (playerCaster != null ? playerCaster.des : ""！未绑定"") ", VisibleIf = "@usePlayerCaster")]
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")] public PointerPlayerSystemObjectCaster playerCaster_;
        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;

        // Start is called before the first frame update
        void Start()
        {
           
        }
        private void OnEnable()
        {
            inputAction.Enable();
        }
        private void OnDisable()
        {
            inputAction.Disable();
        }
        // Update is called once per frame
        void Update()
        {
            if (useWasPress&&inputAction.WasPressedThisFrame())
            {
                pointerNoneForWasPress?.Pick();
            }
            if (useIsPress)
            {
                if (inputAction.IsPressed())
                {
                    pointerNoneForIsPress?.Pick();
                    readBool = true;
                }
                else
                {
                    readBool = false;
                }
            }
            if (useWasRelease && inputAction.WasReleasedThisFrame())
            {
                pointerNoneForWasRelease?.Pick();
            }
            if (useWasPerformed && inputAction.WasPerformedThisFrame())
            {
                pointerNoneForWasPerformed?.Pick();
            }

            switch (readValueType)
            {
                case ReadValueType.Float:
                    readFloat = inputAction.ReadValue<float>();
                    break;
                case ReadValueType.Vector2:
                    readVector2 = inputAction.ReadValue<Vector2>();
                    break;
                case ReadValueType.Vector3:
                    readVector3 = inputAction.ReadValue<Vector3>();
                    break;
                case ReadValueType.Bool:
                    if(!useIsPress) readBool = inputAction.IsPressed();
                    break;
            }
            if (usePlayerCaster && playerCaster_ != null)
            {
                playerCaster_.Recieve(readBool);
            }
        }

        public float Cast()
        {
            return readFloat;
        }

        Vector2 ICaster<Vector2>.Cast()
        {
            return readVector2;
        }

        Vector3 ICaster<Vector3>.Cast()
        {
            return readVector3;
        }

        bool ICaster<bool>.Cast()
        {
            return readBool;
        }
    }
}
