using ES.EvPointer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


using UnityEngine.InputSystem;
using static ES.EnumCollect;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.Utilities;
using System.Runtime.CompilerServices;

namespace ES
{
    //Key_ ESValue Matching 是一系列键值对映射或者静态处理方法，用于解耦的功能合集
    public static partial class KeyValueMatchingUtility
    {
        #region  找键器
        public static T FindByIKey<T, Key>(IEnumerable<T> ts, Key key) where T : IWithKey<Key> where Key : IKey
        {
            if (ts != null)
            {
                foreach (var i in ts)
                {
                    if (i.key.Equals(key)) return i;
                }
            }
            return default(T);
        }
        /* public static CopyToWhere FindByKey<CopyToWhere, TypeSelect_>(IEnumerable<CopyToWhere> ts, TypeSelect_ key) where CopyToWhere : IWithKey<object> where TypeSelect_ : IKey
         {
             if (ts != null)
             {
                 foreach (var i in ts)
                 {
                     if (i.key.Equals(key)) return i;
                 }
             }
             return default(CopyToWhere);
         }*/
        public static T FindByAKey<T, Key>(IEnumerable<T> ts, Key key) where T : IWithKey<IKey<Key>>
        {
            if (ts != null)
            {
                foreach (var i in ts)
                {
                    // Debug.Log($"Compare{i},{i.key},{key}");
                    if (i.key.Equals(key)) return i;
                }
            }
            return default(T);
        }
        public static bool ContainsByIKey<T, Key>(IEnumerable<T> ts, Key key) where T : IWithKey<Key> where Key : IKey
        {
            if (ts != null)
            {
                foreach (var i in ts)
                {
                    if (i.key.Equals(key)) return true;
                }
            }
            return false;
        }
        public static bool ContainsByAKey<T, Key>(IEnumerable<T> ts, Key key) where T : IWithKey<IKey<Key>>
        {
            if (ts != null)
            {
                foreach (var i in ts)
                {
                    if (i.key.Equals(key)) return true;
                }
            }
            return false;
        }
        #endregion
        //匹配器
        public static class Matcher
        {
            public static string EnumToString_SkillPointState(EnumCollect.SkillPointOneLevelState state)
            {
                switch (state)
                {
                    case EnumCollect.SkillPointOneLevelState.None: return "无_不显示";
                    case EnumCollect.SkillPointOneLevelState.UnknownDetail: return "未知详情-显示为?";
                    case EnumCollect.SkillPointOneLevelState.CantUnlock: return "不允许解锁";
                    case EnumCollect.SkillPointOneLevelState.CanUnlockButOptionNotFeet: return "可解锁但条件未达到";
                    case EnumCollect.SkillPointOneLevelState.CanUnlockComplete: return "条件完全达成";
                    case EnumCollect.SkillPointOneLevelState.Unlock: return "解锁";
                }
                return "空定义";
            }

            public static float CalculateActorPropertyStrength(ESEntitySharedData actorData)
            {
                if (actorData == null) return 100;
                float data1 = actorData.Health * 0.1f +
                        actorData.VisionRangeAngle * 0.3f +
                         actorData.VisionRangeDis * 2f +
                          actorData.PatrolSpeed * 2f +
                           actorData.ChaseSpeed * 3f +
                           actorData.ChaseDistance * 0.1f;
                if (actorData.Attacks_.Count > 0)
                {
                    float allAttack = 0;
                    foreach (var i in actorData.Attacks_)
                    {
                        allAttack += i.CalculatePower();

                    }

                    data1 += allAttack / actorData.Attacks_.Count;
                }

                return data1
                           ;
            }
            public static T SystemObjectToT<T>(object from)
            {
                Type type = typeof(T);
                return (T)SystemObjectToT(from, type);
            }
            public static object SystemObjectToT(object from, Type type)
            {
                if (type == typeof(float))
                {
                    return Convert.ChangeType(Convert.ToSingle(from), typeof(float));
                }
                else
                {
                    return Convert.ChangeType(from, type);
                }

            }
        }

        //GUI Color 颜色库
        public static class ColorSelector
        {
            //使用方法↓
            //GUIColor("@OLDKeyValueMatchingUtilityOLD.ColorSelector.Color_03")
            public static Color Color_01 = new Color(0.988f, 0.758f, 0.763f, 1);
            public static Color Color_02 = new Color(0.9988f, 0.958f, 0.163f, 1);
            public static Color Color_03 = new Color(0.9988f, 0.958f, 0f, 1);//黄色
            public static Color Color_04 = new Color(0.1588f, 0.958f, 0.9f, 1);//色
            public static Color Color_05 = new Color(0.7588f, 0.758f, 0.25f, 1);//色
            public static Color Color_06 = new Color(0.4588f, 0.758f, 0.45f, 1);//色

            public static Color ColorForDes = new Color(0.682f, 0.8392f, 0.945f);//备注信息  --偏白
            public static Color ColorForPlayerReadMe = new Color(0.49f, 0.2353f, 0.596f);//播放器注释信息  --偏白
            public static Color ColorForCaster = new Color(0.365f, 0.6784f, 0.886f);//投射器 --偏蓝
            public static Color ColorForCatcher = new Color(0.8314f, 0.6745f, 0.051f);//抓取器   --偏橙色
            public static Color ColorForESValue = new Color(0.153f, 0.682f, 0.376f);//ES值    --偏绿
            public static Color ColorForUpdating = new Color(0.804f, 0.67843f, 0);//更新中    --偏绿

