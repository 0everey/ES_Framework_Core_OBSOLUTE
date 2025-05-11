using DG.Tweening;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ES.ClipStateMachine_CrashDodge;

namespace ES
{
    #region Link表//性能未知，先不用
    /*
    [Serializable,TypeRegistryItem("Link_一个实体")]
    public struct Link_AEntity : ILink
    {
        [LabelText("一个实体")] public Entity entity;
    }
    [Serializable, TypeRegistryItem("Link_一些实体")]
    public struct Link_SomeEntity : ILink
    {
        [LabelText("一个实体")] public List<Entity> entity;
    }*/

    #endregion
    #region 原始声明
    public interface IReleasablePointerChain : IPointerChain
    {

    }


    #endregion
    public interface IPointerForEntity_Only : IPointerOnlyBack<Entity>
    {

    }
    public interface IPointerForSomeEntity_Only : IPointerOnlyBackList<Entity>
    {

    }
    public interface IPointerOnlyByEntityYarnEntity : IPointer<object, Entity, Entity, EntityState_Skill>
    {

    }
    //释放技能本质上是一个对Entity的遍历 这个是 应用器
    public interface IReleasablePointerOnlyByEntityYarnEntity : IPointerOnlyByEntityYarnEntity
    {
       
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick(a as Entity,b as Entity,c as EntityState_Skill);
        }
    }
    /*从一个实体获得多个实体*/
    public interface IPointerForSomeEntityByEntityYarnEntity : IPointerChainAny<List<Entity>, Entity,Entity>
    {
        //by 被操作 yarn 发起人 back 最终目的
    }
    /*从多个实体获得多个实体*/
    public interface IPointerForSomeEntityBySomeEntityYarnEntity : IPointerChainAny<List<Entity>,List<Entity>, Entity>
    {

    }
    /*释放专用：单实体变多实体*/
    public interface IReleasablePointerForSomeEntityByEntityYarnEntity : IPointerForSomeEntityByEntityYarnEntity,IReleasablePointerChain
    {
         object IPointer.Pick(object a, object b, object c)
        {
            if (a is Entity e)
            {
                return Pick(e, b as Entity);
            }
            else if (a is List<Entity> es)
            {
                return Pick(es.First(), b as Entity);
            }
            return null;
        }
    }
    /*释放专用：多实体变多实体*/
    public interface IReleasablePointerForSomeEntityBySomeEntityYarnEntity : IPointerForSomeEntityBySomeEntityYarnEntity, IReleasablePointerChain
    {
         object IPointer.Pick(object a, object b, object c)
        {
            if (a is List<Entity> es)
            {
                return Pick(es, b as Entity);
            }else if(a is Entity e)
            {
                return Pick(new List<Entity>() { e }, b as Entity);
            }
            return null;
        }
    }


    #region 从一个实体获得多个实体的Class
    [Serializable,TypeRegistryItem("单实体=>>多实体:只有我自己的列表")]
    public class ReleasablePointer_EntitySelf : IReleasablePointerForSomeEntityByEntityYarnEntity
    {
        public List<Entity> Pick(Entity by = null, Entity yarn = null, object on = null)
        {
            return new List<Entity>() { by };
        }

       
    }
    [Serializable,TypeRegistryItem("单实体=>>多实体:看到的目标")]
    public class ReleasablePointer_EntityVision : IReleasablePointerForSomeEntityByEntityYarnEntity
    {
        [LabelText("是否立刻重新刷新目标")] public bool ReFreshRightly = false;
        [LabelText("最多目标"),SerializeReference] public IPointerForInt_Only max = new PointerForInt_Direct() { int_ = 5 };
        public List<Entity> Pick(Entity by = null, Entity yarn = null, object on = null)
        {
            return KeyValueMatchingUtility.ESBack.ForEntityBack.GetEntityVision(by,max?.Pick()??5,ReFreshRightly);
        }
    }
    [Serializable, TypeRegistryItem("单实体=>>多实体:我身边的队友")]
    public class ReleasablePointer_EntityAroundFriend : IReleasablePointerForSomeEntityByEntityYarnEntity
    {
        [SerializeReference,LabelText("查找范围半径")]
        public IPointerForFloat_Only distanceR=new PointerForFloat_Direct() { float_=5 };
        [SerializeReference, LabelText("是否是相对坐标偏移")]
        public bool IsRele = true;
        [SerializeReference, LabelText("向量偏移量")]
        public IPointerForVector3_Only ReleV3 = new PointerForVector3_Direct() {  vector=default };
        [LabelText("包括我自己？")]
        public bool containsThis = true;
        public List<Entity> Pick(Entity by = null, Entity yarn = null, object on = null)
        {
            List<Entity> es = new List<Entity>();
            if (containsThis) es.Add(by);
            var friends = KeyValueMatchingUtility.ESBack.ForEntityBack.GetEntityAroundFriend(
                by,distanceR?.Pick()??5, 
                IsRele? by.transform.TransformPoint(ReleV3?.Pick()??default)  : ReleV3?.Pick()??default);
            es.AddRange(friends);
            return es;
        }
    }
    [Serializable, TypeRegistryItem("单实体=>>多实体:来自我的默认缓冲池")]
    public class ReleasablePointer_EntityCacheMainTarget : IReleasablePointerForSomeEntityByEntityYarnEntity
    {
        
        [LabelText("使用后就清理")]
        public bool UseAndClear = true;
        public List<Entity> Pick(Entity by = null, Entity yarn = null, object on = null)
        {
            List<Entity> es = KeyValueMatchingUtility.ESBack.ForEntityBack.GetEntityTargetCache(by,useAndClear:UseAndClear);
            return es;
        }
    }
    [Serializable, TypeRegistryItem("单实体=>>多实体:来自我的某个缓冲池")]
    public class ReleasablePointer_EntityCacheWhichTarget : IReleasablePointerForSomeEntityByEntityYarnEntity
    {
        [LabelText("使用后就清理")]
        public bool UseAndClear = true;
        [LabelText("哪一个缓冲池")]
        public string whichCache = "缓冲池标记";
        public List<Entity> Pick(Entity by = null, Entity yarn = null, object on = null)
        {
            List<Entity> es = KeyValueMatchingUtility.ESBack.ForEntityBack.GetEntityTargetCache(by, whichCache, useAndClear: UseAndClear);
            return es;
        }
    }
    #endregion

    #region 从多个实体获得多个实体
    [Serializable, TypeRegistryItem("多实体=>>多实体:筛选Tag和距离")]
    public class ReleasablePointer_SelectTagsAndDistance : IReleasablePointerForSomeEntityBySomeEntityYarnEntity
    {
        [LabelText("筛选Tag(置空不筛选)")]
        public PointerForStringList_Tag tags = new PointerForStringList_Tag();
        [LabelText("最大距离(为负不筛选)")]
        public float r = 100;
        public List<Entity> Pick(List<Entity> by = null, Entity launcher = null, object on = null)
        {
          //  Debug.Log("最后一步" + by+"and"+launcher);
            List<string> useTags = tags?.Pick() ?? null;
            if (useTags == null || useTags.Count == 0) return by.Where((e) =>(e!=null&&(r < 0 || Vector3.Distance(launcher.transform.position, e.transform.position) < r))).ToList();
            return by.Where((e) => useTags.Contains(e.gameObject.tag)&&(r<0|| Vector3.Distance(launcher.transform.position,e.transform.position) < r)).ToList();
        }

       
    }


    #endregion

    #region 操作单个对象
    [Serializable, TypeRegistryItem("A常规：扣除血量")]
    public class EntityHandle_Damage : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            Debug.Log("扣除血量" + by + yarn + on.GetKey());
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            } 
            return 5;
        }

      
    }
    [Serializable, TypeRegistryItem("A常规：获得Buff")]
    public class EntityHandle_AddBuff : IReleasablePointerOnlyByEntityYarnEntity
    {
        [LabelText("直接引用SO(可选)")]public BuffSoInfo useInfo;
        [LabelText("用键查询(可选)")] public KeyString_BuffUse key = new KeyString_BuffUse();
        [LabelText("使用-自定义Buff开始状态")] public bool IsSelfDefineStartBuffStatus = true;
        [LabelText("输入自定义Buff开始状态")] public BuffStatusTest BuffStatusTest = new BuffStatusTest() { duration = 10 };
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                var use = useInfo;
               
                if (use == null)
                {
                    use = KeyValueMatchingUtility.DataInfoPointer.PickBuffSoInfoByKey(key.Key());
                }
                if (use != null)
                {
                    KeyValueMatchingUtility.DataApply.ApplyBuffInfoToEntity(use, by, IsSelfDefineStartBuffStatus ? BuffStatusTest : null);
                    
                }
                }
                return null;
        }


    }
    [Serializable, TypeRegistryItem("A常规:移除Buff")]
    public class EntityHandle_RemoveBuff : IReleasablePointerOnlyByEntityYarnEntity
    {
        [LabelText("直接引用SO(可选)")] public BuffSoInfo useInfo;
        [LabelText("用键查询(可选)")] public KeyString_BuffUse key = new KeyString_BuffUse();

        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            
            if (by != null)
            {
                if (by != null)
                {
                    var use = useInfo;
                    if (use == null)
                    {
                        use = KeyValueMatchingUtility.DataInfoPointer.PickBuffSoInfoByKey(key.Key());
                    }
                    if (use != null) KeyValueMatchingUtility.DataApply.Apply_Remove_BuffInfoToEntity(useInfo,by);
                }
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("B特殊:解除负面效果")]
    public class EntityHandle_CancelNagativeEffect : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("B特殊:解除正面效果")]
    public class EntityHandle_CancelPositiveEffect : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("B特殊:晕眩控制")]
    public class EntityHandle_Controll : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("C移动:瞬间刚体力")]
    public class EntityHandle_AddRigidForce : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("D主动：直接生成预制件在实体原地附近")]
    public class EntityHandle_BirthAtHere : IReleasablePointerOnlyByEntityYarnEntity
    {
        [LabelText("生成预制件")] public GameObject prefab;
        [LabelText("用对象池")] public bool UsePool = true;
        [LabelText("坐标向量"),SerializeReference] public IPointerForVector3_Only vector_only=new PointerForVector3_Direct();
        [LabelText("旋转"),SerializeReference] public IPointerForQuaternion_Only quaternion_Only = new PointerForQuaternion_Direc(); 
        [LabelText("坐标偏移模式")] public EnumCollect.PlacePosition placePos;
        [LabelText("方向偏移模式")] public EnumCollect.PlaceRotation placeRot;
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            Debug.Log("pick");
            if (by != null)
            {
                Debug.Log("pick2");
                Vector3 lookat = (yarn.transform.position - by.transform.position).normalized;
                GameObject gg = UsePool ? Ev_PoolManager.Instance.GetInPool(prefab) : MonoBehaviour.Instantiate(prefab);
                Vector3 vv = vector_only?.Pick() ?? default;
                Quaternion rot = quaternion_Only?.Pick() ?? default;
                if(placePos== EnumCollect.PlacePosition.WorldSpace)
                {
                    gg.transform.position = by.transform.position + vv;
                }else if(placePos == EnumCollect.PlacePosition.SelfSpace)
                {
                    gg.transform.position = by.transform.position + by.transform.TransformDirection(vv);
                }else
                {
                    Quaternion quaternion1 = Quaternion.LookRotation(lookat);
                    gg.transform.position = by.transform.position + quaternion1 * vv;
                }

                if(placeRot== EnumCollect.PlaceRotation.WorldSpace)
                {
                    gg.transform.rotation = rot;
                }else if(placeRot== EnumCollect.PlaceRotation.SelfSpace)
                {
                    gg.transform.rotation = (by.transform.rotation * rot);
                }
                else
                {
                    Quaternion quaternion1 = Quaternion.LookRotation(lookat);
                    gg.transform.rotation =quaternion1 * rot;
                }
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    
    
    [Serializable, TypeRegistryItem("D主动：迅速闪身移动")]
    public class EntityHandle_Dodge : IReleasablePointerOnlyByEntityYarnEntity
    {
        [LabelText("闪身运动"),InlineProperty]
        public Applyable_CrashDodge ApplyCrash=new Applyable_CrashDodge() { 
            baseOn= EnumCollect.ToDestionationBaseOn.ESCurveModule,
            duration=0.25f, CoolDownNext=0.25f,
            vector=new Vector3(0,0,0.8f), 
            vectorHandle= EnumCollect.ToDestinationVectorSpace.SelfSpace
        };
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            Debug.Log(by+"闪身");
            if (by != null)
            {
                var crash = by.StateMachineDomain.Module_CrashDodge;
                if (crash != null)
                {
                    crash.TryAddCrashDodge(ref ApplyCrash);
                }
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("E动画：强制原生动画触发器")]
    public class EntityHandle_AnimatorTrigger : IReleasablePointerOnlyByEntityYarnEntity
    {
        
        [LabelText("触发器参数名")]public string name = "触发器参数";
        [LabelText("是否启用")]public bool SetOrReset = true;
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                if(SetOrReset) by.Anim.SetTrigger(name);
                else by.Anim.ResetTrigger(name);
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("E动画：过渡播放原生动画")]
    public class EntityHandle_AnimatorCrossFade : IReleasablePointerOnlyByEntityYarnEntity
    {
        [LabelText("过渡动画参数名")] public string name = "动画状态名参数";
        [LabelText("过渡时间")] public float tranTime = 0.2f;
        [LabelText("层级Index")] public int AnimLayer = 0;
        [LabelText("启用固定过渡时间")] public bool UseFixTran = false;
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                if (UseFixTran)
                {
                    by.Anim.CrossFadeInFixedTime(name, tranTime, AnimLayer);
                }
                else
                {
                    by.Anim.CrossFade(name,tranTime,AnimLayer);
                }
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("E动画：逼近层级权重")]
    public class EntityHandle_AnimatorLayerWeight : IReleasablePointerOnlyByEntityYarnEntity
    {
        [LabelText("哪一层")] public int LayerIndex = 0;
        [LabelText("目标值")] public float targetWeight = 1;
        [LabelText("逼近时间")] public float lerpTime = 0.5f;
        [LabelText("多久退出？(默认随着技能)")] public float exitTime = 2;

        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                
                var Use= DOTween.To(()=>by.Anim.GetLayerWeight(LayerIndex),(to)=>by.Anim.SetLayerWeight(LayerIndex,to),targetWeight,lerpTime);
                var delayExit = DOTween.Sequence();
                delayExit.AppendInterval(lerpTime);
                delayExit.AppendCallback(()=>Use.Rewind());
                on.OnExit += (t) =>
                {
                    delayExit.Kill();
                    Use.Rewind();
                };
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("E动画：IK控制-手部预设")]
    public class EntityHandle_IKControl_HandPreset : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("F缓冲：到默认实体缓冲")]
    public class EntityHandle_Cache_CacheEntityToMain : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("F缓冲：到指定实体缓冲")]
    public class EntityHandle_Cache_CacheEntityToWhich : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("F缓冲：到默认坐标缓冲")]
    public class EntityHandle_Cache_CachePosToMain : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("F缓冲：到指定坐标缓冲")]
    public class EntityHandle_Cache_CachePosToWhich : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("G反相：逐渐看向")]
    public class EntityHandle_Inverse_LookAt : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("G反相：接近")]
    public class EntityHandle_Inverse_Approach : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("G反相：瞄准发射")]
    public class EntityHandle_Inverse_AimFire : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("G反相：处决")]
    public class EntityHandle_Inverse_Execute : IReleasablePointerOnlyByEntityYarnEntity
    {
        public object Pick(Entity by = null, Entity yarn = null, EntityState_Skill on = null)
        {
            if (by != null)
            {
                //LinkForEntityAttackEntity 备忘录
            }
            return 5;
        }


    }
    #endregion
    /*创建筛选器*/

    [Serializable, TypeRegistryItem("目标筛选链(自己出发，获得实体列表)")]
    public class SomeEntitySelectorFromSelf : PointerPackForDynamicChain<List<Entity>, IPointerForSomeEntityByEntityYarnEntity, IReleasablePointerChain, IPointerForSomeEntityBySomeEntityYarnEntity>
        , IPointerForSomeEntity_Only
    {
        public override IPointerForSomeEntityByEntityYarnEntity head => head_;

        [LabelText("链的起点(从自己出发)",SdfIconType.BoxArrowInRight),PropertyOrder(-1), SerializeReference,GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public IReleasablePointerForSomeEntityByEntityYarnEntity head_ = new ReleasablePointer_EntitySelf();
        [LabelText("链的终点(输出应用目标)",SdfIconType.BoxArrowRight), PropertyOrder(1), SerializeReference, GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCatcher")] public IReleasablePointerForSomeEntityBySomeEntityYarnEntity end_ = new ReleasablePointer_SelectTagsAndDistance();
        public override IPointerForSomeEntityBySomeEntityYarnEntity end => end_;

        //指向
        List<Entity> IPointer<List<Entity>, object, object, object>.Pick(object by, object yarn, object on)
        {
            return Pick(by,yarn,on);
        }
    }

    [Serializable,TypeRegistryItem("目标执行链(遍历筛选的实体列表")]
    public class EntityHandle
    {
        [SerializeReference]
        public List<IReleasablePointerOnlyByEntityYarnEntity> handles = new List<IReleasablePointerOnlyByEntityYarnEntity>();
    }


}
