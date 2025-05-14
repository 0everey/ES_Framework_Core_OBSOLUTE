using ES.EvPointer;
using ES;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static EnumCollect;
using static UnityEngine.Rendering.DebugUI;


namespace ES
{
    public class ValueType_ : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
[Serializable, TypeRegistryItem("队列安全脏列表_持久")]
public class SafeUpdateList_EasyQueue_SeriNot_Dirty<T>
{
    [LabelText("正在更新", SdfIconType.ArrowRepeat),SerializeReference, ShowInInspector, GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForUpdating")]
    public List<T> valuesNow_ = new List<T>(10);
    [FoldoutGroup("缓冲")]
    [ShowInInspector, LabelText("缓冲添加队列", SdfIconType.BoxArrowInLeft)]
    private Queue<T> valuesToAdd = new Queue<T>();
    [FoldoutGroup("缓冲")]
    [ShowInInspector, LabelText("缓冲移除队列", SdfIconType.BoxArrowRight)]
    private Queue<T> valuesToRemove = new Queue<T>();
    private bool isDirty { get; set; }
    public void TryAdd(T add)
    {
        valuesToAdd.Enqueue(add);
        isDirty = true;
    }
    public void TryRemove(T add)
    {
        valuesToRemove.Enqueue(add);
        isDirty = true;
    }
    public bool TryContains(T who)
    {
        if (valuesToRemove.Contains(who)) return false;
        if (valuesToAdd.Contains(who)) return true;
        return valuesNow_.Contains(who);
    }
    [Button("强制更新")]
    [FoldoutGroup("缓冲")]
    private void ForceUpdate()
    {
        Update(true);
    }
    public void Update(bool forceUpdate = false)
    {
        if (isDirty || forceUpdate)
        {
            isDirty = false;
            while (valuesToAdd.Count > 0)
            {
                valuesNow_.Add(valuesToAdd.Dequeue());
            }
            while (valuesToRemove.Count > 0)
            {
                valuesNow_.Remove(valuesToRemove.Dequeue());
            }
        }
    }
}
[Serializable, TypeRegistryItem("队列安全脏集合_不持久")]
public class SafeUpdateSet_EasyQueue_SeriNot_Dirty<T>
{
    [LabelText("正在更新", SdfIconType.ArrowRepeat),ShowInInspector, GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForUpdating")]
    public HashSet<T> valuesNow_ = new HashSet<T>(10);
    [FoldoutGroup("缓冲")]
    [ShowInInspector, LabelText("缓冲添加队列", SdfIconType.BoxArrowInLeft)]
    private Queue<T> valuesToAdd = new Queue<T>();
    [FoldoutGroup("缓冲")]
    [ShowInInspector, LabelText("缓冲移除队列", SdfIconType.BoxArrowRight)]
    private Queue<T> valuesToRemove = new Queue<T>();
    private bool isDirty { get; set; }
    public void TryAdd(T add)
    {
        valuesToAdd.Enqueue(add);
        isDirty = true;
    }
    public void TryAdd(ref T add)
    {
        valuesToAdd.Enqueue(add);
        isDirty = true;
    }
    public void TryRemove(T add)
    {
        valuesToRemove.Enqueue(add);
        isDirty = true;
    }
    [Button("强制更新")]
    [FoldoutGroup("缓冲")]
    private void ForceUpdate()
    {
        Update(true);
    }
    public void Update(bool forceUpdate=false)
    {
        if (isDirty||forceUpdate)
        {
            isDirty = false;
            while (valuesToAdd.Count > 0)
            {
                valuesNow_.Add(valuesToAdd.Dequeue());
            }
            while (valuesToRemove.Count > 0)
            {
                valuesNow_.Remove(valuesToRemove.Dequeue());
            }
        }
    }
}
[Serializable,TypeRegistryItem("队列安全集合_不持久")]
public class SafeUpdateSet_EasyQueue_SeriNot<T>
{

