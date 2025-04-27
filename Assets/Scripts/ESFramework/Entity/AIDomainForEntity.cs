using ES;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public class AIDomainForEntity : DomainBase<Entity, AIDomainClipForDomainForEntity, AIDomainForEntity>
    {
        public override string Description_ => "实体的AI行为域";
        protected override void CreateLink()
        {
            base.CreateLink();
            usingCore.AIDomain = this;
        }
    }
    public abstract class AIDomainClipForDomainForEntity : DomainClipForEntity
    {

    }
 
}
