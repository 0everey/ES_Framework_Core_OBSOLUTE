using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 扩展时 建议创建新的脚本 
 修改文件时 使用 #region + 自己的名字
 格式尽量统一 
 多交流 --- Everey
 */
namespace ES.EvPointer
{
    //核心 Ev针支持 关于变换 Transform部分
    #region 变换获取部分
    #region 变换接口抽象和包
    public interface IPointerForTransform<By, Yarn, On> : IPointer<Transform, By, Yarn, On> { }
    public interface IPointerForTransform_Only : IPointerOnlyBack<Transform>, IPointerForTransform<object, object, object>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    public interface IPointerForTransformList : IPointerOnlyBackList<Transform>
    {

    }
    [Serializable, TypeRegistryItem("变换针包_选中几个")]
    public class PointerForTransform_PackerSelectSome : PointerPackerForSelectSomeBack<Transform, IPointerForTransform_Only>, IPointerForTransformList
    {

    }
    [Serializable, TypeRegistryItem("变换针包_选中一个")]
    public class PointerForTransform_PackerSelect : PointerPackerForOnlySelectBackOne<Transform, IPointerForTransform_Only>, IPointerForTransform_Only
    {

    }
    [Serializable, TypeRegistryItem("变换值包_选中几个")]
    public class PointerForTransformValueListSelectSome : PointerForValueListSelectSomeBack<Transform>, IPointerForTransformList
    {

    }
    [Serializable, TypeRegistryItem("变换值包_选中一个")]
    public class PointerForTransformValueListSelect : PointerForValueListSelectBackOne<Transform>, IPointerForTransform_Only
    {

    }
    #endregion
    #region 变换功能
    [Serializable, TypeRegistryItem("变换_直接引用", "单值针/变换")]
    public class PointerForTransform_Direct : IPointerForTransform_Only
    {
        [LabelText("直接引用")] public Transform trans;
        public Transform Pick(object by = null, object yarn = null, object on = null)
        {
            return trans;
        }
    }
    [Serializable, TypeRegistryItem("变换_来自脚本的")]
    public class PointerForTransform_OfCompoent : IPointerForTransform_Only
    {
        [LabelText("来自脚本")] public Component compo;
        public Transform Pick(object by = null, object yarn = null, object on = null)
        {
            return compo?.transform;
        }
    }
    [Serializable, TypeRegistryItem("变换_来自游戏物体的")]
    public class PointerForTransform_OfGameObject : IPointerForTransform_Only
    {
        [LabelText("来自游戏物体"), SerializeReference] public IPointerForGameObject_Only ga;
        public Transform Pick(object by = null, object yarn = null, object on = null)
        {
            return ga?.Pick()?.transform;
        }
    }
    [Serializable, TypeRegistryItem("变换_谁的父节点")]
    public class PointerForTransform_ParentOfTransform : IPointerForTransform_Only
    {
        [LabelText("来自谁的父级"), SerializeReference] public IPointerForTransform_Only child;
        public Transform Pick(object by = null, object yarn = null, object on = null)
        {
            return child?.Pick()?.parent;
        }
    }
    [Serializable, TypeRegistryItem("变换_谁的根节点")]
    public class PointerForTransform_RootOfTransform : IPointerForTransform_Only
    {
        [LabelText("来自谁的根"), SerializeReference] public IPointerForTransform_Only child;
        public Transform Pick(object by = null, object yarn = null, object on = null)
        {
            return child?.Pick()?.root;
        }
    }
    [Serializable, TypeRegistryItem("变换查找_根据子索引")]
    public class PointerForTransform_ChildOfTransform_Index : IPointerForTransform_Only
    {
        [LabelText("父级"), SerializeReference] public IPointerForTransform_Only parent;
        [LabelText("子Index(默认0)"), SerializeReference] public IPointerForInt_Only index;
        public Transform Pick(object by = null, object yarn = null, object on = null)
        {
            int useIndex = index?.Pick() ?? 0;
            Transform tt = parent?.Pick();
            if (tt == null) return default;
            if (tt.childCount > useIndex)
            {
                return tt.GetChild(useIndex);
            }
            return default;
        }
    }
    [Serializable, TypeRegistryItem("变换查找_根据子路径名称")]
    public class PointerForTransform_ChildOfTransform_NameWithLayer : IPointerForTransform_Only
    {
        [LabelText("父级"), SerializeReference] public IPointerForTransform_Only parent;
        [LabelText("名字(层级的)"), SerializeReference] public IPointerForString_Only str;
        public Transform Pick(object by = null, object yarn = null, object on = null)
        {
            string ss = str?.Pick() ?? null;
            Transform par = parent?.Pick() ?? null;
            if (ss == null || par == null || par.childCount == 0) return default;
            return par.Find(ss);
        }
    }
    [Serializable, TypeRegistryItem("变换查找_父的随机子变换")]
    public class PointerForTransform_ChildOfTransform_Random : IPointerForTransform_Only
    {
        [LabelText("父级"), SerializeReference] public IPointerForTransform_Only parent;