            public static Color ColorForBinding = new Color(0, 0.97f, 1);//绑定色
            public static Color ColorForSearch = new Color(0.4f, 0.804f, 0.667f);//选择色
            public static Color ColorForApply = new Color(0, 0.804f, 0);//应用色
            static void test()
            {

                Color c = KeyValueMatchingUtility.ColorSelector.Color_01;
            }
        }
        //创建器
        public static class Creator
        {
            //深拷贝＋泛型
            public static T DeepClone<T>(T obj)
            {
                return (T)DeepCloneObject(obj);
            }
            //深拷贝
            public static object DeepCloneObject(object obj)
            {
                if (obj == null)
                {
                    return null;
                }

                Type type = obj.GetType();

                // 如果是值类型或字符串，直接返回（值类型是不可变的，字符串是不可变引用类型）
                if (type.IsValueType || obj is string)
                {
                    return obj;
                }

                // 如果是数组类型
                if (type.IsArray)
                {
                    Type elementType = Type.GetType(type.FullName.Replace("[]", string.Empty));
                    var array = obj as Array;
                    Array copiedArray = Array.CreateInstance(elementType, array.Length);
                    for (int i = 0; i < array.Length; i++)
                    {
                        copiedArray.SetValue(DeepCloneObject(array.GetValue(i)), i);
                    }
                    return copiedArray;
                }

                // 如果是集合类型（如 List、Dictionary 等）
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    var copiedCollection = Activator.CreateInstance(type);
                    var addMethod = type.GetMethod("Add");
                    foreach (var item in (IEnumerable)obj)
                    {
                        addMethod.Invoke(copiedCollection, new[] { DeepCloneObject(item) });
                    }
                    return copiedCollection;
                }

                // 如果是普通引用类型或结构体
                var clonedObject = Activator.CreateInstance(type);
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    object fieldValue = field.GetValue(obj);
                    object clonedValue = DeepCloneObject(fieldValue);

                    // 如果是结构体字段，需要特殊处理
                    if (type.IsValueType && field.FieldType.IsValueType)
                    {
                        // 对于结构体字段，直接赋值即可
                        field.SetValue(clonedObject, clonedValue);
                    }
                    else
                    {
                        // 对于引用类型字段，递归拷贝
                        field.SetValue(clonedObject, clonedValue);
                    }
                }

