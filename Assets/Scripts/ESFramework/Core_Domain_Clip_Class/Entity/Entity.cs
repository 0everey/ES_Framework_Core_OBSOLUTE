using ES;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public class Entity : BaseCore
    {
        
        public BaseDomainForEntity BaseDomain;
        public StateMachineDomainForEntity StateMachineDomain;
        public AIDomainForEntity AIDomain;
        public BuffDomainForEntity BuffDomain;

        protected override void BeforeAwakeBroadCastRegester()
        {
            base.BeforeAwakeBroadCastRegester();
        }
    }
}