    [LabelText("正在更新", SdfIconType.ArrowRepeat), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForUpdating")] 
    public HashSet<T> valuesNow_ = new HashSet<T>(10);
    [FoldoutGroup("缓冲")][ShowInInspector,LabelText("缓冲添加队列", SdfIconType.BoxArrowInLeft)] 
    public Queue<T> valuesToAdd = new Queue<T>();
    [FoldoutGroup("缓冲")][ShowInInspector, LabelText("缓冲移除队列", SdfIconType.BoxArrowRight)] 
    public Queue<T> valuesToRemove = new Queue<T>();
    

    public void Update()
    {
        while (valuesToAdd.Count > 0)
        {
            valuesNow_.Add(valuesToAdd.Dequeue());
        }
        while (valuesToRemove.Count > 0)
        {
            valuesNow_.Remove(valuesToRemove.Dequeue());
        }
    }
}
[Serializable]
public class SafeUpdateList<T>
{
    
    [LabelText("正在更新",SdfIconType.ArrowRepeat), SerializeReference,GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForUpdating")] public List<T> valuesNow_ = new List<T>(10);
    [FoldoutGroup("缓冲")][LabelText("缓冲添加",SdfIconType.BoxArrowInLeft),SerializeReference] public List<T> valuesToAdd = new List<T>();
    [FoldoutGroup("缓冲")][LabelText("缓冲移除",SdfIconType.BoxArrowRight), SerializeReference] public List<T> valuesToRemove = new List<T>();
    public Action<bool, T> OnChange=(Add,What)=> { };

    public void Update()
    {
        foreach(var i in valuesToAdd)
        {
            if (valuesNow_.Contains(i)) continue;
            OnChange.Invoke(true,i);
            valuesNow_.Add(i);
        }
        foreach (var i in valuesToRemove)
        {
            if (!valuesNow_.Contains(i)) continue;
            OnChange.Invoke(false, i);
            valuesNow_.Remove(i);
        }
        valuesToAdd.Clear();
        valuesToRemove.Clear();
    }
}

[Serializable/*Link收发列表IOC*/]
public class LinkRecieveSafeListIOC : TypeSafeListIOC<IReceiveLink>
{
    public override string IOCName => "Link收发表";
    public void SendLink<Link>(Link link) where Link :ILink
    {
        List<IReceiveLink> links = FindList<Link>();
        foreach(var i in links)
        {
            
            if (i is IReceiveLink<Link> irl)
            {
                irl.OnLink(link);
            }
        }
    }
    public IReceiveLink AddElement<Link>(IReceiveLink<Link> e) where Link:ILink
    {
       return AddElement(typeof(Link), e);
    }
    public void RemoveElement<Link>(IReceiveLink<Link> e) where Link:ILink { 
        RemoveElement(typeof(Link), e);
    }
}
[Serializable/*原型全匹配安全列表IOC*/]
public sealed class ArchutectureTypeMatchSafeListIOC : TypeMatchSafeListIOC<IArchitecture>
{
    public override string IOCName => "类型匹配原型表(唯一)";
}
[Serializable/*类型全匹配安全列表IOC*/]
public class TypeMatchSafeListIOC<Element> : TypeSafeListIOC<Element>
{
    public override string IOCName => "类型键直接匹配IOC表";
    public Element AddElement(Element e)
    {
        return AddElement(e.GetType(), e);
    }
    public void RemoveElement(Element e)
    {
        RemoveElement(e.GetType(), e);
    }
    public override Element Find(Type k)
    {
        Element e = base.Find(k);
        if (e != null) return e;
        return AddElement(k, (Element)Activator.CreateInstance(k));
    }
    public override List<Element> FindList(Type k)
    {
        List<Element> es = base.FindList(k);
        if (es != default) return es;
        Debug.Log(k.Name);
        Debug.Log(typeof(Element).Name);
        AddElement(k, (Element)Activator.CreateInstance(k));
        return base.FindList(k);
    }
}
[Serializable/*类型全匹配列表IOC*/]
public class TypeMatchListIOC<Element> : TypeListIOC<Element>
{
    public override string IOCName => "类型键直接匹配IOC表";
    public Element AddElement(Element e)
    {
        return AddElement(e.GetType(), e);
    }
    public void RemoveElement(Element e)
    {
        RemoveElement(e.GetType(), e);
    }
    public override Element Find(Type k)
    {
        Element e = base.Find(k);
        if (e != null) return e;
        return AddElement(k, (Element)Activator.CreateInstance(k));
    }
    public override List<Element> FindList(Type k)
    {
        List<Element> es = base.FindList(k);
        if (es != default) return es;
        Debug.Log(k.Name);
        Debug.Log(typeof(Element).Name);
        AddElement(k, (Element)Activator.CreateInstance(k));
        return base.FindList(k);
    }
}
[Serializable/*基本安全列表IOC*/]
public class TypeSafeListIOC<Element> : SafeListIOC<Type, Element> 
{
    public override string IOCName => "类型键IOC表";
    public virtual Element Find<T>()
    {
        return Find(typeof(T));
    }
    public virtual List<Element> FindList<T>()
    {
        return FindList(typeof(T));
    }
}
[Serializable/*类型列表IOC*/]
public class TypeListIOC<Element> : BaseListIOC_Arch_KeyAndList<Type, Element>
{
    public override string IOCName => "类型键IOC表";
    public virtual Element Find<T>()
    {
        return Find(typeof(T));
    }
    public virtual List<Element> FindList<T>()
    {

        return FindList(typeof(T));
    }
}
[Serializable/*安全列表IOC*/]
public abstract class SafeListIOC<Key, Element> :BaseESModule<IArchitecture>
{
    [SerializeReference]
    [LabelText(@"@  IOCName ",icon:SdfIconType.ListColumnsReverse),GUIColor("IOCColor")]
    public Dictionary<Key, SafeUpdateList<Element>> IOC = new Dictionary<Key, SafeUpdateList<Element>>();
    public Action OnChange=()=> { };
    public virtual string IOCName => "标准安全IOC表";
    public virtual Color IOCColor =>Color.yellow;
    public override IArchitecture GetHost => throw new NotImplementedException();
    public Element AddElementRuntime(Key k, Element e)
    {
        return AddElement(k, e, true);
    }
    public Element AddElement(Key k, Element e,bool isRuntime=false)
    {
        if (e == null) return default;
        
        SafeUpdateList<Element> elements = default;
        if (IOC.ContainsKey(k))
        {
            elements = IOC[k];
            if (isRuntime) elements.valuesToAdd.Add(e);
            else elements.valuesNow_.Add(e);
        }
        else
        {
            elements = new SafeUpdateList<Element>();
            if (isRuntime) elements.valuesToAdd.Add(e);
            else elements.valuesNow_.Add(e);
            IOC.Add(k, elements);
        }
        OnChange.Invoke();
        return e;
    }
    public void RemoveElementRuntime(Key k, Element e)
    {
        RemoveElement(k, e, true);
    }
    public void RemoveElement(Key k, Element e,bool isRuntime=false)
    {
        SafeUpdateList<Element> elements = default;
        if (IOC.ContainsKey(k))
        {
            elements = IOC[k];
            if (isRuntime) elements.valuesToRemove.Add(e);
            else elements.valuesNow_.Remove(e);
            OnChange.Invoke();
        }
        else
        {
#if UNITY_EDITOR
            throw new Exception("IOC没有这种键");
#endif
        }
    }
    public virtual Element Find(Key k)
    {
        
        if (IOC.ContainsKey(k))
        {
            foreach(var i in IOC[k].valuesNow_)
            {
                return i;
            }
        }
        return default;
    }

