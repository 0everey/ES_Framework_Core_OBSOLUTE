using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public class ESUIOriginal : SerializedMonoBehaviour
    {
        
        private void Awake()
        {
            
        }
        [LabelText("正在加载")] public bool IsLoading = false;
        public void TryOpen(ILink link=default)
        {
            if (enabled) return;//还在使用呢
            this.enabled = true;  //可见不一定可用把
            OnOpen(link);
            gameObject.SetActive(true);//打开必可见
        }
        public void TryClose(ILink link = default)
        {
            if (!enabled) return;//已经禁用了哈
            this.enabled = false;
            OnClose(link);
            //关闭不一定可见--或者需要等待不可见
        }
        protected virtual void OnOpen(ILink link)
        {

        }
        protected virtual void OnClose(ILink link)
        {

        }
        //注册和注销
        protected virtual void OnEnable()
        {
            
        }
        //注册和注销
        protected virtual void OnDisable()
        {
            
        }
    }
}

