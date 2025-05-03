using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    
    public class BuffDomainForEntity : DomainBase<Entity, BuffDomainClipForDomainForEntity, BuffDomainForEntity>, IESHosting
    {
        #region 默认的
        [SerializeField,LabelText("Buff支配者")]
        public BuffHosting buffHosting=new BuffHosting();
        //public override IEnumerable<BaseESModule> NormalBeHosted => buffRTLs.valuesNow_;
        protected override void CreateLink()
        {
            base.CreateLink();
            core.BuffDomain = this;
            buffHosting.TrySubmitHosting(this);
        }
        public override void UpdateAsHosting()
        {
            base.UpdateAsHosting();
            buffHosting?.TryUpdate();
        }
        public override void EnableAsHosting()
        {
            base.EnableAsHosting();
            buffHosting?.TryEnableSelf();
        }
        public override void DisableAsHosting()
        {
            base.DisableAsHosting();
            buffHosting?.TryDisableSelf();
        }
        /*        void IESHosting.UpdateAsHosting()
                {

                }

                void IESHosting.AddHandle(BaseESModule i)
                {

                }

                void IESHosting.RemoveHandle(BaseESModule i)
                {
                    throw new System.NotImplementedException();
                }

                bool BaseESModule.TrySubmitHosting(IESHosting hosting)
                {
                    throw new System.NotImplementedException();
                }

                void BaseESModule.Update()
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
public class BuffHosting : BaseESHostingAndModule<BuffRunTimeLogic, BuffDomainForEntity>
{
    
    #region 扩展
    [SerializeField, LabelText("安全Buff列表", SdfIconType.BatteryCharging)]
    public SafeUpdateList<BuffRunTimeLogic> buffRTLs = new SafeUpdateList<BuffRunTimeLogic>();
    #endregion
    public BuffDomainForEntity buffDomain;


    //不要默认刷新
    protected override bool OnSubmitHostingAsNormal(BuffDomainForEntity hosting)
    {
        if (hosting != null)
        {
            buffDomain = hosting;
            return true;
        }
        return false;
    }
    public  void AddHandle(IESModule i, object withKey = null)
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
            if ((logic as IESModule).TrySubmitHosting(this,false))
            {
                buffRTLs.valuesToAdd.Add(logic);
                logic.TryEnableSelf();
                Debug.Log("成功接受了logic");
                GameCenterManager.Instance.GameCenterArchitecture.SendLink(
                    new Link_BuffHandleChangeHappen() {who=buffDomain.core, info=logic.buffSoInfo,add=true });
            }
        }
    }
    public  void RemoveHandle(IESModule i,object withKey=null)
    {
       // base.RemoveHandle(i);
        if (i is BuffRunTimeLogic logic)
        {
            Debug.Log("成功放弃了logic");
            if (logic.GetHost != null)
            {
                logic.TryDisableSelf();
                 }

            buffRTLs.valuesToRemove.Add(logic);
            GameCenterManager.Instance.GameCenterArchitecture.SendLink(
                    new Link_BuffHandleChangeHappen() { who = buffDomain.core, info = logic.buffSoInfo, add = false });

        }
    }
}