    public virtual List<Element> FindList(Key k)
    {
        
        if (IOC.ContainsKey(k))
        {
            
            return IOC[k].valuesNow_;
        }
        
        return (IOC[k]=new SafeUpdateList<Element>()).valuesNow_;
    }
}
[Serializable/*基本列表IOC*/]
public abstract class BaseListIOC_Arch_KeyAndList<Key, Element> : ES.BaseESModule<IArchitecture>
{
    [SerializeReference]
    [LabelText(@"@  IOCName ", icon: SdfIconType.ListColumnsReverse), GUIColor("IOCColor")]
    public Dictionary<Key, List<Element>> IOC = new Dictionary<Key, List<Element>>();
    
    public virtual string IOCName => "标准列表IOC表";
    public virtual Color IOCColor => Color.yellow;
   
    public Element AddElementRuntime(Key k, Element e)
    {
        return AddElement(k, e, true);
    }
    public Element AddElement(Key k, Element e, bool isRuntime = false)
    {
        if (e == null) return default;
        List<Element> elements = default;
        if (IOC.ContainsKey(k))
        {
            elements = IOC[k];
            bool has = false;
            foreach(var i in elements)
            {
                if (i.Equals(e))
                {
                    has = true;
                    break;
                }
            }
            if(!has)
            elements.Add(e);
        }
        else
        {
            elements = new List<Element>();
            elements.Add(e);
            IOC.Add(k, elements);
        }
        return e;
    }
    public void RemoveElementRuntime(Key k, Element e)
    {
        RemoveElement(k, e, true);
    }
    public void RemoveElement(Key k, Element e, bool isRuntime = false)
    {
        List<Element> elements = default;
        if (IOC.ContainsKey(k))
        {
            elements = IOC[k];
            elements.Remove(e);
        }
        else
        {
#if UNITY_EDITOR
            throw new Exception("IOC没有这种键");
#endif
        }
    }
    public virtual Element Find(Key k)
    {

        if (IOC.ContainsKey(k))
        {
            foreach (var i in IOC[k])
            {
                return i;
            }
        }
        return default;
    }