                return clonedObject;

            }
            //创建实时BUff逻辑
            public static BuffRunTimeLogic CreateBuffRunTimeByKey(KeyString use, BuffStatusTest? statusTest = null)
            {

                BuffSoInfo info = KeyValueMatchingUtility.DataInfoPointer.PickBuffSoInfoByKey(use.Key());
                if (info == null || info.BindingLogic == null)
                {
                    return null;
                }
                return CreateBuffRunTimeByInfo(info, statusTest);
            }
            //创建实时BUff逻辑
            public static BuffRunTimeLogic CreateBuffRunTimeByInfo(BuffSoInfo buffSoInfo, BuffStatusTest? statusTest = null)
            {
                BuffRunTimeLogic buffRunTime = Activator.CreateInstance(buffSoInfo.BindingLogic) as BuffRunTimeLogic;

                if (buffRunTime != null)
                {

                    buffRunTime.buffSoInfo = buffSoInfo;
                    buffRunTime.buffStatus = statusTest ?? buffSoInfo.defaultStatus;

                    return buffRunTime;
                }
                return null;
            }
            //创建本体实时状态
            public static IESMicroState CreateStateRunTimeLogicOnlyOne(StateDataInfo info)
            {
                if (info == null) return Activator.CreateInstance<BaseWithableESStandardStateRunTimeLogic>();
                var type = info.BindingSelf ?? typeof(BaseWithableESStandardStateRunTimeLogic);
                IESMicroState state = Activator.CreateInstance(type) as IESMicroState;
                if (info.IsSonMachine)
                {
                    //子状态机 不考虑内容物
                }
                else
                {

                    //
                    state.SharedData = info.stateSharedData;
                    state.VariableData = DeepClone(info.stateStatus);
                }
                return state;
            }
            //递归创建全对象
            public static IESMicroState CreateStateRunTimeLogicComplete(StateDataInfo info)
            {
                if (info == null) return null;
                IESMicroState state = Creator.CreateStateRunTimeLogicOnlyOne(info);
                if (state == null) return null;
                //是子状态机的话 向下递归注册
                if (info.IsSonMachine && state is BaseOriginalStateMachine sonMachine)
                {
                    //状态机也要合并了
                    sonMachine.AsThis = Creator.CreateStateRunTimeLogicOnlyOne(info.BindingStandState);
                    state.SharedData = info.stateSharedData;
                    state.VariableData = DeepClone(info.stateStatus);


                    foreach (var ii in info.BindingAllStates)
                    {
                        IESMicroState stateii = Creator.CreateStateRunTimeLogicComplete(ii);
                        if (ii == null) continue;
                        sonMachine.RegisterNewState_Original(ii.key.Key(), stateii);
                    }
                }
                return state;
            }
        }
        //数据针集合器
        public static class KeyPointer
        {
            //为了性能，分成5种把
            public static string[] PickBuffAllKeys(SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                return configuration?.PackForBuff?.allInfo.Keys.ToArray() ?? default;
            }
            public static string[] PickSKillAllKeys(SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                return configuration?.PackForSkill?.allInfo.Keys.ToArray() ?? default;
            }
            public static string[] PickActorAllKeys(SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                return configuration?.PackForActor?.allInfo.Keys.ToArray() ?? default;
            }
            public static string[] PickItemAllKeys(SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                return configuration?.PackForItem?.allInfo.Keys.ToArray() ?? default;
            }

            public static string[] PickPackAllKeys(ISoDataPack pack)
            {
                var skeys = pack?.allInfo_?.Keys;
                int ii = skeys?.Count ?? 0;
                string[] keys = new string[ii];
                if (ii > 0)
                {
                    for (int i = 0; i < ii; i++)
                    {
                        skeys.CopyTo(keys, 0);
                    }
                }

                return keys;

            }
          
        }
        //取数据器
        public static class DataInfoPointer
        {
            public static BuffSoInfo PickBuffSoInfoByKey(string key, SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                if (configuration.PackForBuff.allInfo.ContainsKey(key)) return configuration.PackForBuff.allInfo[key];
                return default;
            }
            public static SkillDataInfo PickSkillSoInfoByKey(string key, SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                if (configuration.PackForSkill.allInfo.ContainsKey(key)) return configuration.PackForSkill.allInfo[key];
                return default;
            }
            public static ActorDataInfo PickActorSoInfoByKey(string key, SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                if (configuration.PackForActor.allInfo.ContainsKey(key)) return configuration.PackForActor.allInfo[key];
                return default;
            }
            public static ItemDataInfo PickItemSoInfoByKey(string key, SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                if (configuration.PackForItem.allInfo.ContainsKey(key)) return configuration.PackForItem.allInfo[key];
                return default;
            }

            public static T PickASoInfoByKey<T>(string key, ISoDataPack pack = null) where T : class
            {
                var dic = pack?.allInfo_;
                if (dic != null)
                {

                    return dic[key] as T;
                }

                return default;
            }
          
            public static List<T> PickSoInfoListByKey<T>(string[] keys, ISoDataPack pack = null) where T : class
            {
                if (keys == null || keys.Length == 0) return new List<T>();
                var dic = pack?.allInfo_;
                if (dic != null)
                {
                    List<T> ts = new List<T>();
                    foreach (var i in keys)
                    {
                        ts.Add(dic[i] as T);
                    }
                    return ts;
                }

                return new List<T>();
            }
        }
        //变换器
        public static class TransformSetter
        {
            public static void HandleTransformAtParent(Transform t, Transform parent, Vector3 pos = default, bool atWorld = true, bool localRot0 = true, bool localScale0 = true)
            {
                if (t == null) return;
                if (parent != null) t.SetParent(parent);
                if (pos != null)
                {
                    if (atWorld) t.position = pos;
                    else t.localPosition = pos;
                }
                if (localRot0) t.localRotation = Quaternion.identity;
                if (localScale0) t.localScale = Vector3.one;
            }
        }
        //反射
        public static class Reflection
        {
            public static T EasyGetField<T>(object o, string field)
            {
                FieldInfo fieldInfo = o.GetType().GetField(field);
                if (fieldInfo != null)
                {
                    return (T)fieldInfo.GetValue(o);
                }
                return default;
            }
            public static T EasyGetProperty<T>(object o, string field)
            {
                PropertyInfo propertyInfo = o.GetType().GetProperty(field);
                if (propertyInfo != null)
                {
                    return (T)propertyInfo.GetValue(o);
                }
                return default;
            }
            public static void EasySetField<T>(object o, string field, T t)
            {
                FieldInfo fieldInfo = o.GetType().GetField(field);
                if (fieldInfo != null && fieldInfo.FieldType.IsAssignableFrom(typeof(T)))
                {
                    fieldInfo.SetValue(o, t);
                }
            }
            public static void EasyHandleField<T>(object o, string field, T t, EnumCollect.HandleTwoFloatFunction func, Type type = null)
            {
                FieldInfo fieldInfo = o.GetType().GetField(field);
                if (fieldInfo != null)
                {
                    //开始处理 
                    if (t is float f && fieldInfo.FieldType.IsAssignableFrom(typeof(float)))
                    {
                        float left = Convert.ToSingle(fieldInfo.GetValue(o));
                        float right = f;
                        fieldInfo.SetValue(o, KeyValueMatchingUtility.Function.FunctionForTwoFloat(left, right, func));
                    }
                    else if (t is int i && fieldInfo.FieldType.IsAssignableFrom(typeof(int)))
                    {
                        int left = (int)Convert.ToSingle(fieldInfo.GetValue(o));
                        int right = i;
                        fieldInfo.SetValue(o, (int)KeyValueMatchingUtility.Function.FunctionForTwoFloat(left, right, func));
                    }
                    else if (t is int e && typeof(Enum).IsAssignableFrom(fieldInfo.FieldType))
                    {

                        int left = (int)Convert.ToSingle(fieldInfo.GetValue(o));
                        int right = e.GetHashCode();
                        fieldInfo.SetValue(o, (int)KeyValueMatchingUtility.Function.FunctionForTwoFloat(left, right, func));
                    }
                    else if (t is int lay && typeof(LayerMask).IsAssignableFrom(fieldInfo.FieldType))
                    {

                        int left = (LayerMask)fieldInfo.GetValue(o);//LayerMask.GetMask((LayerMask.LayerToName((LayerMask)fieldInfo.GetValue(o))));
                        int right = lay;
                        fieldInfo.SetValue(o, (LayerMask)KeyValueMatchingUtility.Function.FunctionForTwoFloat(left, right, func));
                    }


                }
            }
            public static void EasySetProperty<T>(object o, string field, T t)
            {
                PropertyInfo propertyInfo = o.GetType().GetProperty(field);
                if (propertyInfo != null && propertyInfo.PropertyType.IsAssignableFrom(typeof(T)))
                {
                    propertyInfo.SetValue(o, t);
                }
            }
            public static void EasyHandleProperty<T>(object o, string field, T t, EnumCollect.HandleTwoFloatFunction func)
            {
                PropertyInfo propertyInfo = o.GetType().GetProperty(field);
                if (propertyInfo != null)
                {
                    //开始处理 
                    if (t is float f && propertyInfo.PropertyType.IsAssignableFrom(typeof(float)))
                    {
                        float left = (int)Convert.ToSingle(propertyInfo.GetValue(o));
                        float right = f;
                        propertyInfo.SetValue(o, KeyValueMatchingUtility.Function.FunctionForTwoFloat(left, right, func));
                    }
                    //开始处理 
                    else if (t is int i && propertyInfo.PropertyType.IsAssignableFrom(typeof(int)))
                    {
                        int left = Convert.ToInt32(propertyInfo.GetValue(o));
                        int right = i;
                        propertyInfo.SetValue(o, (int)KeyValueMatchingUtility.Function.FunctionForTwoFloat(left, right, func));
                    }
                    else if (t is int e && typeof(Enum).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        Debug.Log("Yes");
                        int left = (int)Convert.ToSingle(propertyInfo.GetValue(o));
                        int right = e.GetHashCode();
                        propertyInfo.SetValue(o, (int)KeyValueMatchingUtility.Function.FunctionForTwoFloat(left, right, func));
                    }

                }
            }
            public static T EasyGetMethod<T>(object o, string method) where T : Delegate
            {
                MethodInfo methodInfo = o.GetType().GetMethod(method);
                if (methodInfo != null)
                {
                    return (T)methodInfo.CreateDelegate(typeof(T));
                }
                return default;
            }
            public static void EasyInvokeMethod(object o, string method, params object[] objects)
            {
                MethodInfo methodInfo = o.GetType().GetMethod(method);
                if (methodInfo != null)
                {

                    methodInfo.Invoke(o, objects);
                }
            }
        }
        //遍历/递归器
        public static class Foreach
        {
            public static Transform ForeachFindTransform(Transform pa, string name)
            {
                if (pa == null || pa.childCount == 0) return default;
                Transform find = pa.Find(name);
                if (find != null) return find;
                int all = pa.childCount;
                for (int i = 0; i < all; i++)
                {
                    find = ForeachFindTransform(pa.GetChild(i), name);
                    if (find != null)
                    {
                        return find;
                    }
                }
                return default;
            }
        }
        //函数器
        public static class Function
        {
            //操作两个FLoat
            public static float FunctionForTwoFloat(float f1, float f2, EnumCollect.HandleTwoFloatFunction twoFloatFunction)
            {
                switch (twoFloatFunction)
                {
                    case EnumCollect.HandleTwoFloatFunction.Add: return f1 + f2;
                    case EnumCollect.HandleTwoFloatFunction.Sub: return f1 - f2;
                    case EnumCollect.HandleTwoFloatFunction.Muti: return f1 * f2;
                    case EnumCollect.HandleTwoFloatFunction.Divide: if (f2 == 0) f2 = 1; return f1 / f2;
                    case EnumCollect.HandleTwoFloatFunction.Model: if (f2 == 0) f2 = 1; return f1 % f2;
                    case EnumCollect.HandleTwoFloatFunction.Mask_And: return (int)f1 & (int)f2;
                    case EnumCollect.HandleTwoFloatFunction.Mask_Or: return (int)f1 | (int)f2;
                    case EnumCollect.HandleTwoFloatFunction.Mask_Xor: return (int)f1 ^ (int)f2;
                    case EnumCollect.HandleTwoFloatFunction.Mask_And_Not: return (int)f1 & ~(int)f2;
                    default: return f2;
                }
            }

            //比较两个Float
            public static bool FunctionForCompareTwoFloat(float left, float right, EnumCollect.CompareTwoFunction useFunction)
            {
                switch (useFunction)
                {
                    case EnumCollect.CompareTwoFunction.Equal: return left == right;
                    case EnumCollect.CompareTwoFunction.NotEqual: return left != right;
                    case EnumCollect.CompareTwoFunction.Less: return left < right;
                    case EnumCollect.CompareTwoFunction.LessEqual: return left <= right;
                    case EnumCollect.CompareTwoFunction.Greater: return left > right;
                    case EnumCollect.CompareTwoFunction.GreaterEqual: return left >= right;

                    case EnumCollect.CompareTwoFunction.Never: return false;
                    case EnumCollect.CompareTwoFunction.Always: return true;
                    case EnumCollect.CompareTwoFunction.SameDirect: return left * right > 0;
                    case EnumCollect.CompareTwoFunction.NotSameDirect: return left * right < 0;
                    case EnumCollect.CompareTwoFunction.HasZero: return left * right == 0;
                    case EnumCollect.CompareTwoFunction.NoZero: return (left * right) != 0;

                    case EnumCollect.CompareTwoFunction.ModelMatch:
                        if (right == 0) return false;
                        if (left / right == (int)(left / right)) return true;
                        else return false;
                    case EnumCollect.CompareTwoFunction.NotModelMatch:
                        if (right == 0) return false;
                        if (left / right == (int)(left / right)) return false;
                        else return true;
                    case EnumCollect.CompareTwoFunction.Recipprocal: return left * right == 1;
                    case EnumCollect.CompareTwoFunction.NotRecipprocal: return left * right != 0;
                    case EnumCollect.CompareTwoFunction.Mask_And_NotZero: return ((int)left & (int)right) != 0;
                    case EnumCollect.CompareTwoFunction.Mask_ANd_Zero: return ((int)left & (int)right) == 0;

                }
                return false;
            }
            //取出一个
            public static T GetOne<T>(List<T> values, EnumCollect.PointerSelectOneType selectOneType, ref int lastIndex)
            {
                if (values != null)
                {
                    if (values.Count > 0)
                    {
                        int lastP = lastIndex;
                        lastIndex = 0;
                        if (values.Count > 1)
                        {
                            switch (selectOneType)
                            {
                                case EnumCollect.PointerSelectOneType.NotNullFirst:
                                    for (int i = 0; i < values.Count; i++)
                                    {
                                        if (values[i] != null)
                                        {
                                            lastIndex = i;
                                            break;
                                        }
                                    }
                                    break;
                                case EnumCollect.PointerSelectOneType.RandomOnly:
                                    lastIndex = UnityEngine.Random.Range(0, values.Count);
                                    break;
                                case EnumCollect.PointerSelectOneType.Next:
                                    lastIndex = lastP + 1;
                                    lastIndex %= values.Count;
                                    break;
                                case EnumCollect.PointerSelectOneType.Last:
                                    lastIndex = lastP + values.Count - 1;
                                    lastIndex %= values.Count;
                                    break;
                                case EnumCollect.PointerSelectOneType.TrySort:
                                    //Do NothingNow

                                    break;
                                default: break;
                            }
                        }
                    }
                    if (lastIndex >= 0) return values[lastIndex];
                }
                return default;
            }
            //取出部分
            public static List<T> GetSome<T>(List<T> values, EnumCollect.PointerSelectSomeType selectSomeType, ref int lastIndex)
            {
                if (values != null)
                {
                    if (values.Count > 0)
                    {

                        if (values.Count > 1)
                        {
                            List<T> ps = values.Where(n => n != null).ToList();

                            switch (selectSomeType)
                            {
                                case EnumCollect.PointerSelectSomeType.AllNotNull:

                                    break;
                                case EnumCollect.PointerSelectSomeType.StartSome:
                                    int removeTimes = ps.Count - lastIndex;
                                    for (int i = 0; i < removeTimes; i++)
                                    {
                                        if (lastIndex < ps.Count) ps.RemoveAt(lastIndex);
                                    }
                                    break;
                                case EnumCollect.PointerSelectSomeType.EndSome:
                                    int removeTimes2 = ps.Count - lastIndex;
                                    for (int i = 0; i < removeTimes2; i++)
                                    {
                                        if (ps.Count > 1) ps.RemoveAt(0);
                                    }
                                    break;
                                case EnumCollect.PointerSelectSomeType.RandomSome:
                                    int num = Mathf.Min(ps.Count, lastIndex);
                                    List<T> ps2 = ps.OrderBy(n => UnityEngine.Random.value).Take(num).ToList();
                                    ps = ps2;
                                    break;
                                case EnumCollect.PointerSelectSomeType.Selector: break;
                                case EnumCollect.PointerSelectSomeType.TrySort: break;
                            }
                            return ps;
                        }
                    }

                }
                return default;
            }
            //dotween集成
            public static Delegate GetCallBackFromTween(Tween use, EnumCollect.CallBackType callBackType)
            {
                if (use != null)
                {
                    switch (callBackType)
                    {
                        case EnumCollect.CallBackType.OnComplete:
                            return use.onComplete;
                        case EnumCollect.CallBackType.OnKill:
                            return use.onKill;
                        case EnumCollect.CallBackType.OnUpdate:
                            return use.onUpdate;
                        case EnumCollect.CallBackType.OnPlay:
                            return use.onPlay;
                        case EnumCollect.CallBackType.OnPause:
                            return use.onPause;
                        case EnumCollect.CallBackType.OnRewind:
                            return use.onRewind;
                        case EnumCollect.CallBackType.OnStepComplete:
                            return use.onStepComplete;
                        case EnumCollect.CallBackType.OnWayPointChange:
                            return use.onWaypointChange;
                    }
                }
                return default;
            }
            public static void SetCallBackFromTween(Tween use, EnumCollect.CallBackType callBackType, TweenCallback action)
            {
                if (use != null)
                {
                    switch (callBackType)
                    {
                        case EnumCollect.CallBackType.OnComplete:
                            use.OnComplete(action);
                            break;
                        case EnumCollect.CallBackType.OnKill:
                            use.OnKill(action);
                            break;
                        case EnumCollect.CallBackType.OnUpdate:
                            use.OnUpdate(action);
                            break;
                        case EnumCollect.CallBackType.OnPlay:
                            use.OnPlay(action);
                            break;
                        case EnumCollect.CallBackType.OnPause:
                            use.OnPause(action);
                            break;
                        case EnumCollect.CallBackType.OnRewind:
                            use.OnRewind(action);
                            break;
                        case EnumCollect.CallBackType.OnStepComplete:
                            use.OnStepComplete(action);
                            break;
                        case EnumCollect.CallBackType.OnWayPointChange:
                            //use_.OnWaypointChange(action);
                            break;
                    }
                }
                return;
            }
            //Function_OperationValue_InLine
            [MethodImpl(methodImplOptions:MethodImplOptions.AggressiveInlining)]
            public static float OpearationFloat_Inline(float value, float Value, OperationHandleTypeForFloat settleType)
            {
                switch (settleType)
                {
                    case OperationHandleTypeForFloat.Add: return value + Value;
                    case OperationHandleTypeForFloat.Sub: return value - Value;
                    case OperationHandleTypeForFloat.PerUp: return value * (1 + Value);
                    case OperationHandleTypeForFloat.Max: return Mathf.Clamp(value, value, Value);
                    case OperationHandleTypeForFloat.Min: return Mathf.Clamp(value, Value, value);
                    case OperationHandleTypeForFloat.Wave: return value + UnityEngine.Random.Range(-Value, Value);
                    default: return value;
                }
            }
            [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
            public static float OpearationFloat_Cancel_Inline(float value, float Value, OperationHandleTypeForFloat settleType)
            {
                switch (settleType)
                {
                    case OperationHandleTypeForFloat.Add: return value - Value;
                    case OperationHandleTypeForFloat.Sub: return value + Value;
                    case OperationHandleTypeForFloat.PerUp: return value.SafeDivide(1 + Value);
                    case OperationHandleTypeForFloat.Max: return Mathf.Clamp(value, value, Value);
                    case OperationHandleTypeForFloat.Min: return Mathf.Clamp(value, Value, value);
                    case OperationHandleTypeForFloat.Wave: return value + UnityEngine.Random.Range(-Value, Value);
                    default: return value;
                }
            }
        }
        //排序器
        public static class Sorter
        {
            public static List<Vector3> SortPath(List<Vector3> vectors, EnumCollect.PathSortType sortType, Vector3 pos = default, Transform transform = null)
            {
                if (vectors == null) return new List<Vector3>();
                if (vectors.Count <= 1) return vectors;

                switch (sortType)
                {
                    case PathSortType.NoneSort: return vectors;
                    case PathSortType.StartFromNearToFar:
                        return vectors.OrderBy((n) => Vector3.Distance(pos, n)).ToList();
                    case PathSortType.StartFromFarToNear:
                        return vectors.OrderByDescending((n) => Vector3.Distance(pos, n)).ToList();
                    case PathSortType.Yup:
                        return vectors.OrderBy((n) => n.y).ToList();
                    case PathSortType.Ydown:
                        return vectors.OrderByDescending((n) => n.y).ToList();
                    case PathSortType.Xup:
                        return vectors.OrderBy((n) => n.x).ToList();
                    case PathSortType.Xdown:
                        return vectors.OrderByDescending((n) => n.x).ToList();
                    case PathSortType.Zup:
                        return vectors.OrderBy((n) => n.z).ToList();
                    case PathSortType.Zdown:
                        return vectors.OrderByDescending((n) => n.z).ToList();
                    case PathSortType.StartForwardZup:
                        if (transform != null)
                            return vectors.OrderBy((n) => transform.InverseTransformPoint(n).z).ToList();
                        else return vectors.OrderBy((n) => n.z).ToList();
                    case PathSortType.StartForwardZdown:
                        if (transform != null)
                            return vectors.OrderByDescending((n) => transform.InverseTransformPoint(n).z).ToList();
                        else return vectors.OrderByDescending((n) => n.z).ToList();
                    case PathSortType.Random:
                        return vectors.OrderBy((n) => UnityEngine.Random.value).ToList();
                    case PathSortType.AlwaysFirstNear:
                        return SortForLast_Near(vectors, pos);
                    case PathSortType.AlwaysFirstFar:
                        return SortForLast(vectors, pos, (a, b) => -Vector3.Distance(a, b));
                    case PathSortType.AlwaysForwardZup:
                        return SortForLast_Three(vectors, pos, (a, b, c) =>
                        {
                            if (b != c)
                            {
                                return Vector3.Angle(a - b, b - c);
                            }
                            return Vector3.Distance(a, b);

                        });
                    case PathSortType.AlwaysForwardZdown:
                        return SortForLast_Three(vectors, pos, (a, b, c) =>
                        {
                            if (b != c)
                            {
                                return -Vector3.Angle(a - b, b - c);
                            }
                            return -Vector3.Distance(a, b);

                        });
                }
                return vectors;
            }

            public static List<T> SortAny<T>(List<T> vectors, Func<T, Vector3> GetPos, EnumCollect.PathSortType sortType, Vector3 pos = default, Transform transform = null)
            {
                if (vectors == null) return new List<T>();
                if (vectors.Count <= 1) return vectors;

                switch (sortType)
                {
                    case PathSortType.NoneSort: return vectors;
                    case PathSortType.StartFromNearToFar:
                        return vectors.OrderBy((n) => Vector3.Distance(pos, GetPos(n))).ToList();
                    case PathSortType.StartFromFarToNear:
                        return vectors.OrderByDescending((n) => Vector3.Distance(pos, GetPos(n))).ToList();
                    case PathSortType.Yup:
                        return vectors.OrderBy((n) => GetPos(n).y).ToList();
                    case PathSortType.Ydown:
                        return vectors.OrderByDescending((n) => GetPos(n).y).ToList();
                    case PathSortType.Xup:
                        return vectors.OrderBy((n) => GetPos(n).x).ToList();
                    case PathSortType.Xdown:
                        return vectors.OrderByDescending((n) => GetPos(n).x).ToList();
                    case PathSortType.Zup:
                        return vectors.OrderBy((n) => GetPos(n).z).ToList();
                    case PathSortType.Zdown:
                        return vectors.OrderByDescending((n) => GetPos(n).z).ToList();
                    case PathSortType.StartForwardZup:
                        if (transform != null)
                            return vectors.OrderBy((n) => transform.InverseTransformPoint(GetPos(n)).z).ToList();
                        else return vectors.OrderBy((n) => GetPos(n).z).ToList();
                    case PathSortType.StartForwardZdown:
                        if (transform != null)
                            return vectors.OrderByDescending((n) => transform.InverseTransformPoint(GetPos(n)).z).ToList();
                        else return vectors.OrderByDescending((n) => GetPos(n).z).ToList();
                    case PathSortType.Random:
                        return vectors.OrderBy((n) => UnityEngine.Random.value).ToList();

                }
                return vectors;
            }
            public static List<Vector3> SortForLast_Near(List<Vector3> vectors, Vector3 pos)
            {
                List<Vector3> reSort = new List<Vector3>(vectors);

                for (int i = 0; i < vectors.Count; i++)
                {
                    Vector3 last = i == 0 ? pos : reSort[i - 1];

                    float dis = 9999;
                    int minIndex = i;
                    for (int j = i; j < vectors.Count; j++)
                    {
                        float disN;
                        if ((disN = Vector3.Distance(reSort[j], last)) < dis)
                        {
                            minIndex = j;
                            dis = disN;
                        }
                    }
                    Vector3 cache = reSort[i];
                    reSort[i] = reSort[minIndex];
                    reSort[minIndex] = cache;
                }
                return reSort;
            }
            public static List<T> SortForLast<T>(List<T> ts, T start, Func<T, T, float> func)
            {
                List<T> reSort = new List<T>(ts);

                for (int i = 0; i < ts.Count; i++)
                {
                    T last = i == 0 ? start : reSort[i - 1];

                    float dis = 9999;
                    int minIndex = i;
                    for (int j = i; j < ts.Count; j++)
                    {
                        float disN;
                        if ((disN = func.Invoke(reSort[j], last)) < dis)
                        {
                            minIndex = j;
                            dis = disN;
                        }
                    }
                    T cache = reSort[i];
                    reSort[i] = reSort[minIndex];
                    reSort[minIndex] = cache;
                }
                return reSort;
            }
            public static List<T> SortForLast_Three<T>(List<T> ts, T start, Func<T, T, T, float> func)
            {
                List<T> reSort = new List<T>(ts);

                for (int i = 0; i < ts.Count; i++)
                {
                    T last = i == 0 ? start : reSort[i - 1];
                    T lastLast = i <= 2 ? start : reSort[i - 2];
                    float dis = 9999;
                    int minIndex = i;
                    for (int j = i; j < ts.Count; j++)
                    {
                        float disN;
                        if ((disN = func.Invoke(reSort[j], last, lastLast)) < dis)
                        {
                            minIndex = j;
                            dis = disN;
                        }
                    }
                    T cache = reSort[i];
                    reSort[i] = reSort[minIndex];
                    reSort[minIndex] = cache;
                }
                return reSort;
            }
        }
        //数据应用器
        public static class DataApply
        {
            #region Copy
            //完全同类型的具有Class变量-
            public static void CopyToClassSameType_WithSharedAndVariableDataCopyTo<Shared, Variable>(IWithSharedAndVariableData<Shared, Variable> from, IWithSharedAndVariableData<Shared, Variable> to)
                where Shared : ISharedData
                where Variable : class, IVariableData
            {
                if (from != null && to != null)
                {
                    to.SharedData = from.SharedData;
                    if (to.VariableData == null)
                    {
                        to.VariableData = Activator.CreateInstance<Variable>();
                    }

                    if (from.VariableData is ICopyToClass<Variable> copy)
                    {
                        copy.CopyTo(to.VariableData);
                    }
                    else
                    {
                        to.VariableData = Creator.DeepClone<Variable>(from.VariableData);
                    }
                }
            }
            //不同类型的具有Class变量
            public static void CopyToClassDynamic_WithSharedAndVariableDataCopyTo<SharedFrom, VariableFrom, SharedTo, VariableTo>(IWithSharedAndVariableData<SharedFrom, VariableFrom> from, IWithSharedAndVariableData<SharedTo, VariableTo> to)
                where SharedFrom : SharedTo, ISharedData where VariableFrom : class, VariableTo, IVariableData
                where SharedTo : ISharedData where VariableTo : class, IVariableData
            {
                if (from != null && to != null)
                {
                    to.SharedData = from.SharedData;
                    if (to.VariableData == null)
                    {
                        to.VariableData = Activator.CreateInstance<VariableTo>();
                    }
                    if (from.VariableData is ICopyToClass<VariableTo> copy)
                    {
                        copy.CopyTo(to.VariableData);
                    }
                    else
                    {
                        to.VariableData = Creator.DeepClone<VariableFrom>(from.VariableData);
                    }
                }
            }
            //完全同类型的具有Struct变量
            public static void CopyToStructSameType_WithSharedAndVariableDataCopyTo<Shared, Variable>(IWithSharedAndVariableData<Shared, Variable> from, IWithSharedAndVariableData<Shared, Variable> to)
                where Shared : ISharedData
                where Variable : struct, IVariableData
            {
                if (from != null && to != null)
                {
                    to.SharedData = from.SharedData;
                    //结构体不需要实现CopyTo
                    to.VariableData = Creator.DeepClone<Variable>(from.VariableData);
                }
            }
            //不同类型的具有Struct变量
            public static void CopyToStructDynamic_WithSharedAndVariableDataCopyTo<SharedFrom, Variable, SharedTo>(IWithSharedAndVariableData<SharedFrom, Variable> from, IWithSharedAndVariableData<SharedTo, Variable> to)
                where SharedFrom : SharedTo, ISharedData where Variable : struct, IVariableData
                where SharedTo : ISharedData
            {
                if (from != null && to != null)
                {
                    to.SharedData = from.SharedData;
                    //结构体不需要实现CopyTo
                    to.VariableData = Creator.DeepClone<Variable>(from.VariableData);
                }
            }
            #endregion

            public static void ApplyStatePackToMachine(StateDataPack pack, BaseOriginalStateMachine machine)
            {
                if (pack != null && machine != null)
                {
                    foreach (var i in pack.allInfo)
                    {
                        var use = i.Value;
                        if (use == null) continue;
                        //只要第一层的直接注入哈
                        if (use.asFirstLayer)
                        {
                            IESMicroState state = Creator.CreateStateRunTimeLogicComplete(use);
                            if (state == null) continue;
                            machine.RegisterNewState_Original(i.Key, state);
                        }
                    }
                }
            }
            public static BuffRunTimeLogic ApplyBuffInfoToEntity(BuffSoInfo buffSoInfo, Entity entity, BuffStatusTest? buffStatusTest = null)
            {
                if (buffSoInfo != null && entity != null)
                {
                    var create = Creator.CreateBuffRunTimeByInfo(buffSoInfo, buffStatusTest);
                    entity.BuffDomain.buffHosting.AddHandle(create);
                    return create;
                }
                return null;
            }

            public static void Apply_Remove_BuffInfoToEntity(BuffSoInfo buffSoInfo, Entity entity)
            {
                if (buffSoInfo != null && entity != null)
                {
                    string s = buffSoInfo.key.Key();
                    foreach (var i in entity.BuffDomain.buffHosting.buffRTLs.valuesNow_)
                    {
                        if (i.buffSoInfo.key.Key() == s)
                        {
                            entity.BuffDomain.buffHosting.RemoveHandle(i);
                        }
                    }
                }
            }
        }

        //Link事件
        public static class ESLink
        {
            public static class ForEntityLink
            {
                public static void OnEntityLink(Entity entity, ILink linkDefault, bool ApplyOrCancel = true)
                {

                }

            }
            public static class Global
            {
                public static void GlobalLink_EntityAttackEntityTryStart(Link_EntityAttackEntityTryStart link_Attack_Try)
                {
                    if (link_Attack_Try.attacker == null || link_Attack_Try.victim == null) return;
                    Debug.Log("攻击测试开始");
                    //攻击者填充增益
                    link_Attack_Try.attacker.Invoke_TryAttackEntityCalculate(link_Attack_Try.victim, link_Attack_Try.damage);
                    if (link_Attack_Try.damage.canTrigger.Value > 0)
                    {
                        Debug.Log("攻击者测试通过");
                        //被攻击者填充增益
                        link_Attack_Try.victim.Invoke_BeAttackByEntityCalculate(link_Attack_Try.attacker, link_Attack_Try.damage);
                        if (link_Attack_Try.damage.canTrigger.Value > 0)
                        {
                            Debug.Log("被攻击测试通过");
                            //攻击者追加
                            link_Attack_Try.attacker.Invoke_TrulyAttack(link_Attack_Try.victim, link_Attack_Try.damage);
                            //被攻击者追加
                            link_Attack_Try.victim.Invoke_TrulyBeAttack(link_Attack_Try.attacker, link_Attack_Try.damage);
                            GameCenterManager.Instance.GameCenterArchitecture.SendLink(
                                new Link_EntityAttackEntityTruely()
                                {
                                    attacker = link_Attack_Try.attacker,
                                    victim = link_Attack_Try.victim,
                                    damage = link_Attack_Try.damage
                                }
                              ); ;
                        }
                    }


                }
            }
        }

        public static class ESBack
        {
            public static class ForEntityBack
            {
                public static List<Entity> GetEntityAroundFriend(Entity entity, float r, Vector3? center = null)
                {

                    var use = Physics.OverlapSphere(center ?? entity.transform.position, r);
                    List<Entity> entities = new List<Entity>();
                    foreach (var i in use)
                    {
                        Entity ee = i.GetComponent<Entity>();
                        if (ee != null && !entities.Contains(ee)) entities.Add(ee);
                    }
                    //查找把，找啊找r
                    return entities;
                }
                public static List<Entity> GetEntityAround(Entity entity, float r, Vector3? center = null)
                {

                    var use = Physics.OverlapSphere(center ?? entity.transform.position, r);
                    List<Entity> entities = new List<Entity>();
                    foreach (var i in use)
                    {
                        Entity ee = i.GetComponent<Entity>();
                        if (ee != null && !entities.Contains(ee)) entities.Add(ee);
                    }
                    //查找把，找啊找r
                    return entities;
                }
                public static List<Entity> GetEntityTargetEntityCache(Entity entity, string Key = "Main", bool useAndClear = true)
                {

                    //查找把，找啊找r
                    if (entity?.BaseDomain.Module_Cache != null)
                    {
                        if (useAndClear)
                        {
                            return entity.BaseDomain.Module_Cache.CacheEntity.DequeueAll(Key).ToList();
                        }
                        else
                        {
                            return entity.BaseDomain.Module_Cache.CacheEntity.PeekAll(Key);
                        }

                    }
                    return null;//返回缓冲池
                }
                public static List<Vector3> GetEntityTargetVector3Cache(Entity entity, string Key = "Main", bool useAndClear = true)
                {

                    //查找把，找啊找r
                    if (entity?.BaseDomain.Module_Cache != null)
                    {
                        if (useAndClear)
                        {
                            return entity.BaseDomain.Module_Cache.CacheVector3.DequeueAll(Key).ToList();
                        }
                        else
                        {
                            return entity.BaseDomain.Module_Cache.CacheVector3.PeekAll(Key);
                        }

                    }
                    return null;//返回缓冲池
                }
                public static List<Entity> GetEntityVision(Entity entity, int maxCount = 5, bool reTry = false)
                {

                    //查找把，找啊找r
                    return null;
                }
            }
        }
    }
}
