using DG.Tweening;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using static ES.ClipStateMachine_CrashDodge;
using static UnityEngine.UIElements.UxmlAttributeDescription;

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
    public interface IReleasablePointerOnItemChain : IPointerChain
    {

    }


    #endregion


    public interface IPointerOnlyByEntityYarnEntityOnItem : IPointer<object, Entity, Entity, Item>
    {

    }
    //释放技能本质上是一个对Entity的遍历 这个是 应用器
    public interface IReleasablePointerOnlyByEntityYarnEntityOnItem : IPointerOnlyByEntityYarnEntityOnItem
    {

        object IPointer.Pick(object a, object b, object c)
        {
            return Pick(a as Entity, b as Entity, c as Item);
        }
    }
    /*从一个实体获得多个实体*/
    public interface IPointerForSomeEntityByEntityYarnEntityOnItem : IPointerChainAny<List<Entity>, Entity, Entity, Item>
    {
        //by 被操作 yarn 发起人 back 最终目的 on 技能
    }
    /*从多个实体获得多个实体*/
    public interface IPointerForSomeEntityBySomeEntityYarnEntityOnItem : IPointerChainAny<List<Entity>, List<Entity>, Entity, Item>
    {

    }
    /*释放专用：单实体变多实体*/
    public interface IReleasablePointerOnItemForSomeEntityByEntityYarnEntityOnItem : IPointerForSomeEntityByEntityYarnEntityOnItem, IReleasablePointerOnItemChain
    {
        object IPointer.Pick(object a, object b, object c)
        {
            if (a is Entity e)
            {
                return Pick(e, b as Entity, c as Item);
            }
            else if (a is List<Entity> es)
            {
                return Pick(es.First(), b as Entity, c as Item);
            }
            return null;
        }
    }
    /*释放专用：多实体变多实体*/



    #region 操作单个对象
    /*[Serializable, TypeRegistryItem("A0扩展：延迟处理")]
    public class EntityHandle_OnItem_Expand_Delay : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("延迟处理"),SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 0.25f };
        [LabelText("处理对象"), SerializeReference]
        public IReleasablePointerOnItemForSomeEntityByEntityYarnEntityOnItem handle;
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (handle == null) return null;
            float delay = float_Only?.Pick() ?? 0.25f;
     
            if (by != null)
            {
                var use= DOTween.Sequence();
                use.AppendInterval(delay);
                use.AppendCallback(() => { handle?.Pick(by,yarn,on); });
                
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("A0扩展：概率处理")]
    public class EntityHandle_OnItem_Expand_P : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("触发概率"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 0.25f };
        [LabelText("处理对象"), SerializeReference]
        public IReleasablePointerOnlyByEntityYarnEntityOnItem handle;
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (handle == null) return null;
            float P = float_Only?.Pick() ?? 0.25f;
            if (UnityEngine.Random.value < P)
            {
                if (by != null)
                {
                    handle?.Pick(by, yarn, on);
                    //Link_EntityAttackEntityTruely 备忘录
                }
            }
           
            return 5;
        }


    }

    [Serializable, TypeRegistryItem("A0扩展：交付到碰撞<实体>发生处理(Col)")]
    public class EntityHandle_OnItem_Expand_SubMitAsCollider : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {

        [LabelText("触发选项")]
        public OLDEnumCollect.HandleOnWhoEntityColOption option = OLDEnumCollect.HandleOnWhoEntityColOption.byColOnYarnLaucher;
        [LabelText("提交处理的内容"), SerializeReference]
        public IReleasablePointerOnlyByEntityYarnEntityOnItem handle;
        [LabelText("处理碰撞的坐标")]
        public OLDEnumCollect.HandleCacheOption handlePos;
        [LabelText("输入自定义池名"), ShowIf("@handlePos==OLDEnumCollect.HandleCacheOption.ToSelfDefine")]
        public string selfDefine = "自定义池";
        [LabelText("提交后最大触发次数")] public int ColTimes = 5;
        [LabelText("剩余可触发次数")] public int canCol = 0;
        [LabelText("已经提交")] public bool hasSubmitCol = false;
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (handle == null) return null;
            if (by == null || yarn == null) return null;
            if (!hasSubmitCol)
            {
                hasSubmitCol = true;
                canCol = ColTimes;
                by.OnColEntityHappen += OnCol;
            }
            if (canCol == 0)
            {
                if (by != null) by.OnColEntityHappen -= OnCol;
                hasSubmitCol = false;
            };
        
            
            return 5;
            void OnCol(Entity col, Vector3 pos)
            {
                canCol--;
                if(handlePos== OLDEnumCollect.HandleCacheOption.None)
                {

                }
                else if(handlePos== OLDEnumCollect.HandleCacheOption.ToMain)
                {
                    yarn.BaseDomain.Module_Cache?.CacheVector3.AddToQueue("Main",pos);
                }else if(handlePos== OLDEnumCollect.HandleCacheOption.ToSelf)
                {
                    yarn.BaseDomain.Module_Cache?.CacheVector3.AddToQueue("Main", pos);
                }
                else
                {
                    yarn.BaseDomain.Module_Cache?.CacheVector3.AddToQueue(selfDefine, pos);
                }
                if(option== OLDEnumCollect.HandleOnWhoEntityColOption.bySelectorYarnLauncher)
                {
                    handle.Pick(by,yarn,on);
                }
                else if (option == OLDEnumCollect.HandleOnWhoEntityColOption.bySelectorYarnColOn)
                {
                    handle.Pick(by, col, on);
                }
                else if (option == OLDEnumCollect.HandleOnWhoEntityColOption.byLauncherYarnSelector)
                {
                    handle.Pick(yarn, by, on);
                }
                else if (option == OLDEnumCollect.HandleOnWhoEntityColOption.ByLauncherYarnColOn)
                {
                    handle.Pick(yarn, col, on);
                }
                else if (option == OLDEnumCollect.HandleOnWhoEntityColOption.byColOnYarnLaucher)
                {
                    handle.Pick(col, yarn, on);
                }
                else if (option == OLDEnumCollect.HandleOnWhoEntityColOption.byColOnYarnSelector)
                {
                    handle.Pick(col, by, on);
                }
                
            }
        }

    }
    [Serializable, TypeRegistryItem("A0扩展：交付到触发碰撞<实体>发生处理(Tri)")]
    public class EntityHandle_OnItem_Expand_SubMitAsTrigger : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("触发单位选项")]
        public OLDEnumCollect.HandleOnWhoEntityColOption option = OLDEnumCollect.HandleOnWhoEntityColOption.byColOnYarnLaucher;
        [LabelText("提交处理的内容"), SerializeReference]
        public IReleasablePointerOnlyByEntityYarnEntityOnItem handle;
        [LabelText("处理触发碰撞的坐标")]
        public OLDEnumCollect.HandleCacheOption handlePos;
        [LabelText("输入自定义池名"), ShowIf("@handlePos==OLDEnumCollect.HandleCacheOption.ToSelfDefine")]
        public string selfDefine = "自定义池";
        [LabelText("提交后最大触发次数")] public int ColTimes = 5;
        [LabelText("剩余可触发次数")] public int canCol = 0;
        [LabelText("已经提交")] public bool hasSubmitCol = false;
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (handle == null) return null;
            if (by == null || yarn == null) return null;
            if (!hasSubmitCol)
            {
                hasSubmitCol = true;
                canCol = ColTimes;
                by.OnTriEntityHappen += OnTri;
            }
            if (canCol == 0)
            {
                if (by != null) by.OnTriEntityHappen -= OnTri;
                hasSubmitCol = false;
            };
            return -1;
            void OnTri(Entity col, Vector3 pos)
            {
                if (handlePos == OLDEnumCollect.HandleCacheOption.None)
                {

                }
                else if (handlePos == OLDEnumCollect.HandleCacheOption.ToMain)
                {
                    yarn.BaseDomain.Module_Cache?.CacheVector3.AddToQueue("Main", pos);
                }
                else if (handlePos == OLDEnumCollect.HandleCacheOption.ToSelf)
                {
                    yarn.BaseDomain.Module_Cache?.CacheVector3.AddToQueue("Main", pos);
                }
                else
                {
                    yarn.BaseDomain.Module_Cache?.CacheVector3.AddToQueue(selfDefine, pos);
                }
                if (option == OLDEnumCollect.HandleOnWhoEntityColOption.bySelectorYarnLauncher)
                {
                    handle.Pick(by, yarn, on);
                }
                else if (option == OLDEnumCollect.HandleOnWhoEntityColOption.bySelectorYarnColOn)
                {
                    handle.Pick(by, col, on);
                }
                else if (option == OLDEnumCollect.HandleOnWhoEntityColOption.byLauncherYarnSelector)
                {
                    handle.Pick(yarn, by, on);
                }
                else if (option == OLDEnumCollect.HandleOnWhoEntityColOption.ByLauncherYarnColOn)
                {
                    handle.Pick(yarn, col, on);
                }
                else if (option == OLDEnumCollect.HandleOnWhoEntityColOption.byColOnYarnLaucher)
                {
                    handle.Pick(col, yarn, on);
                }
                else if (option == OLDEnumCollect.HandleOnWhoEntityColOption.byColOnYarnSelector)
                {
                    handle.Pick(col, by, on);
                }

            }
        }

    }*/
    [Serializable, TypeRegistryItem("A0扩展：交付到销毁时(危险警告！)")]
    public class EntityHandle_OnItem_Expand_SubMitWhenItemExit : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("有效的销毁原因")]
        public EnumCollect.DestroyWhyOption options = EnumCollect.DestroyWhyOption.Normal;
        [LabelText("处理对象"), SerializeReference]
        public IReleasablePointerOnlyByEntityYarnEntityOnItem handle;
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (handle == null) return null;

            on.OnDestroyHappen += OnDes;
            return 5;
            void OnDes(Link_DestroyWhy why)
            {
                if ((why.options & options) != 0)
                {
                    handle?.Pick(by, yarn, on);
                }
            }
        }


    }
    [Serializable, TypeRegistryItem("A0扩展：瞬间操作列表")]
    public class EntityHandle_OnItem_Expand_HandleList : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {

        [LabelText("全部处理对象"), SerializeReference]
        public List<IReleasablePointerOnlyByEntityYarnEntityOnItem> handles = new List<IReleasablePointerOnlyByEntityYarnEntityOnItem>();
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (handles == null || handles.Count == 0) return null;

            foreach (var i in handles)
            {
                i.Pick(by, yarn, on);
            }

            return 5;
        }


    }
    [Serializable, TypeRegistryItem("A常规：扣除血量")]
    public class EntityHandle_OnItem_Damage : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("使用的攻击"), SerializeReference]
        public Damage ApplyDamage = new Damage();
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {

            if (by != null)
            {

                KeyValueMatchingUtility.ESLink.Global.GlobalLink_EntityAttackEntityTryStart(
                    new Link_EntityAttackEntityTryStart() { attacker = yarn, victim = by, damage = ApplyDamage });
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("A常规：获得Buff")]
    public class EntityHandle_OnItem_AddBuff : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("直接引用SO(可选)")] public BuffSoInfo useInfo;
        [LabelText("用键查询(可选)")] public KeyString_BuffUse key = new KeyString_BuffUse();
        [LabelText("使用-自定义Buff开始状态")] public bool IsSelfDefineStartBuffStatus = true;
        [LabelText("输入自定义Buff开始状态")] public BuffStatusTest BuffStatusTest = new BuffStatusTest() { duration = 10 };
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
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
    public class EntityHandle_OnItem_RemoveBuff : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("直接引用SO(可选)")] public BuffSoInfo useInfo;
        [LabelText("用键查询(可选)")] public KeyString_BuffUse key = new KeyString_BuffUse();

        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
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
                    if (use != null) KeyValueMatchingUtility.DataApply.Apply_Remove_BuffInfoToEntity(useInfo, by);
                }
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("B特殊:解除负面效果")]
    public class EntityHandle_OnItem_CancelNagativeEffect : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null)
            {
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("B特殊:解除正面效果")]
    public class EntityHandle_OnItem_CancelPositiveEffect : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null)
            {
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("B特殊:晕眩控制")]
    public class EntityHandle_OnItem_Controll : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null)
            {
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("C变换:瞬间刚体力")]
    public class EntityHandle_OnItem_AddRigidForce : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("自定义向量"), SerializeReference, InlineProperty] public IPointerForVector3_Only vector3 = new PointerForVector3_Direct() { vector = new Vector3(0, 0, -2) };
        [LabelText("空间模式")] public EnumCollect.PlacePosition placePos;
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null)
            {
                //Link_EntityAttackEntityTruely 备忘录
            }
            if (placePos == EnumCollect.PlacePosition.WorldSpace)
            {
                by.Rigid.AddForce(vector3?.Pick() ?? Vector3.up);
            }
            else if (placePos == EnumCollect.PlacePosition.SelfSpace)
            {
                by.Rigid.AddForce(by.transform.TransformDirection(vector3?.Pick() ?? Vector3.up));
            }
            else
            {
                Quaternion quaternion1 = Quaternion.LookRotation((yarn.transform.position - by.transform.position));
                by.Rigid.AddForce(quaternion1 * vector3?.Pick() ?? Vector3.up);
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("C变换:3轴缩放曲线")]
    public class EntityHandle_OnItem_ScaleCurve : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("X曲线")] public AnimationCurve scaleXCurve = AnimationCurve.Constant(0, 1, 1);
        [LabelText("Y曲线")] public AnimationCurve scaleYCurve = AnimationCurve.Constant(0, 1, 1);
        [LabelText("Z曲线")] public AnimationCurve scaleZCurve = AnimationCurve.Constant(0, 1, 1);
        [LabelText("持续时间"), SerializeReference, InlineProperty] public IPointerForFloat_Only durationP = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("恢复时间")] public float resumeTime = 0.25f;
        [LabelText("恢复到")] public Vector3 resumeTo = Vector3.one;
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null)
            {
                float start = Time.time;
                float duration = durationP?.Pick() ?? 1;
                Tween tween = DOTween.To(() => by.transform.localScale, x => { },
            Vector3.back, duration)
            .SetEase(Ease.Linear); // 设置为线性缓动，因为曲线已经控制了缩放效果

                // 在动画更新时，根据曲线计算当前缩放值
                tween.OnUpdate(() =>
                {
                    float time = Time.time - start;
                    float normalizedTime = time / duration;

                    // 分别计算X、Y、Z轴的缩放值
                    float scaleX = scaleXCurve.Evaluate(normalizedTime);
                    float scaleY = scaleYCurve.Evaluate(normalizedTime);
                    float scaleZ = scaleZCurve.Evaluate(normalizedTime);

                    // 设置物体的缩放
                    by.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                });
                tween.OnComplete(() => { by.transform.DOScale(resumeTo, resumeTime); });

            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("D主动：直接生成预制件在实体原地附近")]
    public class EntityHandle_OnItem_BirthAtHere : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("生成预制件")] public GameObject prefab;
        [LabelText("用对象池")] public bool UsePool = true;
        [LabelText("坐标向量"), SerializeReference] public IPointerForVector3_Only vector_only = new PointerForVector3_Direct();
        [LabelText("旋转"), SerializeReference] public IPointerForQuaternion_Only quaternion_Only = new PointerForQuaternion_Direc();
        [LabelText("坐标偏移模式")] public EnumCollect.PlacePosition placePos;
        [LabelText("方向偏移模式")] public EnumCollect.PlaceRotation placeRot;
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            Debug.Log("pick");
            if (by != null)
            {
                Debug.Log("pick2");
                Vector3 lookat = (yarn.transform.position - by.transform.position).normalized;
                GameObject gg = UsePool ? ES_PoolMaster.Instance.GetInPool(prefab) : MonoBehaviour.Instantiate(prefab);
                Vector3 vv = vector_only?.Pick() ?? default;
                Quaternion rot = quaternion_Only?.Pick() ?? default;
                if (placePos == EnumCollect.PlacePosition.WorldSpace)
                {
                    gg.transform.position = by.transform.position + vv;
                }
                else if (placePos == EnumCollect.PlacePosition.SelfSpace)
                {
                    gg.transform.position = by.transform.position + by.transform.TransformDirection(vv);
                }
                else
                {
                    Quaternion quaternion1 = Quaternion.LookRotation(lookat);
                    gg.transform.position = by.transform.position + quaternion1 * vv;
                }

                if (placeRot == EnumCollect.PlaceRotation.WorldSpace)
                {
                    gg.transform.rotation = rot;
                }
                else if (placeRot == EnumCollect.PlaceRotation.SelfSpace)
                {
                    gg.transform.rotation = (by.transform.rotation * rot);
                }
                else
                {
                    Quaternion quaternion1 = Quaternion.LookRotation(lookat);
                    gg.transform.rotation = quaternion1 * rot;
                }
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }


    }


    [Serializable, TypeRegistryItem("D主动：迅速闪身移动")]
    public class EntityHandle_OnItem_Dodge : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("闪身运动"), InlineProperty]
        public Applyable_CrashDodge ApplyCrash = new Applyable_CrashDodge()
        {
            baseOn = EnumCollect.ToDestionationBaseOn.ESCurveModule,
            duration = 0.25f,
            CoolDownNext = 0.25f,
            vector = new Vector3(0, 0, 0.8f),
            vectorHandle = EnumCollect.ToDestinationVectorSpace.SelfSpace
        };
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            Debug.Log(by + "闪身");
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
    public class EntityHandle_OnItem_AnimatorTrigger : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {

        [LabelText("触发器参数名")] public string name = "触发器参数";
        [LabelText("是否启用")] public bool SetOrReset = true;
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null)
            {
                if (SetOrReset) by.Anim.SetTrigger(name);
                else by.Anim.ResetTrigger(name);
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("E动画：过渡播放原生动画")]
    public class EntityHandle_OnItem_AnimatorCrossFade : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("过渡动画参数名")] public string name = "动画状态名参数";
        [LabelText("过渡时间")] public float tranTime = 0.2f;
        [LabelText("层级Index")] public int AnimLayer = 0;
        [LabelText("启用固定过渡时间")] public bool UseFixTran = false;
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null)
            {
                if (UseFixTran)
                {
                    by.Anim.CrossFadeInFixedTime(name, tranTime, AnimLayer);
                }
                else
                {
                    by.Anim.CrossFade(name, tranTime, AnimLayer);
                }
            }
            return 5;
        }


    }



    [Serializable, TypeRegistryItem("F缓冲：到默认实体缓冲")]
    public class EntityHandle_OnItem_Cache_CacheEntityToMain : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("成功概率"), SerializeReference] public IPointerForFloat_Only pointerForFloat = new PointerForFloat_DirectClamp01() { @float = 1 };
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null)
            {
                float f = pointerForFloat?.Pick() ?? 0.5f;
                if (UnityEngine.Random.value < f)
                {
                    yarn.BaseDomain.Module_Cache?.CacheEntity.AddToQueue("Main", by);
                }
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("F缓冲：到指定实体缓冲")]
    public class EntityHandle_OnItem_Cache_CacheEntityToWhich : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("缓冲名")] public string cacheName = "自定义名字";
        [LabelText("成功概率"), SerializeReference] public IPointerForFloat_Only pointerForFloat = new PointerForFloat_DirectClamp01() { @float = 1 };
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {

            if (by != null)
            {
                float f = pointerForFloat?.Pick() ?? 0.5f;
                if (UnityEngine.Random.value < f)
                {

                    yarn.BaseDomain.Module_Cache?.CacheEntity.AddToQueue(cacheName, by);
                }
            }
            return 5;
        }
    }

    [Serializable, TypeRegistryItem("F缓冲：到默认坐标缓冲")]
    public class EntityHandle_OnItem_Cache_CachePosToMain : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("成功概率"), SerializeReference] public IPointerForFloat_Only pointerForFloat = new PointerForFloat_DirectClamp01() { @float = 1 };
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null)
            {
                float f = pointerForFloat?.Pick() ?? 0.5f;
                if (UnityEngine.Random.value < f)
                {
                    by.BaseDomain.Module_Cache?.CacheVector3.AddToQueue("Main", by.transform.position);
                }
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("F缓冲：到指定坐标缓冲")]
    public class EntityHandle_OnItem_Cache_CachePosToWhich : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("缓冲名")] public string cacheName = "自定义名字";
        [LabelText("成功概率"), SerializeReference] public IPointerForFloat_Only pointerForFloat = new PointerForFloat_DirectClamp01() { @float = 1 };
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null)
            {
                float f = pointerForFloat?.Pick() ?? 0.5f;
                if (UnityEngine.Random.value < f)
                {
                    by.BaseDomain.Module_Cache?.CacheVector3.AddToQueue(cacheName, by.transform.position);
                }
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("G反相：逐渐看向")]
    public class EntityHandle_OnItem_Inverse_LookAt : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("多久能完全面向")] public float faceTime = 0.2f;
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null)
            {
                Vector3 vv = by.transform.position - yarn.transform.position;
                Quaternion end = Quaternion.LookRotation(Vector3.ProjectOnPlane(vv, yarn.transform.up), yarn.transform.up);

                Tween use = yarn.transform.DORotateQuaternion(end, faceTime);


                /* var to = yarn.transform.InverseTransformDirection(vv);
                 var y = yarn.BaseDomain.Module_3DMotion;

                 y.banSource.Add(this);*/

            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("G反相：快速接近")]
    public class EntityHandle_OnItem_Inverse_Approach : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {

        [LabelText("默认持续时间")] public float duration = 0.5f;
        [LabelText("默认最大距离")] public float maxDistance = 6;
        [LabelText("启用高度自定义特殊向量？")] public bool UseSelfDefineVector = false;
        [SerializeReference, LabelText("自定义向量"), ShowIf("UseSelfDefineVector")] public IPointerForVector3_Only selfVector = new PointerForVector3_Direct() { vector = Vector3.forward * 5 };
        [LabelText("路径方式")] public EnumCollect.ToDestinationPath pathType;
        private Applyable_CrashDodge defaultDodge = new Applyable_CrashDodge()
        {
            baseOn = EnumCollect.ToDestionationBaseOn.ESCurveModule,
            duration = 0.5f,
            vectorHandle = EnumCollect.ToDestinationVectorSpace.Target,
            CoolDownNext = 0.1f,
            vector = Vector3.forward,
        };
        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null)
            {

                var crash = yarn.StateMachineDomain.Module_CrashDodge;
                if (crash != null)
                {
                    defaultDodge.duration = duration;
                    defaultDodge.pathType = pathType;
                    if (UseSelfDefineVector)
                    {
                        defaultDodge.vector = selfVector?.Pick() ?? by.transform.position;
                    }
                    else
                    {
                        defaultDodge.vector = by.transform.position;
                    }
                    crash.TryAddCrashDodge(ref defaultDodge);
                }
                /* var to = yarn.transform.InverseTransformDirection(vv);
                 var y = yarn.BaseDomain.Module_3DMotion;

                 y.banSource.Add(this);*/
            }
            return 5;
        }
    }


    [Serializable, TypeRegistryItem("H场景：播放音效(单个)")]
    public class EntityHandle_OnItem_PlaySound : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("使用的音效")] public AudioClip audioClip;
        [LabelText("音量"), SerializeReference] public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("是否播放为空间音效")] public bool playAs3D = false;
        [LabelText("没空间音频源是否强制创建")] public bool ForceCrate = false;

        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null)
            {
                GameCenterManager.Instance.AudioMaster.PlayDirect_Sound_OneShot(audioClip, float_Only?.Pick() ?? 0.8f);
            }
            return 5;
        }
    }

    [Serializable, TypeRegistryItem("H场景：播放音效(随机)")]
    public class EntityHandle_OnItem_PlayOneOfSounds : IReleasablePointerOnlyByEntityYarnEntityOnItem
    {
        [LabelText("使用的音效")] public AudioClip[] audioClips;
        [LabelText("音量"), SerializeReference] public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("是否播放为空间音效")] public bool playAs3D = false;
        [LabelText("没空间音频源是否强制创建")] public bool ForceCrate = false;

        public object Pick(Entity by = null, Entity yarn = null, Item on = null)
        {
            if (by != null && audioClips?.Length > 0)
            {
                var oneOf = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
                GameCenterManager.Instance.AudioMaster.PlayDirect_Sound_OneShot(oneOf, float_Only?.Pick() ?? 0.8f);
            }
            return 5;
        }
    }
    #endregion
    /*创建筛选器*/


    [Serializable, TypeRegistryItem("目标执行列(飞行物专属)")]
    public class EntityHandleOfFlyingItem
    {

        [SerializeReference, LabelText("触发/碰撞时对命中实体施加效果(效果 )")]
        public List<IReleasablePointerOnlyByEntityYarnEntityOnItem> handles_ = new List<IReleasablePointerOnlyByEntityYarnEntityOnItem>();
        [FoldoutGroup("小效果集合")]
        [LabelText("销毁时播放音效条件")] public EnumCollect.DestroyWhyOption optionForPlaySound = EnumCollect.DestroyWhyOption.Normal;
        [FoldoutGroup("小效果集合")]
        [LabelText("销毁时播放音效")] public AudioClip OnDesPlaySound;


        [FoldoutGroup("小效果集合")]
        [LabelText("销毁时生成物体条件")] public EnumCollect.DestroyWhyOption optionForDesBirth = EnumCollect.DestroyWhyOption.Normal;
        [FoldoutGroup("小效果集合")]
        [LabelText("销毁时生成物体")] public GameObject OnDesBirth;
    }

    /*  [Serializable, TypeRegistryItem("坐标执行列(使用一系列坐标完成功能)")]
      public class EntityHandle_OnItem
      {
          [SerializeReference]
          public List<IReleasablePointerOnItemOnlyByEntityYarnEntityOn> handles_ = new List<IReleasablePointerOnItemOnlyByEntityYarnEntityOn>();
      }*/

}
