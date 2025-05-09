using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Timeline.Actions;
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
        //核域通常在一个层级结构下，而核必须为同级或者父级

        //获取特定域
        public T GetDomain<T>() where T : Component
        {
            return GetComponentInChildren<T>();
        }

        //Awake注册
        protected virtual void Awake()
        {
            AwakeBroadCastRegester();
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
