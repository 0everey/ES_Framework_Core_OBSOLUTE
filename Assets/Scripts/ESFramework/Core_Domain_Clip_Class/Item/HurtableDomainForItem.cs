using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumCollect;
using static Unity.Burst.Intrinsics.Arm;

namespace ESFramework
{

}
namespace ES {
    public class HurtableDomainForItem : BaseDomain<Item, HurtableClipForItem>
    {
        //捡起来！
        #region 剪影汇总
        [NonSerialized] public ClipHurtable_FlyingObject Module_Flying; 


        #endregion
        protected override void CreatRelationship()
        {
            base.CreatRelationship();
            core.HurtableDomain = this;
        }
    }
    [Serializable]
    public abstract class HurtableClipForItem : Clip<Item, HurtableDomainForItem>
    {

        
    }
    [Serializable,TypeRegistryItem("可伤害飞行投掷物")]
    public class ClipHurtable_FlyingObject : HurtableClipForItem
    {
        [LabelText("来源实体")] public Entity source;
        [NonSerialized] public ESItem_FlyingSharedData flyingData;
        [FoldoutGroup("移动相关")][LabelText("当前方向")] public Vector3 CurrentDirect;
        [FoldoutGroup("移动相关")][LabelText("目标方向")] public Vector3 TargetDirect;
        [NonSerialized]public Entity target;
        
        [FoldoutGroup("移动相关")]
        [LabelText("设置目标的方向获取")]
        public SetTargetAboutDirecOption selfSetTargetOption = SetTargetAboutDirecOption.Directly;
        [FoldoutGroup("移动相关")]
        [LabelText("设置移动方式原理基于")]
        public FlyingBaseOn flyBaseOn = FlyingBaseOn.RigidFixUpdate;
        [FoldoutGroup("移动相关")]
        [LabelText("调转速度")]
        public float directChangeSpeedLevel = 1;

        


        [FoldoutGroup("伤害")]
        [LabelText("伤害加成(1+%)")]public float DamagePerUp = 0;
        [FoldoutGroup("伤害")]
        [LabelText("伤害加成(add)")] public float DamageAdd = 0; 
        [FoldoutGroup("伤害")]
        [LabelText("可伤害的Tag")] public PointerForStringList_Tag Tags = new PointerForStringList_Tag() { tagNames=new List<string>() { "Enemy" } };

