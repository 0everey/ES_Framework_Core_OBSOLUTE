
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 扩展时 建议创建新的脚本 
 修改文件时 使用 #region + 自己的名字
 格式尽量统一 
 多交流 --- Everey
 */
namespace ES.EvPointer
{
    //核心 Ev针支持 关于非项目主体或者太零碎的 垃圾场  ---部分会注释 不能删除 部分
    #region 字符串键垃圾场
    public abstract class PointerStringDataKey : PointerString
    {
        public virtual string[] Keys { get; }

        [LabelText("数据键"), ValueDropdown("Keys")] public string key;
        public override string Pick(object on= null, object from = null, object with = null)
        {
            return key;
        }
    }
    public abstract class PointerString : IPointerForString_Only
    {
        public virtual string Pick(object on= null, object from = null, object with = null)
        {
            return "null";
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }

    public abstract class DataStringKey
    {
        public virtual ISoDataPack packFrom { get; }
    }
    [Serializable]
    public class PSDG_Buff : PointerStringData_GameCenter
    {
        public override string[] Keys => KeyValueMatchingUtility.KeyPointer.PickBuffAllKeys();
    }
    [Serializable]
    public class PSDG_Item : PointerStringData_GameCenter
    {
        public override string[] Keys => KeyValueMatchingUtility.KeyPointer.PickItemAllKeys();
    }
    [Serializable]
    public class PSDG_Actor : PointerStringData_GameCenter
    {
        public override string[] Keys => KeyValueMatchingUtility.KeyPointer.PickActorAllKeys();
    }
    [Serializable]
    public class PSDG_Skill : PointerStringData_GameCenter
    {
        public override string[] Keys => KeyValueMatchingUtility.KeyPointer.PickSKillAllKeys();
    }

    public abstract class PointerStringData_GameCenter : PointerStringDataKey
    {

    }
    public abstract class PointerStrinfData_InArchitecture : PointerStringDataKey
    {
        public virtual SoDataInfoConfiguration Configuration { get; }
    }

    #endregion

    #region 用来渲染OdinGUI的一些垃圾
    //禁测试
    public static class ValueGetTestOnly
    {
        public static Color Color()
        {
            return UnityEngine.Color.yellow;
        }
        private static void test()
        {
            ValueGetTestOnly.Color();
            var i = ValueGetTestOnly.buffsNames;
        }
        public static string[] buffsNames = new string[] { "暴击强化", "攻速强化", "伤害强化", "吸血强化", };
    }

    #endregion

    #region 小型接口_内容不完善的接口
    public interface IInittable
    {
        void Init(params object[] ps);
    }
    public interface IRunLogic
    {
        object RunLogic(params object[] ps);
    }
    public interface ICancellable
    {
        object Cancel();
    }
    #endregion

    #region 游戏里的战利品相关_直接封掉
    //战利品组
    [Serializable]
    public abstract class PointerForBonus : IPointerNone
    {
        public virtual object Pick(object on= null, object from = null, object with = null)
        {
            return -1;
        }
    }
    [Serializable]
    public class PFBouns_Goblins : PointerForBonus
    {
       /* [SerializeReference, LabelText("获得魔灵数量")]
        public IPointerForInt_Only pointerForInt;
        public override object Pick(object on= null, object from = null, object with = null)
        {
            var useOn = GameCenterManager.Instance.BaseDomain.Module_PlayerState;
            int num;
            useOn.m_soul_ += num = pointerForInt?.Pick() ?? 1;
            useOn.m_soul_ = Mathf.Min(useOn.m_soul_, useOn.m_maxSoul);
            GameCenterManager.Instance.ProactiveInvoke_OnSoulCollected(num);// ProactiveInvoke_OnSoulCollected
            return base.Pick(On, From, With);
        }*/
    }
    [Serializable]
    public class PFBouns_HelathHeal : PointerForBonus
    {
       /* [SerializeReference, LabelText("治愈血量")]
        public IPointerForInt_Only pointerForInt;
        public override object Pick(object on= null, object from = null, object with = null)
        {
            var useOn = GameCenterManager.Instance.BaseDomain.Module_PlayerState;
            useOn.m_healthPoint += pointerForInt?.Pick() ?? 1;
            useOn.m_healthPoint = Mathf.Min(useOn.m_healthPoint, useOn.m_maxHealthPoint);
            return base.Pick(On, From, With);
        }*/
    }
    [Serializable]
    public class PFBouns_BuffKeyGetter : PointerForBonus
    {
        /* [LabelText("Buff的键"),ValueDropdown("@ValueGetTestOnly.buffsNames")]
         public string buffKey;*/
       /* [LabelText("使用的Buff键")]
        public KeyString_BuffUse buffKey = new KeyString_BuffUse();
        [SerializeReference, LabelText("初始状态")] public IPointerForBuffStatus startStatus;
        public override object Pick(object on= null, object from = null, object with = null)
        {
            Entity player = GameCenterManager.Instance.BaseDomain.Module_PlayerState.PlayerSelf;
            if (player != null)
            {
                player.BuffDomain?.buffHosting.AddHandle(KeyValueMatchingUtility.Creator.CreateBuffRunTimeByKey(buffKey, startStatus?.Pick()));
            }
            return base.Pick(On, From, With);
        }*/
    }
    #endregion

