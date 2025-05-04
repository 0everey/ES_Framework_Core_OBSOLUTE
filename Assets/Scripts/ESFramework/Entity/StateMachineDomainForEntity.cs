using ES;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ES
{
    public class StateMachineDomainForEntity : BaseDomain<Entity, StateMachineClipForDomainForEntity>
    {    
        protected override void CreatRelationship()
        {
            base.CreatRelationship();
            core.StateMachineDomain = this;
        }
    }
    public abstract class StateMachineClipForDomainForEntity : Clip<Entity, StateMachineDomainForEntity>
    {
        
    }
}