    public virtual List<Element> FindList(Key k)
    {
        if (IOC.ContainsKey(k))
        {
            return IOC[k];
        }
        return (IOC[k] = new List<Element>());
    }

    

}



[Serializable/*基本字典IOC*/]
public abstract class BaseDicIOC_TypeSelectAndKeyValue<TypeSelect_,Key, Element> : ES.BaseESModule<IArchitecture>, ES.IESModule<IArchitecture>
{
    [SerializeReference]
    [LabelText(@"@  IOCName ", icon: SdfIconType.ListColumnsReverse), GUIColor("IOCColor")]
    public Dictionary<TypeSelect_, Dictionary<Key,Element>> IOC = new Dictionary<TypeSelect_, Dictionary<Key,Element>>();
    
    public virtual string IOCName => "标准字典IOC表";
    public virtual Color IOCColor => Color.yellow;
    
    public Element AddElementRuntime(TypeSelect_ t, Key k, Element e)
    {
        return AddElement(t,k, e, true);
    }
    public Element AddElement(TypeSelect_ t,Key k, Element e, bool isRuntime = false)
    {
        if (e == null) return default;
        Dictionary<Key,Element> elements = default;
        if (IOC.ContainsKey(t))
        {
            elements = IOC[t];

            elements.Add(k,e);
        }
        else
        {
            elements = new Dictionary<Key, Element>();
            elements.Add(k,e);
            IOC.Add(t, elements);
        }
        return e;
    }
    public void RemoveElementRuntime(TypeSelect_ t, Key k)
    {
        RemoveElement(t,k,true);
    }
    public void RemoveElement(TypeSelect_ t, Key k, bool isRuntime = false)
    {
        Dictionary<Key,Element> elements = default;
        if (IOC.ContainsKey(t))
        {
            elements = IOC[t];
            elements.Remove(k);
        }
        else
        {
#if UNITY_EDITOR
            throw new Exception("IOC没有这种键");
#endif
        }
    }
    public virtual Element Find(TypeSelect_ t,Key k)
    {

        if (IOC.ContainsKey(t))
        {
            foreach (var (i,d) in IOC[t])
            {
                if (i.Equals(k))
                {
                    return d;
                }
            }
        }
        return default;
    }

    public virtual Dictionary<Key, Element> FindDic(TypeSelect_ k)
    {
        if (IOC.ContainsKey(k))
        {
            return IOC[k];
        }
        return IOC[k] = new Dictionary<Key, Element>();
    }

