using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace ES
{

    public class BaseDomainForEntity : BaseDomain<Entity, BaseClipForEntity>
    {


        #region 引用
        [NonSerialized] public ClipBase_AB_3DMotion Module_AB_Motion;
        [NonSerialized] public ClipBase_3DStandardMotion Module_3DMotion;
        [NonSerialized] public ClipBase_CacherPoolForSpecialCondition Module_Cache;

        [NonSerialized] public ClipBase_AB_Vision Module_AB_Vision;
        [NonSerialized] public ClipBase_FireFlying Module_Fire;
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
                 var next = GetClip<BaseClipForEntity_改名字>();
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
    [Serializable]
    public abstract class ClipBase_AB_3DMotion : BaseClipForEntity
    {
        [LabelText("控制移动权重比率")] public float SelfControlWeight = 1;
        [FoldoutGroup("常规")][LabelText("标准速度")] public Vector2 StandardSpeed = new Vector2(1, 3);
        [FoldoutGroup("常规")][LabelText("基于刚体")] public bool BaseOnRigid = false;
        [FoldoutGroup("常规")][LabelText("当前正向乘数")] public float CurrentSpeedMutiplerZ = 0;
        [FoldoutGroup("常规")][LabelText("当前斜向乘数")] public float CurrentSpeedMutiplerX = 0;
        [FoldoutGroup("行为方式")][LabelText("使用刚体/直接")] public bool UseRigid = true;
        [FoldoutGroup("约束与限制")][LabelText("位移Y方向")] public Vector3 YUpwards = Vector3.up;
        [FoldoutGroup("约束与限制")][LabelText("最大旋转速度")] public float MaxRotSpeed_ = 360;
        [Header("旋转")]
        [FoldoutGroup("常规")][LabelText("当前Y角速度")] public float CurrentRotationY;
        [FoldoutGroup("其他效果"), LabelText("暂停移动")] public float timeForStay = 0;

        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            Domain.Module_AB_Motion = this;
            if (Core.entitySharedData != null)
            {
                StandardSpeed.y = Core.entitySharedData.PatrolSpeed;
            }
        }
    }
    [Serializable, TypeRegistryItem("3D微型移动")]
    public class ClipBase_3DMicroMotion : ClipBase_AB_3DMotion
    {
        public override void FixedUpdate()
        {
            if (timeForStay > 0) { timeForStay -= Time.fixedDeltaTime; return; }
            base.FixedUpdate();
            float Z = StandardSpeed.y * CurrentSpeedMutiplerZ;//相对Z
            float X = StandardSpeed.x * CurrentSpeedMutiplerX;//相对X

            Vector3 combine = Vector3.ProjectOnPlane(Core.transform.forward, YUpwards).normalized * Z
                + Vector3.ProjectOnPlane(Core.transform.right, YUpwards).normalized * X + Vector3.up * Core.YV;
            if (UseRigid && Core.Rigid != null)
            {
                Core.CharacterController.Move(combine * SelfControlWeight * Time.fixedDeltaTime);
            }
            else
            {
                Core.transform.position += combine * SelfControlWeight * Time.fixedDeltaTime;
            }

            Quaternion onlyY = Quaternion.Euler(0, Mathf.Clamp(CurrentRotationY, -MaxRotSpeed_, MaxRotSpeed_) * SelfControlWeight * Time.deltaTime, 0);


            if (UseRigid && Core.Rigid != null)
            {
                Core.Rigid.rotation *= onlyY;
            }
            else
            {
                Core.transform.rotation *= onlyY;
            }
        }
    }
    [Serializable, TypeRegistryItem("3D标准移动")]
    public class ClipBase_3DStandardMotion : ClipBase_3DMicroMotion
    {
        #region 参数

        [FoldoutGroup("输入源")] public HashSet<object> banSource = new HashSet<object>();
        [Header("位移")]
        [FoldoutGroup("常规")][LabelText("目标正向乘数"), SerializeField] private float TargetSpeedMutiplerZ = 0;
        [FoldoutGroup("常规")][LabelText("目标斜向乘数"), SerializeField] private float TargetSpeedMutiplerX = 0;

        [FoldoutGroup("常规")][LabelText("速度增益(按百分比)")] public Vector2 SpeedGain = new Vector2(0, 0);

        [FoldoutGroup("常规")][LabelText("目标Y角速度")] public float TargetRotationY;


        [FoldoutGroup("行为方式")][LabelText("使用RootMotion")] public bool UseRootMotion = false;




        [FoldoutGroup("约束与限制")][LabelText("位移加速度乘数等级")] public float SpeedAddLevel_ = 10;
        [FoldoutGroup("约束与限制")][LabelText("位移减速度乘数等级")] public float SpeedSubLevel_ = 10f;
        [FoldoutGroup("约束与限制")][LabelText("旋转逼近乘数等级")] public float RotSpeedLevel_ = 20f;
        [FoldoutGroup("约束与限制")][LabelText("位移Y权重")] public float YCut = 0.1f;

        #endregion

        #region 绑定
        protected override void CreateRelationship()
        {
            Domain.Module_3DMotion = this;
            base.CreateRelationship();
        }
        protected override void Update()
        {
            if (timeForStay > 0) { return; }
            base.Update();
            if (Core != null)
            {
                PrivateMethod_LerpSpeed();//Lerp速度

                PrivateMethod_();//其他
            }
        }
        public override void FixedUpdate()
        {
            if (timeForStay > 0) { timeForStay -= Time.fixedDeltaTime; return; }
            PrivateMethod_MotionPosition();//操作移动旋转
            PrivateMethod_MotionRotation();
        }
        private void PrivateMethod_LerpSpeed()
        {
            if (Mathf.Abs(CurrentSpeedMutiplerZ - TargetSpeedMutiplerZ) < 0.01f) CurrentSpeedMutiplerZ = TargetSpeedMutiplerZ;
            else CurrentSpeedMutiplerZ = Mathf.Lerp(CurrentSpeedMutiplerZ, TargetSpeedMutiplerZ, Time.deltaTime *
                (TargetSpeedMutiplerZ > CurrentSpeedMutiplerZ ? SpeedAddLevel_ : SpeedSubLevel_));

            if (Mathf.Abs(CurrentSpeedMutiplerX - TargetSpeedMutiplerX) < 0.01f) CurrentSpeedMutiplerX = TargetSpeedMutiplerX;
            else CurrentSpeedMutiplerX = Mathf.Lerp(CurrentSpeedMutiplerX, TargetSpeedMutiplerX, Time.deltaTime *
                (TargetSpeedMutiplerX > CurrentSpeedMutiplerZ ? SpeedAddLevel_ : SpeedSubLevel_));

            if (Mathf.Abs(CurrentRotationY - TargetRotationY) < 0.01f) CurrentRotationY = TargetRotationY;
            else CurrentRotationY = Mathf.Lerp(CurrentRotationY, TargetRotationY, RotSpeedLevel_ * Time.deltaTime);

        }
        private void PrivateMethod_MotionPosition()
        {
            float Z = StandardSpeed.y * (1 + SpeedGain.y) * CurrentSpeedMutiplerZ;//相对Z
            float X = StandardSpeed.x * (1 + SpeedGain.x) * CurrentSpeedMutiplerX;//相对X

            Vector3 combine = Vector3.ProjectOnPlane(Core.transform.forward, YUpwards).normalized * Z
                + Vector3.ProjectOnPlane(Core.transform.right, YUpwards).normalized * X + Vector3.up * Core.YV;
            if (UseRigid && Core.Rigid != null)
            {
                Core.CharacterController.Move(combine * SelfControlWeight * Time.fixedDeltaTime);
            }
            else
            {
                Core.transform.position += combine * SelfControlWeight * Time.fixedDeltaTime;
            }
        }
        public void Set_TargetVZ(float f, object who)
        {
            if (banSource.Count == 0 || banSource.Contains(who))
                TargetSpeedMutiplerZ = f;
        }
        public void Set_TargetVX(float f, object who)
        {
            if (banSource.Count == 0 || banSource.Contains(who))
                TargetSpeedMutiplerX = f;
        }
        private void PrivateMethod_MotionRotation()
        {
            Quaternion onlyY = Quaternion.Euler(0, Mathf.Clamp(CurrentRotationY, -MaxRotSpeed_, MaxRotSpeed_) * SelfControlWeight * Time.deltaTime, 0);


            if (UseRigid && Core.Rigid != null)
            {
                Core.Rigid.rotation *= onlyY;
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

    [Serializable, TypeRegistryItem("扩展:(3D标准移动)第一人称:InputSystem输入系统")]
    public class ClipBase_Expand3DMotion_FirstMotionControl : BaseClipForEntity
    {
        [FoldoutGroup("常规")][LabelText("移动输入")] public InputAction MoveToV2;
        [FoldoutGroup("常规")][LabelText("旋转输入")] public InputAction RotToV2;
        [FoldoutGroup("常规")][LabelText("跳跃输入")] public InputAction Jump;
        [FoldoutGroup("常规")][LabelText("旋转输出乘数")] public float RotMutipler = 10;
        [FoldoutGroup("绑定")][LabelText("第一人称相机锚定点")] public Transform FirstCameraAnchor;
        [FoldoutGroup("绑定")][LabelText("绑定原始变换")] public Transform OriginalTrans;

        [FoldoutGroup("约束限制"), LabelText("相机最大旋转角度")] public float MaxRotForCamera = 35;

        private ClipBase_3DStandardMotion Refer_3DMotion;
        [FoldoutGroup("缓存"), LabelText("移动输入缓存")] public Vector2 Cache_MoveRead;
        [FoldoutGroup("缓存"), LabelText("旋转输入缓存")] public Vector2 Cache_RotRead;
        protected override void OnEnable()
        {
            base.OnEnable();
            MoveToV2.Enable();
            RotToV2.Enable();
            Jump.Enable();
            Refer_3DMotion = Domain.Module_3DMotion;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            MoveToV2.Disable();
            RotToV2.Disable();
            Jump.Disable();
        }
        protected override void Update()
        {
            base.Update();
            if (Refer_3DMotion == null) return;
            PrivateMethod_Read();
            PrivateMethod_Control3DMotion();
            PrivateMethod_ControlCamera();
            if (Core.IsGrounded && Jump.WasPressedThisFrame())
            {
                Core.YV = 3.5f;
                Core.IsGrounded = false;
            }
        }
        private void PrivateMethod_Read()
        {
            Cache_MoveRead = MoveToV2.ReadValue<Vector2>();
            Vector2 rotRead = RotToV2.ReadValue<Vector2>();
            if (rotRead.magnitude > 100) { }
            else Cache_RotRead = rotRead;
        }
        private void PrivateMethod_Control3DMotion()
        {
            if (Cache_MoveRead.magnitude > 0.01f && Core.StateMachineDomain.StateMachine.TryActiveStateByKey("移动"))
            {
                //进入移动
                Refer_3DMotion.Set_TargetVZ(Cache_MoveRead.y, null);
                Refer_3DMotion.Set_TargetVX(Cache_MoveRead.x, null);
            }
            else
            {
                Refer_3DMotion.Set_TargetVZ(Cache_MoveRead.y, null);
                Refer_3DMotion.Set_TargetVX(Cache_MoveRead.x, null);
            }

            //需要输入才行

            Refer_3DMotion.TargetRotationY = Cache_RotRead.x * RotMutipler;

        }
        private void PrivateMethod_ControlCamera()
        {
            if (OriginalTrans == null) OriginalTrans = Core.transform;
            if (FirstCameraAnchor != null)
            {
                Quaternion Target = FirstCameraAnchor.transform.rotation * Quaternion.Euler(-Mathf.Clamp(Cache_RotRead.y, -10, 10) * RotMutipler * Time.deltaTime, 0, 0);
                if (Quaternion.Angle(Target, OriginalTrans.rotation) > MaxRotForCamera)
                {
                    FirstCameraAnchor.transform.rotation = Quaternion.RotateTowards(OriginalTrans.rotation, Target, MaxRotForCamera);
                }
                else
                {
                    FirstCameraAnchor.transform.rotation = Target;
                }
            }
        }
    }
    [Serializable, TypeRegistryItem("扩展:(3D标准移动)第三人称:InputSystem输入系统")]
    public class ClipBase_Expand3DMotion_SecondMotionControl : BaseClipForEntity
    {

        [FoldoutGroup("常规")][LabelText("移动输入")] public InputAction MoveToV2;
        [FoldoutGroup("常规")][LabelText("旋转输入")] public InputAction RotToV2;
        [FoldoutGroup("常规")][LabelText("第一人称相机")] public Camera FisrtCamera;

        [FoldoutGroup("输出测试"), LabelText("开始输出")] public bool UseDebug = false;
        [FoldoutGroup("输出测试"), LabelText("Move输出"), ShowInInspector] public Vector2 _ReadMove => UseDebug ? MoveToV2?.ReadValue<Vector2>() ?? default : default;
        [FoldoutGroup("输出测试"), LabelText("Move输出"), ShowInInspector] public Vector2 _ReadRot => UseDebug ? RotToV2?.ReadValue<Vector2>() ?? default : default;

    }


    [Serializable, TypeRegistryItem("缓冲池-特殊效果的需求")]
    public class ClipBase_CacherPoolForSpecialCondition : BaseClipForEntity
    {
        [LabelText("实体缓冲区")]
        public ESValueContainer_DicOverQueue_StringKeyEntity CacheEntity = new ESValueContainer_DicOverQueue_StringKeyEntity();
        [LabelText("坐标缓冲区")]
        public ESValueContainer_DicOverQueue_StringKeyVector3 CacheVector3 = new ESValueContainer_DicOverQueue_StringKeyVector3();
        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            Domain.Module_Cache = this;
        }
    }


    [Serializable]
    public abstract class ClipBase_AB_Vision : BaseClipForEntity
    {
        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            Domain.Module_AB_Vision = this;
        }
        public virtual void TrySee()
        {

        }
        public virtual bool MakeSeeAsTarget()
        {
            return false;
        }
    }

    [Serializable, TypeRegistryItem("视觉支持(MC版)")]
    public class ClipBase_Vision_MC : ClipBase_AB_Vision
    {
        [LabelText("视觉筛选标签")]
        public PointerForStringList_Tag TargetSeeTags = new PointerForStringList_Tag();



        //[Line(VirtualColor.Gray, "These settings are overwritten automatically"," when the script is loaded and they are automatically"," downloaded until a solution is found in the editor.")]



        // Field on View
        public List<ESObject> SeeESObjectList = new List<ESObject>();
        Collider[] CollidersList = new Collider[50];
        int objectCount;
        float patrolHeight = 2.5f;




        /*Mesh CreateWedgeMesh(float angle, float range)
        {
            Mesh mesh = new Mesh();

            int segments = 16;
            int numTriangles = (segments * 4) + 2 + 2;
            int numVertices = numTriangles * 3;

            Vector3[] verticles = new Vector3[numVertices];
            int[] triangles = new int[numVertices];

            Vector3 bottomCenter = -Domain.transform.localPosition;
            Vector3 bottomLeft = -Domain.transform.localPosition + Quaternion.Euler(0, -angle, 0) * Vector3.forward * range;
            Vector3 bottomRight = -Domain.transform.localPosition + Quaternion.Euler(0, angle, 0) * Vector3.forward * range;

            Vector3 topCenter = bottomCenter + Vector3.up * patrolHeight;
            Vector3 topLeft = bottomLeft + Vector3.up * patrolHeight;
            Vector3 topRight = bottomRight + Vector3.up * patrolHeight;

            int vert = 0;

            // left side
            verticles[vert++] = bottomCenter;
            verticles[vert++] = bottomLeft;
            verticles[vert++] = topLeft;

            verticles[vert++] = topLeft;
            verticles[vert++] = topCenter;
            verticles[vert++] = bottomCenter;

            // right side
            verticles[vert++] = bottomCenter;
            verticles[vert++] = topCenter;
            verticles[vert++] = topRight;

            verticles[vert++] = topRight;
            verticles[vert++] = bottomRight;
            verticles[vert++] = bottomCenter;

            float currentAngle = -angle;
            float deltaAngle = (angle * 2) / segments;

            for (int i = 0; i < segments; i++)
            {
                bottomLeft = -Domain.transform.localPosition + Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * range;
                bottomRight = -Domain.transform.localPosition + Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * range;

                topLeft = bottomLeft + Vector3.up * patrolHeight;
                topRight = bottomRight + Vector3.up * patrolHeight;

                // far side
                verticles[vert++] = bottomLeft;
                verticles[vert++] = bottomRight;
                verticles[vert++] = topRight;

                verticles[vert++] = topRight;
                verticles[vert++] = topLeft;
                verticles[vert++] = bottomLeft;

                // top
                verticles[vert++] = topCenter;
                verticles[vert++] = topLeft;
                verticles[vert++] = topRight;

                // bottom
                verticles[vert++] = bottomCenter;
                verticles[vert++] = bottomRight;
                verticles[vert++] = bottomLeft;

                currentAngle += deltaAngle;
            }

            for (int i = 0; i < numVertices; i++)
            {
                triangles[i] = i;
            }

            mesh.vertices = verticles;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }*/




        protected bool IsInSight(ESObject obj, float angle)
        {
            Vector3 origin = Domain.transform.position;
            Vector3 dest = obj.transform.position;
            Vector3 direction = dest - origin;
            /* if (direction.y < 0 || direction.y > patrolHeight)
             {
                 return false;
             }*/

            direction.y = 0;
            float deltaAngle = Vector3.Angle(direction, Domain.transform.forward);
            if (deltaAngle > angle)
            {
                return false;
            }

            //origin.y += 
            dest.y = origin.y;
            if (Physics.Linecast(origin, dest, EditorMaster.LayerMaskWall + EditorMaster.LayerMaskGround))
            {
                return false;
            }

            return true;
        }

        public override void TrySee()
        {
            // Object in Range
            objectCount = Physics.OverlapSphereNonAlloc(Domain.transform.position + Core.transform.forward * Core.entitySharedData.VisionRangeDis / 2, Core.entitySharedData.VisionRangeDis, CollidersList, EditorMaster.LayerMaskEntity, QueryTriggerInteraction.Collide);
            SeeESObjectList.Clear();

            for (int i = 0; i < objectCount; i++)
            {
                ESObject esO = CollidersList[i].GetComponent<ESObject>();
                if (esO == null) continue;
                if (IsInSight(esO, Core.entitySharedData.VisionRangeAngle / 2))
                {
                    SeeESObjectList.Add(esO);
                }
            }
        }

        public override bool MakeSeeAsTarget()
        {

            for (int i = 0; i < SeeESObjectList.Count; i++)
            {
                var ee = SeeESObjectList[i].transform.GetComponent<Entity>();
                if (ee != null && (TargetSeeTags.tagNames.Count == 0 && ee.tag == "Player" || TargetSeeTags.tagNames.Contains(ee.gameObject.tag)))
                {
                    Core.AIDomain.Module_AB_AITarget.Target = ee;
                    return true;
                }
            }
            return false;
        }
    }
    [Serializable, TypeRegistryItem("可发射支持")]
    public class ClipBase_FireFlying : BaseClipForEntity
    {
        [LabelText("预制件")] public GameObject prefab;
        [LabelText("发射点和方向")] public Transform FirePoint;
        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            Domain.Module_Fire = this;
        }
        public void Fire(GameObject newOne = null, bool rePlace = true)
        {
            Debug.Log("fire1");
            FirePoint ??= Core.transform;
            GameObject gg = prefab;
            if (newOne != null)
            {
                Debug.Log("fire2");
                gg = newOne;
                if (rePlace)
                {
                    prefab = newOne;
                }
            }
            if (gg != null)
            {
                Debug.Log("fire3");
                GameObject ins = GameCenter.Ins(gg, FirePoint.position, null, FirePoint.rotation);
                var item = ins.GetComponent<Item>();
                var fly = item.HurtableDomain?.Module_Flying;
                if (fly != null)
                {
                    fly.source = Core;
                }

                item.AddIgnoreEntity(Core);

            }
        }
    }

    [Serializable, TypeRegistryItem("可发射扩展：按键发射")]
    public class ClipBase_Expand_ButtonFireQuick : BaseClipForEntity
    {
        [LabelText("发射输入")]
        public InputAction FireInput;
        [LabelText("发射冷却")]
        public float fireCoolDown = 0.2f;
        private float coolTimeHas = 0.1f;
        [LabelText("按住触发时间")]
        public float holdTime = 1.5f;
        [ShowInInspector, ReadOnly, LabelText("按住时间")] private float holdTimeHasGo = 0;
        [LabelText("发射触发时机(可覆盖)")]
        public EnumCollect.MouseTriggerOption triggerOption = EnumCollect.MouseTriggerOption.Down;

        private ClipBase_FireFlying Refer_ModuleFire;
        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            Refer_ModuleFire = Domain.Module_Fire;
            if (FireInput == null || Refer_ModuleFire == null)
            {
                //无效
                this.enabledSelf = false;
                Domain.RemoveClip(this);
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            FireInput.Enable();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            FireInput.Disable();
        }
        protected override void Update()
        {
            base.Update();
            coolTimeHas -= Time.deltaTime;
            if (coolTimeHas > 0) return;
            if (triggerOption.HasFlag(EnumCollect.MouseTriggerOption.Down) && FireInput.WasPressedThisFrame())
            {
                Debug.Log("按下");
                Refer_ModuleFire.Fire();
                coolTimeHas = fireCoolDown;
            }
            else if (triggerOption.HasFlag(EnumCollect.MouseTriggerOption.Up) && FireInput.WasReleasedThisFrame())
            {
                if (triggerOption.HasFlag(EnumCollect.MouseTriggerOption.HoldForTimeAndUp))
                {
                    if (holdTimeHasGo > holdTime)
                    {
                        holdTimeHasGo = 0;
                        Refer_ModuleFire.Fire();
                        coolTimeHas = fireCoolDown;
                    }
                }
                else
                {
                    Refer_ModuleFire.Fire();
                    coolTimeHas = fireCoolDown;
                }

            }
            else if (triggerOption.HasFlag(EnumCollect.MouseTriggerOption.Hold) && FireInput.IsPressed())
            {
                holdTimeHasGo += Time.deltaTime;
                if (triggerOption.HasFlag(EnumCollect.MouseTriggerOption.HoldForTime))
                {
                    if (holdTimeHasGo > holdTime)
                    {
                        holdTimeHasGo = 0;
                        Refer_ModuleFire.Fire();
                        coolTimeHas = fireCoolDown;
                    }
                }
                else if (!triggerOption.HasFlag(EnumCollect.MouseTriggerOption.HoldForTimeAndUp))
                {
                    Refer_ModuleFire.Fire();
                    coolTimeHas = fireCoolDown;
                }


            }
            else
            {
                holdTimeHasGo = 0;
            }

        }

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
