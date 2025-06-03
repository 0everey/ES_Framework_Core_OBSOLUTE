using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ES.EnumCollect;

namespace ESFramework
{

}
namespace ES
{
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
    [Serializable, TypeRegistryItem("可伤害飞行投掷物")]
    public class ClipHurtable_FlyingObject : HurtableClipForItem
    {
        [LabelText("来源实体")] public Entity source;
        [NonSerialized] public ESItem_FlyingSharedData flyingData;
        [FoldoutGroup("移动相关")][LabelText("当前方向")] public Vector3 CurrentDirect;
        [FoldoutGroup("移动相关")][LabelText("目标方向")] public Vector3 TargetDirect;
        [NonSerialized] public Entity target;

        [FoldoutGroup("移动相关")]
        [LabelText("设置目标的方向获取")]
        public EnumCollect.SetTargetAboutDirecOption selfSetTargetOption = EnumCollect.SetTargetAboutDirecOption.Directly;
        [FoldoutGroup("移动相关")]
        [LabelText("设置移动方式原理基于")]
        public EnumCollect.FlyingBaseOn flyBaseOn = EnumCollect.FlyingBaseOn.RigidFixUpdate;
        [FoldoutGroup("移动相关")]
        [LabelText("调转速度")]
        public float directChangeSpeedLevel = 5;
        [FoldoutGroup("移动相关")]
        [LabelText("速度加成")]
        public float SpeedPerUp = 0;



        [FoldoutGroup("伤害")]
        [LabelText("伤害加成(1+%)")] public float DamagePerUp = 0;
        [FoldoutGroup("伤害")]
        [LabelText("伤害加成(add)")] public float DamageAdd = 0;
        [FoldoutGroup("伤害")]
        [LabelText("可伤害的Tag")] public PointerForStringList_Tag Tags = new PointerForStringList_Tag() { tagNames = new List<string>() { "Enemy" } };

