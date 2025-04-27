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
    //核心 Ev针支持 关于动画器 Anamator部分
    //基本实现IPointerNone或者IPointerOnlyAction
    #region 接口支持
    public interface IPointerForAnimatorStateInfo_Only : IPointer<AnimatorStateInfo, object, object, object>, IPointerOnlyBack<AnimatorStateInfo>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    #endregion
    #region 操作动画和动画参数获取功能

    [Serializable, TypeRegistryItem("动画器_设置参数_浮点数", "触发/动画")]
    public class PointerAnimator_SetPara_Float : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("浮点数参数名")] public string ParaName = "aFloat";
        [LabelText("浮点值"), SerializeReference] public IPointerForFloat_Only float_Only = new PointerForFloat_Direct();
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            animator.SetFloat(ParaName, float_Only?.Pick() ?? 0);
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_增益_浮点数", "触发/动画")]
    public class PointerAnimator_Additive_Float : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("浮点数参数名")] public string ParaName = "aFloat";
        [LabelText("增益浮点数值"), SerializeReference] public IPointerForFloat_Only float_Only = new PointerForFloat_Direct();
        [LabelText("乘上帧时间")] public bool mutiTimeDelta = true;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            animator.SetFloat(ParaName, animator.GetFloat(ParaName) + (float_Only?.Pick() ?? 0) * (mutiTimeDelta ? Time.deltaTime : 1));
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_逼近_浮点数", "触发/动画")]
    public class PointerAnimator_Lerp_Float : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("浮点数参数名")] public string ParaName = "aFloat";
        [LabelText("目标浮点数值"), SerializeReference] public IPointerForFloat_Only float_Only = new PointerForFloat_Direct();
        [LabelText("Lerp值"), SerializeReference] public IPointerForFloat_Only lerp_Only = new PointerForFloat_Direct();
        [LabelText("Lerp乘上帧时间")] public bool mutiTimeDelta = true;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            float t = lerp_Only?.Pick() ?? 0;
            animator.SetFloat(ParaName, Mathf.Lerp(animator.GetFloat(ParaName), +(float_Only?.Pick() ?? 0), t * (mutiTimeDelta ? Time.deltaTime : 1)));
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_设置参数_整数", "触发/动画")]
    public class PointerAnimator_SetPara_Int : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("整数参数名")] public string ParaName = "aInt";
        [LabelText("整数值"), SerializeReference] public IPointerForInt_Only int_Only = new PointerForInt_Direct();
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            animator.SetInteger(ParaName, int_Only?.Pick() ?? 0);
            return base.Pick(by, yarn, on);
        }
    }

    [Serializable, TypeRegistryItem("动画器_设置参数_布尔值", "触发/动画")]
    public class PointerAnimator_SetPara_Bool : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("布尔参数名")] public string ParaName = "aBool";
        [LabelText("布尔值"), SerializeReference] public IPointerForBool_Only bool_Only = new PointerForBool_Direc();
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            animator.SetBool(ParaName, bool_Only?.Pick() ?? false);
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_切换真假_布尔值", "触发/动画")]
    public class PointerAnimator_Change_Bool : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("布尔参数名")] public string ParaName = "aBool";

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            animator.SetBool(ParaName, !animator.GetBool(ParaName));
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_设置参数_触发器", "触发/动画")]
    public class PointerAnimator_SetPara_Trigger : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("触发器参数名")] public string ParaName = "aTrigger";
        [LabelText("是否触发"), SerializeReference] public IPointerForBool_Only tri_Only = new PointerForBool_Direc();
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            bool b = tri_Only?.Pick() ?? false;
            if (b) animator.SetTrigger(ParaName);
            else animator.ResetTrigger(ParaName);
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_设置_层级权重", "触发/动画")]
    public class PointerAnimator_SetPara_LayerWeight : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("第几层"), SerializeReference] public IPointerForInt_Only layerIndex = new PointerForInt_Direct();
        [LabelText("权重值"), SerializeReference] public IPointerForFloat_Only float_Only = new PointerForFloat_Direct();
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            int index = layerIndex?.Pick() ?? 0;
            float weight = float_Only?.Pick() ?? 0;
            animator.SetLayerWeight(index, weight);

            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_增益_层级权重", "触发/动画")]
    public class PointerAnimator_Additive_LayerWeight : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("第几层"), SerializeReference] public IPointerForInt_Only layerIndex = new PointerForInt_Direct();
        [LabelText("增益权重值"), SerializeReference] public IPointerForFloat_Only float_Only = new PointerForFloat_Direct();
        [LabelText("乘上帧时间")] public bool mutiTimeDelta = true;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            int index = layerIndex?.Pick() ?? 0;
            float weight = (float_Only?.Pick() ?? 0) * (mutiTimeDelta ? Time.deltaTime : 1);
            animator.SetLayerWeight(index, weight + animator.GetLayerWeight(index));

            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_逼近_层级权重", "触发/动画")]
    public class PointerAnimator_Lerp_LayerWeight : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("第几层"), SerializeReference] public IPointerForInt_Only layerIndex = new PointerForInt_Direct();
        [LabelText("目标权重值"), SerializeReference] public IPointerForFloat_Only float_Only = new PointerForFloat_Direct();
        [LabelText("Lerp值"), SerializeReference] public IPointerForFloat_Only lerp_Only = new PointerForFloat_Direct();
        [LabelText("Lerp乘上帧时间")] public bool mutiTimeDelta = true;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            int index = layerIndex?.Pick() ?? 0;
            float weight = (float_Only?.Pick() ?? 0);
            float t = (lerp_Only?.Pick() ?? 0) * (mutiTimeDelta ? Time.deltaTime : 1);
            animator.SetLayerWeight(index, Mathf.Lerp(animator.GetLayerWeight(index), weight, t));
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_状态_直接播放", "触发/动画")]
    public class PointerAnimator_Play : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("第几层(可空)"), SerializeReference] public IPointerForInt_Only layerIndex = new PointerForInt_Direct();
        [LabelText("目标动画名"), SerializeReference] public IPointerForString_Only str_only = new PointerForString_Direc() { string_direc = "状态名" };
        [LabelText("归一化起始播放位置(可空)"), SerializeReference] public IPointerForFloat_Only start_Only = new PointerForFloat_DirectClamp01();

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null || str_only == null) return -1;
            int index = layerIndex?.Pick() ?? 0;
            float progress = start_Only?.Pick() ?? 0;
            animator.Play(str_only.Pick(), index, progress);
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_状态_过渡播放", "触发/动画")]
    public class PointerAnimator_CrossFade : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("第几层(可空)"), SerializeReference] public IPointerForInt_Only layerIndex = new PointerForInt_Direct();
        [LabelText("目标动画名"), SerializeReference] public IPointerForString_Only str_only = new PointerForString_Direc() { string_direc = "状态名" };
        [LabelText("归一化过渡时间(默认0.15f)"), SerializeReference] public IPointerForFloat_Only cross_Only = new PointerForFloat_DirectClamp01();
        [LabelText("归一化起始播放位置(可空)"), SerializeReference] public IPointerForFloat_Only start_Only = new PointerForFloat_DirectClamp01();

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null || str_only == null) return -1;
            int index = layerIndex?.Pick() ?? 0;
            float progress = start_Only?.Pick() ?? 0;
            float cross = cross_Only?.Pick() ?? 0.15f;
            animator.CrossFade(str_only.Pick(), cross, index, progress);
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_状态_过渡播放_", "触发/动画")]
    public class PointerAnimator_CrossFade_ : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("第几层(可空)"), SerializeReference] public IPointerForInt_Only layerIndex = new PointerForInt_Direct();
        [LabelText("目标动画名"), SerializeReference] public IPointerForString_Only str_only = new PointerForString_Direc() { string_direc = "状态名" };
        [LabelText("归一化过渡时间(默认0.15f)"), SerializeReference] public IPointerForFloat_Only cross_Only = new PointerForFloat_DirectClamp01();
        [LabelText("归一化起始播放位置(可空)"), SerializeReference] public IPointerForFloat_Only start_Only = new PointerForFloat_DirectClamp01();

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null || str_only == null) return -1;
            int index = layerIndex?.Pick() ?? 0;
            float progress = start_Only?.Pick() ?? 0;
            float cross = cross_Only?.Pick() ?? 0.15f;
            animator.CrossFade(str_only.Pick(), cross, index, progress);
            /* animator.gets*/
            return base.Pick(by, yarn, on);

        }
    }
    
    [Serializable, TypeRegistryItem("动画器_状态_获得当前状态信息", "单值针/动画针")]
    public class PointerForAnimatorStateInfo_CurrentFromAnimator : IPointerForAnimatorStateInfo_Only
    {
        [LabelText("目标动画器")] public Animator animator;
        [LabelText("第几层(可空)"), SerializeReference] public IPointerForInt_Only layerIndex = new PointerForInt_Direct();

        public AnimatorStateInfo Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return default;
            int index = layerIndex?.Pick() ?? 0;
            return animator.GetCurrentAnimatorStateInfo(index);
        }
    }
    [Serializable, TypeRegistryItem("动画器_状态_获得下一个状态信息", "单值针/动画针")]
    public class PointerForAnimatorStateInfo_CurrentNextFromAnimator : IPointerForAnimatorStateInfo_Only
    {
        [LabelText("目标动画器")] public Animator animator;
        [LabelText("第几层(可空)"), SerializeReference] public IPointerForInt_Only layerIndex = new PointerForInt_Direct();

        public AnimatorStateInfo Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return default;
            int index = layerIndex?.Pick() ?? 0;
            return animator.GetNextAnimatorStateInfo(index);
        }
    }
    [Serializable, TypeRegistryItem("动画器_获取_浮点参数值")]
    public class PointerForFloat_AnimatorParam : IPointerForFloat_Only
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("浮点数参数名")] public string ParaName = "aFloat";
        public float Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            return animator.GetFloat(ParaName);
        }
    }
    [Serializable, TypeRegistryItem("动画器_获取_整数参数值")]
    public class PointerForInt_AnimatorParam : IPointerForInt_Only
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("整数参数名")] public string ParaName = "aInt";
        public int Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            return animator.GetInteger(ParaName);
        }
    }
    [Serializable, TypeRegistryItem("动画器_获取_布尔参数值")]
    public class PointerForBool_AnimatorParam : IPointerForBool_Only
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("布尔值参数名")] public string ParaName = "aBool";
        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return false;
            return animator.GetBool(ParaName);
        }
    }
    [Serializable, TypeRegistryItem("动画器_获取动画信息状态_归一化时间")]
    public class PointerForFloat_AnimatorStateInfo_NormalizedTime : IPointerForFloat_Only
    {
        [SerializeReference, LabelText("动画状态信息")] public IPointerForAnimatorStateInfo_Only stateInfo = new PointerForAnimatorStateInfo_CurrentFromAnimator();
        public float Pick(object by = null, object yarn = null, object on = null)
        {
            AnimatorStateInfo info = stateInfo?.Pick() ?? default;
            return info.normalizedTime;
        }
    }
    [Serializable, TypeRegistryItem("动画器_获取动画信息状态_短名称Hash值")]
    public class PointerForFloat_AnimatorStateInfo_ShortNameHash : IPointerForInt_Only
    {
        [SerializeReference, LabelText("动画状态信息")] public IPointerForAnimatorStateInfo_Only stateInfo = new PointerForAnimatorStateInfo_CurrentFromAnimator();
        public int Pick(object by = null, object yarn = null, object on = null)
        {
            AnimatorStateInfo info = stateInfo?.Pick() ?? default;
            return info.shortNameHash;
        }
    }
    [Serializable, TypeRegistryItem("动画器_获取动画信息状态_标签 Hash值")]
    public class PointerForFloat_AnimatorStateInfo_TagHash : IPointerForInt_Only
    {
        [SerializeReference, LabelText("动画状态信息")] public IPointerForAnimatorStateInfo_Only stateInfo = new PointerForAnimatorStateInfo_CurrentFromAnimator();
        public int Pick(object by = null, object yarn = null, object on = null)
        {
            AnimatorStateInfo info = stateInfo?.Pick() ?? default;
            return info.tagHash;
        }
    }
    [Serializable, TypeRegistryItem("动画器_获取动画信息状态_全名称Hash值")]
    public class PointerForInt_AnimatorStateInfo_FullNameHash : IPointerForInt_Only
    {
        [SerializeReference, LabelText("动画状态信息")] public IPointerForAnimatorStateInfo_Only stateInfo = new PointerForAnimatorStateInfo_CurrentFromAnimator();
        public int Pick(object by = null, object yarn = null, object on = null)
        {
            AnimatorStateInfo info = stateInfo?.Pick() ?? default;
            return info.fullPathHash;
        }
    }
    [Serializable, TypeRegistryItem("动画器_直接获取Hash值")]
    public class PointerForInt_Animator_Hash : IPointerForInt_Only
    {

        [LabelText("转化名称")] public string name_;
        public int Pick(object by = null, object yarn = null, object on = null)
        {
            return Animator.StringToHash(name_);
        }
    }
    [Serializable, TypeRegistryItem("动画器_初始化获取Hash值")]
    public class PointerForInt_Animator_HashInit : IPointerForInt_Only, IInittable, IPointerForIntCaster
    {

        [LabelText("转化名称")] public string name_;
        [ReadOnly, LabelText("获得Hash值")] public int hash;
        private bool hasInit = false;
        [DetailedInfoBox("", "此处需要引用一个PointerPlayerCaster,把自己的值投射给他它", Message = @"@ ""【绑定投射目标备注："" + (playerCaster != null ? playerCaster.des : ""！未绑定"") ", VisibleIf = "@usePlayerCaster")]
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")] public PointerPlayerSystemObjectCaster playerCaster_;
        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;
        public int Cast()
        {
            return hash;
        }

        public void Init(params object[] ps)
        {
            hash = Animator.StringToHash(name_);
            if (usePlayerCaster && playerCaster_ != null)
            {
                playerCaster_.Recieve(hash);
            }
            hasInit = true;
        }

        public int Pick(object by = null, object yarn = null, object on = null)
        {
            if (!hasInit) Init();
            return hash;
        }
    }
    [Serializable, TypeRegistryItem("动画器_设置参数_浮点数_Hash", "触发/动画")]
    public class PointerAnimator_SetPara_Float_Hash : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("浮点数参数Hash"), SerializeReference] public IPointerForInt_Only NameHash = new PointerForInt_Animator_HashInit();
        [LabelText("浮点值"), SerializeReference] public IPointerForFloat_Only float_Only = new PointerForFloat_Direct();
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            int i = NameHash?.Pick() ?? 0;
            if (i != 0)
            {
                animator.SetFloat(i, float_Only?.Pick() ?? 0);
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_增益_浮点数_Hash", "触发/动画")]
    public class PointerAnimator_Additive_Float_Hash : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("浮点数参数Hash"), SerializeReference] public IPointerForInt_Only NameHash = new PointerForInt_Animator_HashInit();
        [LabelText("增益浮点数值"), SerializeReference] public IPointerForFloat_Only float_Only = new PointerForFloat_Direct();
        [LabelText("乘上帧时间")] public bool mutiTimeDelta = true;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            int i = NameHash?.Pick() ?? 0;
            if (i != 0)
            {
                animator.SetFloat(i, animator.GetFloat(i) + (float_Only?.Pick() ?? 0) * (mutiTimeDelta ? Time.deltaTime : 1));

            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_逼近_浮点数_Hash", "触发/动画")]
    public class PointerAnimator_Lerp_Float_Hash : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("浮点数参数Hash"), SerializeReference] public IPointerForInt_Only NameHash = new PointerForInt_Animator_HashInit();
        [LabelText("目标浮点数值"), SerializeReference] public IPointerForFloat_Only float_Only = new PointerForFloat_Direct();
        [LabelText("Lerp值"), SerializeReference] public IPointerForFloat_Only lerp_Only = new PointerForFloat_Direct();
        [LabelText("Lerp乘上帧时间")] public bool mutiTimeDelta = true;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            float t = lerp_Only?.Pick() ?? 0;
            int i = NameHash?.Pick() ?? 0;
            if (i != 0)
            {
                animator.SetFloat(i, Mathf.Lerp(animator.GetFloat(i), +(float_Only?.Pick() ?? 0), t * (mutiTimeDelta ? Time.deltaTime : 1)));
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_设置参数_整数_Hash", "触发/动画")]
    public class PointerAnimator_SetPara_Int_Hash : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("整数值参数Hash"), SerializeReference] public IPointerForInt_Only NameHash = new PointerForInt_Animator_HashInit();
        [LabelText("整数值"), SerializeReference] public IPointerForInt_Only int_Only = new PointerForInt_Direct();
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            int i = NameHash?.Pick() ?? 0;
            if (i != 0)
            {
                animator.SetInteger(i, int_Only?.Pick() ?? 0);
            }
            return base.Pick(by, yarn, on);
        }
    }

    [Serializable, TypeRegistryItem("动画器_设置参数_布尔值_Hash", "触发/动画")]
    public class PointerAnimator_SetPara_Bool_Hash : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("布尔值参数Hash"), SerializeReference] public IPointerForInt_Only NameHash = new PointerForInt_Animator_HashInit();
        [LabelText("布尔值"), SerializeReference] public IPointerForBool_Only bool_Only = new PointerForBool_Direc();
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            int i = NameHash?.Pick() ?? 0;
            if (i != 0)
            {
                animator.SetBool(i, bool_Only?.Pick() ?? false);
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_切换真假_布尔值_Hash", "触发/动画")]
    public class PointerAnimator_Change_Bool_Hash : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("布尔值参数Hash"), SerializeReference] public IPointerForInt_Only NameHash = new PointerForInt_Animator_HashInit();

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            int i = NameHash?.Pick() ?? 0;
            if (i != 0)
            {
                animator.SetBool(i, !animator.GetBool(i));
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_设置参数_触发器_Hash", "触发/动画")]
    public class PointerAnimator_SetPara_Trigger_Hash : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("触发器参数Hash"), SerializeReference] public IPointerForInt_Only NameHash = new PointerForInt_Animator_HashInit();
        [LabelText("是否触发"), SerializeReference] public IPointerForBool_Only tri_Only = new PointerForBool_Direc();
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            bool b = tri_Only?.Pick() ?? false;
            int i = NameHash?.Pick() ?? 0;
            if (i != 0)
            {
                if (b) animator.SetTrigger(i);
                else animator.ResetTrigger(i);
            }

            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_状态_直接播放_Hash", "触发/动画")]
    public class PointerAnimator_Play_Hash : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("第几层(可空)"), SerializeReference] public IPointerForInt_Only layerIndex = new PointerForInt_Direct();
        [LabelText("动画名Hash"), SerializeReference] public IPointerForInt_Only NameHash = new PointerForInt_Animator_HashInit();
        [LabelText("归一化起始播放位置(可空)"), SerializeReference] public IPointerForFloat_Only start_Only = new PointerForFloat_DirectClamp01();

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null || NameHash == null) return -1;
            int index = layerIndex?.Pick() ?? 0;
            float progress = start_Only?.Pick() ?? 0;
            int i = NameHash?.Pick() ?? 0;
            if (i != 0)
            {
                animator.Play(i, index, progress);
            }

            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_状态_过渡播放_Hash", "触发/动画")]
    public class PointerAnimator_CrossFade_Hash : PointerOnlyAction
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("第几层(可空)"), SerializeReference] public IPointerForInt_Only layerIndex = new PointerForInt_Direct();
        [LabelText("动画名Hash"), SerializeReference] public IPointerForInt_Only NameHash = new PointerForInt_Animator_HashInit();
        [LabelText("归一化过渡时间(默认0.15f)"), SerializeReference] public IPointerForFloat_Only cross_Only = new PointerForFloat_DirectClamp01();
        [LabelText("归一化起始播放位置(可空)"), SerializeReference] public IPointerForFloat_Only start_Only = new PointerForFloat_DirectClamp01();

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null || NameHash == null) return -1;
            int index = layerIndex?.Pick() ?? 0;
            float progress = start_Only?.Pick() ?? 0;
            float cross = cross_Only?.Pick() ?? 0.15f;
            int i = NameHash?.Pick() ?? 0;
            if (i != 0)
            {
                animator.CrossFade(i, cross, index, progress);
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("动画器_获取_浮点参数值_Hash")]
    public class PointerForFloat_AnimatorParam_Hash : IPointerForFloat_Only
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("浮点数参数Hash")][SerializeReference] public IPointerForInt_Only NameHash = new PointerForInt_Animator_HashInit();

        public float Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            int i = NameHash?.Pick() ?? 0;
            if (i != 0)
            {
                animator.GetFloat(i);
            }
            return 0;
        }
    }
    [Serializable, TypeRegistryItem("动画器_获取_整数参数值_Hash")]
    public class PointerForInt_AnimatorParam_Hash : IPointerForInt_Only
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("整数参数Hash")][SerializeReference] public IPointerForInt_Only NameHash = new PointerForInt_Animator_HashInit();
        public int Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return -1;
            int i = NameHash?.Pick() ?? 0;
            if (i != 0)
            {
                animator.GetInteger(i);
            }
            return 0;
        }
    }
    [Serializable, TypeRegistryItem("动画器_获取_布尔参数值_Hash")]
    public class PointerForBool_AnimatorParam_Hash : IPointerForBool_Only
    {
        [LabelText("动画器")] public Animator animator;
        [LabelText("布尔参数Hash")][SerializeReference] public IPointerForInt_Only NameHash = new PointerForInt_Animator_HashInit();
        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            if (animator == null) return false;
            int i = NameHash?.Pick() ?? 0;
            if (i != 0)
            {
                animator.GetBool(i);
            }
            return false;
        }
    }
    #endregion
}
