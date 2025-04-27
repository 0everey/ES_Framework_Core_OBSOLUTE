using ES;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ES
{
    public class StateMachineDomainForEntity : DomainBase<Entity, DomainClipForEntity, StateMachineDomainForEntity>
    {    
        public override string Description_ => "状态机的可扩展域";
        protected override void CreateLink()
        {
            base.CreateLink();
            usingCore.StateMachineDomain = this;
        }
    }
    public abstract class StateMachineDomainClipForDomainForEntity : DomainClipForEntity
    {
        void Test()
        {
            var i = this as DomainClipForEntity;
            
        }
    }
}