    //这个插件还没导入
    /*#region 插件Feel的支持
    [Serializable, TypeRegistryItem("Feel_播放_MMFPlayer", "其他插件支持")]
    public class PointerPickerFeelMMFPlayer_PlayerFeedbacks : PointerOnlyAction
    {
        [LabelText("MMF播放器"), SerializeReference] public MMF_Player mmf_Player;

        [Button("使用_MMF播放器")]
        public override object Pick(object on= null, object from = null, object with = null)
        {
            mmf_Player?.PlayFeedbacks();
            return base.Pick(On, From, With);
        }
    }
    [Serializable, TypeRegistryItem("Feel_控制_MMFPlayer", "其他插件支持")]
    public class PointerPickerFeelMMFPlayer_AllCOntrol : PointerOnlyAction
    {
        [LabelText("MMF播放器"), SerializeReference] public MMF_Player mmf_Player;
        [LabelText("控制选项"), EnumToggleButtons()]
        public EnumCollect.PointerMMFPlayerHandleOptions option;

        [InfoBox("", Message = "@FloatInfo()", VisibleIf = "@ShowFloat()")]
        [ShowIfGroup("显示float", VisibleIf = "@ShowFloat()"), SerializeReference, LabelText("", Text = "@FloatName()"), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public IPointerForFloat_Only valueFloat_ = new PointerForFloat_Direct() { float_ = 1 };
        [ShowIfGroup("显示v3", VisibleIf = "@option==EnumCollect.PointerMMFPlayerHandleOptions.SetRangeCenterAndDistance"), SerializeReference, LabelText("", Text = "生效的范围中心引用"), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")]
        public IPointerForTransform_Only valueTR_ = new PointerForTransform_Direct() { };

        private bool ShowFloat()
        {

            switch (option)
            {
                case EnumCollect.PointerMMFPlayerHandleOptions.SetDirection:
                case EnumCollect.PointerMMFPlayerHandleOptions.SetIntensity:
                case EnumCollect.PointerMMFPlayerHandleOptions.SetTimeScaleMode:
                case EnumCollect.PointerMMFPlayerHandleOptions.SetDurationMultipler:
                case EnumCollect.PointerMMFPlayerHandleOptions.SetCoolDown:
                case EnumCollect.PointerMMFPlayerHandleOptions.SetRangeCenterAndDistance:
                case EnumCollect.PointerMMFPlayerHandleOptions.SetCanPlay:
                    return true;
                default: return false;
            }
        }
        private string FloatName()
        {
            switch (option)
            {
                case EnumCollect.PointerMMFPlayerHandleOptions.SetDirection:
                    return "方向标志";
                case EnumCollect.PointerMMFPlayerHandleOptions.SetIntensity:
                    return "强度值";
                case EnumCollect.PointerMMFPlayerHandleOptions.SetTimeScaleMode:
                    return "时间缩放标志";
                case EnumCollect.PointerMMFPlayerHandleOptions.SetDurationMultipler:
                    return "持续时间乘数";
                case EnumCollect.PointerMMFPlayerHandleOptions.SetCoolDown:
                    return "冷却时间";
                case EnumCollect.PointerMMFPlayerHandleOptions.SetRangeCenterAndDistance:
                    return "范围距离";
                case EnumCollect.PointerMMFPlayerHandleOptions.SetCanPlay:
                    return "可用标志";

                default: return "无用";
            }
        }
        private string FloatInfo()
        {
            switch (option)
            {
                case EnumCollect.PointerMMFPlayerHandleOptions.SetDirection:
                    return "输入正值为正向,负值为负向";
                case EnumCollect.PointerMMFPlayerHandleOptions.SetIntensity:
                    return "直接输入强度值,1为默认值";
                case EnumCollect.PointerMMFPlayerHandleOptions.SetTimeScaleMode:
                    return "0代表不受影响,负值为FixUpdate，正值为正常缩放Update";
                case EnumCollect.PointerMMFPlayerHandleOptions.SetDurationMultipler:
                    return "时间缩放乘数,1为默认情况";
                case EnumCollect.PointerMMFPlayerHandleOptions.SetCoolDown:
                    return "设置冷却时间防止触发频繁";
                case EnumCollect.PointerMMFPlayerHandleOptions.SetRangeCenterAndDistance:
                    return "设置生效范围的距离半径";
                case EnumCollect.PointerMMFPlayerHandleOptions.SetCanPlay:
                    return "正值代表可用";

                default: return "无用";
            }
        }

        [Button("控制_MMF播放器")]
        public override object Pick(object on= null, object from = null, object with = null)
        {
            switch (option)
            {
                case EnumCollect.PointerMMFPlayerHandleOptions.PlayFeedbacks: mmf_Player?.PlayFeedbacks(); break;
                case EnumCollect.PointerMMFPlayerHandleOptions.StopFeedbacks: mmf_Player?.StopFeedbacks(); break;
                case EnumCollect.PointerMMFPlayerHandleOptions.Revert: mmf_Player?.Revert(); break;
                case EnumCollect.PointerMMFPlayerHandleOptions.SkipToTheEnd: mmf_Player?.SkipToTheEnd(); break;
                case EnumCollect.PointerMMFPlayerHandleOptions.Inititialization: mmf_Player?.Initialization(); break;
                case EnumCollect.PointerMMFPlayerHandleOptions.RestoreInitialValues: mmf_Player?.RestoreInitialValues(); break;


                case EnumCollect.PointerMMFPlayerHandleOptions.SetDirection:
                    mmf_Player?.SetDirection(valueFloat_?.Pick() > 0 ?
                    MMFeedbacks.Directions.TopToBottom : MMFeedbacks.Directions.BottomToTop); break;
                case EnumCollect.PointerMMFPlayerHandleOptions.SetIntensity:
                    if (mmf_Player != null)
                    {
                        mmf_Player.FeedbacksIntensity = valueFloat_?.Pick() ?? 1;
                    }
                    break;
                case EnumCollect.PointerMMFPlayerHandleOptions.SetTimeScaleMode:
                    if (mmf_Player != null)
                    {
                        mmf_Player.ForceTimescaleMode = (valueFloat_?.Pick() ?? 1) > 0;
                    }
                    break;
                case EnumCollect.PointerMMFPlayerHandleOptions.SetDurationMultipler:
                    if (mmf_Player != null)
                    {
                        mmf_Player.DurationMultiplier = (valueFloat_?.Pick() ?? 1);
                    }
                    break;
                case EnumCollect.PointerMMFPlayerHandleOptions.SetCoolDown:
                    if (mmf_Player != null)
                    {
                        mmf_Player.CooldownDuration = (valueFloat_?.Pick() ?? 1);
                    }
                    break;
                case EnumCollect.PointerMMFPlayerHandleOptions.SetRangeCenterAndDistance:
                    if (mmf_Player != null)
                    {
                        mmf_Player.RangeCenter = valueTR_?.Pick();
                        mmf_Player.RangeDistance = valueFloat_?.Pick() ?? 10;
                    }
                    break;
                case EnumCollect.PointerMMFPlayerHandleOptions.SetCanPlay: mmf_Player?.SetCanPlay((valueFloat_?.Pick() ?? 1) > 0); break;

            }
            return base.Pick(On, From, With);
        }
    }
    [TypeRegistryItem("Feel_加载场景", "其他插件支持")]
    [Serializable]
    public class PointerPickerFeelSceneLoadTool : PointerOnlyAction
    {
        [LabelText("最终场景名")] public string sceneNameToLoad = "场景名称";
        [LabelText("加载中场景名")] public string sceneNameForLoading = "";
        public override object Pick(object on= null, object from = null, object with = null)
        {
            if (sceneNameForLoading == null || sceneNameForLoading == "")
            {
                MMSceneLoadingManager.LoadScene(sceneNameToLoad);
            }
            else
            {
                MMSceneLoadingManager.LoadScene(sceneNameToLoad, sceneNameForLoading);
            }

            return base.Pick(On, From, With);
        }
    }
    [TypeRegistryItem("Feel_叠加加载场景", "其他插件支持")]
    [Serializable]
    public class PointerPickerFeelSceneLoadAdditiveTool : PointerOnlyAction
    {
        [LabelText("最终场景名")] public string sceneNameToLoad = "场景名称";
        [LabelText("加载中场景名")] public string sceneNameForLoading = "";
        public override object Pick(object on= null, object from = null, object with = null)
        {
            if (sceneNameForLoading == null || sceneNameForLoading == "")
            {
                MMAdditiveSceneLoadingManager.LoadScene(sceneNameToLoad);
            }
            else
            {
                MMAdditiveSceneLoadingManager.LoadScene(sceneNameToLoad, sceneNameForLoading);
            }

            return base.Pick(On, From, With);
        }
    }
    [TypeRegistryItem("Feel_停帧冻结(需要Time管理器支持)", "其他插件支持")]
    [Serializable]
    public class PointerPickerFeel_FreezeFrame : PointerOnlyAction
    {
        [LabelText("冻结时间"), SerializeReference]
        public IPointerForFloat_Only float_ = new PointerForFloat_Direct() { float_ = 0.1f };
        public override object Pick(object on= null, object from = null, object with = null)
        {
            MMFreezeFrameEvent.Trigger(float_?.Pick() ?? 0.1f);
            return base.Pick(On, From, With);
        }
    }
    [TypeRegistryItem("Feel_浮动跳字(需要FloatingText管理器支持)", "其他插件支持")]
    [Serializable]
    public class PointerPickerFeel_FloatingText : PointerOnlyAction
    {
        [LabelText("跳字内容"), SerializeReference]
        public IPointerForString_Only str_ = new PointerForString_Direc() { string_direc = "跳字" };
        [LabelText("官方_通道值")] public MMChannelData channelData;
        [LabelText("诞生位置"), SerializeReference] public IPointerForVector3_Only ve3_Pos = new PointerForVector3_Direct();
        [LabelText("速度方向"), SerializeReference] public IPointerForVector3_Only ve3_direction = new PointerForVector3_Direct();
        [LabelText("速度方向"), SerializeReference] public IPointerForFloat_Only float_iden = new PointerForFloat_Direct() { float_ = 1 };

        public override object Pick(object on= null, object from = null, object with = null)
        {
            MMFloatingTextSpawnEvent.Trigger(channelData, ve3_Pos?.Pick() ?? default, str_?.Pick(), ve3_direction?.Pick() ?? Vector3.up, float_iden?.Pick() ?? 1);
            return base.Pick(On, From, With);
        }
    }
    #endregion*/