    public bool OnSubmitHosting(IArchitecture hosting, bool asVirtual = false)
    {
        throw new NotImplementedException();
    }

    public bool OnWithDrawHosting(IArchitecture hosting, bool asVirtual = false)
    {
        throw new NotImplementedException();
    }

  
}
[Serializable/*基本String键字典IOC*/]
public abstract class BaseDicIOCWithStringKey<TypeSelect_, Element> : BaseDicIOC_TypeSelectAndKeyValue<TypeSelect_,string, Element> 
{

}

#region 特殊IOC —— 原型参数键池专属参数集合
[Serializable, TypeRegistryItem("原型参数IOC")/*技能树参数分类-*/]
public class ArchitectureKeyValuePoolTypeListIOC : BaseListIOC_Arch_KeyAndList<EnumCollect.ArchitectureKeyValuePoolType, IArchitectureKeyValuePoolTypeValue>
{
    public Action<EnumCollect.ArchitectureKeyValuePoolType, IArchitectureKeyValuePoolTypeValue> OnHandle = (at,who) => { };
}

#endregion

#region 值类型
[Serializable, TypeRegistryItem("原型参数值_动态标签")]
public class ArchitectureTypeValue_DynamicTag : ArchitectureKeyValuePoolTypeValue<bool>
{
    [LabelText("标签是否生效")]public bool IsUseable = true;

    public override void SetValue(bool v)
    {
        IsUseable = v;
    }

    public override bool Value()
    {
        return IsUseable;
    }
}
[Serializable, TypeRegistryItem("原型参数值_浮点值")]
public class ArchitectureTypeValue_Float : ArchitectureKeyValuePoolTypeValue<float>
{
    [LabelText("当前浮点值")]public float floatValue = 10;

    public override void SetValue(float v)
    {
        Debug.Log("pp"+v);
        floatValue = v;
    }

    public override float Value()
    {
        return floatValue;
    }
}
[Serializable, TypeRegistryItem("原型参数值_整数值")]
public class ArchitectureTypeValue_Int : ArchitectureKeyValuePoolTypeValue<int>
{
    [LabelText("当前整数值")] public int intValue = 10;

    public override void SetValue(int v)
    {
        intValue = v;
    }

    public override int Value()
    {
        return intValue;
    }
}
[Serializable, TypeRegistryItem("原型参数值_字符串值")]
public class ArchitectureTypeValue_String : ArchitectureKeyValuePoolTypeValue<string>
{
    [LabelText("当前字符串值")] public string strValue = "字符串";

    public override void SetValue(string v)
    {
        strValue = v;
    }

    public override string Value()
    {
        return strValue;
    }
}
[Serializable, TypeRegistryItem("原型参数值_布尔值")]
public class ArchitectureTypeValue_Bool : ArchitectureKeyValuePoolTypeValue<bool>
{
    [LabelText("当前布尔值")] public bool boolValue = false;

    public override void SetValue(bool v)
    {
        boolValue = v;
    }

    public override bool Value()
    {
        return boolValue;
    }
  
}
[Serializable, TypeRegistryItem("原型参数值类型")]
public abstract class ArchitectureKeyValuePoolTypeValue<ValueT> : BaseWithStringKeyValue<ValueT>, IArchitectureKeyValuePoolTypeValue
{
    public IArchitectureKeyValuePoolTypeValue getArch => this;

    public object TheKey => this.key.Key();

    public object TheValue => this.Value();

