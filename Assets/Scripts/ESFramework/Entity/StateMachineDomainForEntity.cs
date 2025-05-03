using ES;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ES
{
    public class StateMachineDomainForEntity : DomainBase<Entity, DomainClipForEntity, StateMachineDomainForEntity>
    {    
        protected override void CreateLink()
        {
            base.CreateLink();
            core.StateMachineDomain = this;
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
