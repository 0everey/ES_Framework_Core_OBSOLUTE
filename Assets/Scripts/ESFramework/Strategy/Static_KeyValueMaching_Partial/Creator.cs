using ES;
using ES.EvPointer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ES
{

    public static partial class KeyValueMatchingUtility
    {
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
            public static BuffRunTimeLogic CreateBuffRunTimeByKey_Rubbish(KeyString use, BuffStatusTest? statusTest = null)
            {

                /*  BuffSoInfo info = KeyValueMatchingUtility.DataInfoPointer.PickBuffSoInfoByKey(use.Key());
                  if (info == null || info.BindingLogic == null)
                  {
                      return null;
                  }*/
                return null;
            }
            //创建实时BUff逻辑
            public static BuffRunTimeLogic CreateBuffRunTimeByInfo_Rubbish(BuffSoInfo buffSoInfo, BuffStatusTest? statusTest = null)
            {
                /*BuffRunTimeLogic buffRunTime = Activator.CreateInstance(buffSoInfo.BindingLogic) as BuffRunTimeLogic;

                if (buffRunTime != null)
                {

                    buffRunTime.buffSoInfo = buffSoInfo;
                    buffRunTime.buffStatus = statusTest ?? buffSoInfo.defaultStatus;

                    return buffRunTime;
                }*/
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
    }
}

