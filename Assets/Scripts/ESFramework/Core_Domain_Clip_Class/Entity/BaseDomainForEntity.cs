using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public class BaseDomainForEntity : BaseDomain<Entity, BaseClipForEntity>
    { 
        protected override void CreatRelationship()
        {
            base.CreatRelationship();
            core.BaseDomain = this;
        }

    }
    [Serializable]
    public abstract class BaseClipForEntity : Clip<Entity, BaseDomainForEntity>
    {

    }

}
