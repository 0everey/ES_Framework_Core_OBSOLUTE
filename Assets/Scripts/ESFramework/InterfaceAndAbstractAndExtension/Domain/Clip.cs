using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace ES
{
    [TypeRegistryItem("空剪影")]
    public interface IClip : IESModule
    {
        public void SetDomainAndCreateRelationship(IDomain Domain);
        public void TrySetupClip();
        public void FixedUpdate_MustSelfDelegate();
        
    }
    [TypeRegistryItem("抽象剪影定义")]
    public abstract class Clip<Core_, Domain_> : BaseESModule<Domain_>, IClip where Core_ : BaseCore where Domain_ : class, IDomain<Core_>
    {
        //所在域-
        [HideInInspector] public Domain_ Domain;
        [LabelText("更新网络权限")] public NetWorkUpdateOption updateOption = NetWorkUpdateOption.Always;
        #region 只读便捷属性
        public Core_ Core => Domain.Core;
        public override Domain_ GetHost => Domain;
        public GameCenterManager GameCenter => GameCenterManager.Instance;
        public DomainForGameCenterManager GameBaseDomain => GameCenterManager.Instance.BaseDomain;

        public ESNetManager ESNet => ESNetManager.Instance;

        #endregion
        public override bool CanUpdating
        {
            get
            {
                if (Core.NO == null ||
                updateOption == NetWorkUpdateOption.Always) return true;
                if(updateOption == NetWorkUpdateOption.OnlyOwner)
                {
                    if (Core.NO.IsOwner) return true;
                    return false;
                }
                if (updateOption == NetWorkUpdateOption.OnlyOther)
                {
                    if (!Core.NO.IsOwner) return true;
                    return false;
                }
                if (updateOption == NetWorkUpdateOption.OnlyClient)
                {
                    if (Core.NO.IsClientInitialized) return true;
                    return false;
                }
                if (updateOption == NetWorkUpdateOption.OnlyServer)
                {
                    if (Core.NO.IsServerInitialized) return true;
                    return false;
                }
                if (updateOption == NetWorkUpdateOption.OnlyHost)
                {
                    if (Core.NO.IsHostInitialized) return true;
                    return false;
                }
                return true;
            }
        }
        #region 托管相关
        protected override void OnSubmitHosting(Domain_ hosting)
        {
            Domain = hosting;
            CreateRelationship();
            base.OnSubmitHosting(hosting);
            
        }

        protected override void OnWithDrawHosting(Domain_ hosting)
        {
            Domain = hosting;
            base.OnWithDrawHosting(hosting);
            
        }
        #endregion

        #region 重写逻辑
        //OnEnable
        //OnDiable
        //Update 都可以重写 √√√√
        
        //创建   域 对自己 的 显性引用关系
        protected virtual void CreateRelationship()
        {
            //演示：  Domain.Module_AABB=this;
        }
        #endregion

        #region 显示提示等
        [ShowInInspector, GUIColor("ColorGetter"), LabelText(SdfIconType.CashCoin, Text = @"@ ""功能：""+ Description_  "),
            PropertyOrder(-100), Indent(-2), Tooltip("剪影细则")]
        protected virtual string Description_ => "描述";
       

        protected virtual Color ColorGetter()
        {
            return Color.yellow;
        }
        #endregion

        #region 辅助功能
        public void SetDomainAndCreateRelationship(IDomain Domain)
        {
            if (Domain is Domain_ domain_)
            {
                this.Domain = domain_;
                CreateRelationship();
            }
        }
        public void TrySetupClip()
        {
            if (Domain == null)
            {
                Debug.LogError("请先通过核心创建临时关系");
#if UNITY_EDITOR
                if(Application.isEditor)
                if (EditorUtility.DisplayDialog("尝试使用预设", $"您正在尝试应用预设<{Preset}>，在游戏未开始前,他需要核心先发起一次临时关系绑定！请去核心处》三小点》<ES>创建临时关系,创建临时的关系绑定后，再继续", "我这就去", "不去也得去！"))
                {
                    
                }
                else
                {
                    
                }
#endif
                return;
            }
            SetupClipByPreset(Preset);
        }
        #endregion

        #region 预设相关

        [ValueDropdown("allPreset"), LabelText("选择预设", SdfIconType.CaretDownSquareFill), InlineButton("TrySetupClip", "使用该预设"), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        public string Preset = "预设1";
        public virtual string[] allPreset => defaultPresets;
        public static string[] defaultPresets = { "预设1", "预设2", "预设3" };
       
        protected virtual void SetupClipByPreset(string preset)
        {
            /* switch (preset)
             {
                 case "预设1":
                     Debug.Log("使用预设1，把对象移动到000处");
                     Core.transform.position = default;
                     break;
                 case "预设2":
                     Debug.Log("使用预设2，给对象添加刚体");
                     Core.gameObject.AddComponent<Rigidbody>();
                         break;
                 case "预设3":
                     Debug.Log("使用预设1，把对象移动到000处");
                     Core.transform.position = default;
                     break;
             }*/
        }

        public virtual void FixedUpdate_MustSelfDelegate()
        {
            
        }

        #endregion
        protected override void OnEnable()
        {
            base.OnEnable();
           
        }
        
        protected override void Update()
        {
            base.Update();
            /*if (GameCenterManager.Instance.NetSupport && Core.NO != null&&false)
            {
                Debug.Log("本人的" + Core.NO.IsOwner);
                Debug.Log("客户的" + Core.NO.IsClientInitialized);
                Debug.Log("服务器的" + Core.NO.IsServerInitialized);
                if (Core.NO.IsOwner)//本人的：拦截
                {

                }
                else //远程的：拦截
                {
                    var ban = ESNetManager.Instance.LocalDomain.Modole_BlockFunc.ForModuleOtherBan;
                    if (ban != null)
                    {
                        foreach (var t in ban.TypeList)
                        {
                            if (t == this.GetType())
                            {
                                Domain.RemoveClip(this);
                                this._TryInActiveAndDisable();
                                break;
                            }
                        }
                    }
                }
            }*/
        }
    }

}