using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ES
{
    public interface IDomainCore : IDelegatedUpdatable
    {
        public void BroadCastRegester();
       
    }
    [DefaultExecutionOrder(-2)]
    public abstract class DomainCore : MonoBehaviour,IDomainCore {
        public bool HasEnable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public T GetDomain<T>() where T : Component
        {
            return GetComponent<T>();
        }
        protected virtual void Awake()
        {
            BroadCastRegester();

        }
        public void BroadCastRegester()
        {
            BeforeRegester();
            //Debug.Log("cast");
            BroadcastMessage("RegesterStart", this,options: SendMessageOptions.DontRequireReceiver);
        }
        protected virtual void BeforeRegester()
        {

        }
        public virtual void Update()
        {
            
        }

        public void OnEnable()
        {
           
        }

        public void OnDisable()
        {
           
        }

        public void TryEnable()
        {
            
        }

        public void TryDisable()
        {
            
        }
        // Start is called before the first frame update
    }
    public abstract class HostingMono : MonoBehaviour,IPrimaryDelegatableAndHosting,IPrimaryUpdatableAndHosting,IDelegatableAndUpdatableAndHosting
    {
        public virtual IEnumerable<IModule> normalBeHosted => default;

        public List<IModule> virtualBeHosted => virtualBeHostedList;
        [LabelText("虚拟托管列表"),SerializeReference]public List<IModule> virtualBeHostedList = new List<IModule>();
        public bool HasEnable { get; set; }

        IEnumerable IHosting.normalBeHosted => normalBeHosted;

        List<object> IHosting.virtualBeHosted {get { List<object> list = new List<object>(virtualBeHosted.Count); foreach (var i in virtualBeHosted) { list.Add(i); } return list; }
    }


    public virtual void Update()
        {
            UpdateAsHosting();
        }
        public virtual void OnEnable()
        {
            EnableAsHosting();
        }
        public virtual void OnDisable()
        {
            DisableAsHosting();
        }

        public virtual void UpdateAsHosting()
        {
            if (normalBeHosted != null)
            {
                foreach (var i in normalBeHosted)
                {
                    if (i is IUpdatable update)
                    {
                        update.Update();
                    }
                }
            }
            if (virtualBeHosted != null)
            {
                foreach (var i in virtualBeHosted)
                {
                    if (i is IUpdatable update)
                    {
                        update.Update();
                    }
                }
            }
        }

        public virtual void EnableAsHosting()
        {
            if (normalBeHosted != null)
            {
                foreach (var i in normalBeHosted)
                {
                    if (i is IDelegatable update)
                    {
                        update.TryEnable();
                    }
                }
            }
            if (virtualBeHostedList != null)
            {
                foreach (var i in virtualBeHostedList)
                {
                    if (i is IDelegatable update)
                    {
                        update.TryEnable();
                    }
                }
            }
        }
        public virtual void DisableAsHosting()
        {
            if (normalBeHosted != null)
            {
                foreach (var i in normalBeHosted)
                {
                    if (i is IDelegatable update)
                    {
                        update.TryDisable();
                    }
                }
            }
            if (virtualBeHostedList != null)
            {
                foreach (var i in virtualBeHostedList)
                {
                    if (i is IDelegatable update)
                    {
                        update.TryDisable();
                    }
                }
            }
        }

        public virtual void AddHandle(IModule i, object withKey = null)
        {
            
        }

        public virtual void RemoveHandle(IModule i, object withKey = null)
        {
           
        }

        public void TryEnable()
        {
            throw new NotImplementedException();
        }

        public void TryDisable()
        {
            throw new NotImplementedException();
        }

        public void AddHandle(object i, object withKey = null)
        {
            if(i is IModule m)
            {
                AddHandle(m);
            }
        }

        public void RemoveHandle(object i, object withKey = null)
        {
            if (i is IModule m)
            {
                RemoveHandle(m);
            }
        }
    }
    public abstract class DomainBase : HostingMono
    {
        public override IEnumerable<IModule> normalBeHosted => default;

        public override  void AddHandle(IModule i, object withKey = null)
        {
            
        }

        public override void RemoveHandle(IModule i, object withKey = null)
        {

        }
    }

    /*public abstract class DomainBase<Core,This> : DomainBase<Core,DomainClip<Core, This>,This> where Core : DomainCore where This : DomainBase
    {

    }*/
    public abstract class DomainBase<Core, Clip,This> : DomainBase,IDelegatableAndUpdatableAndHosting where Core:DomainCore where Clip : DomainClip where This: DomainBase
    {
        [FoldoutGroup("基本"), LabelText("域功能", icon: SdfIconType.Palette), GUIColor("ColorGetter"),ShowInInspector, PropertyOrder(-100)] public virtual string Description_ => "一个域";
        [FoldoutGroup("基本"),LabelText("链接核",icon:SdfIconType.Water),GUIColor("ColorGetter")]public Core usingCore;
        [FoldoutGroup("基本"), LabelText("全部切片"),SerializeReference]public List<Clip> Clips = new List<Clip>();
        public void RegesterStart(Core core)
        {
            //Debug.Log("satrt");
            if (core != null)
            {
                usingCore = core;
                CreateLink();
                RegesterAll();
            }
        }
        public T GetModule<T>()
        {
            foreach (var i in Clips)
            {
                if (i is T t) return t;
            }
            return default;
        }
        protected virtual void CreateLink()
        {
            //用于把core 对自己的引用赋值
        }
        protected virtual void  RegesterAll()
        {
            //Debug.Log("all");
            foreach(var i in Clips)
            {
                i.TrySubmitHosting(this);
            }
        }
        protected virtual Color ColorGetter()
        {
            return Color.green;
        }
        public override IEnumerable<IModule> normalBeHosted =>Clips;
    }
    [Serializable]
    public abstract class DomainClip :UpdatableDelegatableWithHosting<DomainBase>
    {

    }
    [Serializable]
    public abstract class DomainClip<Core> : DomainClip<Core, DomainBase> where Core : DomainCore
    {

    }
    [Serializable]
    public abstract class DomainClip <Core,Domain> : DomainClip where Core : DomainCore  where Domain : DomainBase
    {
        public override DomainBase GetHost => default;
        [HideInInspector]public Domain domain;
        public override bool OnSubmitHosting(DomainBase hosting, bool asVirtual = false)
        {
            domain = hosting as Domain;
            return domain != null;
        }
      
        public override void Update()
        {
            base.Update();
        }
       
        
        [ShowInInspector, GUIColor("ColorGetter"), LabelText(SdfIconType.CashCoin, Text = @"@ ""功能：""+ Description_  "),
            PropertyOrder(-100),Indent(-2),Tooltip("剪影细则")] 
        public virtual string Description_ => "描述";
       
      
        protected virtual Color ColorGetter()
        {
            return Color.yellow;
        }
    }
}