    public void SetValue(object o)
    {
        if(o is ValueT vv)
        {
            (this as BaseWithStringKeyValue<ValueT>).SetValue(vv);
        }
      
    }
    
}
public interface IArchitectureKeyValuePoolTypeValue
{
    public abstract IArchitectureKeyValuePoolTypeValue getArch { get; }
    public object TheKey { get; }
    public object TheValue { get; }
    public void SetValue(object o);
    public void SetKey(object o);
}
#endregion
public static class EnumCollect
{
    public enum LinkEventType
    {
        [InspectorName("先发送Link再Invoke事件")]SendThenInvoke,
        [InspectorName("先Invoke事件再发送Link")] InvokeThenSend,
        [InspectorName("仅发送Link")] OnlySend,
        [InspectorName("仅发送Invoke")] OnlyInvoke
    }
    public enum GameEventMessageType
    {
        [InspectorName("玩家交互")]PlayerInteraction,
        [InspectorName("关卡事件")] LevelEvent,
        [InspectorName("物品收集")] ItemCollect,
    }
    public enum Localization
    {
        [InspectorName("日文")]Japan,
        [InspectorName("中文")]Chinese,
        [InspectorName("英文")]English
    }
    public enum BuffTagForGoodOrBad
    {
        Good,Bad
    }
    //照搬Unity
    public enum CompareTwoFunction
    {
        [InspectorName("相等")]Equal,
        [InspectorName("不等")] NotEqual,
        [InspectorName("小于")] Less,
        [InspectorName("小于等于")] LessEqual,
        [InspectorName("大于")] Greater,
        [InspectorName("大于等于")] GreaterEqual,
        [InspectorName("恒假")] Never,
        [InspectorName("恒真")] Always,
        [InspectorName("同向")] SameDirect,//两者相乘大于0
        [InspectorName("异向")] NotSameDirect,//两者相乘小于0
        [InspectorName("含0")] HasZero,//两者相乘等于0
        [InspectorName("都不为0")] NoZero,//两者相乘不为0
        [InspectorName("整除")] ModelMatch,//两者相乘不为0
        [InspectorName("不整除")] NotModelMatch,//两者相乘不为0
        [InspectorName("互为倒数")] Recipprocal,//两者相乘为1
        [InspectorName("互相不为倒数")] NotRecipprocal,//两者相乘不为1
        [InspectorName("二进制按位与不为0")] Mask_And_NotZero,//按位与不为0
        [InspectorName("二进制按位与为0")] Mask_ANd_Zero,//按位与为0
        
    }
    public enum HandleTwoFloatFunction
    {
        [InspectorName("设置")] Set,
        [InspectorName("加")]Add,
        [InspectorName("减")] Sub,
        [InspectorName("乘")] Muti,
        [InspectorName("除")] Divide,
        [InspectorName("取余")] Model,
        
        [InspectorName("Mask且")] Mask_And,
        [InspectorName("Mask或(开启)")] Mask_Or,
        [InspectorName("Mask异或(切换)")] Mask_Xor,
        [InspectorName("Mask且反(关闭)")] Mask_And_Not
    }

    public enum SendMessageType
    {
        [InspectorName("当前对象层")]SendMessage,
        [InspectorName("包含子对象")] BroadCastMessage,
        [InspectorName("包含父对象")] SendMessageUpWards,
    }

    public enum PointerSelectOneType
    {
        [InspectorName("第一个不为空的")] NotNullFirst,
        [InspectorName("随机选择")] RandomOnly,
        [InspectorName("下一个")] Next,
        [InspectorName("上一个")] Last,
        [InspectorName("尝试排序")] TrySort,
        [InspectorName("筛选器")] Selector,
    }
    public enum PointerSelectSomeType
    {
        [InspectorName("全部不为空的")] AllNotNull,
        [InspectorName("前几个")] StartSome,
        [InspectorName("后几个")] EndSome,
        [InspectorName("随机几个")] RandomSome,
        [InspectorName("筛选器")] Selector,
        [InspectorName("尝试排序")] TrySort,
    }
    public enum PointerMMFPlayerHandleOptions
    {
        [InspectorName("播放")]PlayFeedbacks,
        [InspectorName("停止")] StopFeedbacks,
        [InspectorName("反转")] Revert,
        [InspectorName("跳过")] SkipToTheEnd,
        [InspectorName("手动初始化")] Inititialization,
        [InspectorName("恢复初值")] RestoreInitialValues,


