using ES;
using System;
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
    [Serializable]
    public abstract class StateMachineClipForDomainForEntity : Clip<Entity, StateMachineDomainForEntity>
    {
        
    }
}
