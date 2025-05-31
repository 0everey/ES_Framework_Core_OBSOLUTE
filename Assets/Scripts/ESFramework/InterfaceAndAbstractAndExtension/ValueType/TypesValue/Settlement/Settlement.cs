using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Debug;
namespace ES
{

    public class Settlement<ValueType,Opeation_,This> where Opeation_: SettleOperation<ValueType, This, Opeation_>
    {
        [LabelText("基础值"),SerializeField] private ValueType _baseValue;
        [LabelText("常规结算值"),SerializeField]private ValueType _normalValue;
        [LabelText("最终结算值"), SerializeField] private ValueType _settlementValue;
        [Button("结算")] public void SettleMent() =>Debug.Log(SettlementValue);
        public ValueType BaseValue { get=>_baseValue;set { _baseValue = value;MakeDirty(); } }
        public ValueType SettlementValue { get { if (_isDirty) ReCalculateNormal(); ReCalculateDynamic(); _isDirty = false; return _settlementValue; } }
        [ShowInInspector,FoldoutGroup("操作集"),LabelText("全部常规操作")]protected readonly Dictionary<object, List<Opeation_>> AllNormalOperations = new Dictionary<object, List<Opeation_>>();
        [ShowInInspector, FoldoutGroup("操作集"), LabelText("（排序）全部常规操作")] protected readonly SortedList<int,List<Opeation_>> _sortedNormalOperations = new SortedList<int, List<Opeation_>>(Comparer<int>.Create((a,b)=>-b.CompareTo(a)));
        [ShowInInspector, FoldoutGroup("操作集"), LabelText("全部动态操作")] protected readonly SortedSet<Opeation_> dyncmicOperations = new SortedSet<Opeation_>();
        [FoldoutGroup("操作集"),Button("脏列表")]
        public void MakeDirty()
        {
            _isDirty = true;
        }
        protected bool _isDirty = true;
        
        public void ReCalculateNormal()
        {
            _normalValue = _baseValue;
            foreach(var i in _sortedNormalOperations.Values)
            {
                foreach(var ii in i)
                {
                    if (ii == null) continue;
                    _normalValue = ii.HandleOperation(_normalValue);
                }
            }
        }
        public void ReCalculateDynamic()
        {
            _settlementValue = _normalValue;
            foreach (var i in dyncmicOperations)
            {
                if (i == null) continue;
                _settlementValue = i.HandleOperation(_normalValue);
            }
        }
        public void AddNormalOperation(Opeation_ use)
        {
            if (use == null) return;
            if (use.Source == null) use.Source = GameCenterManager.Instance;
            if(!_sortedNormalOperations.TryGetValue(use.Priority,out var group))
            {
                _sortedNormalOperations.Add(use.Priority,group=new List<Opeation_>());
            }
            group.Add(use);

            if(!AllNormalOperations.TryGetValue(use.Source,out var sourceList))
            {
                sourceList = new List<Opeation_>();
                AllNormalOperations.Add(use.Source,sourceList);
            }
            sourceList.Add(use);

            MakeDirty();
        }
        public void RemoveNormalOperation(Opeation_ use)
        {
            if (use == null) return;
            if (_sortedNormalOperations.TryGetValue(use.Priority, out var group))
            {
                group.Remove(use);
            }
            if (AllNormalOperations.TryGetValue(use.Source, out var sourceList))
            {
                sourceList.Remove(use);
            }
            MakeDirty();
        }
        public void RemoveNormalOperationFromSource(object source)
        {
            if (!AllNormalOperations.TryGetValue(source, out var sourceList)) return;
            foreach(var i in sourceList)
            {
                var group = _sortedNormalOperations[i.Priority];
                group.Remove(i);
            }
            AllNormalOperations.Remove(source);

            MakeDirty();
        }
        [FoldoutGroup("操作集"), Button("重整")]
        public void ReSortAll()
        {
            _sortedNormalOperations.Clear();
            var enum_ = AllNormalOperations.ToDictionary((i)=>i.Key,(i)=>i.Value);
            AllNormalOperations.Clear();
            foreach (var (i,k) in enum_)
            {
                foreach(var ii in k)
                {
                    AddNormalOperation(ii);
                }
            }
        }
        public void AddDynamicOperation(Opeation_ use)
        {
            if (use == null) return;
            if (!dyncmicOperations.Contains(use))
            {
                dyncmicOperations.Add(use);
            }
            MakeDirty();
        }
        public void RemoveDynamicOperation(Opeation_ use)
        {
            if (use == null) return;
            if (dyncmicOperations.Contains(use))
            {
                dyncmicOperations.Remove(use);
            }
            MakeDirty();
        }
    }

