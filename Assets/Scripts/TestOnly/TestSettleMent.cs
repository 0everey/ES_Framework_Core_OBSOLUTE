using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ES
{
    public class TestSettleMent : MonoBehaviour
    {
        [LabelText("结算值测试-攻击力")]
        public SettlementFloat Attack = new SettlementFloat() { BaseValue=10 };
        [LabelText("自动结算")] public bool AutoSettment = false;



        [LabelText("操作1号")] public SettleOperationFloat float1 = new SettleOperationFloat();
        [LabelText("操作2号")] public SettleOperationFloat float2 = new SettleOperationFloat();
        [LabelText("操作3号")] public SettleOperationFloat float3 = new SettleOperationFloat();
        [LabelText("操作4号")] public SettleOperationFloat float4 = new SettleOperationFloat();


        void Start()
        {
            
        }
        [Button("注入1号")] void Add1() { float1.TryStartHandle(Attack); }
        [Button("注入2号")] void Add2() { float2.TryStartHandle(Attack); }
        [Button("注入3号")] void Add3() { float3.TryStartHandle(Attack); }
        [Button("注入4号")] void Add4() { float4.TryStartHandle(Attack); }
        [Button("取消1号")] void Remove1() { float1.TryStopHandle(Attack); }
        [Button("取消2号")] void Remove2() { float2.TryStopHandle(Attack); }
        [Button("取消3号")] void Remove3() { float3.TryStopHandle(Attack); }
        [Button("取消4号")] void Remove4() { float4.TryStopHandle(Attack); }
        void Update()
        {
            if (AutoSettment)
            {
                float f= Attack.SettlementValue;
            }
        }
    }
}
