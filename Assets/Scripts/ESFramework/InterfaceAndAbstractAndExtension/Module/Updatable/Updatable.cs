using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ES
{
    public interface IUpdatable : IModule
    {
        
        void Update();
    }
    public interface IUpdatableWithHosting<Host> : IUpdatable,IWithHosting<Host> where Host:IPrimaryHosting
    {
       
    }
    public interface IDelegatedUpdatable : IDelegatable,IUpdatable
    {

    }
    public interface IHosting
    {
        IEnumerable normalBeHosted { get; }
        public List<object> virtualBeHosted { get; }
        void AddHandle(object i, object withKey = null);
        void RemoveHandle(object i, object withKey = null);
    }
    public interface IHosting<With> : IHosting {
        IEnumerable<With> normalBeHosted { get; }
        public List<With> virtualBeHosted { get; }
        void AddHandle(With i, object withKey = null);
        void RemoveHandle(With i, object withKey = null);
        IEnumerable IHosting.normalBeHosted => normalBeHosted;

        List<object> IHosting.virtualBeHosted { get {  List<object> list = new List<object>(virtualBeHosted.Count);foreach (var i in virtualBeHosted) { list.Add(i); } return list ; } }
         void IHosting.AddHandle(object i, object withKey = null)
        {
            throw new System.NotImplementedException();
        }
        void IHosting.RemoveHandle(object i, object withKey = null)
        {
            throw new System.NotImplementedException();
        }

    }
    public class ssss : IRightlyCompleteHosting
    {
        public IEnumerable<IDelegatedUpdatable> normalBeHosted => throw new System.NotImplementedException();

        public List<IDelegatedUpdatable> virtualBeHosted => throw new System.NotImplementedException();

        IEnumerable<IModule> IHosting<IModule>.normalBeHosted => throw new System.NotImplementedException();

        IEnumerable IHosting.normalBeHosted => throw new System.NotImplementedException();

        List<IModule> IHosting<IModule>.virtualBeHosted => throw new System.NotImplementedException();

        List<object> IHosting.virtualBeHosted => throw new System.NotImplementedException();

        public void AddHandle(IDelegatedUpdatable i, object withKey = null)
        {
            throw new System.NotImplementedException();
        }

        public void AddHandle(IModule i, object withKey = null)
        {
            throw new System.NotImplementedException();
        }

        public void AddHandle(object i, object withKey = null)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveHandle(IDelegatedUpdatable i, object withKey = null)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveHandle(IModule i, object withKey = null)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveHandle(object i, object withKey = null)
        {
            throw new System.NotImplementedException();
        }
    }
    public interface IPrimaryHosting : IHosting<IModule>
    {
       /* IEnumerable<IModule> normalBeHosted { get; }
        public List<IModule> virtualBeHosted { get; }
        void AddHandle(IModule i,object withKey=null);
        void RemoveHandle(IModule i, object withKey = null);*/
    }
    public interface IRightlyCompleteHosting : IHosting<IDelegatedUpdatable> ,IPrimaryHosting
    {
        /*IEnumerable<IDelegatedUpdatable> normalBeHosted { get; }
        public List<IDelegatedUpdatable> virtualBeHosted { get; }
        void AddHandle(IDelegatedUpdatable i, object withKey = null);
        void RemoveHandle(IDelegatedUpdatable i, object withKey = null);*/
    }
    public interface ISubHosting
    {
        bool TrySubmitHosting(IPrimaryHosting hosting, bool asVirtual = false);
        bool TryWithDrawHosting(IPrimaryHosting hosting, bool asVirtual = false);
    }
    public interface ISubHosting<Host> : ISubHosting where Host: IHosting
    {
        bool ISubHosting.TrySubmitHosting(IPrimaryHosting hosting, bool asVirtual = false){
           return  TrySubmitHosting((Host)hosting, asVirtual);
        }
        bool ISubHosting.TryWithDrawHosting(IPrimaryHosting hosting, bool asVirtual = false)
        {
            return TryWithDrawHosting((Host)hosting, asVirtual);
        }
        bool TrySubmitHosting(Host hosting, bool asVirtual = false);
        bool TryWithDrawHosting(Host hosting, bool asVirtual = false);
        bool OnSubmitHosting(Host hosting, bool asVirtual = false);
        bool OnWithDrawHosting(Host hosting, bool asVirtual = false);
        bool HasSubmit { get; set; }
    }
    public interface IWithHosting<Host>  : ISubHosting<Host>,IModule where Host : IHosting
    {
        Host GetHost { get; }
    }
    public interface IDelegatableAndHosting: IDelegatable, IHosting
    {
        void EnableAsHosting();
        void DisableAsHosting();
    }
    public interface IPrimaryDelegatableAndHosting : IDelegatable, IPrimaryHosting
    {
        void EnableAsHosting();
        void DisableAsHosting();
    }
    public interface IRightlyCompleteDelegatableAndHosting : IDelegatable, IRightlyCompleteHosting
    {
        void EnableAsHosting();
        void DisableAsHosting();
    }
    public interface IUpdatableAndHosting : IUpdatable,IHosting
    {
       
        void UpdateAsHosting();
       
    }
    public interface IPrimaryUpdatableAndHosting : IUpdatable, IPrimaryHosting
    {

        void UpdateAsHosting();

    }
    public interface IRightlyCompleteUpdatableAndHosting : IUpdatable, IRightlyCompleteHosting
    {

        void UpdateAsHosting();

    }
    public interface IDelegatableAndUpdatableAndHosting : IUpdatableAndHosting,IDelegatableAndHosting
    {
      
    }
    public abstract class DelegatableAndHosting :Delegatable, IDelegatableAndHosting
    {
        public virtual IEnumerable<IModule> normalBeHosted => default;

        public List<IModule> virtualBeHosted => virtualBeHostedList.valuesNow_;

        IEnumerable IHosting.normalBeHosted => normalBeHosted;

        List<object> IHosting.virtualBeHosted { get { List<object> list = new List<object>(virtualBeHosted.Count); foreach (var i in virtualBeHosted) { list.Add(i); } return list; } }


        [LabelText("虚拟托管")] public ListSafeUpdate<IModule> virtualBeHostedList = new ListSafeUpdate<IModule>();
        public override void OnEnable()
        {
            EnableAsHosting();
        }
        public override void OnDisable()
        {
            DisableAsHosting();
        }

      

        public virtual void EnableAsHosting()
        {
            if (normalBeHosted != null)
            {
                foreach (var i in normalBeHosted)
                {
                    if (i is IDelegatable update)
                    {
                        update.OnEnable();
                    }
                }
            }
            if (virtualBeHosted != null)
            {
                foreach(var i in virtualBeHosted)
                {
                    if (i is IDelegatable update)
                    {
                        update.OnEnable();
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
                        update.OnDisable();
                    }
                }
            }
        }

        public virtual void AddHandle(IModule i, object withKey = null)
        {
            if(i is IDelegatable de)
            {
                de.OnEnable();
            }
        }

        public virtual void RemoveHandle(IModule i, object withKey = null)
        {
            if (i is IDelegatable de)
            {
                de.OnDisable();
            }
        }

        public void AddHandle(object i, object withKey = null)
        {
            if (i is IModule m)
            {
                AddHandle(m, withKey);
            }
        }

        public void RemoveHandle(object i, object withKey = null)
        {
            if(i is IModule m)
            {
                RemoveHandle(m,withKey);
            }
        }
    }
    public abstract class PrimaryDelegatableAndHosting : Delegatable, IPrimaryDelegatableAndHosting
    {
        public virtual IEnumerable<IModule> normalBeHosted => default;

        public List<IModule> virtualBeHosted => virtualBeHostedList.valuesNow_;

      /*  IEnumerable IHosting.normalBeHosted => normalBeHosted;

        List<object> IHosting.virtualBeHosted { get { List<object> list = new List<object>(virtualBeHosted.Count); foreach (var i in virtualBeHosted) { list.Add(i); } return list; } }
*/

        [LabelText("虚拟托管")] public ListSafeUpdate<IModule> virtualBeHostedList = new ListSafeUpdate<IModule>();
        public override void OnEnable()
        {
            EnableAsHosting();
        }
        public override void OnDisable()
        {
            DisableAsHosting();
        }



        public virtual void EnableAsHosting()
        {
            if (normalBeHosted != null)
            {
                foreach (var i in normalBeHosted)
                {
                    if (i is IDelegatable update)
                    {
                        update.OnEnable();
                    }
                }
            }
            if (virtualBeHosted != null)
            {
                foreach (var i in virtualBeHosted)
                {
                    if (i is IDelegatable update)
                    {
                        update.OnEnable();
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
                        update.OnDisable();
                    }
                }
            }
        }

        public virtual void AddHandle(IModule i, object withKey = null)
        {
            if (i is IDelegatable de)
            {
                de.OnEnable();
            }
        }

        public virtual void RemoveHandle(IModule i, object withKey = null)
        {
            if (i is IDelegatable de)
            {
                de.OnDisable();
            }
        }

      /*  public void AddHandle(object i, object withKey = null)
        {
            if (i is IModule m)
            {
                AddHandle(m, withKey);
            }
        }

        public void RemoveHandle(object i, object withKey = null)
        {
            if (i is IModule m)
            {
                RemoveHandle(m, withKey);
            }
        }*/
    }
    public abstract class RightlyCompleteDelegatableAndHosting : Delegatable, IRightlyCompleteHosting
    {
        public virtual IEnumerable<IDelegatedUpdatable> normalBeHosted => default;

        public List<IDelegatedUpdatable> virtualBeHosted => virtualBeHostedList.valuesNow_;

        IEnumerable<IModule> IHosting<IModule>.normalBeHosted => normalBeHosted;

        List<IModule> IHosting<IModule>.virtualBeHosted { get { List<IModule> list = new List<IModule>(virtualBeHosted.Count); foreach (var i in virtualBeHosted) { list.Add(i); } return list; } }

        List<object> IHosting.virtualBeHosted { get { List<object> list = new List<object>(virtualBeHosted.Count); foreach (var i in virtualBeHosted) { list.Add(i); } return list; } }

        IEnumerable IHosting.normalBeHosted => normalBeHosted;

       

        [LabelText("虚拟托管")] public ListSafeUpdate<IDelegatedUpdatable> virtualBeHostedList = new ListSafeUpdate<IDelegatedUpdatable>();
        public override void OnEnable()
        {
            EnableAsHosting();
        }
        public override void OnDisable()
        {
            DisableAsHosting();
        }



        public virtual void EnableAsHosting()
        {
            if (normalBeHosted != null)
            {
                foreach (var i in normalBeHosted)
                {
                    i.TryEnable();
                }
            }
            if (virtualBeHosted != null)
            {
                foreach (var i in virtualBeHosted)
                {
                    i.TryEnable();
                }
            }

        }
        public virtual void DisableAsHosting()
        {
            if (normalBeHosted != null)
            {
                foreach (var i in normalBeHosted)
                {
                    i.TryDisable();
                }
            }
            if (virtualBeHosted != null)
            {
                foreach (var i in virtualBeHosted)
                {
                    i.TryDisable();
                }
            }
        }

        public virtual void AddHandle(IDelegatedUpdatable i, object withKey = null)
        {
            i.TryEnable();
        }

        public virtual void RemoveHandle(IDelegatedUpdatable i, object withKey = null)
        {
            i.TryDisable();
        }

        public void AddHandle(IModule i, object withKey = null)
        {
            if(i is IDelegatedUpdatable de)
            {
                AddHandle(de,withKey);
            }
        }

        public void RemoveHandle(IModule i, object withKey = null)
        {
            if (i is IDelegatedUpdatable de)
            {
                RemoveHandle(de, withKey);
            }
        }

        public void AddHandle(object i, object withKey = null)
        {
            if (i is IDelegatedUpdatable de)
            {
                AddHandle(de, withKey);
            }
        }

        public void RemoveHandle(object i, object withKey = null)
        {
            if (i is IDelegatedUpdatable de)
            {
                RemoveHandle(de, withKey);
            }
        }
    }
    public abstract class DelegatableAndUpdatableAndHosting : DelegatableAndHosting,IDelegatableAndUpdatableAndHosting
    {
        public virtual void Update()
        {
            UpdateAsHosting();
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
    }
    public abstract class PrimaryDelegatableAndUpdatableAndHosting : DelegatableAndHosting, IDelegatableAndUpdatableAndHosting
    {
        public virtual void Update()
        {
            UpdateAsHosting();
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
    }
 
    public abstract class DelegatableAndUpdatableAndHostingWithHosting<Host> : DelegatableAndUpdatableAndHosting, IWithHosting<Host> where Host:IPrimaryHosting
    {
        public Host GetHost => host;
        [LabelText("托管核心",SdfIconType.Bullseye)]
        public Host host;
        public bool HasSubmit { get; set; }
        [ShowInInspector,LabelText("是否已经Submit")]public bool ShowHasSubmit => HasSubmit;
        public virtual bool OnSubmitHosting(Host hosting, bool asVirtual = false) {
            host = hosting;
            return true;
        }
        public virtual bool OnWithDrawHosting(Host hosting, bool asVirtual = false)
        {
            throw new System.NotImplementedException();
        }

        public bool TrySubmitHosting(Host hosting, bool asVirtual = false)
        {
            
            if (HasSubmit) return true;
            
            if (asVirtual)
            {
                if (hosting.virtualBeHosted != null)
                {
                    Debug.Log("加入虚拟列表");
                    if (!hosting.virtualBeHosted.Contains(this))
                    {
                        hosting.virtualBeHosted.Add(this);
                        this.OnEnable();
                    }
                    return HasSubmit=true;
                }
                else
                {
                    Debug.Log("是空的");
                }
            }
            return HasSubmit = OnSubmitHosting(hosting, asVirtual);
        }

        public bool TryWithDrawHosting(Host hosting, bool asVirtual = false)
        {
            if (!HasSubmit) return false;
            if (asVirtual)
            {
                if (hosting.virtualBeHosted != null)
                {
                    if (hosting.virtualBeHosted.Contains(this))
                        hosting.virtualBeHosted.Remove(this);
                    HasSubmit = false;
                    return true;
                }
            }
            return HasSubmit = !OnWithDrawHosting(hosting, asVirtual);
        }
    }
    public abstract class RightlyCompleteDelegatableAndUpdatableAndHosting : RightlyCompleteDelegatableAndHosting, IUpdatable
    {
        public virtual void Update()
        {
            UpdateAsHosting();
        }


        public virtual void UpdateAsHosting()
        {
            if (normalBeHosted != null)
            {
                foreach (var i in normalBeHosted)
                {
                     i.Update();
                }
            }
            if (virtualBeHosted != null)
            {
                foreach (var i in virtualBeHosted)
                {
                    i.Update();
                }
            }
        }
    }
    public abstract class RightlyCompleteDelegatableAndUpdatableAndHostingWithHosting<Host> : RightlyCompleteDelegatableAndUpdatableAndHosting, IWithHosting<Host> where Host : IPrimaryHosting
    {
        public Host GetHost => host;
        [LabelText("托管核心", SdfIconType.Bullseye)]
        public Host host;
        public bool HasSubmit { get; set; }
        [ShowInInspector, LabelText("是否已经Submit")] public bool ShowHasSubmit => HasSubmit;
        public virtual bool OnSubmitHosting(Host hosting, bool asVirtual = false)
        {
            host = hosting;
            return true;
        }
        public virtual bool OnWithDrawHosting(Host hosting, bool asVirtual = false)
        {
            throw new System.NotImplementedException();
        }

        public bool TrySubmitHosting(Host hosting, bool asVirtual = false)
        {

            if (HasSubmit) return true;

            if (asVirtual)
            {
                if (hosting.virtualBeHosted != null)
                {
                    Debug.Log("加入虚拟列表");
                    if (!hosting.virtualBeHosted.Contains(this))
                    {
                        host = hosting;
                        hosting.virtualBeHosted.Add(this);
                        this.TryEnable();
                    }
                    return HasSubmit = true;
                }
                else
                {
                    Debug.Log("是空的");
                }
            }
            return HasSubmit = OnSubmitHosting(hosting, asVirtual);
        }

        public bool TryWithDrawHosting(Host hosting, bool asVirtual = false)
        {
            if (!HasSubmit) return false;
            if (asVirtual)
            {
                if (hosting.virtualBeHosted != null)
                {
                    if (hosting.virtualBeHosted.Contains(this))
                        hosting.virtualBeHosted.Remove(this);
                    HasSubmit = false;
                    return true;
                }
            }
            return HasSubmit = !OnWithDrawHosting(hosting, asVirtual);
        }
    }

}