    [TypeRegistryItem("结算操作"), Serializable]
    public abstract class SettleOperation<ValueType, Ment,This> :IComparable<This> where This: SettleOperation<ValueType, Ment, This>
    {
        [NonSerialized]public object Source;    // 效果来源
        [LabelText("操作值")]public ValueType Value;      // 效果数值
        [LabelText("优先级")]public int Priority;    // 应用优先级（数值越小越优先）
        public int CompareTo(This other)
        {
            return Priority.CompareTo(other.Priority);
        }
        public abstract ValueType HandleOperation(ValueType value);
        
    }

    #region 布尔结算
    public enum SettleTypeForBool
    {
        [InspectorName("或者")] Or,
        [InspectorName("并且")] And,
        [InspectorName("开")] ON,
        [InspectorName("关")] OFF,
        [InspectorName("非")]Not,
    }

    [TypeRegistryItem("布尔结算结果值"), Serializable]
    public class SettlementBool : Settlement<bool, SettleOperationBool, SettlementBool>
    {
        

    }
    [Serializable, TypeRegistryItem("布尔值结算操作")]
    public class SettleOperationBool : SettleOperation<bool, SettlementBool, SettleOperationBool>
    {
        [LabelText("结算操作类型")] public SettleTypeForBool settleType = SettleTypeForBool.Or;
        public override bool HandleOperation(bool value)
        {
            switch (settleType)
            {
                case SettleTypeForBool.Or:return value || Value;
                case SettleTypeForBool.And:return value && Value;
                case SettleTypeForBool.ON:return true;
                case SettleTypeForBool.OFF:return false;
                case SettleTypeForBool.Not:return !value;
                default: return value;
            }
        }
    }

    #endregion

    #region 浮点结算



    #endregion

    

    [Flags]
    public enum SettleSelfTypeForFloat
    {
        [InspectorName("无类型")] None,
        [InspectorName("动态的")] Dynamic=1,
        [InspectorName("可静默的")] MayListenOff=2,
    }
    [TypeRegistryItem("浮点结算结果值"), Serializable]
    public class SettlementFloat : Settlement<float, SettleOperationFloat, SettlementFloat>
    {
        public SettlementFloat()
        {

        }
        public SettlementFloat(float value)
        {
            BaseValue = value;
        }

    }

    [Serializable, TypeRegistryItem("浮点结算操作")]
    public class SettleOperationFloat : SettleOperation<float, SettlementFloat, SettleOperationFloat>
    {
        [LabelText("特殊类型")] public SettleSelfTypeForFloat selfType = SettleSelfTypeForFloat.None;
        [LabelText("结算操作类型")] public OperationHandleTypeForFloat settleType = OperationHandleTypeForFloat.Add;
        public override float HandleOperation(float value)
        {
            return KeyValueMatchingUtility.Function.OpearationFloat_Inline(value,Value,settleType);
        }
        
        public void TryStartHandle(SettlementFloat to,bool ForceNormal=false)
        {
            if (to != null)
            {
                if (!ForceNormal && selfType.HasFlag(SettleSelfTypeForFloat.Dynamic)) to.AddDynamicOperation(this);
                else to.AddNormalOperation(this);
            }
        }
        public void TryStopHandle(SettlementFloat to, bool ForceNormal = false)
        {
            if (to != null)
            {
                if (!ForceNormal && selfType.HasFlag(SettleSelfTypeForFloat.Dynamic)) to.RemoveDynamicOperation(this);
                else to.RemoveNormalOperation(this);
            }
        }
    }
}