        [InspectorName("设置方向")] SetDirection,
        [InspectorName("设置强度")] SetIntensity,
        [InspectorName("设置时间缩放类型")] SetTimeScaleMode,
        [InspectorName("设置持续时间乘数")] SetDurationMultipler,
        [InspectorName("设置冷却")] SetCoolDown,
        [InspectorName("设置生效中心和范围")] SetRangeCenterAndDistance,
        [InspectorName("设置可用")] SetCanPlay
    }


    public enum TransformHandle_ValueSet
    {
        [InspectorName("直接设置")]Set,
        [InspectorName("加上")] Add,
        [InspectorName("减去")] Sub,
        [InspectorName("逼近")] Muti,
        [InspectorName("远离")] Div,
        [InspectorName("循环")] Model
    }
    public enum TransformHandle_ValueRele
    {
        [InspectorName("世界空间")] WorldSpace,
        [InspectorName("局部空间")] LocalSpace,
    }
    public enum ArchitectureKeyValuePoolType
    {
        [InspectorName("动态标签")]DynamicTag,//通常是某个技能解锁后会把自己放入
        [InspectorName("浮点值判据")]FloatValue,//浮点数值 通常是属性数值或者各种无法被整数确定的
        [InspectorName("整数值判据")]IntValue,//整数数值 和浮点类似，适用于精准情况
        [InspectorName("字符串值判据")]StringValue,//字符串值可用于高度自定义的各种情况，是一种加强版的标签
        [InspectorName("布尔值判据")]BoolValue,//布尔值非常简单，适用于流程控制
        [InspectorName("枚举值判据")]EnumValue//枚举值需要额外的支持，可以精确选定目标值
    }
    public enum SkillPointOneLevelState
    {
        [InspectorName("无_不显示")]None,
        [InspectorName("未知详情-显示为?")] UnknownDetail,
        [InspectorName("不允许解锁")] CantUnlock,
        [InspectorName("可解锁但条件未达到")] CanUnlockButOptionNotFeet,
        [InspectorName("条件完全达成")] CanUnlockComplete,
        [InspectorName("解锁")] Unlock
    }
    public enum CallBackType
    {
        [InspectorName("完成时")]OnComplete,
        [InspectorName("击杀时")] OnKill,
        [InspectorName("更新时")] OnUpdate,
        [InspectorName("开启时")] OnPlay,
        [InspectorName("暂停时")] OnPause,
        [InspectorName("回退时")] OnRewind,
        [InspectorName("单次循环完成时")] OnStepComplete,
        [InspectorName("路径修改时")] OnWayPointChange,
    }
    public enum PathSortType
    {
        [InspectorName("不排序")]NoneSort,
        [InspectorName("按初始从近到远")] StartFromNearToFar,
        [InspectorName("按初始从远到近")] StartFromFarToNear,
        [InspectorName("总是最近")] AlwaysFirstNear,
        [InspectorName("总是最远")] AlwaysFirstFar,
        [InspectorName("按Y向上")] Yup,
        [InspectorName("按Y向下")] Ydown,
        [InspectorName("按X增大")] Xup,
        [InspectorName("按X变小")] Xdown,
        [InspectorName("按Z变大")] Zup,
        [InspectorName("按Z变小")] Zdown,
        [InspectorName("按初始面向的变大")] StartForwardZup,
        [InspectorName("按初始面向的变小")] StartForwardZdown,
        [InspectorName("总是按面向的变大")] AlwaysForwardZup,
        [InspectorName("总是按面向的变小")] AlwaysForwardZdown,
        [InspectorName("随机")] Random
    }

