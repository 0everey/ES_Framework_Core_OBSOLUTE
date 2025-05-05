using ES;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;


namespace ES
{
    public interface IDomain : IESHosting
    {
        //编辑器情况下的链接创建
        void RegesterAllButOnlyCreateRelationship(ICore core_);
    }
    public interface IDomain<Core_> : IDomain
    {
        //给Clip抽象定义用
        Core_ Core { get; }
        public void AddClip(IClip clip, bool selfInvoke = true);
        public void RemoveClip(IClip clip, bool selfInvoke = true);
    }
    public abstract class BaseDomain<Core_, Clip> : ESHostingMono<Clip>, IDomain<Core_> where Core_ : BaseCore where Clip : class, IClip, IESModule
    {
        #region 常规属性字段
        //剪影就是我的常规列表
        public override IEnumerable<Clip> NormalBeHosted => Clips.valuesNow_;
        public Core_ Core => core;
        [FoldoutGroup("扩展域原始"), LabelText("域功能解释", icon: SdfIconType.Palette), GUIColor("ColorGetter"), ShowInInspector, PropertyOrder(-100)] public ESReadMeClass readMe = new ESReadMeClass() { readMe = "这是一个扩展区域" };
        [FoldoutGroup("扩展域原始"), LabelText("链接的核", icon: SdfIconType.Water), ReadOnly, GUIColor("ColorGetter")] public Core_ core;
        [FoldoutGroup("扩展域原始"), LabelText("全部剪影"),OdinSerialize] public SafeUpdateList_EasyQueue_SeriNot_Dirty<Clip> Clips = new SafeUpdateList_EasyQueue_SeriNot_Dirty<Clip>();
        #endregion


        #region 初始化
        public void AwakeRegisterDomain(Core_ core)
        {
            if (core != null)
            {
                this.core = core;
                CreatRelationship();
                AwakeRegesterAllClip();
            }
        }
        //显性引用时
        protected virtual void CreatRelationship()
        {

        }
        //初始化注册时(一般不用改
        protected virtual void AwakeRegesterAllClip()
        {
            foreach (var i in NormalBeHosted)
            {
                i.TrySubmitHosting(this, false);
            }
        }
        #endregion

        protected override void Update()
        {
            Clips.Update();
            base.Update();
        }

        #region 辅助方法
        public void RegesterAllButOnlyCreateRelationship(ICore core_)
        {
            if (core_ is Core_ use)
            {
                this.core = use;
                CreatRelationship();
                foreach (var i in NormalBeHosted)
                {
                    i.SetDomainAndCreateRelationship(this);
                }
            }
        }
        //设置显示绑定核心的颜色
        protected virtual Color ColorGetter()
        {
            return Color.green;
        }
        #endregion


        #region 常用功能
        public void AddClip(IClip clip, bool selfInvoke = true)
        {
            if(clip is Clip use)
            {
                Clips.TryAdd(use);
                if (selfInvoke) clip.TrySubmitHosting(this, false);
            }
        }
        
        public void RemoveClip(IClip clip, bool selfInvoke = true)
        {
            if (clip is Clip use)
            {
                if (selfInvoke) clip.TryWithDrawHosting(this, false);
            }
        }
        public override void TryRemoveModuleAsNormal(Clip use)
        {
            Clips.TryRemove(use);
        }
        
        public T GetModule<T>()
        {
            foreach (var i in NormalBeHosted)
            {
                if (i is T t) return t;
            }
            return default;
        }
        #endregion

    }
}