        public Transform Pick(object by = null, object yarn = null, object on = null)
        {

            Transform par = parent?.Pick() ?? null;
            if (par == null || par.childCount == 0) return default;
            int index = UnityEngine.Random.Range(0, par.childCount);
            return par.GetChild(index);
        }
    }
    [Serializable, TypeRegistryItem("变换查找_子递归名称(忽略层级)")]
    public class PointerForTransform_ChildOfTransform_ForeachByName : IPointerForTransform_Only
    {
        [LabelText("父级"), SerializeReference] public IPointerForTransform_Only parent;
        [LabelText("名字(递归的)"), SerializeReference] public IPointerForString_Only str;
        public Transform Pick(object by = null, object yarn = null, object on = null)
        {

            Transform par = parent?.Pick() ?? null;
            string ss = str?.Pick() ?? null;
            if (par == null || par.childCount == 0 || ss == null) return default;
            return KeyValueMatchingUtility.Foreach.ForeachFindTransform(par, ss);
        }
    }
    [Serializable, TypeRegistryItem("变换查找_根据标签")]
    public class PointerForTransform_WithTag : IPointerForTransform_Only
    {

        [LabelText("标签")] public PointerForString_Tag str;
        public Transform Pick(object by = null, object yarn = null, object on = null)
        {
            string ss = str?.Pick() ?? null;
            if (ss == null) return default;
            return UnityEngine.GameObject.FindGameObjectWithTag(ss)?.transform;
        }
    }
    #endregion
    #endregion

    #region 操作变换部分
    //基本都是IPointerNone 或者 IPointerOnlyAction
    [Serializable, TypeRegistryItem("变换操作_世界或者局部空间_坐标", "触发")]
    public class PointerPicker_HandleTransform_WorldAndLocal_Position : PointerOnlyAction, IInittable
    {
        [DetailedInfoBox("关于特殊情况下的数值操作解释", "对于变换来说 \n " +
            "乘的操作当做Towards用 \n " +
            "除的操作当做反向逃离用 \n " +
            "模的操作当做以初始位置为辅助值开始的循环往复运动\n" +
            "")]
        [LabelText("值变更类型"), EnumToggleButtons] public EnumCollect.TransformHandle_ValueSet setValue;
        [Space(10)]
        [LabelText("空间类型"), EnumToggleButtons] public EnumCollect.TransformHandle_ValueRele relaValue;
        [LabelText("针对变换"), SerializeReference] public IPointerForTransform_Only tr = new PointerForTransform_Direct() { };
        [LabelText("首次锁定变换不变")] public bool lockTransform = true;
        [LabelText("智能地乘上deltaTime")] public bool useDeltaTimeMutipler = true;
        [LabelText("标准乘数(建议配合delta)"), SerializeReference] public IPointerForFloat_Only standardMutipler = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("应用值"), SerializeReference] public IPointerForVector3_Only pv3 = new PointerForVector3_Direct() { };
        [Space(10)]
        [LabelText("辅助值"), SerializeReference] public IPointerForVector3_Only pv3_helper_ = new PointerForVector3_Direct() { };

        [LabelText("状态标识"), SerializeReference] public bool state0 = false;
        private Transform lock_;