        [FoldoutGroup("关于附加效果与生命")]
        [InfoBox("在共享数据中配置碰撞实体时触发的效果")]
        [LabelText("是否是Trigger")] public bool asTrigger = true;
        [LabelText("可损耗生命Layer")] public LayerMask TimesSubLayer = 2 << ESEditorRuntimePartMaster.LayerEntity + 2 << ESEditorRuntimePartMaster.LayerWall;
        private float lifeTimeHasGo = 10;
        private int canColTimes = 2;
        [FoldoutGroup("特殊功能"), LabelText("延迟时间")]
        public float delayTime = 0;


        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            Domain.Module_Flying = this;
            flyingData = Core.sharedData as ESItem_FlyingSharedData ?? new ESItem_FlyingSharedData();
            if (flyingData == null) Domain.RemoveClip(this);//没有存在的必要了
            lifeTimeHasGo = 0;
            canColTimes = flyingData.maxTimes;
            if (TargetDirect == default)
            {
                TargetDirect = Core.transform.forward;
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
                        TargetDirect = (e.transform.position + Vector3.up - (Core.transform.position)).normalized;
                    }
                    else if (setDirecQuick_ == SetTargetAboutDirecOption.Parabola)
                    {
                        TargetDirect = (e.transform.position - Core.transform.position + Vector3.up).normalized;
                    }
                    else if (setDirecQuick_ == SetTargetAboutDirecOption.RadAndFollow)
                    {
                        TargetDirect = (Vector3.Lerp(e.transform.position - Core.transform.position, e.transform.right, UnityEngine.Random.Range(-0.5f, 0.5f))).normalized; ;
                    }
                }
                if (flyBaseOn == FlyingBaseOn.RigidVelocityOnce)
                {
                    Core.Rigid.velocity = TargetDirect.normalized * flyingData.speed * (1 + SpeedPerUp);
                }
            }
        }
        protected override void Update()
        {
            delayTime -= Time.deltaTime;
            if (delayTime > 0) return;
            PrivateMethod_Lerp();
            PrivateMethod_LifeTime();
            if (flyBaseOn == FlyingBaseOn.TransUpdate)
            {
                Core.transform.position += CurrentDirect * Time.deltaTime * flyingData.speed * (1 + SpeedPerUp);
            }
            base.Update();
        }
        public override void FixedUpdate_MustSelfDelegate()
        {
            base.FixedUpdate_MustSelfDelegate();
            if (delayTime > 0) return;
            if (flyBaseOn == FlyingBaseOn.RigidVelocityUpdating)
            {
                Core.Rigid.velocity = CurrentDirect.normalized * flyingData.speed * (1 + SpeedPerUp);
            }
            else if (flyBaseOn == FlyingBaseOn.RigidFixUpdate)
            {
                Core.Rigid.position += CurrentDirect.normalized * Time.fixedDeltaTime * flyingData.speed * (1 + SpeedPerUp);
            }
        }
        private void PrivateMethod_Lerp()
        {
            CurrentDirect = Vector3.Lerp(CurrentDirect, TargetDirect, Time.deltaTime * directChangeSpeedLevel);
        }
        private void PrivateMethod_LifeTime()
        {
            lifeTimeHasGo += Time.deltaTime;
            if (lifeTimeHasGo > flyingData.missileLife_)
            {
                Core.whyDes = new Link_DestroyWhy() { options = DestroyWhyOption.LifeTime };
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
            Domain.OnFixedUpdate += FixedUpdate_MustSelfDelegate;
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
            Domain.OnFixedUpdate -= FixedUpdate_MustSelfDelegate;
        }
        private void PassiveDelegate_OnTriOrColEntityHandles(Entity who, Vector3 at)
        {
            if (Tags.tagNames.Contains(who.tag))
            {

                KeyValueMatchingUtility.ESLink.Global.GlobalLink_EntityAttackEntityTryStart
                    (new Link_EntityAttackEntityTryStart()
                    {
                        attacker = source,
                        damage = new Damage() { damage = flyingData.damage + source?.entityVariableData.DamagePerUp ?? 0 },
                        victim = who
                    });
                if (flyingData.entityHandleOfItem != null)
                {
                    var apply = flyingData.entityHandleOfItem.handles_;
                    if (apply != null && apply.Count > 0)
                    {
                        foreach (var i in apply)
                        {
                            i.Pick(who, source, this);
                        }
                    }
                }
            }
        }
        private void PassiveDelegate_OnColEvery(Collision who, Vector3 at, bool b)
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
            if (handle.OnDesBirth != null && (handle.optionForDesBirth & why.options) > 0)
            {
                Debug.Log(666);
                ESSpawnMaster.Instance.Ins(handle.OnDesBirth, Core.transform.position, null);
            }

            if (handle.OnDesPlaySound != null && (handle.optionForPlaySound & why.options) > 0)
            {
                GameCenterManager.Instance.AudioMaster.PlayDirect_Sound_OneShot(handle.OnDesPlaySound, 0.8f);
            }
        }
        #region 预设


        #endregion
    }

    [Serializable, TypeRegistryItem("可伤害飞行物扩展：追踪")]
    public class ClipHurtable_FlyingObject_Expand : HurtableClipForItem
    {
        [NonSerialized] ClipHurtable_FlyingObject Refer_Fly;
        [LabelText("追踪方向改变速度")] public AnimationCurve animationCurve = AnimationCurve.Constant(0, 1, 1);
        [LabelText("追踪时间")] public float followTime = 5;
        [LabelText("延迟时仍然追踪")] public bool FollowSWhenDelay = true;
        [LabelText("已经追踪")] public float hasFollow = 0;
        [LabelText("追踪强度")] public float followStrengthMutipler = 1;
        [FoldoutGroup("随机化")][LabelText("启用随机的追踪时间")] public bool enableRandomFollow = false;
        [FoldoutGroup("随机化")][LabelText("启用随机的追踪时间")] public PointerForFloat_Random30 randomTime30 = new PointerForFloat_Random30() { float_range = new Vector2(3, 6) };

        [FoldoutGroup("随机化")][LabelText("启用随机的追踪强度")] public bool enableRandomStrength = false;
        [FoldoutGroup("随机化")][LabelText("启用随机的追踪强度")] public PointerForFloat_Random randomStrength30 = new PointerForFloat_Random() { float_range = new Vector2(0.1f, 2) };

        protected override void OnEnable()
        {
            base.OnEnable();
            Refer_Fly = Domain.Module_Flying;
            hasFollow = 0;
            if (enableRandomFollow)
            {
                followTime = randomTime30?.Pick() ?? followTime;
            }
            if (enableRandomStrength)
            {
                followStrengthMutipler = randomStrength30?.Pick() ?? followStrengthMutipler;
            }
            followTime = Mathf.Max(followTime, 0.2f);
        }
        protected override void Update()
        {
            base.Update();
            if (Refer_Fly != null && Refer_Fly.target != null)
            {
                if (Refer_Fly.delayTime <= 0 || FollowSWhenDelay)
                {
                    hasFollow += Time.deltaTime;
                    if (hasFollow < followTime)
                    {
                        float progress = hasFollow / followTime;
                        float strength = animationCurve.Evaluate(progress) * followStrengthMutipler;
                        Refer_Fly.TargetDirect = Vector3.Lerp(Refer_Fly.TargetDirect, Refer_Fly.target.transform.position + Vector3.up - (Core.transform.position), strength * Time.deltaTime);
                    }
                    else
                    {
                        TryDisableSelf();
                    }
                }
            }
        }
    }
}