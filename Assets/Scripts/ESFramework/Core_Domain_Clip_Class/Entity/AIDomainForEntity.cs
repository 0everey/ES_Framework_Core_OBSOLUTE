using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public class AIDomainForEntity : BaseDomain<Entity, BaseAIClipForDomainForEntity>
    {
        protected override void CreatRelationship()
        {
            base.CreatRelationship();
            core.AIDomain = this;
        }
    }
    [Serializable]
    public abstract class BaseAIClipForDomainForEntity : Clip<Entity, AIDomainForEntity>
    {

    }
 
}