        public void Init(params object[] ps)
        {

        }

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            Transform tt = lock_;
            Vector3 vector3 = pv3?.Pick() ?? default;
            Vector3 vector3_helper = pv3_helper_?.Pick() ?? default;
            float mutiDelta = (useDeltaTimeMutipler ? Time.deltaTime : 1) * standardMutipler?.Pick() ?? 1;
            //测试可用
            if (tt != null)
            {
                //通过

            }
            else
            {
                tt = tr?.Pick();
                if (lockTransform)
                {
                    lock_ = tt;
                }
                if (tt == null) return -1;
            }
            //应用
            if (relaValue == EnumCollect.TransformHandle_ValueRele.WorldSpace)
            {
                switch (setValue)
                {
                    case EnumCollect.TransformHandle_ValueSet.Set:
                        tt.position = vector3;
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Add:
                        tt.position += vector3 * mutiDelta;
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Sub:
                        tt.position -= vector3 * mutiDelta;
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Muti:

                        tt.position = Vector3.MoveTowards(tt.position, vector3, mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Div:
                        tt.position = Vector3.MoveTowards(tt.position, vector3 + ((tt.position == vector3) ? Vector3.up : default), -mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Model:
                        Vector3 targetNow = state0 ? vector3 : vector3_helper;
                        tt.position = Vector3.MoveTowards(tt.position, targetNow, mutiDelta);
                        if (Vector3.Distance(tt.position, targetNow) < 0.25f)
                        {
                            state0 = !state0;
                        }
                        break;
                }
            }
            else if (relaValue == EnumCollect.TransformHandle_ValueRele.LocalSpace)
            {
                switch (setValue)
                {
                    case EnumCollect.TransformHandle_ValueSet.Set:
                        tt.localPosition = vector3;
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Add:
                        tt.localPosition += vector3 * mutiDelta;
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Sub:
                        tt.localPosition -= vector3 * mutiDelta;
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Muti:

                        tt.localPosition = Vector3.MoveTowards(tt.localPosition, vector3, mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Div:
                        tt.localPosition = Vector3.MoveTowards(tt.localPosition, vector3 + ((tt.localPosition == vector3) ? Vector3.up : default), -mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Model:
                        Vector3 targetNow = state0 ? vector3 : vector3_helper;
                        tt.localPosition = Vector3.MoveTowards(tt.localPosition, targetNow, mutiDelta);
                        if (Vector3.Distance(tt.localPosition, targetNow) < 0.25f)
                        {
                            state0 = !state0;
                        }
                        break;
                }
            }


            return base.Pick(by, yarn, on);

        }
    }

    [Serializable, TypeRegistryItem("变换操作_模拟空间_坐标", "触发")]
    public class PointerPicker_HandleTransform_Simulation_Position : PointerOnlyAction, IInittable
    {
        [DetailedInfoBox("关于特殊情况下的数值操作解释", "对于变换来说 \n " +
            "乘的操作当做Towards用 \n " +
            "除的操作当做反向逃离用 \n " +
            "模的操作当做以初始位置为辅助值开始的循环往复运动\n" +
            "")]
        [LabelText("值变更类型"), EnumToggleButtons] public EnumCollect.TransformHandle_ValueSet setValue;
        [Space(10)]

        [LabelText("针对变换"), SerializeReference] public IPointerForTransform_Only tr = new PointerForTransform_Direct() { };
        [LabelText("首次锁定变换不变")] public bool lockTransform = true;
        [LabelText("智能地乘上deltaTime")] public bool useDeltaTimeMutipler = true;
        [LabelText("标准乘数(建议配合delta)"), SerializeReference] public IPointerForFloat_Only standardMutipler = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("应用值"), SerializeReference] public IPointerForVector3_Only pv3 = new PointerForVector3_Direct() { };
        [Space(10)]
        [LabelText("辅助值"), SerializeReference] public IPointerForVector3_Only pv3_helper_ = new PointerForVector3_Direct() { };
        [Space(10)]

        [LabelText("模拟坐标值"), SerializeReference] public IPointerForVector3_Only pv3_simulation = new PointerForVector3_Transform_Init() { };
        [LabelText("模拟方向值"), SerializeReference] public IPointerForQuaternion_Only q_simulation = new PointerForQuaternionOfTransformRotation_Init() { };

        [LabelText("状态标识"), SerializeReference] public bool state0 = false;
        private Transform lock_trans;

        public void Init(params object[] ps)
        {

        }

        public override object Pick(object by = null, object yarn = null, object on = null)
        {

            Transform tt = lock_trans;

            Vector3 vector3 = pv3?.Pick() ?? default;
            Vector3 vector3_helper = pv3_helper_?.Pick() ?? default;
            float mutiDelta = (useDeltaTimeMutipler ? Time.deltaTime : 1) * standardMutipler?.Pick() ?? 1;
            //测试可用
            if (tt != null)
            {
                //通过

            }
            else
            {
                tt = tr?.Pick();
                if (lockTransform)
                {
                    lock_trans = tt;
                }
                if (tt == null) return -1;
            }
            //应用
            Vector3 startPos = pv3_simulation?.Pick() ?? default;
            Quaternion qF = q_simulation?.Pick() ?? Quaternion.identity;
            switch (setValue)
            {
                case EnumCollect.TransformHandle_ValueSet.Set:
                    Vector3 target0 = startPos + (qF * Vector3.forward) * vector3.z + (qF * Vector3.right) * vector3.x + (qF * Vector3.up) * vector3.y;
                    tt.position = target0;
                    break;
                case EnumCollect.TransformHandle_ValueSet.Add:
                    Vector3 shoudAdd = (qF * Vector3.forward) * vector3.z + (qF * Vector3.right) * vector3.x + (qF * Vector3.up) * vector3.y;

                    tt.position += shoudAdd * mutiDelta;
                    break;
                case EnumCollect.TransformHandle_ValueSet.Sub:
                    Vector3 shoudAdd2 = (qF * Vector3.forward) * vector3.z + (qF * Vector3.right) * vector3.x + (qF * Vector3.up) * vector3.y;

                    tt.position -= shoudAdd2 * mutiDelta;
                    break;
                case EnumCollect.TransformHandle_ValueSet.Muti:
                    Vector3 target = startPos + (qF * Vector3.forward) * vector3.z + (qF * Vector3.right) * vector3.x + (qF * Vector3.up) * vector3.y;
                    tt.position = Vector3.MoveTowards(tt.position, target, mutiDelta);
                    break;
                case EnumCollect.TransformHandle_ValueSet.Div:
                    Vector3 target2 = startPos + (qF * Vector3.forward) * vector3.z + (qF * Vector3.right) * vector3.x + (qF * Vector3.up) * vector3.y;

                    tt.position = Vector3.MoveTowards(tt.position, target2 + ((tt.position == target2) ? tt.forward : default), -mutiDelta);
                    break;
                case EnumCollect.TransformHandle_ValueSet.Model:
                    Vector3 targetNow_unPack = state0 ? vector3 : vector3_helper;
                    Vector3 target3 = startPos + (qF * Vector3.forward) * targetNow_unPack.z + (qF * Vector3.right) * targetNow_unPack.x + (qF * Vector3.up) * targetNow_unPack.y;

                    tt.position = Vector3.MoveTowards(tt.position, target3, mutiDelta);
                    if (Vector3.Distance(tt.position, target3) < 0.25f)
                    {
                        state0 = !state0;
                    }
                    break;
            }
            return base.Pick(by, yarn, on);

        }
    }
    [Serializable, TypeRegistryItem("变换操作_加减自身移动且其他目标面向_坐标", "触发")]
    public class PointerPicker_HandleTransform_Self_Position : PointerOnlyAction, IInittable
    {
        [DetailedInfoBox("关于本功能特殊情况下的数值操作解释", "对于变换来说 \n " +
            "设置是移动到一个点并且相对面向用 \n " +
            "加减是单纯的相对自身变换的移动" +
            "乘除在逼近的同时，也会面向过去" +

            "模的操作当做以初始位置为辅助值开始的循环往复运动，逼近两处、\n" +
            "")]
        [LabelText("值变更类型"), EnumToggleButtons] public EnumCollect.TransformHandle_ValueSet setValue;
        [Space(10)]

        [LabelText("针对变换"), SerializeReference] public IPointerForTransform_Only tr = new PointerForTransform_Direct() { };
        [LabelText("首次锁定变换不变")] public bool lockTransform = true;
        [LabelText("智能地乘上deltaTime")] public bool useDeltaTimeMutipler = true;
        [LabelText("标准乘数(建议配合delta)"), SerializeReference] public IPointerForFloat_Only standardMutipler = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("应用值"), SerializeReference] public IPointerForVector3_Only pv3 = new PointerForVector3_Direct() { };
        [Space(10)]
        [LabelText("辅助值"), SerializeReference] public IPointerForVector3_Only pv3_helper_ = new PointerForVector3_Direct() { };
        [Space(10)]


        [LabelText("状态标识"), SerializeReference] public bool state0 = false;
        private Transform lock_trans;

        public void Init(params object[] ps)
        {

        }

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            Transform tt = lock_trans;
            Vector3 vector3 = pv3?.Pick() ?? default;
            Vector3 vector3_helper = pv3_helper_?.Pick() ?? default;
            float mutiDelta = (useDeltaTimeMutipler ? Time.deltaTime : 1) * standardMutipler?.Pick() ?? 1;
            //测试可用
            if (tt != null)
            {
                //通过

            }
            else
            {
                tt = tr?.Pick();
                if (lockTransform)
                {
                    lock_trans = tt;
                }
                if (tt == null) return -1;
            }
            //应用

            switch (setValue)
            {
                case EnumCollect.TransformHandle_ValueSet.Set:
                    if (vector3 != tt.position) tt.rotation = Quaternion.LookRotation(vector3 - tt.position);
                    tt.position = vector3;
                    break;
                case EnumCollect.TransformHandle_ValueSet.Add:
                    // if (vector3.magnitude > 0f) tt.rotation = Quaternion.Lerp(tt.rotation, Quaternion.LookRotation( tt.TransformVector(vector3), tt.TransformVector(vector3)), mutiDelta);
                    tt.Translate(vector3 * mutiDelta);
                    break;
                case EnumCollect.TransformHandle_ValueSet.Sub:

                    //if (vector3.magnitude > 0f) tt.rotation = Quaternion.Lerp(tt.rotation, Quaternion.LookRotation(-tt.TransformVector(vector3), -tt.TransformVector(vector3)), mutiDelta);
                    tt.Translate(-vector3 * mutiDelta);
                    break;
                case EnumCollect.TransformHandle_ValueSet.Muti:

                    if ((vector3 - tt.position).magnitude > 0.1f) tt.rotation = Quaternion.Lerp(tt.rotation, Quaternion.LookRotation(vector3 - tt.position), mutiDelta);
                    tt.position = Vector3.MoveTowards(tt.position, vector3, mutiDelta);
                    break;
                case EnumCollect.TransformHandle_ValueSet.Div:
                    if ((vector3 - tt.position).magnitude > 0.1f) tt.rotation = Quaternion.Lerp(tt.rotation, Quaternion.LookRotation(tt.position - vector3), mutiDelta);
                    tt.position = Vector3.MoveTowards(tt.position, vector3 + (vector3 == tt.position ? Vector3.up : default), -mutiDelta); break;
                case EnumCollect.TransformHandle_ValueSet.Model:
                    Vector3 targetNow_unPack = state0 ? vector3 : vector3_helper;

                    if ((vector3 - tt.position).magnitude > 0.1f) tt.rotation = Quaternion.Lerp(tt.rotation, Quaternion.LookRotation(targetNow_unPack - tt.position), mutiDelta);
                    tt.position = Vector3.MoveTowards(tt.position, targetNow_unPack, mutiDelta);
                    if (Vector3.Distance(tt.position, targetNow_unPack) < 0.25f)
                    {
                        state0 = !state0;
                    }
                    break;
            }
            return base.Pick(by, yarn, on);

        }
    }
    [Serializable, TypeRegistryItem("变换操作_圆柱极坐标空间_坐标", "触发")]
    public class PointerPicker_HandleTransform_CYPolar_Position : PointerOnlyAction, IInittable
    {
        [DetailedInfoBox("关于特殊情况下的数值操作解释", "对于变换来说 \n " +
            "乘的操作当做Towards用 \n " +
            "除的操作当做反向逃离用 \n " +
            "模的操作当做以初始位置为辅助值开始的循环往复运动\n" +
            "")]
        [LabelText("值变更类型"), EnumToggleButtons] public EnumCollect.TransformHandle_ValueSet setValue;
        [Space(10)]

        [LabelText("针对变换"), SerializeReference] public IPointerForTransform_Only tr = new PointerForTransform_Direct() { };
        [LabelText("首次锁定变换不变")] public bool lockTransform = true;
        [LabelText("智能地乘上deltaTime")] public bool useDeltaTimeMutipler = true;
        [LabelText("标准乘数(建议配合delta)"), SerializeReference] public IPointerForFloat_Only standardMutipler = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("应用值"), SerializeReference] public IPointerForVector3_Only pv3 = new PointerForVector3_Direct() { };
        [Space(10)]
        [LabelText("锚定值"), SerializeReference] public IPointerForVector3_Only pv3_anchor_ = new PointerForVector3_Direct() { };

        [LabelText("辅助值"), SerializeReference] public IPointerForVector3_Only pv3_helper_ = new PointerForVector3_Direct() { };
        [Space(10)]

        [LabelText("圆心坐标值"), SerializeReference] public IPointerForVector3_Only pv3_simulation = new PointerForVector3_Transform_Init() { };
        [LabelText("平面法线方向值"), SerializeReference] public IPointerForQuaternion_Only q_simulation = new PointerForQuaternionOfTransformRotation_Init() { };

        [LabelText("状态标识"), SerializeReference] public bool state0 = false;
        private Transform lock_trans;

        public void Init(params object[] ps)
        {

        }

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            Transform tt = lock_trans;
            Vector3 vector3 = pv3?.Pick() ?? default;
            Vector3 vector3_helper = pv3_helper_?.Pick() ?? default;
            Vector3 vector3_anchor = pv3_anchor_?.Pick() ?? default;
            float mutiDelta = (useDeltaTimeMutipler ? Time.deltaTime : 1) * standardMutipler?.Pick() ?? 1;
            //测试可用
            if (tt != null)
            {
                //通过

            }
            else
            {
                tt = tr?.Pick();
                if (lockTransform)
                {
                    lock_trans = tt;
                }
                if (tt == null) return -1;
            }
            //应用
            Vector3 rela = tt.position - vector3_anchor;
            Vector3 centerPos = pv3_simulation?.Pick() ?? default;
            Vector3 FD = (q_simulation?.Pick() * Vector3.forward ?? Vector3.up);
            Vector3 anchorRela = vector3_anchor - centerPos;
            Vector3 pro = Vector3.ProjectOnPlane(anchorRela, FD);
            float angle = Mathf.Atan2(pro.x, pro.z);
            Vector3 yAnchorDirec = Vector3.Project(anchorRela, FD);
            float zAnchorDis = pro.magnitude;
            float R = pro.magnitude;

            switch (setValue)
            {

                case EnumCollect.TransformHandle_ValueSet.Set:

                    float xd = vector3.x;
                    float angleEnd = angle + (xd / (R));
                    Vector3 x_Make = Mathf.Cos(angleEnd) * pro * R + Mathf.Sin(angleEnd) * Vector3.Cross(FD, pro) * R;
                    float yd = vector3.y;
                    Vector3 y_Make = yAnchorDirec + yd * FD;
                    Vector3 posOnlyX = centerPos + x_Make;
                    float zd = vector3.z + zAnchorDis;

                    tt.position = centerPos + (posOnlyX - centerPos).normalized * zd + y_Make;
                    break;
                case EnumCollect.TransformHandle_ValueSet.Add:
                    float xd2 = vector3.x * mutiDelta;
                    float angleRot = (xd2 / (R)) * Mathf.Rad2Deg;
                    tt.RotateAround(centerPos, FD, angleRot);

                    //Y
                    tt.position += FD * vector3.y * mutiDelta;
                    //Z
                    tt.position += Vector3.ProjectOnPlane(tt.position - centerPos, FD).normalized * vector3.z * mutiDelta;
                    /* Vector3 shoudAdd = (qF * Vector3.forward) * p3.z + (qF * Vector3.right) * p3.x + (qF * Vector3.up) * p3.y;

                     tt.position += shoudAdd * mutiDelta;*/
                    break;
                case EnumCollect.TransformHandle_ValueSet.Sub:
                    /* Vector3 shoudAdd2 = (qF * Vector3.forward) * p3.z + (qF * Vector3.right) * p3.x + (qF * Vector3.up) * p3.y;

                     tt.position -= shoudAdd2 * mutiDelta;*/
                    break;
                case EnumCollect.TransformHandle_ValueSet.Muti:
                    /*     Vector3 target = centerPos + (qF * Vector3.forward) * p3.z + (qF * Vector3.right) * p3.x + (qF * Vector3.up) * p3.y;
                         tt.position = Vector3.MoveTowards(tt.localPosition, target, mutiDelta);*/
                    break;
                case EnumCollect.TransformHandle_ValueSet.Div:
                    /* Vector3 target2 = centerPos + (qF * Vector3.forward) * p3.z + (qF * Vector3.right) * p3.x + (qF * Vector3.up) * p3.y;

                     tt.position = Vector3.MoveTowards(tt.localPosition, target2 + ((tt.position == target2) ? tt.forward : default), -mutiDelta);*/
                    break;
                case EnumCollect.TransformHandle_ValueSet.Model:
                    /*  Vector3 targetNow_unPack = state0 ? p3 : p3_helper;
                      Vector3 target3 = centerPos + (qF * Vector3.forward) * targetNow_unPack.z + (qF * Vector3.right) * targetNow_unPack.x + (qF * Vector3.up) * targetNow_unPack.y;

                      tt.position = Vector3.MoveTowards(tt.position, target3, mutiDelta);
                      if (Vector3.Distance(tt.position, target3) < 0.25f)
                      {
                          state0 = !state0;
                      }*/
                    break;
            }
            return base.Pick(by, yarn, on);

        }
    }

    [Serializable, TypeRegistryItem("变换操作_世界或者局部空间_旋转", "触发")]
    public class PointerPicker_HandleTransform_WorldAndLocal_Rotation : PointerOnlyAction, IInittable
    {
        [DetailedInfoBox("关于特殊情况下的数值操作解释", "对于变换来说 \n " +
            "乘的操作当做Towards用 \n " +
            "除的操作当做反向逃离用 \n " +
            "模的操作当做以初始位置为辅助值开始的循环往复运动\n" +
            "")]
        [LabelText("值变更类型"), EnumToggleButtons] public EnumCollect.TransformHandle_ValueSet setValue;
        [Space(10)]
        [LabelText("空间类型"), EnumToggleButtons] public EnumCollect.TransformHandle_ValueRele relaValue;
        [LabelText("加减是否使用右乘")] public bool useRightMuti = true;
        [LabelText("针对变换"), SerializeReference] public IPointerForTransform_Only tr = new PointerForTransform_Direct() { };
        [LabelText("首次锁定变换不变")] public bool lockTransform = true;
        [LabelText("智能地乘上deltaTime")] public bool useDeltaTimeMutipler = true;
        [LabelText("标准乘数(建议配合delta)"), SerializeReference] public IPointerForFloat_Only standardMutipler_ = new PointerForFloat_Direct() { float_ = 90 };
        [LabelText("应用值"), SerializeReference] public IPointerForQuaternion_Only pv3 = new PointerForQuaternionFromVector3() { };
        [Space(10)]
        [LabelText("辅助值"), SerializeReference] public IPointerForQuaternion_Only pv3_helper_ = new PointerForQuaternionFromVector3() { };

        [LabelText("状态标识"), SerializeReference] public bool state0 = false;
        private Transform lock_;

        public void Init(params object[] ps)
        {

        }

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            Transform tt = lock_;
            Quaternion p3 = pv3?.Pick() ?? Quaternion.identity;
            Quaternion p3_helper = pv3_helper_?.Pick() ?? Quaternion.identity;
            float mutiDelta = (useDeltaTimeMutipler ? Time.deltaTime : 1) * standardMutipler_?.Pick() ?? 1;
            //测试可用
            if (tt != null)
            {
                //通过

            }
            else
            {
                tt = tr?.Pick();
                if (lockTransform)
                {
                    lock_ = tt;
                }
                if (tt == null) return -1;
            }
            //应用
            if (relaValue == EnumCollect.TransformHandle_ValueRele.WorldSpace)
            {
                switch (setValue)
                {
                    case EnumCollect.TransformHandle_ValueSet.Set:

                        tt.rotation = p3;
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Add:
                        tt.rotation = Quaternion.RotateTowards(tt.rotation,
                        useRightMuti ? tt.rotation * p3 : p3 * tt.rotation, mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Sub:
                        tt.rotation = Quaternion.RotateTowards(tt.rotation,
                       (useRightMuti ? tt.rotation * p3 : p3 * tt.rotation)
                       * Quaternion.LookRotation(-Vector3.forward),
                       mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Muti:

                        tt.rotation = Quaternion.RotateTowards(tt.rotation,
                         p3, mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Div:
                        tt.rotation = Quaternion.RotateTowards(tt.rotation,
                         p3 * (Quaternion.LookRotation(-Vector3.forward)), mutiDelta); break;
                    case EnumCollect.TransformHandle_ValueSet.Model:
                        Quaternion targetNow = state0 ? p3 : p3_helper;
                        tt.rotation = Quaternion.RotateTowards(tt.rotation,
                          targetNow, mutiDelta);
                        if (Quaternion.Angle(tt.rotation, targetNow) < 3f)
                        {
                            state0 = !state0;
                        }
                        break;
                }
            }
            else if (relaValue == EnumCollect.TransformHandle_ValueRele.LocalSpace)
            {
                switch (setValue)
                {
                    case EnumCollect.TransformHandle_ValueSet.Set:

                        tt.localRotation = p3;
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Add:
                        tt.localRotation = Quaternion.RotateTowards(tt.localRotation,
                        useRightMuti ? tt.localRotation * p3 : p3 * tt.localRotation, mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Sub:
                        tt.localRotation = Quaternion.RotateTowards(tt.localRotation,
                       (useRightMuti ? tt.localRotation * p3 : p3 * tt.localRotation)
                       * Quaternion.LookRotation(-Vector3.forward),
                       mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Muti:

                        tt.localRotation = Quaternion.RotateTowards(tt.localRotation,
                         p3, mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Div:
                        tt.localRotation = Quaternion.RotateTowards(tt.localRotation,
                         p3 * (Quaternion.LookRotation(-Vector3.forward)), mutiDelta); break;
                    case EnumCollect.TransformHandle_ValueSet.Model:
                        Quaternion targetNow = state0 ? p3 : p3_helper;
                        tt.localRotation = Quaternion.RotateTowards(tt.localRotation,
                          targetNow, mutiDelta);
                        if (Quaternion.Angle(tt.localRotation, targetNow) < 3f)
                        {
                            state0 = !state0;
                        }
                        break;
                }
            }


            return base.Pick(by, yarn, on);

        }
    }
    [Serializable, TypeRegistryItem("变换操作_世界或者局部空间_缩放", "触发")]
    public class PointerPicker_HandleTransform_WorldAndLocal_Scale : PointerOnlyAction, IInittable
    {
        [DetailedInfoBox("关于特殊情况下的数值操作解释", "对于变换来说 \n " +
            "乘的操作当做Towards用 \n " +
            "除的操作当做反向逃离用 \n " +
            "模的操作当做以初始位置为辅助值开始的循环往复运动\n" +
            "")]
        [LabelText("值变更类型"), EnumToggleButtons] public EnumCollect.TransformHandle_ValueSet setValue;
        [Space(10)]
        [LabelText("空间类型"), EnumToggleButtons] public EnumCollect.TransformHandle_ValueRele relaValue;
        [LabelText("针对变换"), SerializeReference] public IPointerForTransform_Only tr = new PointerForTransform_Direct() { };
        [LabelText("首次锁定变换不变")] public bool lockTransform = true;
        [LabelText("智能地乘上deltaTime")] public bool useDeltaTimeMutipler = true;
        [LabelText("标准乘数(建议配合delta)"), SerializeReference] public IPointerForFloat_Only standardMutipler = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("应用值"), SerializeReference] public IPointerForVector3_Only pv3 = new PointerForVector3_Direct() { };
        [Space(10)]
        [LabelText("辅助值"), SerializeReference] public IPointerForVector3_Only pv3_helper_ = new PointerForVector3_Direct() { };

        [LabelText("状态标识"), SerializeReference] public bool state0 = false;
        private Transform lock_;

        public void Init(params object[] ps)
        {

        }

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            Transform tt = lock_;
            Vector3 vector3 = pv3?.Pick() ?? default;
            Vector3 vector3_helper = pv3_helper_?.Pick() ?? default;
            float mutiDelta = (useDeltaTimeMutipler ? Time.deltaTime : 1) * standardMutipler?.Pick() ?? 1;
            //测试可用
            if (tt != null)
            {
                //通过

            }
            else
            {
                tt = tr?.Pick();
                if (lockTransform)
                {
                    lock_ = tt;
                }
                if (tt == null) return -1;
            }
            //应用
            if (relaValue == EnumCollect.TransformHandle_ValueRele.WorldSpace)
            {
                Vector3 Div = tt.parent != null ? tt.parent.lossyScale : Vector3.one;
                Vector3 Mut = new Vector3(1 / (Div.x + (Div.x == 0 ? 1 : 0)), 1 / (Div.y + (Div.y == 0 ? 1 : 0)), 1 / (Div.z + (Div.z == 0 ? 1 : 0)));
                switch (setValue)
                {
                    case EnumCollect.TransformHandle_ValueSet.Set:
                        tt.localScale = new Vector3(vector3.x * Mut.x, vector3.y * Mut.y, vector3.z * Mut.z);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Add:
                        tt.localScale += new Vector3(vector3.x * Mut.x, vector3.y * Mut.y, vector3.z * Mut.z) * mutiDelta;
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Sub:
                        tt.localScale -= new Vector3(vector3.x * Mut.x, vector3.y * Mut.y, vector3.z * Mut.z) * mutiDelta;
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Muti:

                        tt.localScale = Vector3.MoveTowards(tt.localScale, new Vector3(vector3.x * Mut.x, vector3.y * Mut.y, vector3.z * Mut.z), mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Div:
                        tt.localScale = Vector3.MoveTowards(tt.localScale, new Vector3(vector3.x * Mut.x, vector3.y * Mut.y, vector3.z * Mut.z) + ((tt.localScale == new Vector3(vector3.x * Mut.x, vector3.y * Mut.y, vector3.y * Mut.y)) ? Vector3.one : default), -mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Model:
                        Vector3 targetNow = state0 ? new Vector3(vector3.x * Mut.x, vector3.y * Mut.y, vector3.z * Mut.z) : new Vector3(vector3_helper.x * Mut.x, vector3_helper.y * Mut.y, vector3_helper.z * Mut.z);
                        tt.localScale = Vector3.MoveTowards(tt.localScale, targetNow, mutiDelta);
                        if (Vector3.Distance(tt.localScale, targetNow) < 0.01f)
                        {
                            state0 = !state0;
                        }
                        break;
                }
            }
            else if (relaValue == EnumCollect.TransformHandle_ValueRele.LocalSpace)
            {
                switch (setValue)
                {
                    case EnumCollect.TransformHandle_ValueSet.Set:
                        tt.localScale = vector3;
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Add:
                        tt.localScale += vector3 * mutiDelta;
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Sub:
                        tt.localScale -= vector3 * mutiDelta;
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Muti:

                        tt.localScale = Vector3.MoveTowards(tt.localScale, vector3, mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Div:
                        tt.localScale = Vector3.MoveTowards(tt.localScale, vector3 + ((tt.localScale == vector3) ? Vector3.up : default), -mutiDelta);
                        break;
                    case EnumCollect.TransformHandle_ValueSet.Model:
                        Vector3 targetNow = state0 ? vector3 : vector3_helper;
                        tt.localScale = Vector3.MoveTowards(tt.localScale, targetNow, mutiDelta);
                        if (Vector3.Distance(tt.localScale, targetNow) < 0.01f)
                        {
                            state0 = !state0;
                        }
                        break;
                }
            }


            return base.Pick(by, yarn, on);

        }
    }
    #endregion

}
