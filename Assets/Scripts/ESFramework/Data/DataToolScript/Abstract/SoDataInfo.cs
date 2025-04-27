using ES.EvPointer;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ES
{
    public interface ISoDataInfo: IWithKey<KeyString_Direct>
    {
        public new KeyString_Direct key { get; }
    }
    public abstract class SoDataInfo : SerializedScriptableObject, ISoDataInfo
    {
        public KeyString_Direct key => DataKey;
        [LabelText("数据键", SdfIconType.KeyFill)]
        public KeyString_Direct DataKey = new KeyString_Direct();
        public void SetKey(object o)
        {
            if (o is string s)
            {
                DataKey = new KeyString_Direct() { str_direc = s };
            }
        }

    }
 

    [Serializable]
    public abstract class BuffRunTimeLogic : UpdatableDelegatableWithHosting<BuffHosting>
    {
        public override BuffHosting GetHost => host;
        public BuffHosting host;
        public BuffSoInfo buffSoInfo;
        public BuffStatusTest buffStatus;
        public override void Update()
        {
            base.Update();
            buffStatus.duration -= Time.deltaTime;
            if(buffStatus.duration<0) host?.RemoveHandle(this);
        }
        public override bool OnSubmitHosting(BuffHosting hosting, bool asVirtual = false)
        {
            if (HasSubmit) return true;
            HasSubmit = true;
            return (host = hosting as BuffHosting) != null;
        }
        public override bool OnWithDrawHosting(BuffHosting hosting, bool asVirtual = false)
        {
            if (!HasSubmit) return false;
            HasSubmit = false;
            return (host =null) != null;
        }
        
    }
    [Serializable]
    public class BuffBTL_UP_CriticalHit : BuffRunTimeLogic
    {        /*

*/
       /* public override void OnEnable()
        {
            GameCenterManager.Instance.BaseDomain.Module_PlayerState.mm_CriticalhitsP += 0.5f;
            base.OnEnable();
        }
        public override void OnDisable()
        {
            GameCenterManager.Instance.BaseDomain.Module_PlayerState.mm_CriticalhitsP -= 0.5f;
            base.OnDisable();
        }*/
    }
    [Serializable]
    public class BuffBTL_UP_Vampire : BuffRunTimeLogic
    {        /*

*/
       /* public override void OnEnable()
        {
            GameCenterManager.Instance.BaseDomain.Module_PlayerState.mm_VampirePercent += 0.5f;
            base.OnEnable();
        }
        public override void OnDisable()
        {
            GameCenterManager.Instance.BaseDomain.Module_PlayerState.mm_VampirePercent -= 0.5f;
            base.OnDisable();
        }*/
    }
    [Serializable]
    public class BuffBTL_UP_AttackSpeed: BuffRunTimeLogic
    {        /*

*/
      /*  public override void OnEnable()
        {
            GameCenterManager.Instance.BaseDomain.Module_PlayerState.mm_AttackSpeedMutiLevel += 0.5f;
            base.OnEnable();
        }
        public override void OnDisable()
        {
            GameCenterManager.Instance.BaseDomain.Module_PlayerState.mm_AttackSpeedMutiLevel -= 0.5f;
            base.OnDisable();
        }*/
    }
    [Serializable]
    public class BuffBTL_UP_AttackDamage : BuffRunTimeLogic
    {        /*

*/
     /*   public override void OnEnable()
        {
            GameCenterManager.Instance.BaseDomain.Module_PlayerState.mm_AttackeDamageMutiLevel += 0.5f;
            base.OnEnable();
        }
        public override void OnDisable()
        {
            GameCenterManager.Instance.BaseDomain.Module_PlayerState.mm_AttackeDamageMutiLevel -= 0.5f;
            base.OnDisable();
        }*/
    }
}
