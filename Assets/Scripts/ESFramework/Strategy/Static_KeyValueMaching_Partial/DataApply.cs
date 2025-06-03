using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public static partial class KeyValueMatchingUtility
    {
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
                    var create = Creator.CreateBuffRunTimeByInfo_Rubbish(buffSoInfo, buffStatusTest);
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
    }
}

