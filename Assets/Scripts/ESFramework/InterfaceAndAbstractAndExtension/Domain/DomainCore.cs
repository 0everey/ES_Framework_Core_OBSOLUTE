using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace ES
{
    public interface IDomainCore
    {
        public void BroadCastRegester();//初始化创建链接
    }
    [DefaultExecutionOrder(-2)]//顺序在前
    public abstract class DomainCore : ESHostingMono, IDomainCore
    {
        //核域通常在一个名目下
        public T GetDomain<T>() where T : Component
        {
            return GetComponentInChildren<T>();
        }
        protected virtual void Awake()
        {
            BroadCastRegester();
        }
        public void BroadCastRegester()
        {
            BeforeStartRegister();
            
            BroadcastMessage("RegesterStart", this, options: SendMessageOptions.DontRequireReceiver);
        }
        protected virtual void BeforeStartRegister()
        {

        }
    }
    
    
  
    public interface IDomainBase : IESHosting
    {
        
    }

    public abstract class DomainBase<Core, Clip, This> : ESHostingMono<Clip>, IDomainBase where Core : DomainCore where Clip :class, IDomainClip, IESModule where This : IDomainBase
    {
        public override IEnumerable<Clip> NormalBeHosted => Clips.valuesNow_;
        [FoldoutGroup("扩展域"), LabelText("域功能", icon: SdfIconType.Palette), GUIColor("ColorGetter"), ShowInInspector, PropertyOrder(-100)] public ESReadMeClass readMe=new ESReadMeClass() { readMe="这是一个扩展区域" };
        [FoldoutGroup("扩展域"), LabelText("链接的核", icon: SdfIconType.Water),ReadOnly, GUIColor("ColorGetter")] public Core core;
        [FoldoutGroup("扩展域"), LabelText("全部切片")] public SafeUpdateList_EasyQueue_SeriNot_Dirty<Clip> Clips = new SafeUpdateList_EasyQueue_SeriNot_Dirty<Clip>();
        
        public void RegesterStart(Core core)
        {
            
            if (core != null)
            {
                this.core = core;
                CreateLink();
                RegesterAll();
            }
        }
        public T GetModule<T>()
        {
            foreach (var i in NormalBeHosted)
            {
                if (i is T t) return t;
            }
            return default;
        }
        protected virtual void CreateLink()
        {
            
        }
        protected virtual void RegesterAll()
        {
            foreach (var i in NormalBeHosted)
            {
                i.TrySubmitHosting(this,false);
            }
        }
        protected virtual Color ColorGetter()
        {
            return Color.green;
        }
        public void AddClip(Clip clip)
        {
            Clips.TryAdd(clip);
            clip.TrySubmitHosting(this,false);
        }
        public void RemoveClip(Clip clip)
        {
            Clips.TryRemove(clip);
            clip.TryWithDrawHosting(this, false);
        }
        protected override void Update()
        {
            Clips.Update();
            base.Update();
        }
    }
    [TypeRegistryItem("空剪影")]
    public interface IDomainClip : IESModule
    {

    }
    [TypeRegistryItem("抽象剪影定义")]
    public abstract class DomainClip<Core, Domain> :BaseESModule<Domain>, IDomainClip where Core : DomainCore where Domain :class, IDomainBase
    {
        public override Domain GetHost => default;
        [HideInInspector] public Domain domain;
        protected override bool OnSubmitHosting(Domain hosting)
        {
            domain = hosting;
            return true;
        }
        protected override bool OnWithDrawHosting(Domain host)
        {
            domain = host;
            return false;
        }

        protected override void Update()
        {
            base.Update();
        }


        [ShowInInspector, GUIColor("ColorGetter"), LabelText(SdfIconType.CashCoin, Text = @"@ ""功能：""+ Description_  "),
            PropertyOrder(-100), Indent(-2), Tooltip("剪影细则")]
        public virtual string Description_ => "描述";


        protected virtual Color ColorGetter()
        {
            return Color.yellow;
        }
    }
}
