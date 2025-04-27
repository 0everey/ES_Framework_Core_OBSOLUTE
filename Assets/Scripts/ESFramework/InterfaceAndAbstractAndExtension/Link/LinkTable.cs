using ES.EvPointer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [Serializable]
    public class Link_AttackHappen: LinkDirect<int, int, int>
    {

    }
    [Serializable]
    public struct Link_SelfDefine : ILink
    {
        public string name_;
    }
    [Serializable]
    public struct Link_DestrolyCollideWall : ILink
    {

    }
    public class LinkTable : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
