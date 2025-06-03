using FishNet.Object;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
#endif
using UnityEngine;

namespace ES
{
    /*核心功能*/
    public interface ICore:IESHosting
    {
        public void AwakeBroadCastRegester();//初始化创建链接
    }
    [DefaultExecutionOrder(-2)]//顺序在前
    public abstract class BaseCore : ESHostingMono, ICore
    {
        [TitleGroup("网络支持")]
        [Required(errorMessage: "如果你制作网络游戏，请考虑好他的必要性"), PropertyOrder(-10), PropertySpace(5, 15),InlineButton("DebugNO","输出NO信息")]
        [LabelText("链接为网络对象")] public NetworkObject NO;
        //核域通常在一个层级结构下，而核必须为同级或者父级

        private void DebugNO()
        {
            
            Debug.Log("本人的" +NO.IsOwner);
            Debug.Log("客户的" + NO.IsClientInitialized);
            Debug.Log("服务器的" +NO.IsServerInitialized);
        }

        //获取特定域
        public T GetDomain<T>() where T : Component
        {
            return GetComponentInChildren<T>();
        }

        //Awake注册
        protected virtual void Awake()
        {
           /* if (NO == null)*/
                AwakeBroadCastRegester();
            /*else Invoke(nameof(AwakeBroadCastRegester),0.1f);*/
        }
        //注册发生前发生的事儿
        protected virtual void BeforeAwakeBroadCastRegester()
        {
            
        }
        //注册完成后发生的事儿
        protected virtual void AfterAwakeBroadCastRegester()
        {

        }

        //注册
        public void AwakeBroadCastRegester()
        {
            BeforeAwakeBroadCastRegester();
            
            BroadcastMessage("AwakeRegisterDomain", this, options: SendMessageOptions.DontRequireReceiver);

            AfterAwakeBroadCastRegester();
        }
        

        //编辑器模式下的临时关联
#if UNITY_EDITOR
        [ContextMenu("<ES>创建临时关系")] 
        public void CreateCacheRelationship()
        {
            var all = GetComponentsInChildren<IDomain>();
            foreach(var i in all)
            {
                i.RegesterAllButOnlyCreateRelationship(this);
            }
        }
#endif
    }





}
