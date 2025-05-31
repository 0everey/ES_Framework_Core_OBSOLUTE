using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public class PointerSetParticles : MonoBehaviour
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
    public abstract class PointerSetOneParticleSystem : IPointerNone
    {
        [LabelText("引用粒子系统")]
        public ParticleSystem system;
        public object Pick(object on= null, object from = null, object with = null)
        {
            if (system != null)
            {
                PickTruly(system);
            }
            return -1;
        }
        public abstract void PickTruly(ParticleSystem system_);
    }
    [Serializable, TypeRegistryItem("粒子系统_播放")]
    public class PointerSetOneParticleSystem_Play : PointerSetOneParticleSystem
    {
        [LabelText("携带子物体一起")] public bool withChild = true;
        public override void PickTruly(ParticleSystem system_)
        {
            system_.Play(withChild);
        }
    }
    [Serializable, TypeRegistryItem("粒子系统_关闭")]
    public class PointerSetOneParticleSystem_Stop : PointerSetOneParticleSystem
    {
        [LabelText("携带子物体一起")] public bool withChild = true;
        public override void PickTruly(ParticleSystem system_)
        {
            system_.Stop(withChild);
        }
    }
    [Serializable, TypeRegistryItem("粒子系统_纯发射")]
    public class PointerSetOneParticleSystem_Emit : PointerSetOneParticleSystem
    {
        [LabelText("发射数量")] public IPointerForInt_Only emitNum = new PointerForInt_Random30() { int_range = new Vector2Int(10, 30) };
        [LabelText("默认数量")] public int emitNum_Default = 5;
        public override void PickTruly(ParticleSystem system_)
        {
            system_.Emit(emitNum?.Pick()?? emitNum_Default);
        }
    }
    [Serializable, TypeRegistryItem("粒子系统_暂停")]
    public class PointerSetOneParticleSystem_Pause: PointerSetOneParticleSystem
    {
        [LabelText("携带子物体一起")] public bool withChild = true;
        public override void PickTruly(ParticleSystem system_)
        {
            system_.Pause(withChild);
        }
    }
}