        [FoldoutGroup("关于附加效果与生命")]
        [InfoBox("在共享数据中配置碰撞实体时触发的效果")]
        [LabelText("是否是Trigger")] public bool asTrigger = true;
        [LabelText("可损耗生命Layer")] public LayerMask TimesSubLayer = 2 << EditorMaster.LayerEntity + 2 << EditorMaster.LayerWall;
        private float lifeTimeHasGo=10;
        private int canColTimes = 2;
        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            Domain.Module_Flying = this;
            flyingData = Core.sharedData as ESItem_FlyingSharedData ?? new ESItem_FlyingSharedData();
            if (flyingData == null) Domain.RemoveClip(this);//没有存在的必要了
            lifeTimeHasGo =0;
            canColTimes = flyingData.maxTimes;
            if (TargetDirect == default)
            {
                TargetDirect = Core.transform.forward * flyingData.speed;
            }

        }
        [Button("设置目标测试")]
        public void SetTarget(Entity e, SetTargetAboutDirecOption setDirecQuick_ = SetTargetAboutDirecOption.BySelfDefault)
        {
            if (e != null)
            {
                target = e;
                {
                    if (setDirecQuick_ == SetTargetAboutDirecOption.BySelfDefault)
                    {
                        setDirecQuick_ = selfSetTargetOption;
                    }
                    if (setDirecQuick_ == SetTargetAboutDirecOption.None)
                    {

                    }
                    else if (setDirecQuick_ == SetTargetAboutDirecOption.Directly)
                    {
                        TargetDirect = (e.transform.position - Core.transform.position).normalized;
                    }
                    else if (setDirecQuick_ == SetTargetAboutDirecOption.Parabola)
                    {
                        TargetDirect = (e.transform.position - Core.transform.position + Vector3.up).normalized;
                    }
                    else if (setDirecQuick_ == SetTargetAboutDirecOption.RadAndFollow)
                    {
                        TargetDirect =( Vector3.Lerp(e.transform.position - Core.transform.position, e.transform.right, UnityEngine.Random.Range(-0.5f, 0.5f))).normalized; ;
                    }
                }
                if(flyBaseOn== FlyingBaseOn.RigidVelocityOnce)
                {
                    Core.Rigid.velocity = TargetDirect.normalized * flyingData.speed;
                }
            }
        }
        protected override void Update()
        {
            PrivateMethod_Lerp();
            PrivateMethod_LifeTime();
            if(flyBaseOn== FlyingBaseOn.TransUpdate)
            {
                Core.transform.position += CurrentDirect * Time.deltaTime * flyingData.speed;
            }
            base.Update();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(flyBaseOn== FlyingBaseOn.RigidVelocityUpdating)
            {
                Core.Rigid.velocity = CurrentDirect.normalized * flyingData.speed;
            }else if (flyBaseOn == FlyingBaseOn.RigidFixUpdate)
            {
                Core.Rigid.position += CurrentDirect.normalized * Time.fixedDeltaTime * flyingData.speed;
            }
        }
        private void PrivateMethod_Lerp()
        {
            CurrentDirect = Vector3.Lerp(CurrentDirect,TargetDirect,Time.deltaTime*directChangeSpeedLevel);
        }
        private void PrivateMethod_LifeTime()
        {
            lifeTimeHasGo += Time.deltaTime;
            if (lifeTimeHasGo > flyingData.missileLife_)
            {
                Core.whyDes = new Link_DestroyWhy() { options= DestroyWhyOption.LifeTime };
                Core.TryDestroyThisESObject();
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (asTrigger)
            {
                Core.OnTriEntityHappen += PassiveDelegate_OnTriOrColEntityHandles;
                Core.OnTriHappen += PassiveDelegate_OnTriEvery;
            }
            else
            {
                Core.OnColHappen += PassiveDelegate_OnColEvery;
                Core.OnColEntityHappen += PassiveDelegate_OnTriOrColEntityHandles;
            }
            Core.OnDestroyHappen += PassiveDelegate_OnDeS;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (asTrigger)
            {
                Core.OnTriHappen -= PassiveDelegate_OnTriEvery;
                Core.OnTriEntityHappen -= PassiveDelegate_OnTriOrColEntityHandles;
            }
            else
            {
                Core.OnColHappen -= PassiveDelegate_OnColEvery;
                Core.OnColEntityHappen -= PassiveDelegate_OnTriOrColEntityHandles;
            }
            Core.OnDestroyHappen -= PassiveDelegate_OnDeS;
            Debug.Log("妈耶");
        }
        private void PassiveDelegate_OnTriOrColEntityHandles(Entity who,Vector3 at)
        {
            if (Tags.tagNames.Contains(who.tag))
            {
                if (flyingData.entityHandleOfItem != null)
                {
                    var apply = flyingData.entityHandleOfItem.handles_;
                    if (apply != null&&apply.Count>0)
                    {
                        foreach(var i in apply)
                        {
                            i.Pick(who,source,this);
                        }
                    }
                }
            }
        }
        private void PassiveDelegate_OnColEvery(Collision who, Vector3 at,bool b)
        {
            if (((2 << who.gameObject.layer) & TimesSubLayer) > 0)
            {
                canColTimes--;
                if (canColTimes <= 0)
                {
                    Core.TryDestroyThisESObject();
                }
            }
        }
        private void PassiveDelegate_OnTriEvery(Collider who, Vector3 at, bool b)
        {
            Debug.Log(who.gameObject+""+who.gameObject.layer+TimesSubLayer.GetHashCode());
            if (((1 << who.gameObject.layer) & TimesSubLayer) > 0)
            {
                canColTimes--;
                if (canColTimes <= 0)
                {
                    Core.whyDes.options = DestroyWhyOption.OnTriEntity;
                    Core.TryDestroyThisESObject();
                }
            }
        }
        private void PassiveDelegate_OnDeS(Link_DestroyWhy why)
        {
            Debug.Log("妈耶");
            var handle = flyingData.entityHandleOfItem;
            Debug.Log(333+""+ handle.OnDesBirth != null+ handle.optionForDesBirth.ToString()+ why.options.ToString());
            if (handle.OnDesBirth!=null&&(handle.optionForDesBirth & why.options) > 0)
            {
                Debug.Log(666);
                GameCenterManager.Instance.Ins(handle.OnDesBirth,Core.transform.position,null);
            }

            if(handle.OnDesPlaySound!=null && (handle.optionForPlaySound & why.options) > 0)
            {
                GameCenterManager.Instance.AudioMaster.PlayDirect_Sound_OneShot(handle.OnDesPlaySound,0.8f);
            }
        }
        #region 预设


        #endregion
    }
}