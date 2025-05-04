using ES;
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
   
    public abstract class BaseClipForEntity : Clip<Entity, BaseDomainForEntity>
    {

    }
}
