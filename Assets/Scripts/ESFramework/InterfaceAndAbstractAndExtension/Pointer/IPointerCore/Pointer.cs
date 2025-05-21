#if UNITY_EDITOR
using UnityEditorInternal;
#endif


using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


using static ES.EnumCollect;

//两者共性
using IPointerForSystemObject = ES.EvPointer.IPointer;



namespace ES.EvPointer
  
{
    #region 针与针包的初始定义
    //原始接口
    public interface IPointer
    {
        public object Pick(object a = default, object b = default, object c = default);
        public string PickToString()
        {
            return Pick()?.ToString();
        }
    }
    #region 分类扩展接口
        #region 常用扩展
    //纯值针
    public interface IPointerOnlyBack<out Back> : IPointer<Back, object, object, object>
    {

    }
    //接口
    public interface IPointer<out Back, in ByON, in Yarn, in From> : IPointer
    {
        Back Pick(ByON by = default, Yarn yarn = default, From on = default);
    }
    public interface IPointerOnlyBackList<Single> : IPointerOnlyBackIEnumerable<List<Single>>
    {

    }
    //单值针
    public interface IPointerOnlyBackSingle<Back> : IPointerOnlyBack<Back>
    {

    }
    #endregion
        #region 废案或者还没怎么用的扩展
    public interface IPointer<Back, in Link,Head> : IPointerChainAny< Back,Link,Head,object> where Link : ILink
    {
        Back Pick(Link link);
        // Back PickByLink(Link link) { if (link != null) return Pick(link.By_, link.Yarn_, link.On_); return default(Back); }

    }
    //多值针
    public interface IPointerOnlyBackIEnumerable<IE> : IPointerOnlyBack<IE>
    {

    }
    public class PointerOnlyBackFunc<Back> : IPointerOnlyBack<Back>
    {
        public Back Pick(object by = null, object yarn = null, object on = null)
        {
            return backFunc.Invoke();
        }
        public Func<Back> backFunc => default;
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }

