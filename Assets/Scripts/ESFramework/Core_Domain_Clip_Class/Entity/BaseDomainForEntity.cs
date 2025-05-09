using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ES
{

    public class BaseDomainForEntity : BaseDomain<Entity, BaseClipForEntity>
    {


        #region 引用
        [NonSerialized] public ClipBase_3DMotion Module_3DMotion;

        #endregion

        protected override void CreatRelationship()
        {
            base.CreatRelationship();
            core.BaseDomain = this;
        }

       /* float timeDis = 2;
        protected override void Update()
        {
            timeDis -= Time.deltaTime;
            if (timeDis < 0)
            {
                var next = GetModule<BaseClipForEntity_改名字>();
                RemoveClip(next);
                timeDis = 2;
            }
            base.Update();
        }*/
    }
    [Serializable]
    public abstract class BaseClipForEntity : Clip<Entity, BaseDomainForEntity>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }
    }
    [Serializable,TypeRegistryItem("3D标准移动")]
    public class ClipBase_3DMotion : BaseClipForEntity
    {
        #region 参数
        [LabelText("控制移动权重比率")]public float SelfControlWeight = 1;
        [Header("位移")]
        [FoldoutGroup("常规")][LabelText("当前正向乘数")]public float CurrentSpeedMutiplerZ = 0;
        [FoldoutGroup("常规")][LabelText("当前斜向乘数")]public float CurrentSpeedMutiplerX = 0;
        [FoldoutGroup("常规")][LabelText("目标正向乘数")] public float TargetSpeedMutiplerZ = 0;
        [FoldoutGroup("常规")][LabelText("目标斜向乘数")] public float TargetSpeedMutiplerX = 0;
        [FoldoutGroup("常规")][LabelText("标准速度")] public Vector2 StandardSpeed = new Vector2(1, 3);
        [FoldoutGroup("常规")][LabelText("速度增益(按百分比)")] public Vector2 SpeedGain = new Vector2(0, 0);
        [Header("旋转")]
        [FoldoutGroup("常规")][LabelText("当前Y角速度")] public float CurrentRotationY;
        [FoldoutGroup("常规")][LabelText("目标Y角速度")] public float TargetRotationY;
        

        [FoldoutGroup("行为方式")][LabelText("使用RootMotion")] public bool UseRootMotion = false;
        [FoldoutGroup("行为方式")][LabelText("使用刚体/直接")] public bool UseRigid = true;

        

        [FoldoutGroup("约束与限制")][LabelText("位移加速度乘数等级")] public float SpeedAddLevel_ = 10;
        [FoldoutGroup("约束与限制")][LabelText("位移减速度乘数等级")] public float SpeedSubLevel_ = 10f;
        [FoldoutGroup("约束与限制")][LabelText("旋转逼近乘数等级")] public float RotSpeedLevel_ = 20f;
        [FoldoutGroup("约束与限制")][LabelText("位移Y权重")] public float YCut = 0.1f;
        [FoldoutGroup("约束与限制")][LabelText("位移Y方向")] public Vector3 YUpwards =Vector3.up;
        [FoldoutGroup("约束与限制")][LabelText("最大旋转速度")] public float MaxRotSpeed_ = 360;
        #endregion

        #region 绑定
        protected override void CreateRelationship()
        {
            Domain.Module_3DMotion = this;
            base.CreateRelationship();
        }
        protected override void Update()
        {
            base.Update();
            if (Core != null)
            {
                PrivateMethod_LerpSpeed();//Lerp速度
                
                PrivateMethod_();//其他
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PrivateMethod_MotionPosition();//操作移动旋转
            PrivateMethod_MotionRotation();
        }
        private void PrivateMethod_LerpSpeed()
        {
            if (Mathf.Abs(CurrentSpeedMutiplerZ - TargetSpeedMutiplerZ) < 0.01f) CurrentSpeedMutiplerZ = TargetSpeedMutiplerZ;
            else CurrentSpeedMutiplerZ = Mathf.Lerp(CurrentSpeedMutiplerZ,TargetSpeedMutiplerZ,Time.deltaTime*
                (TargetSpeedMutiplerZ>CurrentSpeedMutiplerZ?SpeedAddLevel_:SpeedSubLevel_));

            if (Mathf.Abs(CurrentSpeedMutiplerX - TargetSpeedMutiplerX) < 0.01f) CurrentSpeedMutiplerX = TargetSpeedMutiplerX;
            else CurrentSpeedMutiplerX = Mathf.Lerp(CurrentSpeedMutiplerX, TargetSpeedMutiplerX, Time.deltaTime *
                (TargetSpeedMutiplerX > CurrentSpeedMutiplerZ ? SpeedAddLevel_ : SpeedSubLevel_));

            if (Mathf.Abs(CurrentRotationY - TargetRotationY) < 0.01f) CurrentRotationY = TargetRotationY;
            else CurrentRotationY = Mathf.Lerp(CurrentRotationY, TargetRotationY, RotSpeedLevel_*Time.deltaTime);

        }
        private void PrivateMethod_MotionPosition()
        {
            float Z = StandardSpeed.y * (1 + SpeedGain.y)*CurrentSpeedMutiplerZ;//相对Z
            float X = StandardSpeed.x * (1 + SpeedGain.x)*CurrentSpeedMutiplerX;//相对X

            Vector3 combine = Vector3.ProjectOnPlane(Core.transform.forward, YUpwards).normalized * Z
                + Vector3.ProjectOnPlane(Core.transform.right, YUpwards).normalized * X;
            if (UseRigid && Core.Rigid != null)
            {
                Core.Rigid.position += combine * SelfControlWeight*Time.fixedDeltaTime;
            }
            else
            {
                Core.transform.position += combine * SelfControlWeight * Time.fixedDeltaTime;
            }
        }
        private void PrivateMethod_MotionRotation()
        {
            Quaternion onlyY = Quaternion.Euler(0,Mathf.Clamp( CurrentRotationY, -MaxRotSpeed_ , MaxRotSpeed_ )* SelfControlWeight*Time.deltaTime,0);

           
            if (UseRigid && Core.Rigid != null)
            {
                Core.Rigid.rotation*= onlyY;
            }
            else
            {
                Core.transform.rotation *= onlyY;
            }
        }
        private void PrivateMethod_()
        {

        }
        #endregion
    }

    [Serializable,TypeRegistryItem("扩展:(3D标准移动)第一人称:InputSystem输入系统")]
    public class ClipBasr_Expand3DMotion_FirstMotionControl : BaseClipForEntity
    {
        [FoldoutGroup("常规")][LabelText("移动输入")] public InputAction MoveToV2;
        [FoldoutGroup("常规")][LabelText("旋转输入")] public InputAction RotToV2;
        [FoldoutGroup("常规")][LabelText("旋转输出乘数")] public float RotMutipler = 10;
        [FoldoutGroup("绑定")][LabelText("第一人称相机")] public Camera FirstCamera;
        [FoldoutGroup("绑定")][LabelText("绑定原始变换")] public Transform OriginalTrans;

        [FoldoutGroup("约束限制"), LabelText("相机最大旋转角度")] public float MaxRotForCamera=35;

        private ClipBase_3DMotion Refer_3DMotion;
        [FoldoutGroup("缓存"),LabelText("移动输入缓存")]public Vector2 Cache_MoveRead;
        [FoldoutGroup("缓存"), LabelText("旋转输入缓存")] public Vector2 Cache_RotRead;
        protected override void OnEnable()
        {
            base.OnEnable();
            MoveToV2.Enable();
            RotToV2.Enable();
            Refer_3DMotion = Domain.Module_3DMotion;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            MoveToV2.Disable();
            RotToV2.Dispose();
        }
        protected override void Update()
        {
            base.Update();
            if (Refer_3DMotion == null) return;
            PrivateMethod_Read();
            PrivateMethod_Control3DMotion();
            PrivateMethod_ControlCamera();
        }
        private void PrivateMethod_Read()
        {
            Cache_MoveRead =MoveToV2.ReadValue<Vector2>();
            Vector2 rotRead=RotToV2.ReadValue<Vector2>();
            if (rotRead.magnitude > 100) { }
            else Cache_RotRead = rotRead;
        }
        private void PrivateMethod_Control3DMotion()
        {
            Refer_3DMotion.TargetSpeedMutiplerZ = Cache_MoveRead.y;
            Refer_3DMotion.TargetSpeedMutiplerX = Cache_MoveRead.x;
            Refer_3DMotion.TargetRotationY =Cache_RotRead.x*RotMutipler;
        }
        private void PrivateMethod_ControlCamera()
        {
            if (OriginalTrans == null) OriginalTrans = Core.transform;
            if (FirstCamera != null)
            {
                Quaternion Target = FirstCamera.transform.rotation * Quaternion.Euler(-Mathf.Clamp(Cache_RotRead.y, -10, 10)*RotMutipler * Time.deltaTime,0,0);
                if(Quaternion.Angle(Target,OriginalTrans.rotation)> MaxRotForCamera)
                {
                    FirstCamera.transform.rotation = Quaternion.RotateTowards(OriginalTrans.rotation,Target,MaxRotForCamera);
                }
                else
                {
                    FirstCamera.transform.rotation = Target;
                }
            }
        }
    }
    [Serializable, TypeRegistryItem("扩展:(3D标准移动)第三人称:InputSystem输入系统")]
    public class ClipBasr_Expand3DMotion_SecondMotionControl : BaseClipForEntity
    {

        [FoldoutGroup("常规")][LabelText("移动输入")] public InputAction MoveToV2;
        [FoldoutGroup("常规")][LabelText("旋转输入")] public InputAction RotToV2;
        [FoldoutGroup("常规")][LabelText("第一人称相机")] public Camera FisrtCamera;

        [FoldoutGroup("输出测试"), LabelText("开始输出")] public bool UseDebug = false;
        [FoldoutGroup("输出测试"), LabelText("Move输出"),ShowInInspector] public Vector2 _ReadMove => UseDebug ? MoveToV2?.ReadValue<Vector2>() ?? default : default;
        [FoldoutGroup("输出测试"), LabelText("Move输出"), ShowInInspector] public Vector2 _ReadRot => UseDebug ? RotToV2?.ReadValue<Vector2>() ?? default : default;

    }
    /*   [Serializable,TypeRegistryItem("改名字模块")]
       public class BaseClipForEntity_改名字: BaseClipForEntity
       {
           public string newName = "依薇尔";
           private string pre;
           protected override void CreateRelationship()
           {
               base.CreateRelationship();
               Domain.Module_RenameThis = this;
           }
           protected override void OnEnable()
           {
               pre = Core.gameObject.content;
               Core.gameObject.content = newName;
               base.OnEnable();
           }
           protected override void OnDisable()
           {
               Core.gameObject.content = pre;
               base.OnDisable();
           }
           protected override void Update()
           {
               base.Update();
           }
       }*/
}
