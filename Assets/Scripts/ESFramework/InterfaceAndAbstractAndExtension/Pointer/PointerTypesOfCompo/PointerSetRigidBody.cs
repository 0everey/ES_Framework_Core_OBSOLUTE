using ES.EvPointer;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public class PointerSetRigidBody : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        public abstract class PointerSetRigidBody_Abstract : IPointerNone
        {
            [LabelText("直接引用刚体")]public Rigidbody rigidbody;
            public object Pick(object by = null, object yarn = null, object on = null)
            {
                if(rigidbody!=null)
                PickTruely(rigidbody);
                return -1;
            }
            public abstract void PickTruely(Rigidbody rigidbody);
        }
        [Serializable, TypeRegistryItem("刚体3D_模拟状态运动学")]
        public class PointerSetRigidBody_Kinematic : PointerSetRigidBody_Abstract
        {
            [LabelText("设置为运动学的")]public bool isKine = false;
            public override void PickTruely(Rigidbody rigidbody)
            {
                rigidbody.isKinematic = isKine;
            }
        }
        [Serializable, TypeRegistryItem("刚体3D_使用重力")]
        public class PointerSetRigidBody_UseGravity : PointerSetRigidBody_Abstract
        {
            [LabelText("使用重力的")] public bool useG = false;
            public override void PickTruely(Rigidbody rigidbody)
            {
                rigidbody.useGravity = useG;
            }
        }
        [Serializable, TypeRegistryItem("刚体3D_施加力量")]
        public class PointerSetRigidBody_AddForce : PointerSetRigidBody_Abstract
        {
            [LabelText("施加力量Vector3")] public IPointerForVector3_Only ve3 = new PointerForVector3_Direct();
            public bool isReletive = false;
            public ForceMode forceMode;
            public override void PickTruely(Rigidbody rigidbody)
            {
                rigidbody.AddForce(ve3?.Pick()??default);
            }
        }
    }
}
