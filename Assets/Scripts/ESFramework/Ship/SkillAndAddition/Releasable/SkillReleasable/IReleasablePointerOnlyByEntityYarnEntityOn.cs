using DG.Tweening;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public interface IPointerOnlyByEntityYarnEntityOnSkill : IPointer<object, Entity, Entity, EntityState_Skill>
    {

    }
    //释放技能本质上是一个对Entity的遍历 这个是 应用器
    public interface IReleasablePointerOnlyByEntityYarnEntityOn : IPointerOnlyByEntityYarnEntityOnSkill
    {

        object IPointer.Pick(object a, object b, object c)
        {
            return Pick(a as Entity, b as Entity, c as EntityState_Skill);
        }
    }
    /*从一个实体获得多个实体*/
    public interface IPointerForSomeEntityByEntityYarnEntityOnSkill : IPointerChainAny<List<Entity>, Entity, Entity, EntityState_Skill>
    {
        //on 被操作 from 发起人 back 最终目的 with 技能
    }
    /*从多个实体获得多个实体*/
    public interface IPointerForSomeEntityBySomeEntityYarnEntityOnSKill : IPointerChainAny<List<Entity>, List<Entity>, Entity, EntityState_Skill>
    {

    }
    /*释放专用：单实体变多实体*/
    public interface IReleasablePointerForSomeEntityByEntityYarnEntityOnSKill : IPointerForSomeEntityByEntityYarnEntityOnSkill, IReleasablePointerChain
    {
        object IPointer.Pick(object a, object b, object c)
        {
            if (a is Entity e)
            {
                return Pick(e, b as Entity, c as EntityState_Skill);
            }
            else if (a is List<Entity> es)
            {
                return Pick(es.First(), b as Entity, c as EntityState_Skill);
            }
            return null;
        }
    }
    /*释放专用：多实体变多实体*/
    public interface IReleasablePointerForSomeEntityBySomeEntityYarnEntityOnSKill : IPointerForSomeEntityBySomeEntityYarnEntityOnSKill, IReleasablePointerChain
    {
        object IPointer.Pick(object a, object b, object c)
        {
            if (a is List<Entity> es)
            {
                return Pick(es, b as Entity, c as EntityState_Skill);
            }
            else if (a is Entity e)
            {
                return Pick(new List<Entity>() { e }, b as Entity, c as EntityState_Skill);
            }
            return null;
        }
    }


    #region 从一个实体获得多个实体的Class
    [Serializable, TypeRegistryItem("单实体=>>多实体:只有我自己的列表")]
    public class ReleasablePointer_EntitySelf : IReleasablePointerForSomeEntityByEntityYarnEntityOnSKill
    {
        public List<Entity> Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            return new List<Entity>() { on };
        }


    }
    [Serializable, TypeRegistryItem("单实体=>>多实体:看到的目标")]
    public class ReleasablePointer_EntityVision : IReleasablePointerForSomeEntityByEntityYarnEntityOnSKill
    {
        [LabelText("是否立刻重新刷新目标")] public bool ReFreshRightly = false;
        [LabelText("最多目标"), SerializeReference] public IPointerForInt_Only max = new PointerForInt_Direct() { int_ = 5 };
        public List<Entity> Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            return KeyValueMatchingUtility.ESBack.ForEntityBack.GetEntityVision(on, max?.Pick() ?? 5, ReFreshRightly);
        }
    }
    [Serializable, TypeRegistryItem("单实体=>>多实体:我身边的队友")]
    public class ReleasablePointer_EntityAroundFriend : IReleasablePointerForSomeEntityByEntityYarnEntityOnSKill
    {
        [SerializeReference, LabelText("查找范围半径")]
        public IPointerForFloat_Only distanceR = new PointerForFloat_Direct() { float_ = 5 };
        [SerializeReference, LabelText("是否是相对坐标偏移")]
        public bool IsRele = true;
        [SerializeReference, LabelText("向量偏移量")]
        public IPointerForVector3_Only ReleV3 = new PointerForVector3_Direct() { vector = default };
        [LabelText("包括我自己？")]
        public bool containsThis = true;
        public List<Entity> Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            List<Entity> es = new List<Entity>();
            if (containsThis) es.Add(on);
            var friends = KeyValueMatchingUtility.ESBack.ForEntityBack.GetEntityAroundFriend(
                on, distanceR?.Pick() ?? 5,
                IsRele ? on.transform.TransformPoint(ReleV3?.Pick() ?? default) : ReleV3?.Pick() ?? default);
            es.AddRange(friends);
            return es;
        }
    }
    [Serializable, TypeRegistryItem("单实体=>>多实体:我身边的人")]
    public class ReleasablePointer_EntityAround : IReleasablePointerForSomeEntityByEntityYarnEntityOnSKill
    {
        [SerializeReference, LabelText("查找范围半径")]
        public IPointerForFloat_Only distanceR = new PointerForFloat_Direct() { float_ = 5 };
        [SerializeReference, LabelText("是否是相对坐标偏移")]
        public bool IsRele = true;
        [SerializeReference, LabelText("向量偏移量")]
        public IPointerForVector3_Only ReleV3 = new PointerForVector3_Direct() { vector = default };
        [LabelText("包括我自己？")]
        public bool containsThis = true;
        public List<Entity> Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            List<Entity> es = new List<Entity>();
            if (containsThis) es.Add(on);
            var friends = KeyValueMatchingUtility.ESBack.ForEntityBack.GetEntityAround(
                on, distanceR?.Pick() ?? 5,
                IsRele ? on.transform.TransformPoint(ReleV3?.Pick() ?? default) : ReleV3?.Pick() ?? default);
            es.AddRange(friends);
            Debug.Log("筛选到" + es.Count);
            return es;
        }
    }
    [Serializable, TypeRegistryItem("单实体=>>多实体:来自我的默认缓冲池")]
    public class ReleasablePointer_EntityCacheMainTarget : IReleasablePointerForSomeEntityByEntityYarnEntityOnSKill
    {

        [LabelText("使用后就清理")]
        public bool UseAndClear = true;
        public List<Entity> Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            List<Entity> es = KeyValueMatchingUtility.ESBack.ForEntityBack.GetEntityTargetEntityCache(on, useAndClear: UseAndClear);
            return es;
        }
    }
    [Serializable, TypeRegistryItem("单实体=>>多实体:来自实体的某个缓冲池")]
    public class ReleasablePointer_EntityCacheWhichTarget : IReleasablePointerForSomeEntityByEntityYarnEntityOnSKill
    {
        [LabelText("使用后就清理")]
        public bool UseAndClear = true;
        [LabelText("哪一个缓冲池")]
        public string whichCache = "缓冲池标记";
        public List<Entity> Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            List<Entity> es = KeyValueMatchingUtility.ESBack.ForEntityBack.GetEntityTargetEntityCache(on, whichCache, useAndClear: UseAndClear);
            return es;
        }
    }
    [Serializable, TypeRegistryItem("单实体=>>多实体:来自本技能的缓冲池")]
    public class ReleasablePointer_EntityCacheSelfSKill : IReleasablePointerForSomeEntityByEntityYarnEntityOnSKill
    {
        [LabelText("使用后就清理")]
        public bool UseAndClear = true;
        public List<Entity> Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            List<Entity> es = with.SelfCacheEntites.ToList();
            if (UseAndClear)
            {
                with.SelfCacheEntites.Clear();
            }
            return es;
        }
    }
    #endregion

    #region 从多个实体获得多个实体
    [Serializable, TypeRegistryItem("多实体=>>多实体:筛选两大Tag和距离")]
    public class ReleasablePointer_SelectTagsAndDistance : IReleasablePointerForSomeEntityBySomeEntityYarnEntityOnSKill
    {
        [LabelText("筛选ESTag(置空不筛选)")]
        public ESTag[] GiveTags;
        [LabelText("筛选Tag(置空不筛选)")]
        public PointerForStringList_Tag tags = new PointerForStringList_Tag();
        [LabelText("最大距离(为负不筛选)")]
        public float r = 100;
        public List<Entity> Pick(List<Entity> on = null, Entity launcher = null, EntityState_Skill with = null)
        {
            List<Entity> end = new List<Entity>(on.Count / 2 + 1);
            List<string> useTags = tags?.Pick() ?? null;

            foreach (var i in on)
            {

                if (i == null) continue;
                if (r < 0 || Vector3.Distance(launcher.transform.position, i.transform.position) < r)
                {

                }
                else
                {
                    continue;
                }

                if (useTags == null || useTags.Count == 0)
                {

                }
                else if (useTags.Contains(i.gameObject.tag))
                {

                }
                else
                {
                    continue;
                }
                if (GiveTags == null || GiveTags.Length == 0)
                {

                }
                else
                {
                    bool use = false;
                    foreach (var my in GiveTags)
                    {
                        if (i.ESTagsC.Tags.Contains(my))
                        {

                            use = true;
                            break;
                        }
                    }
                    if (!use) continue;
                }
                end.Add(i);
            }
            return end;
            //  Debug.Log("最后一步" + on+"and"+launcher);

        }


    }


    #endregion

    #region 操作单个对象
    [Serializable, TypeRegistryItem("A0扩展：延迟处理")]
    public class EntityHandle_Expand_Delay : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("延迟处理"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 0.25f };
        [LabelText("处理对象"), SerializeReference]
        public IReleasablePointerOnlyByEntityYarnEntityOn handle;
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (handle == null) return null;
            float delay = float_Only?.Pick() ?? 0.25f;

            if (on != null)
            {
                var use = DOTween.Sequence();
                use.AppendInterval(delay);
                use.AppendCallback(() => { handle?.Pick(with,from,with); });
                with.OnExit += (f) => { use.Kill(); };
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("A0扩展：概率处理")]
    public class EntityHandle_Expand_P : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("触发概率"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 0.25f };
        [LabelText("处理对象"), SerializeReference]
        public IReleasablePointerOnlyByEntityYarnEntityOn handle;
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (handle == null) return null;
            float P = float_Only?.Pick() ?? 0.25f;
            if (UnityEngine.Random.value < P)
            {
                if (on != null)
                {
                    handle?.Pick(with,from,with);
                    //Link_EntityAttackEntityTruely 备忘录
                }
            }

            return 5;
        }


    }

    [Serializable, TypeRegistryItem("A0扩展：交付到碰撞<实体>发生处理(Col)")]
    public class EntityHandle_Expand_SubMitAsCollider : IReleasablePointerOnlyByEntityYarnEntityOn
    {

        [LabelText("触发选项")]
        public EnumCollect.HandleOnWhoEntityColOption option = EnumCollect.HandleOnWhoEntityColOption.byColOnYarnLaucher;
        [LabelText("提交处理的内容"), SerializeReference]
        public IReleasablePointerOnlyByEntityYarnEntityOn handle;
        [LabelText("处理碰撞的坐标")]
        public EnumCollect.HandleCacheOption handlePos;
        [LabelText("输入自定义池名"), ShowIf("@handlePos==EnumCollect.HandleCacheOption.ToSelfDefine")]
        public string selfDefine = "自定义池";
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (handle == null) return null;
            if (on == null || from == null) return null;
            on.OnColEntityHappen += OnCol;
            with.OnExit += (f) => { if (on != null) on.OnColEntityHappen -= OnCol; };
            return 5;
            void OnCol(Entity col, Vector3 pos)
            {
                if (handlePos == EnumCollect.HandleCacheOption.None)
                {

                }
                else if (handlePos == EnumCollect.HandleCacheOption.ToMain)
                {
                    from.BaseDomain.Module_Cache?.CacheVector3.AddToQueue("Main", pos);
                }
                else if (handlePos == EnumCollect.HandleCacheOption.ToSelf)
                {
                    with.SelfCacheVector3.Add(pos);
                }
                else
                {
                    from.BaseDomain.Module_Cache?.CacheVector3.AddToQueue(selfDefine, pos);
                }
                if (option == EnumCollect.HandleOnWhoEntityColOption.bySelectorYarnLauncher)
                {
                    handle.Pick(with,from,with);
                }
                else if (option == EnumCollect.HandleOnWhoEntityColOption.bySelectorYarnColOn)
                {
                    handle.Pick(on, col, with);
                }
                else if (option == EnumCollect.HandleOnWhoEntityColOption.byLauncherYarnSelector)
                {
                    handle.Pick(from, on, with);
                }
                else if (option == EnumCollect.HandleOnWhoEntityColOption.ByLauncherYarnColOn)
                {
                    handle.Pick(from, col, with);
                }
                else if (option == EnumCollect.HandleOnWhoEntityColOption.byColOnYarnLaucher)
                {
                    handle.Pick(col, from, with);
                }
                else if (option == EnumCollect.HandleOnWhoEntityColOption.byColOnYarnSelector)
                {
                    handle.Pick(col, on, with);
                }

            }
        }

    }
    [Serializable, TypeRegistryItem("A0扩展：交付到触发碰撞<实体>发生处理(Tri)")]
    public class EntityHandle_Expand_SubMitAsTrigger : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("触发单位选项")]
        public EnumCollect.HandleOnWhoEntityColOption option = EnumCollect.HandleOnWhoEntityColOption.byColOnYarnLaucher;
        [LabelText("提交处理的内容"), SerializeReference]
        public IReleasablePointerOnlyByEntityYarnEntityOn handle;
        [LabelText("处理触发碰撞的坐标")]
        public EnumCollect.HandleCacheOption handlePos;
        [LabelText("输入自定义池名"), ShowIf("@handlePos==EnumCollect.HandleCacheOption.ToSelfDefine")]
        public string selfDefine = "自定义池";
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (handle == null) return null;
            if (on == null || from == null) return null;
            on.OnTriEntityHappen += OnCol;
            with.OnExit += (f) => { if (on != null) on.OnTriEntityHappen -= OnCol; };
            return 5;
            void OnCol(Entity col, Vector3 pos)
            {
                if (handlePos == EnumCollect.HandleCacheOption.None)
                {

                }
                else if (handlePos == EnumCollect.HandleCacheOption.ToMain)
                {
                    from.BaseDomain.Module_Cache?.CacheVector3.AddToQueue("Main", pos);
                }
                else if (handlePos == EnumCollect.HandleCacheOption.ToSelf)
                {
                    with.SelfCacheVector3.Add(pos);
                }
                else
                {
                    from.BaseDomain.Module_Cache?.CacheVector3.AddToQueue(selfDefine, pos);
                }
                if (option == EnumCollect.HandleOnWhoEntityColOption.bySelectorYarnLauncher)
                {
                    handle.Pick(with,from,with);
                }
                else if (option == EnumCollect.HandleOnWhoEntityColOption.bySelectorYarnColOn)
                {
                    handle.Pick(on, col, with);
                }
                else if (option == EnumCollect.HandleOnWhoEntityColOption.byLauncherYarnSelector)
                {
                    handle.Pick(from, on, with);
                }
                else if (option == EnumCollect.HandleOnWhoEntityColOption.ByLauncherYarnColOn)
                {
                    handle.Pick(from, col, with);
                }
                else if (option == EnumCollect.HandleOnWhoEntityColOption.byColOnYarnLaucher)
                {
                    handle.Pick(col, from, with);
                }
                else if (option == EnumCollect.HandleOnWhoEntityColOption.byColOnYarnSelector)
                {
                    handle.Pick(col, on, with);
                }

            }
        }

    }
    [Serializable, TypeRegistryItem("A0扩展：交付到技能结束时(危险警告！)")]
    public class EntityHandle_Expand_SubMitWhenSkillExit : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("退出有效的时间范围")]
        [MinMaxSlider(0, 5)]
        public Vector2 range = new Vector2(0, 1);
        [LabelText("处理对象"), SerializeReference]
        public IReleasablePointerOnlyByEntityYarnEntityOn handle;
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (handle == null) return null;

            with.OnExit += (f) => { Debug.Log(f); if (f > range.x && f < range.y) { handle?.Pick(with,from,with); } };



            return 5;
        }


    }
    [Serializable, TypeRegistryItem("A0扩展：瞬间操作列表")]
    public class EntityHandle_Expand_HandleList : IReleasablePointerOnlyByEntityYarnEntityOn
    {

        [LabelText("全部处理对象"), SerializeReference]
        public List<IReleasablePointerOnlyByEntityYarnEntityOn> handles = new List<IReleasablePointerOnlyByEntityYarnEntityOn>();
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (handles == null || handles.Count == 0) return null;

            foreach (var i in handles)
            {
                i.Pick(with,from,with);
            }

            return 5;
        }


    }

    [Serializable, TypeRegistryItem("A常规：扣除血量")]
    public class EntityHandle_Damage : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("使用的攻击"), SerializeReference]
        public Damage ApplyDamage = new Damage();
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {

            if (on != null)
            {
                KeyValueMatchingUtility.ESLink.Global.GlobalLink_EntityAttackEntityTryStart(
                     new Link_EntityAttackEntityTryStart() { attacker = from, victim = on, damage = ApplyDamage });
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("A常规：获得Buff")]
    public class EntityHandle_AddBuff : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("直接引用SO(可选)")] public BuffSoInfo useInfo;
        [LabelText("用键查询(可选)")] public KeyString_BuffUse key = new KeyString_BuffUse();
        [LabelText("使用-自定义Buff开始状态")] public bool IsSelfDefineStartBuffStatus = true;
        [LabelText("输入自定义Buff开始状态")] public BuffStatusTest BuffStatusTest = new BuffStatusTest() { duration = 10 };
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                var use = useInfo;

                if (use == null)
                {
                    use = KeyValueMatchingUtility.DataInfoPointer.PickBuffSoInfoByKey(key.Key());
                }
                if (use != null)
                {
                    KeyValueMatchingUtility.DataApply.ApplyBuffInfoToEntity(use, on, IsSelfDefineStartBuffStatus ? BuffStatusTest : null);

                }
            }
            return null;
        }


    }
    [Serializable, TypeRegistryItem("A常规:移除Buff")]
    public class EntityHandle_RemoveBuff : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("直接引用SO(可选)")] public BuffSoInfo useInfo;
        [LabelText("用键查询(可选)")] public KeyString_BuffUse key = new KeyString_BuffUse();

        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {

            if (on != null)
            {
                if (on != null)
                {
                    var use = useInfo;
                    if (use == null)
                    {
                        use = KeyValueMatchingUtility.DataInfoPointer.PickBuffSoInfoByKey(key.Key());
                    }
                    if (use != null) KeyValueMatchingUtility.DataApply.Apply_Remove_BuffInfoToEntity(useInfo, on);
                }
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("B特殊:解除负面效果")]
    public class EntityHandle_CancelNagativeEffect : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("B特殊:解除正面效果")]
    public class EntityHandle_CancelPositiveEffect : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("B特殊:晕眩控制")]
    public class EntityHandle_Controll : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("C变换:瞬间刚体力")]
    public class EntityHandle_AddRigidForce : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("自定义向量"), SerializeReference, InlineProperty] public IPointerForVector3_Only vector3 = new PointerForVector3_Direct() { vector = new Vector3(0, 0, -2) };
        [LabelText("空间模式")] public EnumCollect.PlacePosition placePos;
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                //Link_EntityAttackEntityTruely 备忘录
            }
            if (placePos == EnumCollect.PlacePosition.WorldSpace)
            {
                on.Rigid.AddForce(vector3?.Pick() ?? Vector3.up);
            }
            else if (placePos == EnumCollect.PlacePosition.SelfSpace)
            {
                on.Rigid.AddForce(on.transform.TransformDirection(vector3?.Pick() ?? Vector3.up));
            }
            else
            {
                Quaternion quaternion1 = Quaternion.LookRotation((from.transform.position - on.transform.position));
                on.Rigid.AddForce(quaternion1 * vector3?.Pick() ?? Vector3.up);
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("C变换:3轴缩放曲线")]
    public class EntityHandle_ScaleCurve : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("X曲线")] public AnimationCurve scaleXCurve = AnimationCurve.Constant(0, 1, 1);
        [LabelText("Y曲线")] public AnimationCurve scaleYCurve = AnimationCurve.Constant(0, 1, 1);
        [LabelText("Z曲线")] public AnimationCurve scaleZCurve = AnimationCurve.Constant(0, 1, 1);
        [LabelText("持续时间"), SerializeReference, InlineProperty] public IPointerForFloat_Only durationP = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("恢复时间")] public float resumeTime = 0.25f;
        [LabelText("恢复到")] public Vector3 resumeTo = Vector3.one;
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                float start = Time.time;
                float duration = durationP?.Pick() ?? 1;
                Tween tween = DOTween.To(() => on.transform.localScale, x => { },
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
                    on.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                });
                tween.OnComplete(() => { on.transform.DOScale(resumeTo, resumeTime); });

                with.OnExit += (f) =>
                {
                    tween.Kill();
                    on.transform.DOScale(resumeTo, resumeTime);
                };
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("D主动：直接生成预制件在实体原地附近")]
    public class EntityHandle_BirthAtHere : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("生成预制件")] public GameObject prefab;
        [LabelText("用对象池")] public bool UsePool = true;
        [LabelText("坐标向量"), SerializeReference] public IPointerForVector3_Only vector_only = new PointerForVector3_Direct();
        [LabelText("旋转"), SerializeReference] public IPointerForQuaternion_Only quaternion_Only = new PointerForQuaternion_Direc();
        [LabelText("偏移相对于技能拥有者/目标实体")] public bool PosReleAsSkillLauncher = true;
        [LabelText("坐标偏移模式")] public EnumCollect.PlacePosition placePos;

        [LabelText("方向偏移模式")] public EnumCollect.PlaceRotation placeRot;
        [Title("飞行物专用效果")]
        [LabelText("是飞行物预制体")] public bool IsFlying = false;
        [LabelText("设置为攻击目标(改方向)"), ShowIfGroup("fly", VisibleIf = "@IsFlying")] public bool AsTargetDirect = true;
        [LabelText("移动方向对准面向"), ShowIfGroup("fly")] public bool moveDirecFollowForward = true;
        [LabelText("把on设置为攻击目标"), ShowIfGroup("fly")] public bool TargetAsBy = true;


        [LabelText("额外旋转偏移"), ShowIfGroup("fly")] public bool offsetRotAddition = false;
        [LabelText("补充设置：延迟停留"), ShowIfGroup("fly")] public float delay = 0;
        [LabelText("补充设置：速度加成(最小-1)"), ShowIfGroup("fly")] public float speedUp = 0;
        [LabelText("按面向方向偏移(可叠加设置为攻击目标)"), ShowIfGroup("fly"), SerializeReference]
        public IPointerForQuaternion_Only quaternion_Offset_ = new PointerForQuaternion_Direc() {  quaternion = Quaternion.identity };


        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {

            if (on != null)
            {
                Transform relePosOn = PosReleAsSkillLauncher ? from.transform : on.transform;
                Transform other = PosReleAsSkillLauncher ? on.transform : from.transform;

                Vector3 lookat = (other.transform.position - relePosOn.transform.position).normalized;
                GameObject gg = UsePool ? ES_PoolMaster.Instance.GetInPool(prefab) : MonoBehaviour.Instantiate(prefab);
                {
                    Vector3 vv = vector_only?.Pick() ?? default;
                    Quaternion rot = quaternion_Only?.Pick() ?? Quaternion.identity;
                    if (placePos == EnumCollect.PlacePosition.WorldSpace)
                    {
                        gg.transform.position = relePosOn.position + vv;
                    }
                    else if (placePos == EnumCollect.PlacePosition.SelfSpace)
                    {
                        gg.transform.position = relePosOn.position + relePosOn.TransformDirection(vv);
                    }
                    else
                    {
                        Quaternion quaternion1 = Quaternion.LookRotation(lookat);
                        gg.transform.position = relePosOn.position + quaternion1 * vv;
                    }

                    if (placeRot == EnumCollect.PlaceRotation.WorldSpace)
                    {
                        gg.transform.rotation = rot;
                    }
                    else if (placeRot == EnumCollect.PlaceRotation.SelfSpace)
                    {
                        Debug.Log("自身空间" + relePosOn.rotation + rot + relePosOn.rotation * rot + Quaternion.identity + relePosOn.rotation * Quaternion.identity);
                        gg.transform.rotation = (relePosOn.rotation * rot);
                    }
                    else
                    {
                        Quaternion quaternion1 = Quaternion.LookRotation(lookat);
                        gg.transform.rotation = quaternion1 * rot;
                    }
                }
                {
                    if (IsFlying)
                    {
                        Item ii = gg.GetComponent<Item>();
                        var fly = ii?.HurtableDomain.Module_Flying;
                        if (fly != null)
                        {
                            ii.AddIgnoreEntity(from);
                            if (moveDirecFollowForward)
                            {
                                fly.CurrentDirect = fly.TargetDirect = relePosOn.transform.forward;
                            }
                            if (AsTargetDirect)
                            {
                                fly.SetTarget(on);
                            }
                            fly.SpeedPerUp = speedUp;
                            fly.delayTime = delay;
                            if (offsetRotAddition)
                            {
                                Quaternion qq = quaternion_Offset_?.Pick() ?? default;
                                gg.transform.rotation *= qq;
                                fly.TargetDirect = qq * fly.TargetDirect;
                            }
                        }
                    }
                }
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }


    }


    [Serializable, TypeRegistryItem("D主动：迅速闪身移动")]
    public class EntityHandle_Dodge : IReleasablePointerOnlyByEntityYarnEntityOn
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
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            Debug.Log(on + "闪身");
            if (on != null)
            {
                var crash = on.StateMachineDomain.Module_CrashDodge;
                if (crash != null)
                {
                    crash.TryAddCrashDodge(ref ApplyCrash);
                }
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("E动画：强制原生动画触发器")]
    public class EntityHandle_AnimatorTrigger : IReleasablePointerOnlyByEntityYarnEntityOn
    {

        [LabelText("触发器参数名")] public string name = "触发器参数";
        [LabelText("是否启用")] public bool SetOrReset = true;
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                if (SetOrReset) on.Anim.SetTrigger(name);
                else on.Anim.ResetTrigger(name);
            }
            return 5;
        }


    }


    #region MC2扩展

    [Serializable, TypeRegistryItem("A0扩展：间隔操作列表")]
    public class EntityHandle_Expand_HandleList_WithInterval : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("间隔时间"), SerializeReference]
        public IPointerForFloat_Only interval = new PointerForFloat_Direct() { float_ = 0.15f };
        [LabelText("全部处理对象"), SerializeReference]
        public List<IReleasablePointerOnlyByEntityYarnEntityOn> handles = new List<IReleasablePointerOnlyByEntityYarnEntityOn>();
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (handles == null || handles.Count == 0) return null;
            Sequence sequence = DOTween.Sequence();

            foreach (var i in handles)
            {
                if (i == null) continue;
                sequence.AppendCallback(() => i.Pick(with,from,with));
                sequence.AppendInterval(interval?.Pick() ?? 0.14f);
            }
            with.OnExit += (f) => { sequence.Kill(); };
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("A0扩展：循环操作")]
    public class EntityHandle_Expand_HandleList_Loop : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("使用间隔时间？")]
        public bool useInterval = false;
        [LabelText("间隔时间"), SerializeReference]
        public IPointerForFloat_Only interval = new PointerForFloat_Direct() { float_ = 0.15f };
        [LabelText("间隔次数"), SerializeReference]
        public IPointerForInt_Only times = new PointerForInt_Direct() { int_ = 3 };
        [LabelText("需要循环的处理"), SerializeReference]
        public IReleasablePointerOnlyByEntityYarnEntityOn handle;
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (handle == null) return null;

            int times_ = times?.Pick() ?? 3;
            if (useInterval)
            {
                Sequence sequence = DOTween.Sequence();
                for (int i = 0; i < times_; i++)
                {
                    sequence.AppendCallback(() => handle.Pick(with,from,with));
                    sequence.AppendInterval(interval?.Pick() ?? 0.14f);
                }
                with.OnExit += (f) => { sequence.Kill(); };
            }
            else
            {
                for (int i = 0; i < times_; i++)
                {
                    handle.Pick(with,from,with);
                }
            }


            return 5;
        }


    }

    [Serializable, TypeRegistryItem("D主动：向本技能缓冲池闪身移动")]
    public class EntityHandle_MoveToWithCrash : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("使用就销毁")] public bool UseAndClear = true;
        [LabelText("首个/随机")] public bool FirstOrRandom = true;
        [LabelText("使用Lerp值")] public float lerpValue = 1;
        [LabelText("额外的Y增益"), SerializeReference] public IPointerForFloat_Only yAdding = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("闪身运动"), InlineProperty]
        public Applyable_CrashDodge ApplyCrash = new Applyable_CrashDodge()
        {
            baseOn = EnumCollect.ToDestionationBaseOn.ESCurveModule,
            duration = 0.25f,
            CoolDownNext = 0.25f,
            vector = new Vector3(0, 0, 0.8f),
            vectorHandle = EnumCollect.ToDestinationVectorSpace.Target
        };
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            Vector3 use = FirstOrRandom ? with.SelfCacheVector3.First() : with.SelfCacheVector3.First();
            if (UseAndClear) with.SelfCacheVector3.Clear();
            if (on != null)
            {
                var useA = ApplyCrash;
                useA.vector = Vector3.Lerp(on.transform.position, use, lerpValue) + yAdding?.Pick() * Vector3.up ?? Vector3.up;
                var crash = on.StateMachineDomain.Module_CrashDodge;
                if (crash != null)
                {
                    crash.TryAddCrashDodge(ref useA);
                }
            }
            return 5;
        }


    }

    [Serializable, TypeRegistryItem("D主动：格挡尝试发起的攻击")]
    public class EntityHandle_Block : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("持续时间")]
        public float sustainTime = 1.5f;
        [LabelText("生效音效")]
        public AudioClip triggerClip;
        [LabelText("额外效果触发条件-距离条件")]
        public float AdditionDistance = 5;
        [LabelText("额外效果触发条件-伤害原因")]
        public string NONE = "还没想好";
        [LabelText("对攻击者附加效果"), SerializeReference]
        public IReleasablePointerOnlyByEntityYarnEntityOn apply = null;
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            var use = DOTween.Sequence();
            on.OnTryBeAttack += OnTryBeAttack;
            use.AppendInterval(sustainTime);
            use.AppendCallback(() => { on.OnTryBeAttack -= OnTryBeAttack; Debug.Log("结束格挡"); });

            with.OnExit += (f) => { use.Complete(true); };
            void OnTryBeAttack(Entity who, Damage da)
            {
                da.canTrigger.Value -= 2;
                if (triggerClip != null)
                {
                    GameCenterManager.Instance.AudioMaster.PlaySoundByESObject(on, triggerClip, 0.75f);
                }
                if (apply != null)
                {
                    if (AdditionDistance > Vector3.Distance(who.transform.position, on.transform.position))
                    {
                        apply.Pick(who, on, with);
                    }
                }
                Debug.Log("格挡测试");
            }
            return 5;
        }


    }

    [Serializable, TypeRegistryItem("D主动：停止普通行动")]
    public class EntityHandle_StopSimpleMotion : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("持续时间")]
        public float sustainTime = 1.5f;

        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            var use = on.BaseDomain?.Module_AB_Motion;
            if (use != null) use.timeForStay = sustainTime;
            return 5;
        }


    }
    #endregion



    [Serializable, TypeRegistryItem("E动画：过渡播放原生动画")]
    public class EntityHandle_AnimatorCrossFade : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("过渡动画参数名")] public string name = "动画状态名参数";
        [LabelText("过渡时间")] public float tranTime = 0.2f;
        [LabelText("层级Index")] public int AnimLayer = 0;
        [LabelText("启用固定过渡时间")] public bool UseFixTran = false;
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                if (UseFixTran)
                {
                    on.Anim.CrossFadeInFixedTime(name, tranTime, AnimLayer);
                }
                else
                {
                    on.Anim.CrossFade(name, tranTime, AnimLayer);
                }
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("E动画：逼近层级权重")]
    public class EntityHandle_AnimatorLayerWeight : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("哪一层")] public int LayerIndex = 1;
        [LabelText("目标值")] public float targetWeight = 1;
        [LabelText("逼近时间")] public float lerpTime = 0.5f;
        [LabelText("多久退出？(默认随着技能)")] public float exitTime = 2;

        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                float start = on.Anim.GetLayerWeight(LayerIndex);
                var Use = DOTween.To(() => on.Anim.GetLayerWeight(LayerIndex), (to) => on.Anim.SetLayerWeight(LayerIndex, to), targetWeight, lerpTime);
                var delayExit = DOTween.Sequence();
                delayExit.AppendInterval(exitTime);
                delayExit.Append(DOTween.To(() => on.Anim.GetLayerWeight(LayerIndex), (to) => on.Anim.SetLayerWeight(LayerIndex, to), start, lerpTime));
                with.OnExit += (t) =>
                {
                    delayExit.Kill();
                    Use.Kill();
                    DOTween.To(() => on.Anim.GetLayerWeight(LayerIndex), (to) => on.Anim.SetLayerWeight(LayerIndex, to), start, lerpTime);
                };
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("E动画：IK控制-手部预设")]
    public class EntityHandle_IKControl_HandPreset : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("F缓冲：实体到本技能缓冲")]
    public class EntityHandle_Cache_CacheEntityToSelfSkill : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("成功概率"), SerializeReference] public IPointerForFloat_Only pointerForFloat = new PointerForFloat_DirectClamp01() { @float = 1 };
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                float f = pointerForFloat?.Pick() ?? 0.5f;
                if (UnityEngine.Random.value < f)
                {
                    with.SelfCacheEntites.Add(on);
                }
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("F缓冲：到默认实体缓冲")]
    public class EntityHandle_Cache_CacheEntityToMain : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("成功概率"), SerializeReference] public IPointerForFloat_Only pointerForFloat = new PointerForFloat_DirectClamp01() { @float = 1 };
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                float f = pointerForFloat?.Pick() ?? 0.5f;
                if (UnityEngine.Random.value < f)
                {
                    from.BaseDomain.Module_Cache?.CacheEntity.AddToQueue("Main", on);
                }
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("F缓冲：到指定实体缓冲")]
    public class EntityHandle_Cache_CacheEntityToWhich : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("缓冲名")] public string cacheName = "自定义名字";
        [LabelText("成功概率"), SerializeReference] public IPointerForFloat_Only pointerForFloat = new PointerForFloat_DirectClamp01() { @float = 1 };
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {

            if (on != null)
            {
                float f = pointerForFloat?.Pick() ?? 0.5f;
                if (UnityEngine.Random.value < f)
                {

                    from.BaseDomain.Module_Cache?.CacheEntity.AddToQueue(cacheName, on);
                }
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("F缓冲：坐标到本技能缓冲")]
    public class EntityHandle_Cache_CachePosToSelf : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("成功概率"), SerializeReference] public IPointerForFloat_Only pointerForFloat = new PointerForFloat_DirectClamp01() { @float = 1 };
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                float f = pointerForFloat?.Pick() ?? 0.5f;
                if (UnityEngine.Random.value < f)
                {
                    with.SelfCacheVector3.Add(on.transform.position);
                    Debug.Log("缓冲了" + on.transform.position);
                }
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("F缓冲：到默认坐标缓冲")]
    public class EntityHandle_Cache_CachePosToMain : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("成功概率"), SerializeReference] public IPointerForFloat_Only pointerForFloat = new PointerForFloat_DirectClamp01() { @float = 1 };
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                float f = pointerForFloat?.Pick() ?? 0.5f;
                if (UnityEngine.Random.value < f)
                {
                    on.BaseDomain.Module_Cache?.CacheVector3.AddToQueue("Main", on.transform.position);
                }
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("F缓冲：到指定坐标缓冲")]
    public class EntityHandle_Cache_CachePosToWhich : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("缓冲名")] public string cacheName = "自定义名字";
        [LabelText("成功概率"), SerializeReference] public IPointerForFloat_Only pointerForFloat = new PointerForFloat_DirectClamp01() { @float = 1 };
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                float f = pointerForFloat?.Pick() ?? 0.5f;
                if (UnityEngine.Random.value < f)
                {
                    on.BaseDomain.Module_Cache?.CacheVector3.AddToQueue(cacheName, on.transform.position);
                }
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("G反相：逐渐看向")]
    public class EntityHandle_Inverse_LookAt : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("多久能完全面向")] public float faceTime = 0.2f;
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                Vector3 vv = on.transform.position - from.transform.position;
                Quaternion end = Quaternion.LookRotation(Vector3.ProjectOnPlane(vv, from.transform.up), from.transform.up);

                Tween use = from.transform.DORotateQuaternion(end, faceTime);


                /* var to = from.transform.InverseTransformDirection(vv);
                 var y = from.BaseDomain.Module_3DMotion;

                 y.banSource.Add(this);*/
                with.OnExit += (f) =>
                {
                    use.Kill();
                    /*y.banSource.Remove(this);*/
                };
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("G反相：快速接近")]
    public class EntityHandle_Inverse_Approach : IReleasablePointerOnlyByEntityYarnEntityOn
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
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {

                var crash = from.StateMachineDomain.Module_CrashDodge;
                if (crash != null)
                {
                    defaultDodge.duration = duration;
                    defaultDodge.pathType = pathType;
                    if (UseSelfDefineVector)
                    {
                        defaultDodge.vector = selfVector?.Pick() ?? on.transform.position;
                    }
                    else
                    {
                        defaultDodge.vector = on.transform.position;
                    }
                    crash.TryAddCrashDodge(ref defaultDodge);
                }
                /* var to = from.transform.InverseTransformDirection(vv);
                 var y = from.BaseDomain.Module_3DMotion;

                 y.banSource.Add(this);*/
            }
            return 5;
        }
    }
    [Serializable, TypeRegistryItem("G反相：瞄准发射")]
    public class EntityHandle_Inverse_AimFire : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }


    }
    [Serializable, TypeRegistryItem("G反相：处决")]
    public class EntityHandle_Inverse_Execute : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                //Link_EntityAttackEntityTruely 备忘录
            }
            return 5;
        }


    }

    [Serializable, TypeRegistryItem("H场景：播放音效(单个)")]
    public class EntityHandle_PlaySound : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("使用的音效")] public AudioClip audioClip;
        [LabelText("音量"), SerializeReference] public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("是否播放为空间音效")] public bool playAs3D = false;
        [LabelText("没空间音频源是否强制创建")] public bool ForceCrate = false;

        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null)
            {
                GameCenterManager.Instance.AudioMaster.PlayDirect_Sound_OneShot(audioClip, float_Only?.Pick() ?? 0.8f);
            }
            return 5;
        }
    }

    [Serializable, TypeRegistryItem("H场景：播放音效(随机)")]
    public class EntityHandle_PlayOneOfSounds : IReleasablePointerOnlyByEntityYarnEntityOn
    {
        [LabelText("使用的音效")] public AudioClip[] audioClips;
        [LabelText("音量"), SerializeReference] public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("是否播放为空间音效")] public bool playAs3D = false;
        [LabelText("没空间音频源是否强制创建")] public bool ForceCrate = false;

        public object Pick(Entity on = null, Entity from = null, EntityState_Skill with = null)
        {
            if (on != null && audioClips?.Length > 0)
            {
                var oneOf = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
                GameCenterManager.Instance.AudioMaster.PlayDirect_Sound_OneShot(oneOf, float_Only?.Pick() ?? 0.8f);
            }
            return 5;
        }
    }
    #endregion
    /*创建筛选器*/

    [Serializable, TypeRegistryItem("目标筛选链(自己出发，获得实体列表)")]
    public class SomeEntitySelectorFromSelf : PointerPackForDynamicChain<List<Entity>, IPointerForSomeEntityByEntityYarnEntityOnSkill, IReleasablePointerChain, IPointerForSomeEntityBySomeEntityYarnEntityOnSKill>
        , IPointerForSomeEntity_Only
    {
        public override IPointerForSomeEntityByEntityYarnEntityOnSkill head => head_;

        [PropertySpace(10, 10)]
        [LabelText("链的起点(从自己出发)", SdfIconType.BoxArrowInRight), PropertyOrder(-1), SerializeReference, GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")]
        public IReleasablePointerForSomeEntityByEntityYarnEntityOnSKill head_ = new ReleasablePointer_EntitySelf();
        [Space(10)]
        [LabelText("链的终点(输出应用目标)", SdfIconType.BoxArrowRight), PropertyOrder(1), SerializeReference, GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCatcher")]
        public IReleasablePointerForSomeEntityBySomeEntityYarnEntityOnSKill end_ = new ReleasablePointer_SelectTagsAndDistance();
        public override IPointerForSomeEntityBySomeEntityYarnEntityOnSKill end => end_;

        //指向
        List<Entity> IPointer<List<Entity>, object, object, object>.Pick(object on, object from, object with)
        {
            return Pick(with,from,with);
        }
    }

    [Serializable, TypeRegistryItem("目标执行列(遍历筛选的实体列表")]
    public class EntityHandle
    {
        [SerializeReference]
        public List<IReleasablePointerOnlyByEntityYarnEntityOn> handles = new List<IReleasablePointerOnlyByEntityYarnEntityOn>();
    }
    /*  [Serializable, TypeRegistryItem("坐标执行列(使用一系列坐标完成功能)")]
      public class EntityHandle
      {
          [SerializeReference]
          public List<IReleasablePointerOnlyByEntityYarnEntityOn> handles_ = new List<IReleasablePointerOnlyByEntityYarnEntityOn>();
      }*/

}
