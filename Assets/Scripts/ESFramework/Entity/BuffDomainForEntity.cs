using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    
    public class BuffDomainForEntity : DomainBase<Entity, BuffDomainClipForDomainForEntity, BuffDomainForEntity>, IDelegatableAndUpdatableAndHosting
    {
        #region 默认的
        public override string Description_ => "实体携带的Buff处理域s";
        [SerializeField,LabelText("Buff支配者")]
        public BuffHosting buffHosting=new BuffHosting();
        //public override IEnumerable<IModule> normalBeHosted => buffRTLs.valuesNow_;
        protected override void CreateLink()
        {
            base.CreateLink();
            usingCore.BuffDomain = this;
            buffHosting.TrySubmitHosting(this);
        }
        public override void UpdateAsHosting()
        {
            base.UpdateAsHosting();
            buffHosting?.Update();
        }
        public override void EnableAsHosting()
        {
            base.EnableAsHosting();
            buffHosting?.TryEnable();
        }
        public override void DisableAsHosting()
        {
            base.DisableAsHosting();
            buffHosting?.OnDisable();
        }
        /*        void IUpdatableAndHosting.UpdateAsHosting()
                {

                }

                void IUpdatableAndHosting.AddHandle(IUpdatable i)
                {

                }

                void IUpdatableAndHosting.RemoveHandle(IUpdatable i)
                {
                    throw new System.NotImplementedException();
                }

                bool IUpdatable.TrySubmitHosting(IUpdatableAndHosting hosting)
                {
                    throw new System.NotImplementedException();
                }

                void IUpdatable.Update()
                {
                    throw new System.NotImplementedException();
                }*/
        #endregion

    }
    public abstract class BuffDomainClipForDomainForEntity : DomainClipForEntity
    {

    }
  
}
[Serializable]
public class BuffHosting : DelegatableAndUpdatableAndHosting,IWithHosting<BuffDomainForEntity>
{
   
    #region 扩展
    [SerializeField, LabelText("安全Buff列表", SdfIconType.BatteryCharging)]
    public ListSafeUpdate<BuffRunTimeLogic> buffRTLs = new ListSafeUpdate<BuffRunTimeLogic>();
    #endregion
    public BuffDomainForEntity buffDomain;
    public BuffDomainForEntity GetHost => buffDomain;
    public override IEnumerable<IModule> normalBeHosted => buffRTLs.valuesNow_;

    public bool HasSubmit { get; set; }

    //不要默认刷新
    public override void UpdateAsHosting()
    {
       // buffRTLs?.Update();
        foreach (var i in normalBeHosted)
        {
            if (i is IUpdatable iu)
            {
                iu.Update();
            }
            else
            {
                buffRTLs.valuesToRemove.Add(null);
            }
        }
        if (virtualBeHosted != null)
        {
            foreach (var i in virtualBeHosted)
            {
                if (i is IUpdatable update)
                {
                    update.Update();
                }
            }
        }
    }
    public bool OnSubmitHosting(BuffDomainForEntity hosting, bool asVirtual = false)
    {
        if (hosting != null)
        {
            buffDomain = hosting;
            return true;
        }
        return false;
    }
    public override void AddHandle(IModule i, object withKey = null)
    {
       // base.AddHandle(i);
        if (i is BuffRunTimeLogic logic)
        {
            foreach (var l in buffRTLs.valuesNow_)
            {
                if (l.buffSoInfo.key.Equals(logic.buffSoInfo.key))
                {
                    l.buffStatus.duration = Mathf.Max(l.buffStatus.duration, logic.buffStatus.duration);
                    return;
                }
            }
            if (logic.TrySubmitHosting(this))
            {
                buffRTLs.valuesToAdd.Add(logic);
                logic.OnEnable();
                Debug.Log("成功接受了logic");
                GameCenterManager.Instance.GameCenterArchitecture.SendLink(
                    new Link_BuffHandleChangeHappen() {who=buffDomain.usingCore, info=logic.buffSoInfo,add=true });
            }
        }
    }
    public override void RemoveHandle(IModule i,object withKey=null)
    {
       // base.RemoveHandle(i);
        if (i is BuffRunTimeLogic logic)
        {
            Debug.Log("成功放弃了logic");
            if (logic.GetHost != null)
            {
                logic.OnDisable();
                 }

            buffRTLs.valuesToRemove.Add(logic);
            GameCenterManager.Instance.GameCenterArchitecture.SendLink(
                    new Link_BuffHandleChangeHappen() { who = buffDomain.usingCore, info = logic.buffSoInfo, add = false });

        }
    }

    public bool OnWithDrawHosting(BuffDomainForEntity hosting, bool asVirtual = false)
    {
        return true;
    }

    public bool TrySubmitHosting(BuffDomainForEntity hosting, bool asVirtual = false)
    {

        if (HasSubmit) return true;
        if (asVirtual)
        {
            if (hosting.virtualBeHosted != null)
            {
                if (!hosting.virtualBeHosted.Contains(this))
                    hosting.virtualBeHosted.Add(this);
                return true;
            }
        }
        return HasSubmit = OnSubmitHosting(hosting, asVirtual);
    }

    public bool TryWithDrawHosting(BuffDomainForEntity hosting, bool asVirtual = false)
    {
        if (!HasSubmit) return false;
        if (asVirtual)
        {
            if (hosting.virtualBeHosted != null)
            {
                if (hosting.virtualBeHosted.Contains(this))
                    hosting.virtualBeHosted.Remove(this);
                HasSubmit = false;
                return true;
            }
        }
        return HasSubmit = !OnWithDrawHosting(hosting, asVirtual);
    }
}
