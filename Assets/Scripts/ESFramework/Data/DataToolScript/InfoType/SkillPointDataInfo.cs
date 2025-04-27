using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Rendering;

namespace ES
{
    public class SkillPointDataInfo : SoDataInfo
    {
        [LabelText("等级列（Index就是等级）"), OdinSerialize, NonSerialized,InlineProperty(LabelWidth = 230)]
        public List<SkillPointLevelAllTransfomor> allLevel = new List<SkillPointLevelAllTransfomor>();


        [Serializable] 
        public class SkillPointLevelAllTransfomor
        {
            [Space(25)]
            [LabelText("显示名称",SdfIconType.Alarm),GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03"), PropertyOrder(-1)] public string displayName = "技能点-";
            
            [DisplayAsString(fontSize: 22),ShowInInspector,HideLabel,PropertyOrder(-1)] public string s_ => "↑↑↑应该被作为动态标签判据↑↑↑";
            //[LabelText("显示图标集合")]
            [InlineProperty(LabelWidth =50),Space(10),TabGroup("<1>无->未知",TextColor = "@KeyValueMatchingUtility.ColorSelector.Color_03")]
            [LabelText("从 无 到 未知详情",SdfIconType.ExclamationDiamondFill)]public ADefaultBoolAndBoolAKVP forNone2UnKnownDetail = new ADefaultBoolAndBoolAKVP() { pointer=new PointerForBoolByArKVP_TagInPool() };
            [InlineProperty(LabelWidth = 50), Space(10), TabGroup("<2>未知->不允许解锁", TextColor = "@KeyValueMatchingUtility.ColorSelector.Color_01")]
            [LabelText("从 未知详情 到 不允许解锁", SdfIconType.PatchQuestionFill)] public ADefaultBoolAndBoolAKVP forUnKnownDetail2CantUnlock = new ADefaultBoolAndBoolAKVP();
            [InlineProperty(LabelWidth = 50), Space(10), TabGroup("<3>不允许解锁->条件未达到", TextColor = "@KeyValueMatchingUtility.ColorSelector.Color_03")]
            [LabelText("从 不允许 到 可解但条件未达", SdfIconType.FileEarmarkLock2Fill)] public ADefaultBoolAndBoolAKVP forCantUnlock2CanUnlockButNotFeet= new ADefaultBoolAndBoolAKVP() { pointer = new PointerForBoolByArKVP_TagInPool() };
            [InlineProperty(LabelWidth = 50), Space(10), TabGroup("<4>条件未达到->条件达成", TextColor = "@KeyValueMatchingUtility.ColorSelector.Color_03")]
            [LabelText("从 可解但条件未达 到 条件完全达成", SdfIconType.ShieldLockFill)] public ADefaultBoolAndBoolAKVP forCanUnlockButNotFeet2CanUnlockComplete = new ADefaultBoolAndBoolAKVP() { pointer = new PointerForBoolByArKVP_FloatInPool() { function= EnumCollect.CompareTwoFunction.GreaterEqual } };
            [InlineProperty(LabelWidth = 50), Space(10), TabGroup("<5>条件达成->解锁", TextColor = "@KeyValueMatchingUtility.ColorSelector.Color_01")]
            [LabelText("从 条件完全达成 到 解锁", SdfIconType.ClipboardCheck)] public ADefaultBoolAndBoolAKVP forCanUnlockComplete2Unlock = new ADefaultBoolAndBoolAKVP() { defaultForNull = false };
            [InlineProperty(LabelWidth = 50), Space(10), TabGroup("<6>解锁操作", TextColor = "@KeyValueMatchingUtility.ColorSelector.Color_03")]
            [LabelText("解锁时触发的操作", SdfIconType.UnlockFill)] public PointerNonePackOnlyActionByArKVP_LoopOnce whenUnlock = new PointerNonePackOnlyActionByArKVP_LoopOnce() { pointers=new List<PointerNoneByArchitectureKeyValuePoolTypeListIOC>() { new PointerNoneByArKVP_FloatInPool() { function= EnumCollect.HandleTwoFloatFunction.Sub },new PointerNoneByArKVP_AddTagInPool() {  } } };
            [InlineProperty(LabelWidth = 50), Space(10), TabGroup("<6>解锁操作", TextColor = "@KeyValueMatchingUtility.ColorSelector.Color_03")]
            [LabelText("取消时触发的操作", SdfIconType.LockFill)] public PointerNonePackOnlyActionByArKVP_LoopOnce whenCancel_ = new PointerNonePackOnlyActionByArKVP_LoopOnce() { pointers = new List<PointerNoneByArchitectureKeyValuePoolTypeListIOC>() { new PointerNoneByArKVP_FloatInPool() { function = EnumCollect.HandleTwoFloatFunction.Add }, new PointerNoneByArKVP_RemoveTagInPool() { } } };

            [InlineProperty(LabelWidth = 50), Space(10), TabGroup("<7>应用操作", TextColor = "@KeyValueMatchingUtility.ColorSelector.Color_03")]
            [LabelText("应用时触发的操作", SdfIconType.UnlockFill)] public PointerNonePackOnlyActionByArKVP_LoopOnce whenApply_ = new PointerNonePackOnlyActionByArKVP_LoopOnce() { pointers = new List<PointerNoneByArchitectureKeyValuePoolTypeListIOC>() { new PointerNoneByArKVP_IntInPool() {function= EnumCollect.HandleTwoFloatFunction.Set } } };
            [InlineProperty(LabelWidth = 50), Space(10), TabGroup("<7>应用操作", TextColor = "@KeyValueMatchingUtility.ColorSelector.Color_03")]
            [LabelText("取消应用时触发的操作", SdfIconType.LockFill)] public PointerNonePackOnlyActionByArKVP_LoopOnce whenDeApply_ = new PointerNonePackOnlyActionByArKVP_LoopOnce() { pointers = new List<PointerNoneByArchitectureKeyValuePoolTypeListIOC>() { new PointerNoneByArKVP_IntInPool() { function = EnumCollect.HandleTwoFloatFunction.Set } } };




            [InlineProperty(LabelWidth = 50), LabelText("状态图标"), TabGroup("<->精灵图配置", TextColor = "@KeyValueMatchingUtility.ColorSelector.Color_04"), OdinSerialize,NonSerialized]public SkillPointSprites sprites = new SkillPointSprites();

            [Serializable,TypeRegistryItem("参数判据和默认值")]
            public class ADefaultBoolAndBoolAKVP
            {
                [LabelText("默认值")]public bool defaultForNull = true;
                [SerializeReference,LabelText("条件"),InlineProperty]public PointerForBoolByArchitectureKeyValuePoolTypeListIOC pointer;
            }
            [Serializable]
            public class SkillPointSprites
            {
                public bool b;
                [OdinSerialize]
                public Dictionary<EnumCollect.SkillPointOneLevelState, show> keyValues=new Dictionary<EnumCollect.SkillPointOneLevelState, show>() {
                    {EnumCollect.SkillPointOneLevelState.None,new show() },
                    {EnumCollect.SkillPointOneLevelState.UnknownDetail,new show() },
                    {EnumCollect.SkillPointOneLevelState.CantUnlock,new show() },
                    {EnumCollect.SkillPointOneLevelState.CanUnlockButOptionNotFeet,new show() },
                    {EnumCollect.SkillPointOneLevelState.CanUnlockComplete,new show() },
                    {EnumCollect.SkillPointOneLevelState.Unlock,new show() },
                };
                [Serializable, TypeRegistryItem("精灵与颜色")]
                public class show
                {
                    [LabelText("配置精灵")]public Sprite sprite;
                    [LabelText("配置颜色")] public Color color_=Color.white;
                }
            }
        }
       
    }
}
