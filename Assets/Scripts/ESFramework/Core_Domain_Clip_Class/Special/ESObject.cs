using ES;
using FishNet.Object;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ES
{
    [TypeRegistryItem("Entity,Item的父类")]
 public class  ESObject  : BaseCore
{
       
        [FoldoutGroup("固有"), LabelText("刚体")] public Rigidbody Rigid;
        #region 委托事件

        [FoldoutGroup("委托事件(Unity)")] public Action<Collision,Vector3,bool> OnColHappen = (col,where,isEntity) => { };
        [FoldoutGroup("委托事件(Unity)")] public Action<Collider,Vector3,bool> OnTriHappen = (tri, where, isEntity) => { };

        [FoldoutGroup("委托事件(Unity)")] public Action<Entity, Vector3> OnColEntityHappen = (col, where) => { };
        [FoldoutGroup("委托事件(Unity)")] public Action<Entity, Vector3> OnTriEntityHappen = (tri, where) => { };

        [FoldoutGroup("委托事件(Unity)")] public Action<Link_DestroyWhy> OnDestroyHappen=(why)=> { };


        #endregion

        #region 隐藏属性表
        [NonSerialized] public Link_DestroyWhy whyDes;
        [NonSerialized] public Queue<Entity> ignoreEntities = new Queue<Entity>();
        #endregion
        //显性声明扩展域
        //public BaseDomainForXXX BaseDomain;
        //public 02DomainForXXX StateMachineDomain;

        //注册前的操作
        protected override void BeforeAwakeBroadCastRegester()
        {
            base.BeforeAwakeBroadCastRegester();
            Rigid = GetComponent<Rigidbody>();
        }

        #region 回调
        
        public void OnDestroy()
        {
          
        }
        private void OnTriggerEnter(Collider other)
        {
            Vector3 center = (other.transform.position + transform.position) / 2;
            bool isEntity = false;
            if (other.gameObject.layer == ESEditorRuntimePartMaster.LayerEntity)
            {
              
                var e = other.GetComponent<Entity>();
                if (e != null)
                {
                   
                    if (ignoreEntities.Contains(e)) return;
                    
                    OnTriEntityHappen?.Invoke(e, center);
                    isEntity = true;
                }
                
            }
            OnTriHappen?.Invoke(other, center, isEntity);
        }
        private void OnCollisionEnter(Collision collision)
        {
           
            bool isEntity = false;
            Vector3 pos = collision.contacts[0].point;
            if (collision.gameObject.layer == ESEditorRuntimePartMaster.LayerEntity)
            {
                var e = collision.gameObject.GetComponent<Entity>();
                if (e != null)
                {
                    if (ignoreEntities.Contains(e)) return;
                    OnColEntityHappen?.Invoke(e, pos);
                    isEntity = true;
                }

            }
            OnColHappen?.Invoke(collision, pos,isEntity);
        }
        public virtual void TryDestroyThisESObject()
        {
            OnDestroyHappen?.Invoke(whyDes);
            Destroy(gameObject);
        }
        public void AddIgnoreEntity(Entity e)
        {
            ignoreEntities.Enqueue(e);
            
            if (ignoreEntities.Peek() == null)
            {
                ignoreEntities.Dequeue();
            }
        }
        #endregion
    }
}
   