    public enum ToDestinationVectorSpace
    {
        [InspectorName("到目标")] Target,
        [InspectorName("按世界空间位移")] WorldSpace,
        [InspectorName("按自身空间位移")] SelfSpace,
    }
    public enum PlacePosition
    {
        [InspectorName("按世界偏移")]WorldSpace,
        [InspectorName("按本体坐标偏移")]SelfSpace,
        [InspectorName("从发起者到目标者方向偏移")]LerpSpace,
    }
    public enum PlaceRotation
    {
        [InspectorName("按世界偏移")] WorldSpace,
        [InspectorName("按本体坐标偏移")] SelfSpace,
        [InspectorName("从发起者到目标者方向开始偏移")] LerpSpace,
    }
    public enum ToDestinationPath
    {
        [InspectorName("直接")]Direct,
        [InspectorName("跳起落地")]JumpAndDown,
        [InspectorName("弧形(不支持)")]Rad,
        [InspectorName("按寻路(不支持)")]AIPath
    }
    public enum ToDestionationBaseOn
    {
        [InspectorName("用Dotween位移")]DotweenDoMove,
        [InspectorName("用ES曲线托管移动位移")] ESCurveModule,
    }
    public enum HandleOnWhoEntityColOption
    {
        [InspectorName("作用在选择者,被发起者")]bySelectorYarnLauncher,
        [InspectorName("作用在选择者,被碰撞者")] bySelectorYarnColOn,
        [InspectorName("作用在被碰撞者,被发起者")] byColOnYarnLaucher,
        [InspectorName("作用在被碰撞者,被选择者")] byColOnYarnSelector,
        [InspectorName("作用在发起者,被选择者")] byLauncherYarnSelector,
        [InspectorName("作用在发起者,被碰撞者")] ByLauncherYarnColOn
    }
    public enum HandleCacheOption
    {
        [InspectorName("不处理")]None,
        [InspectorName("缓冲到Main主池")]ToMain,
        [InspectorName("缓冲到技能本体池")] ToSelf,
        [InspectorName("缓冲到自定义池")] ToSelfDefine
    }
    public enum SetTargetAboutDirecOption
    {
        [InspectorName("不改变方向")]None,
        [InspectorName("直接面向")] Directly,
        [InspectorName("抛物线")] Parabola,
        [InspectorName("弧形")] RadAndFollow,
        [InspectorName("按默认设置")]BySelfDefault
    }
    public enum FlyingBaseOn
    {
        [InspectorName("刚体固定更新")] RigidFixUpdate,
        [InspectorName("刚体设置一次速度")] RigidVelocityOnce,
        [InspectorName("普通变换更新")] TransUpdate,
        [InspectorName("刚体速度渐进")] RigidVelocityUpdating,
    }
    [Flags]
    public enum DestroyWhyOption
    {
        [InspectorName("生命周期到期")]LifeTime=1,
        [InspectorName("触碰实体")] OnTriEntity=2,
        [InspectorName("被环境阻挡")] OnBeBlockEnviroment=4,
        [InspectorName("被实体格挡")] OnBeBlockByEntity=8,
        [InspectorName("被飞行物碰到")] OnBeBlockByFlying=16,
        [InspectorName("加载销毁")] OnLoadKill=32,
        [InspectorName("常用")]Normal=OnTriEntity|LifeTime
    }
    public enum TargetSelectType
    {
        [InspectorName("数字计数")] Numerically = 0,
        [InspectorName("随机")] Random = 1,
        [InspectorName("程序化生成")] ProcedurallyWaypoints = 2,
        [InspectorName("玩家作为坐标")] PlayerTarget = 3
    }
    public enum AttackType
    {
        [InspectorName("近战")]Melle=1,
        [InspectorName("远程")]Range=2,
        [InspectorName("近战和远程")]MelleAndRange=3
    }
    [Flags]
    public enum AttackOnType
    {
        [InspectorName("直接命中")] Direct = 1,
        [InspectorName("按动作")] AsAction = 2,
        [InspectorName("执行序列")] HandleList =4
    }
    public enum AttackConditionRequire
    {
        [InspectorName("必须目标在范围内")]MustTargetInRange,
        [InspectorName("必须有目标即可")]MustTargetHas,
        [InspectorName("无条件攻击")]AlsoAttack,
    }

}

public static class Test233
{
    [SirenixEditorConfig]
    public class TypeRegistryUserConfig : GlobalConfig<TypeRegistryUserConfig>, IGlobalConfigEvents
    {
       
    }
}

//垃圾
#region BuffStatus
[Serializable]
public struct BuffStatusTest
{
    [LabelText("持续时间")] public float duration;
    [LabelText("等级")] public float level;
}
#endregion
