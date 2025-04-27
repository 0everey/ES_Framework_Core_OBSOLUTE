using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public class Architecture_ : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
    [Serializable,TypeRegistryItem("标准原型-拥有事件收发和配置")]
    public class BaseArchitectureWithLinkAndConfiguration : IArchitecture
    {
        
        [LabelText("配置")]public SoDataInfoConfiguration configuration;
        [LabelText("Link池")]public LinkRecieveSafeListIOC LinkIOC = new LinkRecieveSafeListIOC();
        public IEnumerable<IModule> normalBeHosted => throw new System.NotImplementedException();

        public string Name_ => description;

        public List<IModule> virtualBeHosted => throw new NotImplementedException();

        [LabelText("<<↑原型>>", SdfIconType.CpuFill),GUIColor("descriptionGUIColor_"), ShowInInspector,PropertyOrder(-1)]
        public string description="标准原型";
        [HideInInspector]public Color descriptionGUIColor_ =new Color(1f,0.588f,1f,1);
        public void AddRecieveLink<Link>(IReceiveLink<Link> link) where Link:ILink
        {
            LinkIOC.AddElement<Link>(link);
        }
        public void RemoveRecieveLink<Link>(IReceiveLink<Link> link) where Link : ILink
        {
            LinkIOC.RemoveElement<Link>(link);
        }
        public void SendLink<Link>(Link link)where Link:ILink
        {
            LinkIOC.SendLink(link);
        }



        public void AddHandle(IModule i, object withKey = null)
        {
            
        }

        public void RemoveHandle(IModule i, object withKey = null)
        {
           
        }
    }
    //一个架构内的Message，具有一组Link，一组针池
    //原型具有一个 Link的IOC 
    [Serializable, TypeRegistryItem("标准原型-键池")]
    public class BaseArchitectureWithKeyValuePool : IArchitecture,IInittable
    {
        public string Name_ => description;
        [LabelText("<<↑原型>>", SdfIconType.CpuFill), GUIColor("descriptionGUIColor_"), ShowInInspector, PropertyOrder(-1)]
        public string description = "标准键针池原型";
        [HideInInspector] public Color descriptionGUIColor_ = new Color(1f, 0.588f, 1f, 1);
        public IEnumerable<IModule> normalBeHosted => throw new NotImplementedException();

        public List<IModule> virtualBeHosted => throw new NotImplementedException();

        [LabelText("键值参数池"),SerializeReference]
        public ArchitectureKeyValuePoolTypeListIOC KeyValueParamIOC = new ArchitectureKeyValuePoolTypeListIOC();
        [LabelText("参数初始化数据包")]
        public ArchitectureDataPack applyPack;
        public void AddHandle(IModule i, object withKey = null)
        {
            
        }

        public void RemoveHandle(IModule i, object withKey = null)
        {
            
        }
        [DetailedInfoBox("为何是预览??","IOC不支持持久序列化，仅支持运行时，所以已经准备好一个数据包，在Start时载入数据来初始化,这样做的话也有利于一套数据多次使用，并且有望实现减少运行时数据量(只存动态标签即可)",InfoMessageType.Warning)]
        [Button("初始化预览")]
        public void Init(params object[] ps)
        {
            KeyValueParamIOC ??= new ArchitectureKeyValuePoolTypeListIOC();
            //
            if (!KeyValueParamIOC.IOC.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.DynamicTag))
            {
                KeyValueParamIOC.IOC.Add( EnumCollect.ArchitectureKeyValuePoolType.DynamicTag,new List<IArchitectureKeyValuePoolTypeValue>());
            }
            if (!KeyValueParamIOC.IOC.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.FloatValue))
            {
                KeyValueParamIOC.IOC.Add(EnumCollect.ArchitectureKeyValuePoolType.FloatValue, new List<IArchitectureKeyValuePoolTypeValue>());
            }
            if (!KeyValueParamIOC.IOC.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.IntValue))
            {
                KeyValueParamIOC.IOC.Add(EnumCollect.ArchitectureKeyValuePoolType.IntValue, new List<IArchitectureKeyValuePoolTypeValue>());
            }
            if (!KeyValueParamIOC.IOC.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.BoolValue))
            {
                KeyValueParamIOC.IOC.Add(EnumCollect.ArchitectureKeyValuePoolType.BoolValue, new List<IArchitectureKeyValuePoolTypeValue>());
            }
            if (!KeyValueParamIOC.IOC.ContainsKey(EnumCollect.ArchitectureKeyValuePoolType.StringValue))
            {
                KeyValueParamIOC.IOC.Add(EnumCollect.ArchitectureKeyValuePoolType.StringValue, new List<IArchitectureKeyValuePoolTypeValue>());
            }
            if(applyPack != null)
            {
                foreach(var i in applyPack.allInfo)
                {
                    var arch = i.Value.getArch;
                    if(arch is ArchitectureTypeValue_DynamicTag tag)
                    {
                        KeyValueParamIOC.AddElement(EnumCollect.ArchitectureKeyValuePoolType.DynamicTag, tag);
                    }else if (arch is ArchitectureTypeValue_Float f)
                    {
                        KeyValueParamIOC.AddElement(EnumCollect.ArchitectureKeyValuePoolType.FloatValue, f);
                    }
                    else if (arch is ArchitectureTypeValue_Int int_)
                    {
                        KeyValueParamIOC.AddElement(EnumCollect.ArchitectureKeyValuePoolType.IntValue, int_);
                    }
                    else if (arch is ArchitectureTypeValue_String str)
                    {
                        KeyValueParamIOC.AddElement(EnumCollect.ArchitectureKeyValuePoolType.StringValue, str);
                    }
                    else if (arch is ArchitectureTypeValue_Bool bo)
                    {
                        KeyValueParamIOC.AddElement(EnumCollect.ArchitectureKeyValuePoolType.BoolValue, bo);
                    }
                  /*  else if (arch is ArchitectureTypeValue_DynamicTag g)
                    {
                        KeyValueParamIOC.AddElement(EnumCollect.ArchitectureKeyValuePoolType.DynamicTag, arch);
                    }*/
                }
            }
        }

       
    }
    public interface IArchitecture : IPrimaryHosting
    {
        public abstract string Name_ { get; }
    }
    
    public interface IWithArchitecture 
    {
        public abstract IArchitecture GetArchitecture { get; }
    }
}

#region 测试圆形


#endregion