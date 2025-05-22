using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace ES
{
    [Serializable,TypeRegistryItem("技能序列")]
    public class ReleasableSkillsSequence
    {
        public static ReleaseableSkillClip NullSkillCLip = new ReleaseableSkillClip() { name="这是NULL技能片段不可用，请点击右侧新建片段！！" };
        [HorizontalGroup("总数据")]

        [VerticalGroup("总数据/数据"), PropertyOrder(-2)][LabelText("技能总时间",SdfIconType.ClockFill)] public float skillDuration = 1f;
        [Space(5)]
       
        [Space(5)]

        [LabelText("预览全部序列"),HideInInspector] public List<ReleaseableSkillClip> AllClips = new List<ReleaseableSkillClip>() { new ReleaseableSkillClip() { triggerAtTime=0.25f } };
        [FoldoutGroup("总数据/数据/整个技能数据细节"),LabelText("前后摇曲线(<0.5视为)")] public AnimationCurve skillActiveCurve = AnimationCurve.Constant(0.1f,0.9f,1);
        [FoldoutGroup("总数据/数据/整个技能数据细节"), LabelText("技能绑定的ES状态配置")] public StateDataInfo bindingStateInfo; 
        [FoldoutGroup("总数据/数据/整个技能数据细节"), LabelText("词条")] public List<string> skillTags = new List<string>();
        [FoldoutGroup("总数据/数据/整个技能数据细节"), LabelText("冷却时间"), SerializeReference] public IPointerForFloat_Only coolDown = new PointerForFloat_Random30() { float_range = new Vector2(5,15) };

        [DisplayAsString(fontSize:50,Alignment = TextAlignment.Center), VerticalGroup("总数据/数据"),HideLabel]
        
        public string s = ".........";



        [InfoBox("空的技能序列！！", InfoMessageType.Warning, VisibleIf = "@GetClipsLength()==0")]

        [PropertyOrder(-1)]

        [BoxGroup("分栏目/当前", CenterLabel = true, LabelText = "【编辑单个片段】")]
        [InlineButton("Save", "保存")]
        [InlineButton("Next", "下一个")]
        [InlineButton("Last", "上一个")]
        [LabelText("当前选中", Text = "@GetShowName()"), OnValueChanged("OnChangeSlider"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCatcher"),
          PropertyRange(0, 5, MaxGetter = "GetSliderMax")]
        public int CurrentIndex;
      

        [HorizontalGroup("分栏目",width: 0.9f)]
        [ShowInInspector,PropertyOrder(-1),BoxGroup("分栏目/当前"),DisplayAsString(fontSize:25), HideLabel, GUIColor("GetShowStartEditColor")]
        public string _ss=> GetShowStartEditName();






       
        [BoxGroup("分栏目/当前"),HideLabel,InlineProperty]

        public ReleaseableSkillClip currentEditClip_ = null;
        
        [HorizontalGroup("分栏目", width: 0.1f)]
        [PropertySpace(15)]
        [BoxGroup("分栏目/片段操作按钮列")]
        [Button("在末尾新建",ButtonHeight =50), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForUpdating")]
        public void AddAtEnd() {
            AllClips.Add(new ReleaseableSkillClip() { name="新建片段"+(AllClips.Count+1) });
            HandleAndRefreshCurrentIndex();
        }
        [BoxGroup("分栏目/片段操作按钮列")]
        [PropertySpace(15)]
        [Button("在当前接着新建", ButtonHeight = 50), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForUpdating")]
        public void AddAtHere() {
            AllClips.Insert(CurrentIndex+1,new ReleaseableSkillClip() { name = "新建片段"+(CurrentIndex+2) });
            HandleAndRefreshCurrentIndex();
        }

        [BoxGroup("分栏目/片段操作按钮列")]
        [PropertySpace(15)]
        [Button("按触发时间排序", ButtonHeight = 50), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForUpdating")]
        public void Sort()
        {
            AllClips.Sort((le, ri) => { if (le.triggerAtTime >= ri.triggerAtTime) return 1;return -1; });
            HandleAndRefreshCurrentIndex();
        }

        [BoxGroup("分栏目/片段操作按钮列")]
        [PropertySpace(15)]
        [Button("删除", ButtonHeight = 50), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")]
        public void RemoveThis() {
            if(currentEditClip_!=null)
            AllClips.Remove(currentEditClip_);
            currentEditClip_ = null;
            HandleAndRefreshCurrentIndex();
        }
        #region 辅助
        private string GetShowName()
        {
            return "当前选中 第【" + (CurrentIndex+1) + "/" + GetClipsLength() + "】";
        }
        private string GetShowStartEditName()
        {
            if (AllClips.Count == 0) return "警告！！目前没有片段，下面的片段为NULL状态不可使用";
            return "开始编辑 第【" + (CurrentIndex + 1) + "/" + GetClipsLength() + "】 个片段";
        }
        private Color GetShowStartEditColor()
        {
            if (AllClips.Count == 0) return KeyValueMatchingUtility.ColorSelector.Color_02;
            return KeyValueMatchingUtility.ColorSelector.ColorForCatcher;
        }
        private int GetClipsLength()
        {
            if (AllClips == null)
            {
                AllClips = new List<ReleaseableSkillClip>() { new ReleaseableSkillClip() { name="新建技能片段1", triggerAtTime = 0.25f } };
            }else if (AllClips.Count == 0)
            {
                currentEditClip_ = NullSkillCLip;
            }
            
            return AllClips.Count;
        }
        private int GetSliderMax()
        {
            return GetClipsLength() - 1;
        }
        private void HandleAndRefreshCurrentIndex(bool reTry=false)
        {
            if (AllClips.Contains(currentEditClip_))
            {
                CurrentIndex = AllClips.IndexOf(currentEditClip_);
                return;
            }
            if (currentEditClip_ == null) { CurrentIndex = Mathf.Clamp(CurrentIndex, 0, GetClipsLength()-1); }
            else { currentEditClip_ = null; }

            OnChangeSlider();
        }
        public void OnChangeSlider()
        {
            if (AllClips.Count == 0) return;
            CurrentIndex = Mathf.Clamp(CurrentIndex,0,AllClips.Count - 1);
            currentEditClip_ = AllClips[CurrentIndex];
        }
        private void Next()
        {
            CurrentIndex++;
            OnChangeSlider();
        }
        private void Last(){
            CurrentIndex--;
            OnChangeSlider();
        }
        private void Save()
        {
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        #endregion
    }

    [Serializable, TypeRegistryItem("单个技能片段")]
    public class ReleaseableSkillClip
    {
        [HorizontalGroup("总数据")]
        [LabelText("该片段触发时机(秒)")] public float triggerAtTime = 0.25f;
        [PropertySpace(2,15)]
        [LabelText("该片段名字")] public string name = "新技能片段";
        


        [/*TabGroup("绑定状态与动画",true,TabName ="【绑定源】") ,*/GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForBinding")]
        [LabelText("切换状态")]
        [TabGroup("【绑定状态与动画】", TextColor = "@KeyValueMatchingUtility.ColorSelector.ColorForBinding")]
        public bool useStateSwitch = false;

        [LabelText("动画层级")]
        [TabGroup("【绑定状态与动画】", TextColor = "@KeyValueMatchingUtility.ColorSelector.ColorForBinding")]
        public int layer = 0;

        [LabelText("绑定原生状态")]
        [TabGroup("【绑定状态与动画】", TextColor = "@KeyValueMatchingUtility.ColorSelector.ColorForBinding")]
        public string st = "技能/技能1";

        /* [TabGroup("绑定状态与动画")]*/
        [TabGroup("【绑定状态与动画】")]
        [LabelText("过渡时间(归一化)")]
        public PointerForFloat_DirectClamp01 crossFade = new PointerForFloat_DirectClamp01() { @float = 0.2f };

        [Space(10)]
        [TabGroup("【绑定状态与动画】")]
        [LabelText("设置原生参数命令(待完善)")]
        public string FloatParamName = "float";





        [TabGroup("【筛选对象】", TextColor = "@KeyValueMatchingUtility.ColorSelector.ColorForSearch")]

        [LabelText("筛选继承机制"), Space(10)]
        public SelectorOverrideOptions optionForOverrideLast = SelectorOverrideOptions.UpdateAll;
        [TabGroup("【筛选对象】")][LabelText("筛选传递机制"), Space(10)]
        public SelectorNextOptions optionForNext = SelectorNextOptions.UpdateAll;
        [/*LabelText("筛选对象"),*/ SerializeReference]
        [Space(20)]
        [TabGroup("【筛选对象】"), HideLabel] public IPointerForSomeEntity_Only Selector = new SomeEntitySelectorFromSelf();






        [TabGroup("【作用效果】", TextColor = "@KeyValueMatchingUtility.ColorSelector.ColorForApply")]

        [LabelText("启用触发间隔")] public bool UseTimeDis = true;

        [TabGroup("【作用效果】")]
        [LabelText("触发间隔"),SerializeReference] public IPointerForFloat_Only TriggerTimeDis_ =new PointerForFloat_Direct(){ float_=0.25f};

        [TabGroup("【作用效果】")]
        [LabelText("排序模式(《总是系列禁用》)")] public EnumCollect.PathSortType sortType;
        

        

        [TabGroup("【作用效果】")]
        [/*LabelText("对筛选目标逐个执行"),*/InlineProperty,HideLabel]
        public EntityHandle Applier =new EntityHandle();


        #region 筛选继承机制
        
        public enum SelectorOverrideOptions
        {
            [InspectorName("完全重制")]UpdateAll,
            [InspectorName("直接使用上一级的")] DirectUse,
            [InspectorName("忽略Head继承并且重筛选")] IgnoreHeadAndReSelect,
        }
        public enum SelectorNextOptions
        {
            [InspectorName("更新覆盖")] UpdateAll,
            [InspectorName("不影响之后的")] DontEffectNext,
            [InspectorName("添加到以前的")] AddTo,
            [InspectorName("从以前的中移除")] RemoveFrom,
            [InspectorName("清空")]ClearAll
        }
        #endregion
    }


}
