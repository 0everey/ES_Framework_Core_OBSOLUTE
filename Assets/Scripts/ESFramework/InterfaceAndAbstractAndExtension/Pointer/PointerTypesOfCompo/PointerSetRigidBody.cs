using ES.EvPointer;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    #region 刚体3D
    public abstract class PointerSetRigidbody_Abstract : IPointerNone
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
        public class PointerSetRigidbody_Kinematic : PointerSetRigidbody_Abstract
        {
            [LabelText("设置为运动学的")]public bool isKine = false;
            public override void PickTruely(Rigidbody rigidbody)
            {
                rigidbody.isKinematic = isKine;
            }
        }
        [Serializable, TypeRegistryItem("刚体3D_使用重力")]
        public class PointerSetRigidbody_UseGravity : PointerSetRigidbody_Abstract
        {
            [LabelText("使用重力的")] public bool useG = false;
            public override void PickTruely(Rigidbody rigidbody)
            {
                rigidbody.useGravity = useG;
            }
        }
        [Serializable, TypeRegistryItem("刚体3D_施加力量")]
        public class PointerSetRigidbody_AddForce : PointerSetRigidbody_Abstract
        {
            [LabelText("施加力量Vector3"), SerializeReference] public IPointerForVector3_Only ve3 = new PointerForVector3_Direct();
            
            public ForceMode forceMode;
            public override void PickTruely(Rigidbody rigidbody)
            {
                rigidbody.AddForce(ve3?.Pick()??default);
            }
        }
        [Serializable, TypeRegistryItem("刚体3D_施加相对力量")]
        public class PointerSetRigidbody_AddRelativeForce : PointerSetRigidbody_Abstract
        {
            [LabelText("施加力量Vector3"), SerializeReference] public IPointerForVector3_Only ve3 = new PointerForVector3_Direct();
            
            public ForceMode forceMode;
            public override void PickTruely(Rigidbody rigidbody)
            {
                rigidbody.AddRelativeForce(ve3?.Pick() ?? default);
            }
        }
        [Serializable, TypeRegistryItem("刚体3D_设置速度")]
        public class PointerSetRigidbody_SetVelocity : PointerSetRigidbody_Abstract
        {
            [LabelText("施加力量Vector3"),SerializeReference] public IPointerForVector3_Only ve3 = new PointerForVector3_Direct();
            
            public ForceMode forceMode;
            public override void PickTruely(Rigidbody rigidbody)
            {
                rigidbody.velocity=(ve3?.Pick() ?? default);
            }
        }
        [Serializable, TypeRegistryItem("刚体3D_施加向量速度")]
        public class PointerSetRigidbody_AddVelocity : PointerSetRigidbody_Abstract
        {
            [LabelText("施加向量速度"),SerializeReference] public IPointerForVector3_Only ve3 = new PointerForVector3_Direct();
           
            public ForceMode forceMode;
            public override void PickTruely(Rigidbody rigidbody)
            {
                rigidbody.velocity += (ve3?.Pick() ?? default);
            }
        }
        [Serializable, TypeRegistryItem("刚体3D_速度乘浮点数")]
        public class PointerSetRigidbody_VelocityMuti : PointerSetRigidbody_Abstract
        {
            [LabelText("速度乘浮点数"),SerializeReference] public IPointerForFloat_Only @float = new PointerForFloat_Direct();

            public ForceMode forceMode;
            public override void PickTruely(Rigidbody rigidbody)
            {
                rigidbody.velocity *= (@float?.Pick() ?? 1);
            }
        }
    #endregion
    #region 刚体2D
    public abstract class PointerSetRigidbody2D_Abstract : IPointerNone
    {
        [LabelText("直接引用刚体")] public Rigidbody2D rigidbody2D;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (rigidbody2D != null)
                PickTruely(rigidbody2D);
            return -1;
        }
        public abstract void PickTruely(Rigidbody2D rigidbody2D);
    }
    [Serializable, TypeRegistryItem("刚体2D_模拟状态运动学")]
    public class PointerSetRigidbody2D_Kinematic : PointerSetRigidbody2D_Abstract
    {
        [LabelText("设置为运动学的")] public bool isKine = false;
        public override void PickTruely(Rigidbody2D rigidbody2D)
        {
            rigidbody2D.isKinematic = isKine;
        }
    }
    [Serializable, TypeRegistryItem("刚体2D_重力乘数")]
    public class PointerSetRigidbody2D_UseGravity : PointerSetRigidbody2D_Abstract
    {
        [LabelText("重力乘数"),SerializeReference] public IPointerForFloat_Only useG = new PointerForFloat_Direct() { float_=1 };
        public override void PickTruely(Rigidbody2D rigidbody2D)
        {
            rigidbody2D.gravityScale = useG?.Pick()??1;
        }
    }
    [Serializable, TypeRegistryItem("刚体2D_施加力量")]
    public class PointerSetRigidbody2D_AddForce : PointerSetRigidbody2D_Abstract
    {
        [LabelText("施加力量Vector2"), SerializeReference] public IPointerForVector2_Only ve3 = new PointerForVector2_Direct();

        public ForceMode forceMode;
        public override void PickTruely(Rigidbody2D rigidbody2D)
        {
            rigidbody2D.AddForce(ve3?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("刚体2D_施加相对力量")]
    public class PointerSetRigidbody2D_AddRelativeForce : PointerSetRigidbody2D_Abstract
    {
        [LabelText("施加力量Vector2"), SerializeReference] public IPointerForVector2_Only ve3 = new PointerForVector2_Direct();

        public ForceMode forceMode;
        public override void PickTruely(Rigidbody2D rigidbody2D)
        {
            rigidbody2D.AddRelativeForce(ve3?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("刚体2D_设置速度")]
    public class PointerSetRigidbody2D_SetVelocity : PointerSetRigidbody2D_Abstract
    {
        [LabelText("施加力量Vector2"), SerializeReference] public IPointerForVector2_Only ve3 = new PointerForVector2_Direct();

        public ForceMode forceMode;
        public override void PickTruely(Rigidbody2D rigidbody2D)
        {
            rigidbody2D.velocity = (ve3?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("刚体2D_施加向量速度")]
    public class PointerSetRigidbody2D_AddVelocity : PointerSetRigidbody2D_Abstract
    {
        [LabelText("施加向量速度"), SerializeReference] public IPointerForVector2_Only ve3 = new PointerForVector2_Direct();

        public ForceMode forceMode;
        public override void PickTruely(Rigidbody2D rigidbody2D)
        {
            rigidbody2D.velocity += (ve3?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("刚体2D_速度乘浮点数")]
    public class PointerSetRigidbody2D_VelocityMuti : PointerSetRigidbody2D_Abstract
    {
        [LabelText("速度乘浮点数"), SerializeReference] public IPointerForFloat_Only @float = new PointerForFloat_Direct();

        public ForceMode forceMode;
        public override void PickTruely(Rigidbody2D rigidbody2D)
        {
            rigidbody2D.velocity *= (@float?.Pick() ?? 1);
        }
    }
    #endregion
}