    #region Buff测试—默认过滤而已
    public interface IPointerForBuffStatus : IPointerOnlyBackSingle<BuffStatusTest>
    {

    }
    [Serializable]
    public class PointerForBuffStatus_Base : IPointerForBuffStatus
    {
        [LabelText("直接使用基本Buff状态")] public BuffStatusTest BuffStatusTest;
        public BuffStatusTest Pick(object on= null, object from = null, object with = null)
        {
            if (BuffStatusTest.duration == 0) BuffStatusTest.duration = 10;
            if (BuffStatusTest.level == 0) BuffStatusTest.level = 1;
            return BuffStatusTest;

        }
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as IPointerForBuffStatus)?.Pick();
        }
    }
    #endregion


    #region Key与Value 原型IOC支持目前有点画蛇添足
   /* [Serializable]
    public class KeyEnumTestInventory_Direct : KeyEnum<ES.GameKeyType>
    {
        [LabelText("直接输入枚举值键")] public ES.GameKeyType enum_direc = GameKeyType.BlueKey;
        public override ES.GameKeyType Key_()
        {
            return enum_direc;
            //return base.TypeSelect_();
        }
    }*/
    [Serializable]
    public class KeyInt_Direct : KeyInt
    {
        [LabelText("直接输入Int键")] public int int_direc = 6;
        public override int Key()
        {
            return int_direc;
            //return base.TypeSelect_();
        }
    }
    [Serializable]
    public class KeyString_BuffUse : KeyString
    {
        [LabelText("Buff的键"), ValueDropdown("@ValueGetTestOnly.buffsNames")]
        public string buffKey;
        public override string Key()
        {
            if (buffKey != null) return buffKey;
            return base.Key();
            
        }
    }
    [Serializable]
    public class KeyString_Direct : KeyString
    {
        [LabelText("直接输入字符串键")] public string str_direc = "键";
        public override string Key()
        {
            if (str_direc != null) return str_direc;
            return base.Key();
        }
    }
    [Serializable, TypeRegistryItem("IOC支持键")]
    public class KeyString_IOCKey : KeyString
    {
        [LabelText("输入字符串键")] public string str_direc = "IOC键";
        public override string Key()
        {
            if (str_direc != null) return str_direc;
            return base.Key();
        }
    }
    public abstract class KeyEnum<Enum_> : BaseKey<Enum_> where Enum_ : Enum
    {
    }
    public abstract class KeyInt : BaseKey<int>
    {
    }

    public abstract class KeyString : BaseKey<string>
    {

    }
    public abstract class BaseKey<KeyType> : IKey<KeyType>
    {
        public virtual KeyType Key()
        {
            return default(KeyType);
        }
        public override int GetHashCode()
        {
            return Key()?.GetHashCode() ?? base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            // Debug.Log("Equals"+ typeof(GameKeyType));
            if (obj is IKey<KeyType> Ikey)
            {
                // Debug.Log("EqualsKey" + typeof(GameKeyType)+"handler"+TypeSelect_()+"  and  "+ IValue.TypeSelect_()+"end"+ TypeSelect_()?.Equals(IValue.TypeSelect_()));
                return (Key()?.Equals(Ikey.Key())) ?? false;
            }
            else if (obj is string s && typeof(string) == typeof(KeyType))
            {
                //  Debug.Log("str" + (TypeSelect_())+"/ "+s);
                return (Key())?.Equals(s) ?? false;
            }
            else if (obj is int i && typeof(int) == typeof(KeyType))
            {
                // Debug.Log("int" + typeof(GameKeyType));
                return (Key()?.Equals(i)) ?? false;
            }
            else if (obj is Enum e && obj.GetType() == typeof(KeyType))
            {
                return (Key()?.Equals(e)) ?? false;
            }
            else if (obj != null && typeof(KeyType) == obj.GetType())
            {
                Debug.Log("other" + typeof(KeyType));
                return Key()?.Equals((KeyType)obj) ?? false;
            }
            return base.Equals(obj);
        }
    }
    public interface IKey<out KeyType> : IKey
    {
        KeyType Key();
    }
    public interface IKey
    {

    }
    public interface IWithStringKey : IWithKey<KeyString>
    {

    }
    public interface IWithKey<out With> : IWithKey where With : IKey
    {
        With key { get; }
    }
    public interface IWithKey
    {
        void SetKey(object o);
    }
    public interface ISortable
    {
        int priority { get; }
        int Compare(ISortable i);
    }
    public interface IValue
    {

    }
    public interface IValue<out ValueT> : IValue
    {
        ValueT Value();
    }
    public abstract class BaseValue<ValueT> : IValue<ValueT>
    {
        public abstract ValueT Value();
        public abstract void SetValue(ValueT v);
        public override int GetHashCode()
        {
            return Value()?.GetHashCode() ?? base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            // Debug.Log("Equals"+ typeof(GameKeyType));
            if (obj is IValue<ValueT> IValue)
            {
                // Debug.Log("EqualsKey" + typeof(GameKeyType)+"handler"+TypeSelect_()+"  and  "+ IValue.TypeSelect_()+"end"+ TypeSelect_()?.Equals(IValue.TypeSelect_()));
                return (Value()?.Equals(IValue.Value())) ?? false;
            }
            else if (obj is string s && typeof(string) == typeof(ValueT))
            {
                //  Debug.Log("str" + (TypeSelect_())+"/ "+s);
                return (Value())?.Equals(s) ?? false;
            }
            else if (obj is int i && typeof(int) == typeof(ValueT))
            {
                // Debug.Log("int" + typeof(ValueT));
                return (Value()?.Equals(i)) ?? false;
            }
            else if (obj is Enum e && obj.GetType() == typeof(ValueT))
            {
                return (Value()?.Equals(e)) ?? false;
            }
            else if (obj != null && typeof(ValueT) == obj.GetType())
            {
                Debug.Log("other" + typeof(ValueT));
                return Value()?.Equals((ValueT)obj) ?? false;
            }
            return base.Equals(obj);
        }
    }
    public abstract class BaseWithKeyValue<AIKey, ValueT> : BaseValue<ValueT>, IWithKey<AIKey> where AIKey : IKey
    {
        public abstract AIKey key { get; }
        public abstract void SetKey(object o);
    }
    //对原型池的IOC支持
    public abstract class BaseWithStringKeyValue<ValueT> : BaseWithKeyValue<KeyString, ValueT>
    {
        [LabelText("-键-", SdfIconType.KeyboardFill), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")] public KeyString_IOCKey IOCKey = new KeyString_IOCKey();

        public override void SetKey(object o)
        {
            if (IOCKey != null)
            {
                IOCKey.str_direc = o.ToString();
            }
        }
        public override KeyString key => IOCKey;

    }
    #endregion
}