    public class PointerOnlyBackDirect<Back> : IPointerOnlyBack<Back>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
        public Back Pick(object by = null, object yarn = null, object on = null)
        {
            return back;
        }
        public Back back;
    }
    #endregion
    #endregion
    #region 基础包
    //基础针包
    public abstract class PointerPackerBase<Back, By, Yarn, On, USE> : IPointer<Back, By, Yarn, On>
    {
        public virtual string intName => "上次指针";
        [BoxGroup("针包", showLabel: false), LabelText(@"@intName", SdfIconType.Pin)] public int intHepler;
        [BoxGroup("针包", showLabel: false), Indent(2), LabelText("针包", SdfIconType.BagCheckFill), SerializeReference, PropertyOrder(1)] public List<USE> pointers = new List<USE>();

        public virtual Back Pick(By by = default, Yarn yarn = default, On on = default)
        {
            return default;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    //基础值集合
    public abstract class PointerForValueListBase<Back, By, Yarn, On, Use> : IPointer<Back, By, Yarn, On>
    {
        public virtual string intName => "上次指针";
        [BoxGroup("全部值", showLabel: false), LabelText(@"@intName", SdfIconType.Pin)] public int intHepler;
        [BoxGroup("全部值", showLabel: false), Indent(2), LabelText("全部值", SdfIconType.BagCheckFill), PropertyOrder(1)] public List<Use> values = new List<Use>();

        public virtual Back Pick(By by = default, Yarn yarn = default, On on = default)
        {
            return default;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    [Serializable, TypeRegistryItem("字符串针包_选中几个")]
    public class PointerForBaseIPointer_PackerSelectSome : PointerPackerForSelectSomeBack<String, IPointerForString_Only>, IPointerForStringList
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as PointerPackerForSelectSomeBack<String, IPointerForString_Only>)?.Pick();
        }
    }
    #endregion
    #region 常规重写包
    //多back针包
    public abstract class PointerPackerForSelectSomeBack<Back, Use> : PointerPackerBase<List<Back>, object, object, object, Use>, IPointerOnlyBackList<Back> where Use : IPointer<Back, object, object, object>
    {
        public override string intName => "预期长度";
        [LabelText("筛选方式", SdfIconType.Shuffle), Indent(-1), PropertyOrder(-1)]
        public EnumCollect.PointerSelectSomeType selectSomeType;
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
        public override List<Back> Pick(object by = null, object yarn = null, object on = null)
        {
            if (pointers != null)
            {
                if (pointers.Count > 0)
                {

                    if (pointers.Count > 1)
                    {
                        List<Use> ps = pointers.Where(n => n != null).ToList();
                        List<Back> bs = new List<Back>();
                        switch (selectSomeType)
                        {
                            case EnumCollect.PointerSelectSomeType.AllNotNull:
                                foreach (var i in ps)
                                {
                                    Back b = i.Pick();
                                    if (b != null) bs.Add(b);
                                }
                                break;
                            case EnumCollect.PointerSelectSomeType.StartSome:
                                for (int i = 0; i < ps.Count && i < intHepler; i++)
                                {
                                    Back b = ps[i].Pick();
                                    if (b != null) bs.Add(b);
                                }
                                break;
                            case EnumCollect.PointerSelectSomeType.EndSome:
                                for (int i = 0; i < ps.Count && i < intHepler; i++)
                                {
                                    Back b = ps[ps.Count - (1 + i)].Pick();
                                    if (b != null) bs.Add(b);
                                }
                                break;
                            case EnumCollect.PointerSelectSomeType.RandomSome:
                                int num = Mathf.Min(ps.Count, intHepler);
                                List<Use> ps2 = ps.OrderBy(n => UnityEngine.Random.value).Take(num).ToList();
                                foreach (var i in ps2)
                                {
                                    Back b = i.Pick();
                                    if (b != null) bs.Add(b);
                                }

                                break;
                            case EnumCollect.PointerSelectSomeType.Selector: break;
                            case EnumCollect.PointerSelectSomeType.TrySort: break;
                        }
                        return bs;
                    }
                }
            }
            return base.Pick(by, yarn, on);
        }
        public virtual void Addition(List<Back> backs)
        {

        }
    }
    //选一个back针
    public abstract class PointerPackerForOnlySelectBackOne<Back, USE> : PointerPackerBase<Back, object, object, object, USE>, IPointerOnlyBackSingle<Back> where USE : class, IPointer<Back, object, object, object>
    {
        [InfoBox("还没更改过该枚举值，确定收起警告？", VisibleIf = "@!ISee&&PointerSelectOneType.GetHashCode()==0"), GUIColor("@Color.red")]
        [ShowIfGroup("警告", VisibleIf = "@!ISee&&PointerSelectOneType.GetHashCode()==0"), PropertyOrder(-1)] public bool ISee = false;
        [LabelText("筛选方式", SdfIconType.Shuffle), Indent(-1), PropertyOrder(-1), GUIColor("@Color.gray"), EnumToggleButtons, OnValueChanged("Isee_")] public EnumCollect.PointerSelectOneType PointerSelectOneType;
        private void Isee_()
        {
            ISee = true;
        }

        public override Back Pick(object by = null, object yarn = null, object on = null)
        {

            var p = KeyValueMatchingUtility.Function.GetOne(pointers, PointerSelectOneType, ref intHepler);
            if (intHepler >= 0 && p != null) return p.Pick();
            return base.Pick(by, yarn, on);
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    //back值包
    public abstract class PointerForValueListSelectSomeBack<Back> : PointerForValueListBase<List<Back>, object, object, object, Back>, IPointerOnlyBackList<Back>
    {
        public override string intName => "预期长度";
        [LabelText("筛选方式", SdfIconType.Shuffle), Indent(-1), PropertyOrder(-1)]
        public EnumCollect.PointerSelectSomeType selectSomeType;

        public override List<Back> Pick(object by = null, object yarn = null, object on = null)
        {
            var p = KeyValueMatchingUtility.Function.GetSome(values, selectSomeType, ref intHepler);
            if (intHepler >= 0 && p != null) return p;

            return base.Pick(by, yarn, on);
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    //单back有值包
    public abstract class PointerForValueListSelectBackOne<Back> : PointerForValueListBase<Back, object, object, object, Back>, IPointerOnlyBackSingle<Back>
    {
        [InfoBox("还没更改过该枚举值，确定收起警告？", VisibleIf = "@!ISee&&PointerSelectOneType.GetHashCode()==0"), GUIColor("@Color.red")]
        [ShowIfGroup("警告", VisibleIf = "@!ISee&&PointerSelectOneType.GetHashCode()==0"), PropertyOrder(-1)] public bool ISee = false;
        [LabelText("筛选方式", SdfIconType.Shuffle), Indent(-1), PropertyOrder(-1), GUIColor("@Color.gray"), EnumToggleButtons, OnValueChanged("Isee_")] public EnumCollect.PointerSelectOneType PointerSelectOneType;
        private void Isee_()
        {
            ISee = true;
        }

        public override Back Pick(object by = null, object yarn = null, object on = null)
        {

            var p = KeyValueMatchingUtility.Function.GetOne(values, PointerSelectOneType, ref intHepler);
            if (intHepler >= 0 && p != null) return p;
            return base.Pick(by, yarn, on);
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    #endregion
    #region 特殊重写包
    //同by 包
    public abstract class PointerPackForSameByOnlyOne<Back, By, USE> : PointerPackerBase<Back, By, object, object, USE>
    {
        public abstract By byFrom { get; }

        public EnumCollect.PointerSelectOneType selectSomeType;
        public override Back Pick(By by = default, object yarn = null, object on = null)
        {
            var p = KeyValueMatchingUtility.Function.GetOne(pointers, selectSomeType, ref intHepler);
            if (intHepler >= 0 && p != null) return PickOne(p);
            return default;
        }

        public abstract Back PickOne(USE use);
    }
    //链模式
    public abstract class PointerPackForSimpleChain<Back, USE> : PointerPackerBase<Back, object, object, object, USE>, IPointerOnlyBack<Back> where USE : IPointerChain<Back>
    {
        public abstract Back head { get; }
        public override Back Pick(object by = default, object yarn = null, object on = null)
        {
            Back back = head;
            if (pointers == null || pointers.Count == 0) return default;
            for (int i = 0; i < pointers.Count; i++)
            {
                if (pointers[i] == null) continue;
                back = pointers[i].Pick(back);
            }
            return back;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }

    public abstract class PointerPackForDynamicChain<Back,AtFirst,USE,AtLast> : PointerPackerBase<Back, object, object, object, USE>, IPointerOnlyBack<Back> where USE:IPointerChain,IPointer
        where AtFirst:IPointer,IPointerChain
        where AtLast:IPointer, IPointerChain
    {
        public abstract AtFirst head { get; }
        public abstract  AtLast end { get; }
        public override Back Pick(object launcherEntity = default, object yarn = null, object on = null)
        {
            if (head == null || end == null) { Debug.LogError("链条首位不完整"); return default(Back); }
            object headUse=head?.Pick(launcherEntity,launcherEntity,on);
            //Debug.Log("筛选的头部"+headUse);
            
            return PickAfterHead(headUse, launcherEntity,on);
        }
        public  Back PickAfterHead(object startAsHead, object launcherEntity, object on = null)
        {
            object headUse;
            object current;
            headUse = current = startAsHead;
            if (pointers == null || pointers.Count == 0) { }
            else
            {
                for (int i = 0; i < pointers.Count; i++)
                {
                    if (pointers[i] == null) { /*Debug.LogError("链条断裂");*/ continue; }
                    current = pointers[i].Pick(current, headUse);
                }
            }
            //Debug.Log("尾部"+current+headUse+launcherEntity);
            Back back = KeyValueMatchingUtility.Matcher.SystemObjectToT<Back>(end.Pick(current, launcherEntity));
            return back;

        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }

    public abstract class PointerPackerForSelectSome_BackSelfDefine<Back, BackForUse, Use> : PointerPackerBase<Back, object, object, object, Use>, IPointerOnlyBack<Back> where Use : IPointer<BackForUse, object, object, object>
    {
        public override string intName => "预期长度";
        [LabelText("筛选方式", SdfIconType.Shuffle), Indent(-1), PropertyOrder(-1)]
        public EnumCollect.PointerSelectSomeType selectSomeType;

        public override Back Pick(object by = null, object yarn = null, object on = null)
        {
            if (pointers != null)
            {
                if (pointers.Count > 0)
                {

                    if (pointers.Count > 1)
                    {
                        List<Use> ps = pointers.Where(n => n != null).ToList();
                        List<BackForUse> bs = new List<BackForUse>();
                        switch (selectSomeType)
                        {
                            case EnumCollect.PointerSelectSomeType.AllNotNull:
                                foreach (var i in ps)
                                {
                                    BackForUse b = i.Pick();
                                    if (b != null) bs.Add(b);
                                }
                                break;
                            case EnumCollect.PointerSelectSomeType.StartSome:
                                for (int i = 0; i < ps.Count && i < intHepler; i++)
                                {
                                    BackForUse b = ps[i].Pick();
                                    if (b != null) bs.Add(b);
                                }
                                break;
                            case EnumCollect.PointerSelectSomeType.EndSome:
                                for (int i = 0; i < ps.Count && i < intHepler; i++)
                                {
                                    BackForUse b = ps[ps.Count - (1 + i)].Pick();
                                    if (b != null) bs.Add(b);
                                }
                                break;
                            case EnumCollect.PointerSelectSomeType.RandomSome:
                                int num = Mathf.Min(ps.Count, intHepler);
                                List<Use> ps2 = ps.OrderBy(n => UnityEngine.Random.value).Take(num).ToList();
                                foreach (var i in ps2)
                                {
                                    BackForUse b = i.Pick();
                                    if (b != null) bs.Add(b);
                                }

                                break;
                            case EnumCollect.PointerSelectSomeType.Selector: break;
                            case EnumCollect.PointerSelectSomeType.TrySort: break;
                        }
                        return Addition(bs);
                    }
                }
            }
            return base.Pick(by, yarn, on);
        }
        public virtual Back Addition(List<BackForUse> backs)
        {
            return default;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    #endregion
    #endregion

    #region 重要扩展接口-链
    public interface IPointerChain:IPointer { }

    public interface IPointerChain<T> : IPointer<T, T, object, object>, IPointerChain
    {

    }
    public interface IPointerChainAny<Next,in Last, Head,On> : IPointer<Next, Last, Head, On>, IPointerChain
    {

    }
    public interface IPointerChainLink<Next,in Last> : IPointer<Next, Last, object, object>, IPointerChain where Next:ILink where Last:ILink
    {

    }
    #endregion


    #region 杂碎功能
    #region 返回针的针
    public interface IPointerForIPointer<By, Yarn, On> : IPointer<IPointer, By, Yarn, On>
    {

    }
    public interface IPointerForIPointer_Only : IPointerForIPointer<object, object, object>, IPointerOnlyBack<IPointer>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    #endregion

  

    #endregion





    #region 数据结构LINK
    #region 完成支持的结构类型声明
    [Serializable,TypeRegistryItem("自定义事件名Link")]
    public struct Link_StringNameEvent : ILink
    {
        [LabelText("事件名称")]public string eventName;
        [LabelText("传输的数据")]public object param;
    }
    public class Link_BuffHandleChangeHappen : ILink
    {
        public Entity who;
        public BuffSoInfo info;
        public bool add;
    }

    #endregion
        #region Link相关接口
    public interface ILink
    {

    }
    public interface IReceiveLink
    {
         
    }
    public interface ISendLink
    {

    }
    public interface ILinkMatchPin : ILink
    {

    }
    public interface ILink<out By, out Yarn, out On> : ILinkMatchPin
    {
        By By_ { get; }
        Yarn Yarn_ { get; }
        On On_ { get; }
        object UsePointer<Back>(IPointer<Back, By, Yarn, On> pointer)
        {
            return pointer.Pick(By_, Yarn_, On_);
        }
       
    }

    public interface ISendLink<Link> : ISendLink where Link : ILink
    {
        public LinkUnityEvent<Link> linkedEvent { get; }

        public void SendLink(string Key)
        {

        }
    }
    
    public interface IReceiveLink<Link> : IReceiveLink where Link : ILink
    {
        void OnLink(Link link);
    }
    public interface IReceiveAnyLink : IReceiveLink<ILink>
    {

    }
    #endregion
        #region Link扩展
    [Serializable]
    public class LinkFunc<By, Yarn, On>:ILink<By,Yarn,On>
    {
        
        [SerializeReference]public IPointerOnlyBack<By> by;
        [SerializeReference] public IPointerOnlyBack<Yarn> yarn;
        [SerializeReference] public IPointerOnlyBack<On> on;
        
        public By By_ => by.Pick();

        public Yarn Yarn_ => yarn.Pick();

        public On On_ => on.Pick();

        
    }
    [Serializable]
    public class LinkDirect<By, Yarn, On> : ILink<By,Yarn,On>
    {
        [SerializeReference] public By by;
        [SerializeReference] public On on;
        [SerializeReference] public Yarn yarn;
        
        public By By_ => by;

        public Yarn Yarn_ => yarn;

        public On On_ => on;
        
    }
    #endregion
        #region UnityEvent支持
    [Serializable]
    public class LinkUnityEvent<Link> where Link : ILink
    {

        [ShowInInspector, GUIColor("yellow"), PropertyOrder(-1), LabelText("LinkUnity", SdfIconType.BroadcastPin)] public virtual string description => "Link事件";
        [HorizontalGroup("水平")]
        //不需要
        [FoldoutGroup("水平/事件配置"), LabelText("触发类型", SdfIconType.Signal), GUIColor("@new Color(0.7f,0.8f,0,1)")] public EnumCollect.LinkEventType triggerType;
        //不需要
        [FoldoutGroup("水平/事件配置"), LabelText("默认Link", SdfIconType.Signal), GUIColor("@new Color(0.5f,1f,0,1)"), Indent(-1)] public Link defaultLink;
        [FoldoutGroup("水平/事件配置"), LabelText("UnityEvent", SdfIconType.EarFill)] public UnityEvent<Link> unityEvent;
        /// <summary>
        /// 
        /// </summary>
        /// <param content="architecture">原型针</param>
        [HorizontalGroup("水平", width: 200)]
        [FoldoutGroup("水平/按钮测试")]
        [InfoBox("输入原型>绑定原型>游戏默认原型"), ShowInInspector, PropertyOrder(1), HideLabel]
        public string delta => "测试触发{←默认Link}";
        [FoldoutGroup("水平/按钮测试")]
        [Button(name: "发射(默认Link)", icon: SdfIconType.SendPlusFill, Style = ButtonStyle.FoldoutButton, ButtonHeight = 40), GUIColor("orange"), PropertyOrder(1)]
        public void SendLinkSelf(BaseArchitectureWithLinkAndConfiguration architecture)
        {
            SendLinkSelf(architecture, defaultLink);
        }
        [PropertySpace(20)]
        [ShowInInspector]
        [FoldoutGroup("水平/按钮测试"), PropertyOrder(2), HideLabel]
        public string delta2 => "测试触发{自定义Link}";
        [FoldoutGroup("水平/按钮测试")]
        [Button(name: "发射(自定义Link)", icon: SdfIconType.SendPlusFill, Style = ButtonStyle.FoldoutButton, ButtonHeight = 40), GUIColor("orange"), PropertyOrder(2)]
        public void SendLinkSelf(BaseArchitectureWithLinkAndConfiguration architecture, Link link)
        {
            switch (triggerType)
            {
                case EnumCollect.LinkEventType.SendThenInvoke:
                    architecture ??= GetArchitecture();
                    architecture.SendLink(link);
                    unityEvent?.Invoke(defaultLink);
                    break;
                case EnumCollect.LinkEventType.InvokeThenSend:
                    unityEvent?.Invoke(defaultLink);
                    architecture ??= GetArchitecture();
                    architecture.SendLink(link);
                    break;
                case EnumCollect.LinkEventType.OnlySend:
                    architecture ??= GetArchitecture();
                    architecture.SendLink(link);
                    break;
                case EnumCollect.LinkEventType.OnlyInvoke:
                    unityEvent?.Invoke(defaultLink);
                    break;
                default: break;
            }


        }

        public virtual BaseArchitectureWithLinkAndConfiguration GetArchitecture()
        {
            return GameCenterManager.Instance.GameCenterArchitecture;
        }

    }
    /* [Serializable]
     public class PointerForSimpleUnityEvent_WithLink<ByON, Yarn, From> : IPointerOnlyBackSingle<UnityEvent<LinkDirect<ByON, Yarn, From>>>
     {
         [LabelText("unity事件")] public UnityEvent<LinkDirect<ByON,Yarn,From>> unityevent;
         public UnityEvent<LinkDirect<ByON, Yarn, From>> Pick(IPointer launcherEntity = null, IPointer launcherEntity = null, IPointer on = null)
         {
             return unityevent;
         }
     }*/
    //标准的UnityEventLink
    /*[Serializable]
    public class PointerForSimpleUnityEvent_SelfCreateLink<ByON,Yarn,From> : IPointerOnlyBackSingle<UnityEvent<ByON,Yarn,From>>
    {
        [LabelText("unity事件")] public UnityEvent<ByON, Yarn, From> unityevent;
        public UnityEvent<ByON, Yarn, From> Pick(IPointer launcherEntity = null, IPointer launcherEntity = null, IPointer on = null)
        {
            return unityevent;
        }
    }*/
    [Serializable]
    public class PointerForSimpleUnityEvent_Direct : IPointerOnlyBackForSimpleUnityEvent
    {
        [LabelText("unity事件")] public UnityEvent unityevent;
        public UnityEvent Pick(object by = null, object yarn = null, object on = null)
        {
            return unityevent;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    public interface IPointerOnlyBackForSimpleUnityEvent : IPointerOnlyBackSingle<UnityEvent>
    {

    }
    #endregion
    #endregion



}

