using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
/*
 扩展时 建议创建新的脚本 
 修改文件时 使用 #region + 自己的名字
 格式尽量统一 
 多交流 --- Everey
 */
namespace ES.EvPointer
{
    //核心 Ev针支持 关于无参无返回的触发功能 部分
        // Pick器 Test器 通常也实现自IPointerNone 见文件-- PointerAboutPick_Invoke_Test.cs
    #region 触发针None部分
    #region 触发针接口抽象和包
    public interface IPointerNone : IPointer<object, object, object, object>, IPointerOnlyBack<object>
    {

    }
    public interface IPointerNoneSome : IPointer<List<object>, object, object, object>, IPointerOnlyBackList<object>
    {

    }
    public abstract class PointerPackerForNone : PointerPackerBase<object, object, object, object, IPointerNone>, IPointerNone
    {

    }
    [Serializable]
    public class PointerNonePickSome : IPointerNone
    {
        [SerializeReference, LabelText("调用列表")] public IPointerNoneSome some;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return some?.Pick();
        }
    }
    public interface IPointerOnlyAction : IPointerNone
    {
    }
    //没啥必要说是
    public abstract class PointerOnlyAction : IPointerOnlyAction
    {
        public virtual object Pick(object by = null, object yarn = null, object on = null)
        {
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("触发针包_选中一个")]
    public class PointerForIPointerNone_PackerSelect : PointerPackerForOnlySelectBackOne<object, IPointerNone>, IPointerNone
    {

    }
    [Serializable, TypeRegistryItem("触发针包_选中几个")]
    public class PointerForIPointerNone_PackerSelectSome : PointerPackerForSelectSomeBack<object, IPointerNone>, IPointerNoneSome
    {

    }
    #endregion
        #region 触发针功能
    //Pick器
    [Serializable, TypeRegistryItem("委托针_用触发针生成委托")]
    public class PointerForAction_FromPointerNone : IPointerForAction_Only
    {
        [LabelText("来自触发针"), SerializeReference] public IPointerNone pointerNone = new PointerPickerEveryThing();
        public Action Pick(object by = null, object yarn = null, object on = null)
        {
            if (pointerNone != null)
            {
                return () => pointerNone?.Pick();
            }
            return null;
        }
    }
   
    [Serializable, TypeRegistryItem("触发针_设置游戏物体活动状态_超简易")]
    public class PoinerSetGameObjectActiveEasy : IPointerNone
    {
        [LabelText("使用的游戏物体")] public GameObject gameObject;
        [LabelText("设置状态")] public bool activeTo;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (gameObject != null)
                gameObject.SetActive(activeTo);
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("触发针_设置游戏物体活动状态_简易来源针")]
    public class PoinerSetGameObjectActive_Pointer : IPointerNone
    {
        [LabelText("使用的游戏物体"), SerializeReference] public IPointerForGameObject_Only gameObjectP;
        [LabelText("设置状态")] public bool activeTo;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            GameObject gg = gameObjectP?.Pick();
            if (gg != null) gg.SetActive(activeTo);
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("触发针_设置脚本活动状态_超简易")]
    public class PoinerSetMonoEnableEasy : IPointerNone
    {
        [LabelText("使用的脚本")] public MonoBehaviour mono;
        [LabelText("设置状态")] public bool activeTo;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (mono != null)
                mono.enabled = (activeTo);
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("触发针_设置脚本活动状态_简易来源针")]
    public class PoinerSetMonoEnable_Pointer : IPointerNone
    {
        [LabelText("使用的脚本"), SerializeReference] public IPointerForComponent_Only comP;
        [LabelText("设置状态")] public bool activeTo;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            MonoBehaviour mono = comP?.Pick() as MonoBehaviour;
            if (mono != null) mono.enabled = (activeTo);
            return -1;
        }
    }
    /*[Serializable, TypeRegistryItem("触发针_设置游戏物体活动状态_操作函数")]
    public class PoinerSetGameObjectActive_PointerHandle : PointerCreateGameObjectByPool
    {
        [LabelText("使用的游戏物体")] public IPointerForGameObject_Only comP;
        [LabelText("操作类型")]public EnumCollect.
        [LabelText("操作布尔参数")] public bool handle;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            GameObject mono = comP?.Pick();
            KeyValueMatchingUtility.Function.
            if (mono != null) mono.SetActive(activeTo);
            return -1;
        }
    }*/
    //抽象基类，生成物体
    public abstract class PointerCreateGameObjectByPool : PointerOnlyAction
    {
        [LabelText("初始化数量")] public int num = 10;
    }
    [Serializable, TypeRegistryItem("触发针_对象池_直接使用预制件")]
    public class PoinerCreateGameObjectPoolByGameObject : PointerCreateGameObjectByPool
    {
        [LabelText("使用的游戏物体")] public GameObject gameObject;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (gameObject == null) return -1;
            ES_PoolMaster.Instance.CreatePool(gameObject, num);
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("触发针_创建对象池_引用预制件的(脚本)")]
    public class PoinerCreateGameObjectPoolByMono : PointerCreateGameObjectByPool
    {
        [LabelText("使用的游戏脚本")] public Component mono;

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (mono == null) return -1;
            ES_PoolMaster.Instance.CreatePool(mono.gameObject, num);
            return base.Pick(by, yarn, on);
        }
    }
    //触发针_实例化游戏物体抽象类
    public abstract class PoinerInstantiateGameObject : PointerOnlyAction, IPointerForGameObjectCaster
    {
        [DetailedInfoBox("", "此处需要引用一个PointerPlayerCaster,把自己的值投射给他它", Message = @"@ ""【绑定投射目标备注："" + (playerCaster != null ? playerCaster.des : ""！未绑定"") ", VisibleIf = "@usePlayerCaster")]
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public PointerPlayerSystemObjectCaster playerCaster_;

        [LabelText("位置点"), SerializeReference] public IPointerForVector3_Only pointerForVector3;
        [LabelText("进入父级")] public Transform parent;
        [LabelText("处于世界空间")] public bool atWorld = true;
        [LabelText("缩放归一")] public bool localScaleIndentity = true;
        [LabelText("旋转归一")] public bool localRotationIndentity = true;
        [LabelText("上一个生成")] public GameObject last;

        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;

        public GameObject Cast()
        {
            return last;
        }

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            return base.Pick(by, yarn, on);
        }
        protected void Apply(Transform t)
        {
            if (t != null)
            {
                last = t.gameObject;
                if (usePlayerCaster) playerCaster_?.Recieve(last);
            }
            KeyValueMatchingUtility.TransformSetter.HandleTransformAtParent
                (t, parent, pointerForVector3?.Pick() ?? default, atWorld, localScaleIndentity, localRotationIndentity);
        }
    }
    [Serializable, TypeRegistryItem("触发针_实例化游戏物体_直接使用预制件")]
    public class PoinerInstantiateGameObject_Direct : PoinerInstantiateGameObject
    {
        [LabelText("直接使用游戏物体")]
        public GameObject prefab;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (prefab != null)
            {
                GameObject g = MonoBehaviour.Instantiate(prefab);
                if(g!=null) Apply(g.transform);

            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("触发针_实例化游戏物体_使用预制件脚本")]
    public class PoinerInstantiateGameObject_ByMono : PoinerInstantiateGameObject
    {
        [LabelText("使用脚本")]
        public Component mono;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (mono != null)
            {
                Apply(MonoBehaviour.Instantiate(mono).transform);
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("触发针_实例化游戏物体依赖对象池_直接使用预制件")]
    public class PoinerInstantiateGameObjectByGameObject_Pool : PoinerInstantiateGameObject, IInittable
    {
        [LabelText("直接使用游戏物体")]
        public GameObject prefab;
        private bool hasInit = false;
        public void Init(params object[] ps)
        {
            ES_PoolMaster.Instance.CreatePool(prefab, 10);
            hasInit = true;
        }

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (prefab != null)
            {
                if (!hasInit) Init();
                Apply(ES_PoolMaster.Instance.GetInPool(prefab).transform);
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("触发针_实例化游戏物体依赖对象池_使用预制件脚本")]
    public class PoinerInstantiateGameObjectByMono_Pool : PoinerInstantiateGameObject, IInittable
    {
        [LabelText("使用游戏脚本")]
        public Component mono;
        private bool hasInit = false;
        public void Init(params object[] ps)
        {
            ES_PoolMaster.Instance.CreatePool(mono.gameObject, 10);
            hasInit = true;
        }

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (mono != null)
            {
                if (!hasInit) Init();
                Apply(ES_PoolMaster.Instance.GetInPool(mono.gameObject).transform);
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("触发针_销毁游戏物体(针来源)")]
    public class PointerDestroyGameObject : IPointerNone
    {
        [LabelText("摧毁游戏物体"), SerializeReference] public IPointerForGameObject_Only pointerFor;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (pointerFor != null)
            {
                GameObject g = pointerFor?.Pick();

                if (g != null)
                {
                    MonoBehaviour.Destroy(g);
                }
            }
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("触发针_延迟销毁游戏物体(针来源)")]
    public class PointerDestroyGameObject_Delay : IPointerNone
    {
        [LabelText("延迟摧毁游戏物体"), SerializeReference] public IPointerForGameObject_Only pointerFor;
        [LabelText("延迟时间"), SerializeReference] public IPointerForFloat_Only pointerTime;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            float delayTime = pointerTime?.Pick() ?? 1;
            if (pointerFor != null)
            {
                GameObject g = pointerFor?.Pick();

                GameCenterManager.Instance.StartCoroutine(CoroutineMaker.DelayCoroutine(() =>
                {
                    if (g != null)
                    {
                        MonoBehaviour.Destroy(g);
                    }
                },
                     delayTime

                 ));

            }
            return -1;
        }
    }
    #endregion
    #endregion
}
