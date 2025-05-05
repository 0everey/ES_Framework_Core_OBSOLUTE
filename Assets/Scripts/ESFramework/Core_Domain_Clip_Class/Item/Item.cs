using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
 public class  Item  : BaseCore
{
        
        //显性声明扩展域
        public HoldableDomainForItem HoldableDomain;
        //public 02DomainForXXX StateMachineDomain;
        
        //注册前的操作
        protected override void BeforeAwakeBroadCastRegester()
        {
            base.BeforeAwakeBroadCastRegester();
        }
}
}
   

