using ES.EvPointer;
using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public class PinByArchitectureKeyValuePoolTypeListIOC_ : MonoBehaviour
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
    [Serializable/*为原型参数键值池准备的专用针*/]
    public abstract class PointerByArchitectureKeyValuePoolTypeListIOC<Back> : IPointer<Back, ArchitectureKeyValuePoolTypeListIOC, object, object>
    {
        public virtual Back Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            throw new System.NotImplementedException();
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    #region bool支持
    [Serializable/*为原型参数键值池准备的专用针*/]
    public abstract class PointerForBoolByArchitectureKeyValuePoolTypeListIOC : PointerByArchitectureKeyValuePoolTypeListIOC<bool>
    {
       
        

        public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            return false;
        }
    }
    [Serializable,TypeRegistryItem("原形键值池_布尔从完全自定义的逻辑获得", "原型键值池")]
    public  class PointerForBoolByArKVP_RunLogic : PointerForBoolByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("自定义逻辑"),SerializeReference]public IRunLogic runLogic;
        public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            object o = runLogic?.RunLogic();
            if(o is bool b)
            {
                return b;
            }
            return false;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_布尔从布尔针", "原型键值池")]
    public class PointerForBoolByArKVP_BoolPick : PointerForBoolByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("为空时的默认值")] public bool defaultIfNull = false;
        [LabelText("源布尔针"), SerializeReference] public IPointerForBool_Only bool_;
        
        public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            bool b = bool_?.Pick()?? defaultIfNull;
            return b;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_布尔比较池中浮点数", "原型键值池")]
    public class PointerForBoolByArKVP_FloatInPool : PointerForBoolByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("为空时的默认值")] public bool defaultIfNull = false;
        [LabelText("操作函数")] public EnumCollect.CompareTwoFunction function= EnumCollect.CompareTwoFunction.GreaterEqual;
        [LabelText("输入取出键"), SerializeReference] public string key;
        [LabelText("输入浮点数比较值"), SerializeReference] public float compare;

        public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            var dic = by?.IOC;
            if (dic != null)
            {
                
                if (dic.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.FloatValue))
                {
                    var list = dic[EnumCollect.ArchitectureKeyValuePoolType.FloatValue];
                    if (list != null)
                    {
                        foreach(var i in list)
                        {
                            if (i.TheKey.Equals(key))
                            {
                               if(i.TheValue is float left)
                                {
                                    return KeyValueMatchingUtility.Function.FunctionForCompareTwoFloat(left,compare,useFunction:function);
                                }
                            }
                        }
                    }
                }
            }
            return defaultIfNull;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_布尔比较池中整数", "原型键值池")]
    public class PointerForBoolByArKVP_IntInPool : PointerForBoolByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("为空时的默认值")] public bool defaultIfNull = false;
        [LabelText("操作函数")] public EnumCollect.CompareTwoFunction function = EnumCollect.CompareTwoFunction.GreaterEqual;
        [LabelText("输入取出键"), SerializeReference] public string key;
        [LabelText("输入整数比较值"), SerializeReference] public int compare;

        public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            var dic = by?.IOC;
            if (dic != null)
            {
                if (dic.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.IntValue))
                {
                    var list = dic[EnumCollect.ArchitectureKeyValuePoolType.IntValue];
                    if (list != null)
                    {
                        foreach (var i in list)
                        {
                            if (i.TheKey.Equals(key))
                            {
                                if (i.TheValue is int left)
                                {
                                    return KeyValueMatchingUtility.Function.FunctionForCompareTwoFloat(left, compare, useFunction: function);
                                }
                            }
                        }
                    }
                }
            }
            return defaultIfNull;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_布尔返回池中布尔值", "原型键值池")]
    public class PointerForBoolByArKVP_BoolInPool : PointerForBoolByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("为空时的默认值")] public bool defaultIfNull = false;
        [LabelText("输入取出键"), SerializeReference] public string key;
        public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            var dic = by?.IOC;
            if (dic != null)
            {
                if (dic.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.BoolValue))
                {
                    var list = dic[EnumCollect.ArchitectureKeyValuePoolType.BoolValue];
                    if (list != null)
                    {
                        foreach (var i in list)
                        {
                            if (i.TheKey.Equals(key))
                            {
                                if (i.TheValue is bool b)
                                {
                                    return b;
                                }
                            }
                        }
                    }
                }
            }
            return defaultIfNull;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_布尔比较池中字符串相等", "原型键值池")]
    public class PointerForBoolByArKVP_StrInPool : PointerForBoolByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("为空时的默认值")] public bool defaultIfNull = false;
        
        [LabelText("输入取出键"), SerializeReference] public string key;
        [LabelText("输入字符串比较值"), SerializeReference] public string compare;

        public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            var dic = by?.IOC;
            if (dic != null)
            {
                if (dic.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.EnumValue))
                {
                    var list = dic[EnumCollect.ArchitectureKeyValuePoolType.EnumValue];
                    if (list != null)
                    {
                        foreach (var i in list)
                        {
                            if (i.TheKey.Equals(key))
                            {
                                if (i.TheValue is string left)
                                {
                                    return left == compare;
                                }
                            }
                        }
                    }
                }
            }
            return defaultIfNull;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_布尔比较池中有标签", "原型键值池")]
    public class PointerForBoolByArKVP_TagInPool : PointerForBoolByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("为空时的默认值")] public bool defaultIfNull = false;

        [LabelText("输入取出键"), SerializeReference] public string key;
        

        public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            var dic = by?.IOC;
            if (dic != null)
            {
                if (dic.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.DynamicTag))
                {
                    var list = dic[EnumCollect.ArchitectureKeyValuePoolType.DynamicTag];
                    if (list != null)
                    {
                        foreach (var i in list)
                        {
                            if (i.TheKey.Equals(key))
                            {
                                if (i.TheValue is bool use)
                                {
                                    return use;
                                }
                            }
                        }
                    }
                }
            }
            return defaultIfNull;
        }
    }
    #region 组合

    [Serializable, TypeRegistryItem("原形键值池_布尔逻辑_否", "原型键值池")]
    public class PointerForBoolByArKVP_Not : PointerForBoolByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("!运算默认值")] public bool default_;
        [LabelText("!运算Bool针"), SerializeReference] public PointerForBoolByArchitectureKeyValuePoolTypeListIOC bool_;

        public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            return (!bool_?.Pick(by)) ?? default_;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_布尔逻辑_并且", "原型键值池")]

    public class PointerForBoolByArKVP_And : PointerForBoolByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("&&默认值")] public bool default_;
        [LabelText("&&Bool针1"), SerializeReference] public PointerForBoolByArchitectureKeyValuePoolTypeListIOC bool_1;
        [LabelText("&&Bool针2"), SerializeReference] public PointerForBoolByArchitectureKeyValuePoolTypeListIOC bool_2;

        public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            if (bool_1 == null || bool_2 == null) return default_;
            return bool_1.Pick(by) && bool_2.Pick(by);
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_布尔逻辑_或者", "原型键值池")]
    public class PointerForBoolByArKVP_Or : PointerForBoolByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("||运算默认值")] public bool default_;
        [LabelText("||运算Bool针1"), SerializeReference] public PointerForBoolByArchitectureKeyValuePoolTypeListIOC bool_1;
        [LabelText("||运算Bool针2"), SerializeReference] public PointerForBoolByArchitectureKeyValuePoolTypeListIOC bool_2;

        public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            if (bool_1 == null || bool_2 == null) return default_;
            return bool_1.Pick(by) || bool_2.Pick(by);
        }
    }

    [Serializable, TypeRegistryItem("原形键值池_布尔逻辑_全部为否", "原型键值池")]
    public class PointerForBoolByArKVP_NotAll : PointerForBoolByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("运算结果默认值")] public bool default_;
        [LabelText("全部为否Bool针"), SerializeReference] public List<PointerForBoolByArchitectureKeyValuePoolTypeListIOC> bools_=new List<PointerForBoolByArchitectureKeyValuePoolTypeListIOC>();

        public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            if (bools_ == null || bools_.Count == 0) return default_;
            foreach(var i in bools_)
            {
                if (i != null && i.Pick()==true) return false;
            }
            return true;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_布尔逻辑_全部满足", "原型键值池")]

    public class PointerForBoolByArKVP_AndAll : PointerForBoolByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("运算结果默认值")] public bool default_;
        [LabelText("全部满足Bool针"), SerializeReference] public List<PointerForBoolByArchitectureKeyValuePoolTypeListIOC> bools_ = new List<PointerForBoolByArchitectureKeyValuePoolTypeListIOC>();

        public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            if (bools_ == null || bools_.Count == 0) return default_;
            foreach (var i in bools_)
            {
                if (i != null && i.Pick() == false) return false;
            }
            return true;
        }
       
        
    }
    [Serializable, TypeRegistryItem("原形键值池_布尔逻辑_有一个即可", "原型键值池")]
    public class PointerForBoolByArKVP_HaveOne : PointerForBoolByArchitectureKeyValuePoolTypeListIOC
    {
    [LabelText("运算结果默认值")] public bool default_;
    [LabelText("全部Bool针"), SerializeReference] public List<PointerForBoolByArchitectureKeyValuePoolTypeListIOC> bools_ = new List<PointerForBoolByArchitectureKeyValuePoolTypeListIOC>();

    public override bool Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
    {
        if (bools_ == null || bools_.Count == 0) return default_;
        foreach (var i in bools_)
        {
            if (i != null && i.Pick() == true) return true;
        }
        return true;
    }
}

    #endregion
    #endregion
    #region 操作触发支持
    [Serializable/*为原型参数键值池准备的专用针*/]
    public abstract class PointerNoneByArchitectureKeyValuePoolTypeListIOC : PointerByArchitectureKeyValuePoolTypeListIOC<object>,ICancellable
    {
        public abstract object Cancel();
        public override object Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            return false;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_执行完全自定义的逻辑", "原型键值池")]
    public class PointerNoneByArKVP_RunLogic : PointerNoneByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("自定义逻辑"), SerializeReference] public IRunLogic runLogic;

        public override object Cancel()
        {
            if(runLogic is ICancellable cancellable)
            {
                cancellable.Cancel();
            }
            return -1;
        }

        public override object Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            object o = runLogic?.RunLogic();
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_执行任意触发针", "原型键值池")]
    public class PointerNoneByArKVP_BoolPick : PointerNoneByArchitectureKeyValuePoolTypeListIOC
    {
        
        [LabelText("执行"), SerializeReference] public IPointerNone none;

        public override object Cancel()
        {
            if (none is ICancellable cancellable)
            {
                cancellable.Cancel();
            }
            return -1;
        }

        public override object Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            none?.Pick();
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_操作池中浮点数", "原型键值池")]
    public class PointerNoneByArKVP_FloatInPool : PointerNoneByArchitectureKeyValuePoolTypeListIOC
    {
        
        [LabelText("操作函数")] public EnumCollect.HandleTwoFloatFunction function;
        [LabelText("输入取出键"), SerializeReference] public string key;
        [LabelText("用于操作的浮点数"), SerializeReference] public float handler;
        private float memori = 0;
        private ArchitectureKeyValuePoolTypeListIOC use;
        public override object Cancel()
        {
            Pick(use,false);
            return -1;
        }

        public override object Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            var dic = by?.IOC;
            if (dic != null)
            {
                use = by;
                if (dic.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.FloatValue))
                {
                    var list = dic[EnumCollect.ArchitectureKeyValuePoolType.FloatValue];
                    if (list != null)
                    {
                       
                        foreach (var i in list)
                        {
                            
                            if (i.TheKey.Equals(key))
                            {
                               
                                if (i.TheValue is float left)
                                {
                                    if (yarn == null)
                                    {
                                        float f;
                                        i.SetValue(f = KeyValueMatchingUtility.Function.FunctionForTwoFloat(left, handler, function));
                                        memori = f;
                                        by.OnHandle(EnumCollect.ArchitectureKeyValuePoolType.FloatValue, i);
                                    }
                                    else if (yarn is bool b)
                                    {
                                        //撤销情况 只支持加减和设置捏
                                        if (function == EnumCollect.HandleTwoFloatFunction.Set)
                                        {
                                            i.SetValue(memori);
                                            by.OnHandle(EnumCollect.ArchitectureKeyValuePoolType.FloatValue, i);
                                        }else
                                        {
                                            float f;
                                            i.SetValue(f = KeyValueMatchingUtility.Function.FunctionForTwoFloat(left, -handler, function));
                                            memori = f;
                                            by.OnHandle(EnumCollect.ArchitectureKeyValuePoolType.FloatValue, i);
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_操作池中整数", "原型键值池")]
    public class PointerNoneByArKVP_IntInPool : PointerNoneByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("为空时的默认值")] public bool defaultIfNull = false;
        [LabelText("操作函数")] public EnumCollect.HandleTwoFloatFunction function= EnumCollect.HandleTwoFloatFunction.Sub;
        [LabelText("输入取出键"), SerializeReference] public string key;
        [LabelText("用于操作的整数"), SerializeReference] public int handler;
        
        private ArchitectureKeyValuePoolTypeListIOC use;
        public override object Cancel()
        {
            Pick(use, false);
            return -1;
        }
        public override object Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            var dic = by?.IOC;
            if (dic != null)
            {
                if (dic.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.IntValue))
                {
                    var list = dic[EnumCollect.ArchitectureKeyValuePoolType.IntValue];
                    if (list != null)
                    {
                        foreach (var i in list)
                        {
                            if (i.TheKey.Equals(key))
                            {
                                if (i.TheValue is int left)
                                {
                                    i.SetValue((int)KeyValueMatchingUtility.Function.FunctionForTwoFloat(left, handler, function));
                                    by.OnHandle(EnumCollect.ArchitectureKeyValuePoolType.IntValue, i);
                                    
                                }
                            }
                        }
                    }
                }
            }
            return defaultIfNull;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_设置池中布尔值", "原型键值池")]
    public class PointerNoneByArKVP_BoolInPool : PointerNoneByArchitectureKeyValuePoolTypeListIOC
    {
        
        [LabelText("输入取出键"), SerializeReference] public string key;
        [LabelText("用于设置的布尔值"), SerializeReference] public bool handler;
        private ArchitectureKeyValuePoolTypeListIOC use;
        public override object Cancel()
        {
            Pick(use, false);
            return -1;
        }
        public override object Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            var dic = by?.IOC;
            if (dic != null)
            {
                if (dic.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.BoolValue))
                {
                    var list = dic[EnumCollect.ArchitectureKeyValuePoolType.BoolValue];
                    if (list != null)
                    {
                        foreach (var i in list)
                        {
                            if (i.TheKey.Equals(key))
                            {
                                if (i.TheValue is bool b)
                                {
                                    i.SetValue(handler);
                                    by.OnHandle(EnumCollect.ArchitectureKeyValuePoolType.BoolValue, i);
                                }
                            }
                        }
                    }
                }
            }
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_设置池中字符串", "原型键值池")]
    public class PointerNoneByArKVP_StrInPool : PointerNoneByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("输入取出键"), SerializeReference] public string key;
        [LabelText("输入设置字符串"), SerializeReference] public string handler;
        private ArchitectureKeyValuePoolTypeListIOC use;
        public override object Cancel()
        {
            Pick(use, false);
            return -1;
        }
        public override object Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            var dic = by?.IOC;
            if (dic != null)
            {
                if (dic.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.StringValue))
                {
                    var list = dic[EnumCollect.ArchitectureKeyValuePoolType.StringValue];
                    if (list != null)
                    {
                        foreach (var i in list)
                        {
                            if (i.TheKey.Equals(key))
                            {
                                if (i.TheValue is string left)
                                {
                                    i.SetValue(handler);
                                    by.OnHandle(EnumCollect.ArchitectureKeyValuePoolType.StringValue, i);
                                }
                            }
                        }
                    }
                }
            }
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_添加标签", "原型键值池")]
    public class PointerNoneByArKVP_AddTagInPool : PointerNoneByArchitectureKeyValuePoolTypeListIOC
    {
       

        [LabelText("输入取出键"), SerializeReference] public string key;
        private ArchitectureKeyValuePoolTypeListIOC use;
        public override object Cancel()
        {
            Pick(use, false);
            return -1;
        }

        public override object Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            var dic = by?.IOC;
            if (dic != null)
            {
                if (dic.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.DynamicTag))
                {
                    var list = dic[EnumCollect.ArchitectureKeyValuePoolType.DynamicTag];
                    if (list != null)
                    {
                        bool has = false;
                        foreach (var i in list)
                        {
                            if (i.TheKey.Equals(key))
                            {
                                has = true;
                            }
                        }
                        if (!has)
                        {
                            var tag = new ArchitectureTypeValue_DynamicTag();
                            tag.SetKey(key);
                            list.Add(tag);
                        }
                    }
                }
            }
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("原形键值池_删除标签", "原型键值池")]
    public class PointerNoneByArKVP_RemoveTagInPool : PointerNoneByArchitectureKeyValuePoolTypeListIOC
    {
        [LabelText("为空时的默认值")] public bool defaultIfNull = false;

        [LabelText("输入取出键"), SerializeReference] public string key;
        private ArchitectureKeyValuePoolTypeListIOC use;
        public override object Cancel()
        {
            Pick(use, false);
            return -1;
        }

        public override object Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            var dic = by?.IOC;
            if (dic != null)
            {
                if (dic.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.DynamicTag))
                {
                    var list = dic[EnumCollect.ArchitectureKeyValuePoolType.DynamicTag];
                    if (list != null)
                    {
                        ArchitectureTypeValue_DynamicTag it = default;
                        foreach (var i in list)
                        {
                            if (i.TheKey.Equals(key))
                            {
                                it = i as ArchitectureTypeValue_DynamicTag;
                            }
                        }
                        if (it != default)
                        {
                            list.Remove(it);
                        }
                    }
                }
            }
            return defaultIfNull;
        }
    }

    public abstract class PointerPackerForNoneByArKVP : PointerPackerBase<object, ArchitectureKeyValuePoolTypeListIOC, object, object, PointerNoneByArchitectureKeyValuePoolTypeListIOC>
    {

    }
    [Serializable,TypeRegistryItem("原形键值池_遍历触发")]
    public class PointerNonePackOnlyActionByArKVP_LoopOnce : PointerPackerForNoneByArKVP
    {
        public override object Pick(ArchitectureKeyValuePoolTypeListIOC by = null, object yarn = null, object on = null)
        {
            if (pointers != null)
            {
                foreach (var i in pointers)
                {
                    if (i != null)
                    {
                        i.Pick(by);
                    }
                }
            }
            return -1;
        }
    }
    #endregion
}
