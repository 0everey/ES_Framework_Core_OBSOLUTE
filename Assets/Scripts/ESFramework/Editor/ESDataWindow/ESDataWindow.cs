using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using ES;
using Sirenix.Serialization;
using ES.EvPointer;
using System.IO;
using static ES.EvWindowDataAndTool;
using UnityEngine.UIElements;
using System.CodeDom.Compiler;

namespace ES
{
    //窗口总览
    public class ESDataWindow : ESWindowBase_Abstract<ESDataWindow>
    {
        [MenuItem("Tools/ES工具/ES数据窗口")]
        public static void TryOpenWindow()
        {
            OpenWindow();
        }
        public override void ESWindow_OpenHandle()
        {
            base.ESWindow_OpenHandle();
            if (usingWindow.HasDelegate)
            {
                //已经注册委托
            }
            else
            {
                usingWindow.DelegateHandle();
            }
        }
        #region 数据滞留
        public string dataConfigurePageMenuNameAs => "数据生成工具";
        public ESWindowPageBase pageForSodataGroup;
        public ESWindowPageBase pageForSodataPack; 
        [NonSerialized] public Page_Root_StartUse pageForStartUse;
        [NonSerialized]public Page_CreateNewDataConfiguration pageForConfiguration;
        [NonSerialized] public Page_CreateNewInfoDataPack pageForNewInfoDataPack;
        [NonSerialized] public Page_CreateNewInfoGroup pageForNewInfoDataGroup;
        [NonSerialized] public Page_RunTimeGameObjectChooseSceneArchitecture pageForChooseSceneArchitecture;
        public EvWindowDataAndTool.DataType selectPackType_ = DataType.Buff;
        public EvWindowDataAndTool.DataType selectGroupType_ = DataType.Buff;
        private bool HasDelegate = false;
        private int rememberIOCCount = 0;
        #endregion
        /// <summary>
        /// 工具栏打开窗口捏
        /// </summary>
        [MenuItem("Tools/ES工具/ES窗口")]
       
        private void DelegateHandle()
        {
            HasDelegate = true;
            rememberIOCCount = GameCenterManager.Instance.ArchutectureIOC.IOC.Count;
            GameCenterManager.Instance.ArchutectureIOC.OnChange += () => {

                GameCenterManager.Instance.StartCoroutine(CoroutineMaker.DelayOneFrameCoroutine(() => { if(this!=null)ESWindow_RefreshWindow(); }));
            };
        }
        
        /// <summary>
        /// 自动回访滞留窗口和刷新
        /// </summary>
        protected override void OnImGUI()
        {
            if (usingWindow == null)
            {
                usingWindow = this;
            }
            if (usingWindow != null)
            {
                if (pageForSodataGroup != null)
                {
                    if (pageForSodataGroup.shouldRemake())
                    {
                        ESWindow_RefreshWindow();
                    }
                }
                if (pageForChooseSceneArchitecture != null)
                {
                    pageForChooseSceneArchitecture.shouldRemake();
                }
                int now = GameCenterManager.Instance.ArchutectureIOC.IOC.Count;
                if (now != rememberIOCCount)
                {
                    rememberIOCCount = now;
                    ESWindow_RefreshWindow();
                }


            }
            base.OnImGUI();
        }
        /// <summary>
        /// 刷新窗口
        /// </summary>
        public override void ESWindow_RefreshWindow()
        {
            base.ESWindow_RefreshWindow();
            ES_SaveData();
        }
        public string pathForDefaultSetting;
        public override void ES_LoadData()
        {
            Debug.Log("加载");
            if (EvWindowDataAndTool.HasNull(pageForConfiguration.configuration))
            {
                if (PlayerPrefs.HasKey("configuration"))
                {
                    Debug.Log("加载1");
                    string path = PlayerPrefs.GetString("configuration");
                    pageForConfiguration.configuration = AssetDatabase.LoadAssetAtPath<SoDataInfoConfiguration>(path);
                }
               
            }
        }
        public override void ES_SaveData()
        {
            if (EvWindowDataAndTool.AllIsOk(pageForConfiguration.configuration))
            {
                Debug.Log("保存1");
                string path = AssetDatabase.GetAssetPath(pageForConfiguration.configuration);
                PlayerPrefs.SetString("configuration", path);
            } 
        }
        protected override void ES_BuildMenuTree(OdinMenuTree tree)
        {
            base.ES_BuildMenuTree(tree);
            Part_BuildStartPage(tree);
            {
                tree.Add(dataConfigurePageMenuNameAs, new PageRoot_DataTool(), icon: SdfIconType.Braces);
                Part_BuildSoDataConfigureSettingPage(tree);
                Part_BuildSoPackPage(tree);
                Part_BuildSoDataGroupSettingsPage(tree);
            }

            Part_BuildArchutectureShowerPage(tree);
            Part_EasyToolsPage(tree);
            Part_AboutPage(tree);

            ES_LoadData();
            pageForConfiguration.Setup();
        }
        
        private void Part_BuildStartPage(OdinMenuTree tree)
        {
            tree.Add("开始使用！", pageForStartUse??=new Page_Root_StartUse(),SdfIconType.SunFill);
        }
        private void Part_BuildSoDataConfigureSettingPage(OdinMenuTree tree)
        {
            string Menu = $"{dataConfigurePageMenuNameAs}/数据配置:汇总设置";
            tree.Add(Menu, pageForConfiguration ?? (pageForConfiguration = new Page_CreateNewDataConfiguration()), SdfIconType.CodeSlash);

            var all = AssetDatabase.FindAssets("t:ScriptableObject");

            foreach (var i in all)
            {
                GUID id = default; GUID.TryParse(i, out id);
                Type type = AssetDatabase.GetMainAssetTypeFromGUID(id);

                if (typeof(SoDataInfoConfiguration).IsAssignableFrom(type))
                {
                    string path = AssetDatabase.GUIDToAssetPath(id);
                    SoDataInfoConfiguration ob = AssetDatabase.LoadAssetAtPath(path, type) as SoDataInfoConfiguration;
                    tree.Add($"{Menu}/{ob.name}",new Page_Index_QuickSeeDataConfuration() {  configuration=ob}, SdfIconType.Code);

                }
            }
        

                }
        private void Part_BuildSoPackPage(OdinMenuTree tree)
        {
            string Menu = $"{dataConfigurePageMenuNameAs}/数据包:汇总设置";
            Page_CreateNewInfoDataPack pagePack = default;
            tree.Add(Menu, pageForSodataPack = pagePack = (pageForNewInfoDataPack??= new Page_CreateNewInfoDataPack() { createPackType_ = selectPackType_ }),SdfIconType.BoxSeam);
            var all = AssetDatabase.FindAssets("t:ScriptableObject");

            foreach (var i in all)
            {
                GUID id = default; GUID.TryParse(i, out id);
                Type type = AssetDatabase.GetMainAssetTypeFromGUID(id);

                if (typeof(ISoDataPack).IsAssignableFrom(type))
                {
                    string path = AssetDatabase.GUIDToAssetPath(i);
                    ScriptableObject scriptable = AssetDatabase.LoadAssetAtPath(path, type) as ScriptableObject;
                    ISoDataPack pack_ = scriptable as ISoDataPack;
                    if (pack_.getSoType() == EvWindowDataAndTool.TargetInfoType(pagePack.createPackType_))
                    {
                        tree.Add(Menu + $"/key：{pack_.name_} file:{scriptable.name}",  new Page_Index_DataInfoPack() { pack = pack_ },SdfIconType.Box);
                    }

                }
            }
        }
        private void Part_BuildSoDataGroupSettingsPage(OdinMenuTree tree)
        {
            if (Selection.activeObject is ISoDataGroup group_)
            {
                string menuName = $"{dataConfigurePageMenuNameAs}/数据组：配置组>";
                tree.Add(menuName, pageForSodataGroup = new Page_DataInfoGroup() { group = group_ },SdfIconType.BagCheckFill);
                var datagroup = pageForSodataGroup as Page_DataInfoGroup;
                foreach (var i in group_.keys)
                {
                    ISoDataInfo so = group_.GetOne(i);

                    if (so != default)
                    {

                        tree.Add($"{menuName}/{i} 单元", new Page_Index_DataInfoSingle() { data = so, infoGroup = datagroup },SdfIconType.Book); ;
                    }

                }
                datagroup.Check();
            }
            else
            {
                string Menu = $"{dataConfigurePageMenuNameAs}/数据组：汇总设置";
                //啥也没有
                Page_CreateNewInfoGroup pageGroup = default;
                tree.Add(Menu, pageForSodataGroup = pageGroup = pageForNewInfoDataGroup??= new Page_CreateNewInfoGroup() { createGroup_ = selectGroupType_ },SdfIconType.BagPlusFill);

                var all = AssetDatabase.FindAssets("t:ScriptableObject");

                foreach (var i in all)
                {
                    GUID id = default; GUID.TryParse(i, out id);
                    Type type = AssetDatabase.GetMainAssetTypeFromGUID(id);

                    if (typeof(ISoDataGroup).IsAssignableFrom(type))
                    {
                        string path = AssetDatabase.GUIDToAssetPath(i);
                        ScriptableObject scriptable = AssetDatabase.LoadAssetAtPath(path, type) as ScriptableObject;
                        ISoDataGroup pack_ = scriptable as ISoDataGroup;
                        if (pack_.getSoType() == EvWindowDataAndTool.TargetInfoType(pageGroup.createGroup_))
                        {
                            tree.Add(Menu + $"/key：{pack_.name_} file:{scriptable.name}", new Page_Index_DataQuickSee() { group = scriptable },SdfIconType.Bag); ;
                        }

                    }
                }

            }
        }

        private void Part_BuildArchutectureShowerPage(OdinMenuTree tree)
        {
            string menu = "原型工具";
            tree.Add(menu, new Page_Root_Architecture(), SdfIconType.CpuFill);
            tree.Add(menu + "/选中物体携带原型",pageForChooseSceneArchitecture= new Page_RunTimeGameObjectChooseSceneArchitecture(),SdfIconType.CompassFill);
            //场景中的IOC
            tree.Add(menu + "/IOC运行时全集", new Page_RunTimeInGameCenterArchitectureIOC() { IOC = GameCenterManager.Instance.ArchutectureIOC },SdfIconType.Cart4);
            if (Application.isPlaying)
            {
                //运行时才会加
                foreach(var i in GameCenterManager.Instance.ArchutectureIOC.IOC)
                {
                    List<IArchitecture> architectures_ = i.Value.valuesNow_;
                    string menuNow = menu + "/IOC运行时全集/类型分类：" + i.ToString();
                    tree.Add(menuNow, new Page_Index_RunTimeInGameCenterArchitectureIOC_TypeSelect() { architectures = architectures_ }, SdfIconType.Hdd); ;
                    if (architectures_ != null)
                    {
                        foreach(var ii in architectures_)
                        {
                            tree.Add(menuNow + "/单元原型" + ii.Name_, new Page_Index_Index_RunTimeInGameCenterArchitectureIOC_SIngleShow() { aArchitecture = ii }, SdfIconType.Cpu); ;
                        }
                    }
                }
            }

            //场景中的静态原型工具


        }
        private void Part_EasyToolsPage(OdinMenuTree tree)
        {

        }
        private void Part_AboutPage(OdinMenuTree tree)
        {
            tree.Add("关于", new Page_About(),SdfIconType.ChatSquareQuoteFill);
        }
    }
    #region 开始
    //开始使用界面
    public class Page_Root_StartUse
    {

    }
    #endregion
    #region 数据工具
    //创建 数据工具 总页面
    public class PageRoot_DataTool
    {
        [DisplayAsString(fontSize:30),HideLabel]
        public string readMe = "数据层级分为DataInfo(单元),DataGroup(组),DataPack(包),现在开始填表来创建新的数据类型";
        [LabelText("英文数据代码名(省略Info)")]
        public string EnglishCodeName = "DataName";
        [LabelText("中文数据显示名(省略数据单元)")]
        public string ChineseDisplayName = "数据名";
        [LabelText("数据父文件夹"), FolderPath]
        public string folder = "Assets/Scripts/ESFramework/Data/DataToolScript";
        [Button("生成")]
        public void GenerateData()
        {
            if (AssetDatabase.IsValidFolder(folder)) {
                string toInfo = folder + "/InfoType";
                string toGroup = folder + "/GroupType";
                string toPack = folder + "/PackType";
                //查/创建文件夹↓
                {
                    if (!AssetDatabase.IsValidFolder(toInfo))
                    {
                        AssetDatabase.CreateFolder(folder, "InfoType");
                    }
                    if (!AssetDatabase.IsValidFolder(toGroup))
                    {
                        AssetDatabase.CreateFolder(folder, "GroupType");
                    }
                    if (!AssetDatabase.IsValidFolder(toPack))
                    {
                        AssetDatabase.CreateFolder(folder, "PackType");
                    }
                }
                Debug.Log("完毕");
                string infoName= EnglishCodeName + "DataInfo";
                ESTool_ScriptMaker.Instance.CreateScript(toInfo, infoName, parent:": SoDataInfo");
                ESTool_ScriptMaker.Instance.CreateScript(toGroup, EnglishCodeName+"DataGroup", parent: $": SoDataGroup<{infoName}>");
                ESTool_ScriptMaker.Instance.CreateScript(toPack, EnglishCodeName+"DataPack", parent: $": SoDataPack<{infoName}>");
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.DisplayDialog("请选择正确的文件夹", "建议使用【Assets/Scripts/ESFramework/Data/DataToolScript】作为生成总路径哦", "知道了");
            }
        }
    }
    //创建 数据配置 总页面
    public class Page_CreateNewDataConfiguration :ESWindowPageBase
    {
        [TabGroup("总", "设置全局默认配置")]
        [LabelText("默认全局配置"),AssetSelector,OnValueChanged("EditDefaultConfiguration"),GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public SoDataInfoConfiguration configuration;
        [InlineEditor(Expanded =true),ShowInInspector,LabelText("展示内容")]
        [TabGroup("总", "设置全局默认配置")] 
        public SoDataInfoConfiguration show => configuration;
        private SoDataInfoConfiguration cache;
        private void EditDefaultConfiguration()
        {
            //
            
            //不为空
            if (configuration != null)
            {
                
                if (EditorUtility.DisplayDialog("更改全局配置",$"您正在更换新的配置{configuration.name}，这将会应用到游戏管理器影响整个游戏的运行，是否确定","更换配置","取消"))
                {
                    cache = configuration;
                }
                else
                {
                    configuration = cache;
                }
            }
            //为空
            else
            {
                if (EditorUtility.DisplayDialog("全局配置清空", $"您正在尝试置空配置{cache?.name??"无"}，建议您更换而不是直接取消否则后果可能很严重，是否确定", "置空配置", "取消"))
                {

                }
                else
                {
                    configuration = cache;
                }
            }
            ESDataWindow.usingWindow.ES_SaveData();
            var all = UnityEngine.Object.FindObjectsByType<GameCenterManager>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            foreach (var i in all)
            {
                if (i.GameCenterArchitecture != null)
                {
                    i.GameCenterArchitecture.configuration = configuration;
                }
            }
            
        }
        [ShowIfGroup("总/新建配置/安全", VisibleIf = "AllIsOk"),HideLabel,DisplayAsString]
        public string readme2 = "";
        [InfoBox("全部完备！可以安全创建数据配置!!", infoMessageType: InfoMessageType.Info)]
        [ShowIfGroup("总/新建配置/安全")]
        [Button("安全创建",ButtonHeight =50,Icon = SdfIconType.Check2Circle),GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        private void CreateConfiguration_OK()
        {
            Type targetType  =typeof(SoDataInfoConfiguration);
            if (AssetDatabase.IsValidFolder(folder) && targetType != default)
            {
                SoDataInfoConfiguration @object = ScriptableObject.CreateInstance(targetType) as SoDataInfoConfiguration;
                
                @object.name = fileName +(hasChange?"": UnityEngine.Random.Range(0, 99999));
                @object.PackForBuff = PackForBuff;
                @object.PackForSkill = PackForSkill;
                @object.PackForItem = PackForItem;
                @object.PackForActor = PackForActor;
                @object.PackForEventMessage = PackForEventMessage;
                @object.PackForRune = PackForRune;
                @object.PackForRoomGenerate = PackForRoomGenerate;
                AssetDatabase.CreateAsset(@object, folder + "/" + @object.name + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Selection.activeObject = @object;
                ESDataWindow.usingWindow.ES_SaveData();
            }
            else
            {
                Debug.LogError("非法文件夹路径或者类型错误！！");
            }

        }
        [InfoBox("请尽量装填完整的数据包再创建!!", infoMessageType: InfoMessageType.Warning)]

        [ShowIfGroup("总/新建配置/强制", VisibleIf = "HasNULL")]
        [LabelText("尝试自动装填？")]
        public bool enabledAutoSeacrch = true;
        [ShowIfGroup("总/新建配置/强制")]
        [Button("强制创建", ButtonHeight = 30, Icon = SdfIconType.PatchExclamationFill), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")]
        private void CreateConfiguration_Force()
        {
            if (enabledAutoSeacrch) AutoIn();
            //创建
            CreateConfiguration_OK();
        }
        [ShowIfGroup("总/新建配置/强制")]
        [Button("自动装填", ButtonHeight = 30, Icon = SdfIconType.BoxArrowInUpLeft), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_02")]
        private void AutoIn()
        {
            PackForBuff ??= EvWindowDataAndTool.SearchPack(typeof(BuffSoInfo)) as BuffDataPack;
            PackForSkill ??= EvWindowDataAndTool.SearchPack(typeof(SkillDataInfo)) as SkillDataPack;
            PackForActor ??= EvWindowDataAndTool.SearchPack(typeof(ActorDataInfo)) as ActorDataPack;
            PackForItem ??= EvWindowDataAndTool.SearchPack(typeof(ItemDataInfo)) as ItemDataPack;
            PackForEventMessage ??= EvWindowDataAndTool.SearchPack(typeof(GameEventMessageDataPack)) as GameEventMessageDataPack;
            PackForRune ??= EvWindowDataAndTool.SearchPack(typeof(RuneDataPack)) as RuneDataPack;
            PackForRoomGenerate ??= EvWindowDataAndTool.SearchPack(typeof(RoomGenerateDataPack)) as RoomGenerateDataPack;
        }

        private bool HasNULL()
        {
            return EvWindowDataAndTool.HasNull(PackForBuff, PackForActor, PackForSkill, PackForItem);
        }
        private bool AllIsOk()
        {
            return EvWindowDataAndTool.AllIsOk(PackForBuff, PackForActor, PackForSkill, PackForItem);
        }
        [TabGroup("总", "新建配置"), HideLabel, Indent(5)]
        [DisplayAsString(fontSize: 30), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        public string readme = "↓修改配置文件名";


       
        private bool hasChange = false;
        private void ChangeHappen()
        {
            hasChange = true;
        }
        [InfoBox("建议修改一下文件名",infoMessageType:InfoMessageType.Warning,VisibleIf = "@!hasChange")]
        [TabGroup("总", "新建配置"),Space(5), OnValueChanged("ChangeHappen"),LabelText("设置文件名")]
        public string fileName = "数据配置文件";
        [FolderPath,LabelText("选择文件夹")]
        [TabGroup("总", "新建配置"), Space(5)]
        public string folder = "Assets/Scripts/ESFramework/Data";

        [TabGroup("总","新建配置"),HideLabel,Indent(5)]
        [DisplayAsString(fontSize:30),GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        public string con = "↓手动配置各种数据包";
        [LabelText("Buff效果数据包"),TabGroup("总","新建配置"),InfoBox("装填Buff效果包",InfoMessageType.Warning,Icon =SdfIconType.Box),AssetSelector,GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public BuffDataPack PackForBuff;
        [LabelText("Skill技能数据包"), TabGroup("总", "新建配置"), InfoBox("装填Skill技能包", InfoMessageType.Warning, Icon = SdfIconType.Box), AssetSelector, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public SkillDataPack PackForSkill;
        [LabelText("Actor人物数据包"), TabGroup("总", "新建配置"), InfoBox("装填人物数据包", InfoMessageType.Warning, Icon = SdfIconType.Box), AssetSelector, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public ActorDataPack PackForActor;
        [LabelText("Item物品数据包"), TabGroup("总", "新建配置"), InfoBox("装填物品数据包", InfoMessageType.Warning, Icon = SdfIconType.Box), AssetSelector, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public ItemDataPack PackForItem;
        [LabelText("EventMessage事件数据数据包"), TabGroup("总", "新建配置"), InfoBox("装填事件数据包", InfoMessageType.Warning, Icon = SdfIconType.Box), AssetSelector, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public GameEventMessageDataPack PackForEventMessage;
        [LabelText("Rune符文数据包"), TabGroup("总", "新建配置"), InfoBox("装填符文数据包", InfoMessageType.Warning, Icon = SdfIconType.Box), AssetSelector, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public RuneDataPack PackForRune;
        [LabelText("RoomGenerate房间生成数据包"), TabGroup("总", "新建配置"), InfoBox("装填房间生成数据包", InfoMessageType.Warning, Icon = SdfIconType.Box), AssetSelector, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public RoomGenerateDataPack PackForRoomGenerate;

        public override void Setup()
        {
            base.Setup();
            if (configuration != null) cache = configuration;
        }
    }
    //单个配置快速查看和配置
    public class Page_Index_QuickSeeDataConfuration
    {
        [HorizontalGroup("总组")]
        [VerticalGroup("总组/数据")]
        [InlineEditor(Expanded =true),LabelText("配置文件")]
        public SoDataInfoConfiguration configuration;
        [HorizontalGroup("总组", width: 100)]
        
        [VerticalGroup("总组/按钮")]
        [Button("设为默认数据配置"),GUIColor("@ KeyValueMatchingUtility.ColorSelector.Color_03")]
        public void SetToBeDefaultConfigure()
        {

        }
    }
    //创建数据包总页面
    [Serializable]
    public class Page_CreateNewInfoDataPack : ESWindowPageBase
    {
        [Title("开始新建数据包！", "可以先选择预设类型", bold: true, titleAlignment: TitleAlignments.Centered)]

        [HorizontalGroup("总组")]
        [DisplayAsString(fontSize: 30), HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        [VerticalGroup("总组/数据")]
        public string createText = "创建新数据包";
        [LabelText("搜集的数据组"), SerializeReference, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        [VerticalGroup("总组/数据")]
        public List<ISoDataGroup> CachingAddGroups = new List<ISoDataGroup>();


        [DetailedInfoBox("创建一个数据包包含大量数据！", "创建一个数据包！！将会支持Buff,技能,人物,物品等", infoMessageType: InfoMessageType.Warning)]
        [InfoBox("请修改一下文件名否则会分配随机数字后缀", VisibleIf = "@!hasChange", InfoMessageType = InfoMessageType.Warning)]
        [VerticalGroup("总组/数据"), LabelText("文件命名"), Space(5), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04"), OnValueChanged("ChangeHappen")]
        public string createName_ = "Buff新建数据包";
        private bool hasChange = false;
        private void ChangeHappen()
        {
            hasChange = true;

        }
        [FolderPath, OnValueChanged("Refresh")]
        [VerticalGroup("总组/数据"), LabelText("保存文件夹"), Space(5), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")]
        public string FolderPath_ = "Assets/Scripts/ESFramework/Data";
        private void Refresh()
        {
            AssetDatabase.Refresh();

        }

        [DisplayAsString(fontSize: 30), HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        [VerticalGroup("总组/数据")]
        public string createTypeText = "创建新数据包或者查询数据包";


        [OnValueChanged("ResetConfigure"), InfoBox("建议选择一个预设类型的数据包,或者自己创建支持,默认类型无法直接使用", infoMessageType: InfoMessageType.Warning, VisibleIf = "@createPackType_==EvWindowDataAndTool.DataType.None")]
        [VerticalGroup("总组/数据"), Space(5), LabelText("预定义类型")]
        public EvWindowDataAndTool.DataType createPackType_ = EvWindowDataAndTool.DataType.Buff;
        [NonSerialized, OdinSerialize, ShowInInspector, ShowIfGroup("总组/数据/s", VisibleIf = "@createPackType_==EvWindowDataAndTool.DataType.None"), LabelText("自定义类型"), Space(5), TypeSelectorSettings(FilterTypesFunction = "TypeSelectorSettingForDataPack")]
        public Type selfDefineType;
        [HorizontalGroup("总组", width: 100)]
        [VerticalGroup("总组/按钮")]
        [PropertySpace(15)]
        [Button(ButtonHeight = 20, Name = "创建数据包", Icon = SdfIconType.Check2Square), GUIColor("@ KeyValueMatchingUtility.ColorSelector.Color_03")]
        public void CreateInfoPackAsset()
        {
            Type targetType = EvWindowDataAndTool.TargetPackType(createPackType_, selfDefineType);
            if (AssetDatabase.IsValidFolder(FolderPath_) && targetType != default)
            {
                ScriptableObject @object = ScriptableObject.CreateInstance(targetType);
                if (@object is ISoDataPack pack)
                {
                    if (CachingAddGroups != null)
                    {
                        foreach (var i in CachingAddGroups)
                        {
                            if (i != null)
                                pack.AddGroup(i);
                        }
                    }
                }
                @object.name = createName_ + (hasChange ? "" : UnityEngine.Random.Range(0, 99999));
                AssetDatabase.CreateAsset(@object, FolderPath_ + "/" + @object.name + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Selection.activeObject = @object;
            }
            else
            {
                Debug.LogError("非法文件夹路径或者类型错误！！");
            }
        }


        [VerticalGroup("总组/按钮")]
        [PropertySpace(15)]
        [Button(ButtonHeight = 20, Name = "搜集同类数据组"), GUIColor("@ KeyValueMatchingUtility.ColorSelector.Color_04")]
        public void FindAInfoGroupAsset()
        {
            //找到全部数据组
            var all = AssetDatabase.FindAssets("t:ScriptableObject");
            CachingAddGroups = new List<ISoDataGroup>();
            foreach (var i in all)
            {
                GUID id = default; GUID.TryParse(i, out id);
                Type type = AssetDatabase.GetMainAssetTypeFromGUID(id);

                if (typeof(ISoDataGroup).IsAssignableFrom(type))
                {

                    string path = AssetDatabase.GUIDToAssetPath(id);
                    UnityEngine.Object ob = AssetDatabase.LoadAssetAtPath(path, type);
                    if (ob == null) continue;
                    ISoDataGroup dataGroup = ob as ISoDataGroup;
                    if (dataGroup.getSoType() == TargetInfoType(createPackType_))
                    {
                        CachingAddGroups.Add(dataGroup);
                    }
                }
            }
        }
        private void ResetConfigure()
        {
            hasChange = false;
            ESDataWindow.usingWindow.selectPackType_ = createPackType_;
            ESDataWindow.usingWindow.ESWindow_RefreshWindow(); 
            createName_ = EvWindowDataAndTool.RenamePackByEnumType(createPackType_);
        }

        public override bool shouldRemake()
        {
            if (Selection.activeObject is ISoDataPack group2)
            {
                return true;
            }
            return base.shouldRemake();
        }
        private bool TypeSelectorSettingForDataPack(Type type)
        {
            return !type.IsAbstract && !type.IsInterface && typeof(ISoDataPack).IsAssignableFrom(type);
        }
    }
    //子层 搜集的数据包页面
    [Serializable]
    public class Page_Index_DataInfoPack : ESWindowPageBase
    {
        [HorizontalGroup("总组")]

        [Title("开始配置数据包！!", "数据包可以把一系列数据组整合起来使用", titleAlignment: TitleAlignments.Centered, Title = @"@  ""开始配置数据包:※ 【"" + pack.name_  + ""】""   ")]
        [VerticalGroup("总组/数据包")]

        [DisplayAsString(fontSize: 30),ShowInInspector, HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        [VerticalGroup("总组/数据包"),PropertyOrder(-1)]
        public string addText =>EvWindowDataAndTool.RenamePackWantWantGroup(EvWindowDataAndTool.TargetEnum(pack.getSoType()));
           
        [VerticalGroup("总组/数据包"),AssetSelector,SerializeReference, LabelText("拖入用于缓冲的数据组")]
        public List<ISoDataGroup> soInfos = new List<ISoDataGroup>();
        [DisplayAsString(fontSize: 30), HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        [VerticalGroup("总组/数据包")]
        public string showText = "查看数据包详情";
        [InlineEditor(Expanded = true), SerializeReference, LabelText("数据包"), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        [VerticalGroup("总组/数据包")]
        public ISoDataPack pack;
        [HorizontalGroup("总组", width: 100)]
        [VerticalGroup("总组/按钮组")]
        [PropertySpace(15)]
        [Button("自动搜集全部数据组", ButtonHeight = 20), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public void SearchAllGroup()
        {
            //找到全部数据组
            var all = AssetDatabase.FindAssets("t:ScriptableObject");
            soInfos = new List<ISoDataGroup>();
            foreach (var i in all)
            {
                GUID id = default; GUID.TryParse(i, out id);
                Type type = AssetDatabase.GetMainAssetTypeFromGUID(id);

                if (typeof(ISoDataGroup).IsAssignableFrom(type))
                {
                    string path = AssetDatabase.GUIDToAssetPath(id);
                    UnityEngine.Object ob = AssetDatabase.LoadAssetAtPath(path, type);
                    if (ob == null) continue;
                    ISoDataGroup dataGroup = ob as ISoDataGroup;
                    if (dataGroup.getSoType() == pack.getSoType())
                    {
                        soInfos.Add(dataGroup);
                    }
                }
            }

        }
        [VerticalGroup("总组/按钮组")]
        [PropertySpace(15)]
        [Button("缓冲入数据组", ButtonHeight = 20), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")]
        public void PushInDataGroup()
        {
            
            if (soInfos != null)
            {
                
                Undo.RecordObject(this.pack as ScriptableObject, "this");
                foreach (var i in soInfos)
                {
                    if (i == null ) return;
                    if (i is not ScriptableObject) return;
                    
                    if (pack.getSoType() == i.getSoType())
                    {
                        pack.AddGroup(i);
                    }
                    else
                    {
                        Debug.LogError("数据组" + i.name_ + "的类型不合适或者已经销毁");
                    }

                }
                Undo.RecordObject(this.pack as ScriptableObject, "this");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
            }
            
            /* string s = @"@  ""开始配置数据组！！ 【"" + group.name_  + ""】""   ";*/

        }


        [VerticalGroup("总组/按钮组")]
        [PropertySpace(15)]
        [Button("设置为默认数据包", ButtonHeight = 20), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public void Collect()
        {

        }
        public override bool shouldRemake()
        {
            return false;
        }
    }
    //数据组创建页面
    [Serializable]
    public class Page_CreateNewInfoGroup : ESWindowPageBase
    {
        [Title("开始新建配置数据组！", "可以先选择预设类型", bold: true, titleAlignment: TitleAlignments.Centered)]
        [HorizontalGroup("总组")]
        [DisplayAsString(fontSize: 30), HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        [VerticalGroup("总组/数据")]
        public string createText = "创建新数据组文件配置";
        [DetailedInfoBox("未选中任何数据组！", "创建一个数据组来快捷编辑数据内容！！将会支持Buff,技能,人物,物品等", infoMessageType: InfoMessageType.Warning)]
        [InfoBox("请修改一下文件名否则会分配随机数字后缀", VisibleIf = "@!hasChange", InfoMessageType = InfoMessageType.Warning)]
        [VerticalGroup("总组/数据"), LabelText("文件命名"), Space(5), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04"), OnValueChanged("ChangeHappen")]
        public string createName_ = "Buff新建配置组";
        private bool hasChange = false;
        private void ChangeHappen()
        {
            hasChange = true;

        }
        [FolderPath, OnValueChanged("Refresh")]
        [VerticalGroup("总组/数据"), LabelText("保存文件夹"), Space(5), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")]
        public string FolderPath_ = "Assets/Scripts/ESFramework/Data";
        private void Refresh()
        {
            AssetDatabase.Refresh();
            ESDataWindow.usingWindow.selectPackType_ = createGroup_;

        }

        [DisplayAsString(fontSize: 30), HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        [VerticalGroup("总组/数据")]
        public string createTypeText = "创建新数据组类型或者查询";


        [OnValueChanged("ResetConfigure"), InfoBox("建议选择一个预设类型的配置组,或者自己创建支持,默认类型无法直接使用", infoMessageType: InfoMessageType.Warning, VisibleIf = "@createGroup_==EvWindowDataAndTool.DataType.None")]
        [VerticalGroup("总组/数据"), Space(5), LabelText("预定义类型")]
        public DataType createGroup_ = DataType.Buff;
        [NonSerialized, OdinSerialize, ShowInInspector, ShowIfGroup("总组/数据/s", VisibleIf = "@createGroup_==EvWindowDataAndTool.DataType.None"), LabelText("自定义类型"), Space(5), TypeSelectorSettings(FilterTypesFunction = "TypeSelectorSettingForDataGroup")]
        public Type selfDefineType;
        [HorizontalGroup("总组", width: 100)]
        [VerticalGroup("总组/按钮")]
        [PropertySpace(15)]
        [Button(ButtonHeight = 20, Name = "创建数据组", Icon = SdfIconType.Check2Square), GUIColor("@ KeyValueMatchingUtility.ColorSelector.Color_03")]
        public void CreateInfoGroupAsset()
        {
            Type targetType = EvWindowDataAndTool.TargetGroupType(createGroup_, selfDefineType);
            if (AssetDatabase.IsValidFolder(FolderPath_) && targetType != default)
            {
                ScriptableObject @object = ScriptableObject.CreateInstance(targetType);
                @object.name = createName_ + (hasChange ? "" : UnityEngine.Random.Range(0, 99999));
                AssetDatabase.CreateAsset(@object, FolderPath_ + "/" + @object.name + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Selection.activeObject = @object;
            }
            else
            {
                Debug.LogError("非法文件夹路径或者类型错误！！");
            }
        }
        [VerticalGroup("总组/按钮")]
        [PropertySpace(15)]
        [Button(ButtonHeight = 20, Name = "选中数据组", Icon = SdfIconType.Check2Square), GUIColor("@ KeyValueMatchingUtility.ColorSelector.Color_04")]
        public void FindAInfoGroupAsset()
        {
            //找到全部数据组
            var all = AssetDatabase.FindAssets("t:ScriptableObject");
            string defaultGUID = default;
            Type defaultType = default;
            foreach (var i in all)
            {
                GUID id = default; GUID.TryParse(i, out id);
                Type type = AssetDatabase.GetMainAssetTypeFromGUID(id);
                bool apply = false;
                if (typeof(ISoDataGroup).IsAssignableFrom(type))
                {

                    //必须是这种
                    if (createGroup_ == DataType.None)
                    {
                        apply = true;
                        //可以
                    }
                    else if (createGroup_ == DataType.Buff && typeof(BuffDataGroup).IsAssignableFrom(type))
                    {
                        apply = true;
                        //可以
                    }
                    else
                    {
                        continue;//不通过
                    }
                    defaultGUID = i;
                    defaultType = type;
                }
                if (apply)
                {
                    string path = AssetDatabase.GUIDToAssetPath(i);
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, type);
                    return;
                }
            }
            if (defaultGUID != default && defaultType != null)
            {
                string path = AssetDatabase.GUIDToAssetPath(defaultGUID);
                Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, defaultType);
            }

        }
        private void ResetConfigure()
        {
            hasChange = false;
            ESDataWindow.usingWindow.selectGroupType_ = createGroup_;
            ESDataWindow.usingWindow.ESWindow_RefreshWindow();
            createName_ = EvWindowDataAndTool.RenameGroupByEnumType(createGroup_);
            
            
        }

        public override bool shouldRemake()
        {
            if (Selection.activeObject is ISoDataGroup group2)
            {
                return true;
            }
            return base.shouldRemake();
        }
        private bool TypeSelectorSettingForDataGroup(Type type)
        {
            return !type.IsAbstract && !type.IsInterface && typeof(ISoDataGroup).IsAssignableFrom(type);
        }
    }
    //子页面 数据组快速查看
    [Serializable]
    public class Page_Index_DataQuickSee
    {
        [DisplayAsString(fontSize: 30), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01"),HideLabel]
        public string readme = "双击↓SO编辑该数据组";
        [InlineEditor(Expanded = false),ReadOnly, SerializeReference, LabelText("数据组")]
        public ScriptableObject group;
    }
    //数据组配置页面
    [Serializable]
    public class Page_DataInfoGroup : ESWindowPageBase
    {
        [HorizontalGroup("总组")]
        [Title("开始配置数据组！!", "数据组可以把一类数据整合集中配置和保存", titleAlignment: TitleAlignments.Centered, Title = @"@  ""开始配置数据组:※ 【"" + group.name_  + ""】""   ")]
        [VerticalGroup("总组/数据组")]
        [DisplayAsString(fontSize:30),HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        [VerticalGroup("总组/数据组")]
        public string createText = "创建新单元配置";
        [InfoBox("建议修改一下键名或者单元名防止重复！", VisibleIf = "@!hasChange", InfoMessageType = InfoMessageType.Warning)]
        [InfoBox("该元素的键已经出现了！！请修改", VisibleIf = "@!group.CanStore(DataKey)", InfoMessageType = InfoMessageType.Error)]
        [OnValueChanged("Change"), LabelText("数据单元的键")]
        [VerticalGroup("总组/数据组")]
        public string DataKey = "数据键";
        [OnValueChanged("Change"), LabelText("数据单元的文件名")]
        [VerticalGroup("总组/数据组")]
        public string DataFileName = "数据单元";
        private bool hasChange = false;
        private void Change()
        {
            hasChange = true;
        }
        [DisplayAsString(fontSize: 30), HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        [VerticalGroup("总组/数据组")]
        public string addText = "缓冲入单元列表";
        [VerticalGroup("总组/数据组"),LabelText("缓冲子数据"),SerializeReference]
        public List<ISoDataInfo> soInfos = new List<ISoDataInfo>();
        [DisplayAsString(fontSize: 30), HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        [VerticalGroup("总组/数据组")]
        public string showText = "查看数据组详情";
        [InlineEditor(Expanded =true),SerializeReference,LabelText("数据组")]
        [VerticalGroup("总组/数据组")]
        public ISoDataGroup group;
        [HorizontalGroup("总组",width:100)]
        [VerticalGroup("总组/按钮组")]
        [PropertySpace(15)]
        [Button("新建数据组",ButtonHeight =20),GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")]
        public void CreateNewPage()
        {
           /* string s = @"@  ""开始配置数据组！！ 【"" + group.name_  + ""】""   ";*/
            Selection.activeObject = null;
            group = null;
        }
        [VerticalGroup("总组/按钮组")]
        [PropertySpace(15)]
        [Button("新建单元数据", ButtonHeight = 20), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public void CreateNewSoDataInfo()
        {
            Type type = group.getSoType();
            ScriptableObject @object = ScriptableObject.CreateInstance(type);
            @object.name = DataFileName + DataKey + (hasChange?"":UnityEngine.Random.Range(0,99999));
            if(@object is IWithKey with&&group.CanStore(DataKey))
            {
                with.SetKey(DataKey);
                group.Add(DataKey, @object);
                AssetDatabase.AddObjectToAsset(@object, AssetDatabase.GetAssetPath(group as ScriptableObject));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
            }
            else
            {
                Debug.LogError("不合理的值或者重复键");
            }
           
        }
        [VerticalGroup("总组/按钮组")]
        [PropertySpace(15)]
        [Button("检查数据", ButtonHeight = 20), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public void Check()
        {
            Debug.Log("check");
            List<string> ToRemove = new List<string>();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            bool hasChange = false;
            foreach (var i in group.keys)
            {

                ISoDataInfo so = group.GetOne(i);
                Debug.Log(so);
                ScriptableObject so_ = so as ScriptableObject;
                if (so != null&&so is ScriptableObject)
                {
                    Debug.Log("apply");
                    if (so.key.str_direc != i)
                    {
                        so.SetKey(i);
                        hasChange = true;
                    }
                    
                }
                else 
                {
                    Debug.Log("Remove");
                    ToRemove.Add(i);
                    hasChange = true;
                }
            }
            foreach(var i in ToRemove)
            {
                group.Remove(i);
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (hasChange)
            {
                ESDataWindow.usingWindow?.ESWindow_RefreshWindow();
            }

        }
        [VerticalGroup("总组/按钮组")]
        [PropertySpace(15)]
        [Button("缓冲入子数据", ButtonHeight = 20), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public void Collect()
        {
            UnityEngine.Object ob = group as ScriptableObject;
            string groupPath = AssetDatabase.GetAssetPath(ob);
            
            foreach (var i in soInfos)
            {
                ScriptableObject obd = i as ScriptableObject;
                string soPath = AssetDatabase.GetAssetPath(obd);
                Debug.Log(soPath);
                if (soPath.StartsWith(groupPath))
                {
                    group.Add(i.key.str_direc,obd);
                }
            }
           
            
            /*var all = AssetDatabase.FindAssets("t:ScriptableObject");
            foreach (var i in all)
            {
                
                GUID content = default; GUID.TryParse(i, out content);
                Type info = AssetDatabase.GetMainAssetTypeFromGUID(content);


                string path2 = AssetDatabase.GUIDToAssetPath(content);
                Debug.Log(path2);


                if (typeof(ISoDataInfo).IsAssignableFrom(info))
                {
                    string path = AssetDatabase.GUIDToAssetPath(content);
                    Debug.Log(path);
                    //是子数据
                    if (path.StartsWith(groupPath))
                    {
                        Debug.Log(path);
                    }

                }
            }*/
        }
        public override bool shouldRemake()
        {
            if(Selection.activeObject is ISoDataGroup group2)
            {
                if (group2 != group)
                {
                    return true;
                }
            }
            if (group == null) return true;
            return base.shouldRemake();
        }
    }
    //子层 数据单元配置页面
    [Serializable]
    public class Page_Index_DataInfoSingle
    {

        [HorizontalGroup("总组")]
        [Title("开始配置单元数据！!", "配置单个数据应用到游戏逻辑", titleAlignment: TitleAlignments.Centered, Title = @"@  ""开始配置数据文件:※ 【"" +data.key.str_direc  + ""】""   ")]

        [DisplayAsString(fontSize: 30), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04"), HideLabel]
        [VerticalGroup("总组/数据组")]
        public string handleName = "文件配置相关";
        
        [VerticalGroup("总组/数据组"),LabelText("重命名文件"), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_02")]
        public string renameFile = "新文件名";
        [VerticalGroup("总组/数据组")]
        [DisplayAsString(fontSize: 30), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04"), HideLabel]
        public string handleSOData = "配置数据";
        [InlineEditor,LabelText("数据")]
        
        [VerticalGroup("总组/数据组"),SerializeReference]
        public ISoDataInfo data;
        [NonSerialized]public Page_DataInfoGroup infoGroup;
        [HorizontalGroup("总组",width:100)]
        /*[VerticalGroup("总组/按钮组")]
        [PropertySpace(15)]
        [Button("键", ButtonHeight = 20), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")]
        public void RenameKeyThis()
        {
            
            
        }*/
        
        [VerticalGroup("总组/按钮组")]
        [PropertySpace(15)]
        [Button("重命名文件", ButtonHeight = 20), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")]
        public void RenameFileThis()
        {
            var file = data as ScriptableObject;
            if (file != null)
            {
                file.name = renameFile;
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        [VerticalGroup("总组/按钮组")]
        [PropertySpace(15)]
        [Button("删除", ButtonHeight = 20),GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public void DeleteThis()
        {
            Undo.DestroyObjectImmediate(data as ScriptableObject);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            infoGroup.Check();
        }
    }
    #endregion
    #region 原型工具
    //根 原型配置查看工具
    [Serializable]
    public class Page_Root_Architecture
    {

    }

    //任意的 场景中全部 原型监测页面
    [Serializable]
    public class Page_RunTimeGameObjectChooseSceneArchitecture : ESWindowPageBase
    {
        [DisplayAsString(fontSize: 35), HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public string warnIn = "选中任意携带原型的物体的原型显示-只显示首个";

        [HorizontalGroup("总组")]
        
        [VerticalGroup("总组/数据")]
        [DisplayAsString(fontSize: 15),ShowIfGroup("总组/数据/a", VisibleIf = "@aArchitecture==null"), HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public string warnNull= "尚未选中具有原型的游戏物体";
        [VerticalGroup("总组/数据")]
        [LabelText("原型来源物体")]
        public UnityEngine.Object linkCompo;
        [VerticalGroup("总组/数据")]
        [LabelText("运行时-单个原型",SdfIconType.Cpu), SerializeReference]
        public IArchitecture aArchitecture;
        public override bool shouldRemake()
        {
            if (aArchitecture != null||Selection.activeObject== linkCompo) return false;
            Refresh();
            return base.shouldRemake();
        }
        [HorizontalGroup("总组", width: 100)]
        [VerticalGroup("总组/按钮")]
        [Button("手动刷新",ButtonHeight =30)]
        public void Refresh()
        {
            IWithArchitecture with = Selection.activeGameObject?.GetComponent<IWithArchitecture>();
            if (with != null)
            {
                linkCompo = Selection.activeGameObject;
                aArchitecture = with.GetArchitecture;
            }
            IWithArchitecture with2 = Selection.activeObject as IWithArchitecture;
            if (with2 != null)
            {
                linkCompo = Selection.activeObject;
                aArchitecture = with.GetArchitecture;
            }
        }
    }
    //运行时 原型IOC 页面
    [Serializable]
    public class Page_RunTimeInGameCenterArchitectureIOC
    {
        [DisplayAsString(fontSize:35),HideInPlayMode,HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public string warnInEditor = "必须在运行模式下才能看到IOC全集详情！！！";
        [DisplayAsString(fontSize: 35), HideInEditorMode, HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public string warnInPlay = "以下显示被注册的全部以原型类型分类的全部原型池！！！--不推荐直接变动原型";
        [LabelText("运行时-原型IOC全集"),HideInEditorMode]
        public ArchutectureTypeMatchSafeListIOC IOC;
    }
    [Serializable]
    public class Page_Index_RunTimeInGameCenterArchitectureIOC_TypeSelect
    {
        [DisplayAsString(fontSize: 35), HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public string warnInPlay = "以下显示一个类型的原型列表！！！--不推荐直接变动原型";
        [LabelText("运行时-一类原型"), HideInEditorMode,SerializeReference]
        public List<IArchitecture> architectures;
    }
    [Serializable]
    public class Page_Index_Index_RunTimeInGameCenterArchitectureIOC_SIngleShow
    {
        
        [DisplayAsString(fontSize: 35), HideInEditorMode, HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]
        public string warnInPlay = "以下显示单个原型！！！";
        [LabelText("运行时-单个原型"), HideInEditorMode,SerializeReference]
        public IArchitecture aArchitecture;
    }
    #endregion
    //关于页面
    [Serializable]
    public class Page_About
    {
        [Title("欢迎使用依稀开发框架工具包")]
        [LabelText("版本号"), ShowInInspector, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        public string ToolVersion => "ES 0.0.2 测试版本";
        [LabelText("插件依赖"), ShowInInspector, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        public string PluginsDependence => "Odin插件";
        [PropertySpace(15)]
        [Title("开发者","工具开发者相关信息",titleAlignment:TitleAlignments.Split)]
        [LabelText("开发者团队"), ShowInInspector, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_02")]
        public string DeveloperTeam => "Ey Sive企划";
        [LabelText("开发者名称"), ShowInInspector, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_02")]
        public string DeveloperName => "超级依薇尔";
        [LabelText("开发者QQ"), ShowInInspector, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        public string DeveloperQQ => "2650026906";
        [LabelText("开发者邮箱"), ShowInInspector, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_01")]
        public string DeveloperEmail => "2650026906@qq.com";
        [TitleGroup("最后的话", "来自超级依薇尔", alignment: TitleAlignments.Split),HideLabel,PropertyOrder(3),ReadOnly,TextArea(5,10)]
        public string WordsOnEnd = "我是哔哩哔哩的超级依薇尔，欢迎关注我，另外我创建了QQ交流群982703564\n" +
            "欢迎前来进行Unity技术交流讨论和分享自己的作品和开发见解。\n" +
            "本框架特别感谢 凉鞋大佬(Q FrameWork作者)\n" + ""
            ;
    }
    //基本页面
    [Serializable]
    public abstract class ESWindowPageBase
    {
        public virtual void Setup()
        {
             
        }
        public virtual bool shouldRemake()
        {
            return false;
        }
        public virtual ESWindowPageBase ReFresh()
        {
            return this;
        }
    }
    //数据源和辅助工具(待转移到KeyValueMatching)
    public static class EvWindowDataAndTool
    {
        public enum DataType
        {
            [InspectorName("无类型")] None,
            [InspectorName("Buff数据")] Buff,
            [InspectorName("技能数据")] Skill,
            [InspectorName("角色数据")] Actor,
            [InspectorName("物品数据")] Item,
            [InspectorName("事件信息数据")]EventMessage,
            [InspectorName("符文数据")] Rune,
            [InspectorName("房间生成数据")] RoomGenerate,
            [InspectorName("原型参数值")] ArchitectureParam,
            [InspectorName("技能点配置数据")] SkillPoint,
            [InspectorName("状态配置数据")] State,
        }

        
        public static Type TargetInfoType(DataType type)
        {
            Type targetType = default;
            /* if (typeof(ISoDataPack).IsAssignableFrom(selfDefineType))
             {
                 targetEnum = selfDefineType;
             }*/
            switch (type)
            {
                case DataType.None:
                    targetType = typeof(BuffSoInfo);
                    break;
                case DataType.Buff:
                    targetType = typeof(BuffSoInfo);
                    break;
                case DataType.Skill:
                    targetType = typeof(SkillDataInfo);
                    break;
                case DataType.Actor:
                    targetType = typeof(ActorDataInfo);
                    break;
                case DataType.Item:
                    targetType = typeof(ItemDataInfo);
                    break;
                case DataType.EventMessage:
                    targetType = typeof(GameEventMessageDataInfo);
                    break;
                case DataType.Rune:
                    targetType = typeof(RuneDataInfo);
                    break;
                case DataType.RoomGenerate:
                    targetType = typeof(RoomGenerateDataInfo);
                    break;
                case DataType.ArchitectureParam:
                    targetType = typeof(ArchitectureDataInfo);
                    break;
                case DataType.SkillPoint:
                    targetType = typeof(SkillPointDataInfo);
                    break;
                case DataType.State:
                    targetType = typeof(StateDataInfo);
                    break;
            }
            return targetType;
        }
        public static DataType TargetEnum(Type type)
        {
            DataType targetEnum = default;
            if (type == typeof(BuffSoInfo))
            {
                targetEnum = DataType.Buff;
            }
            else if (type == typeof(SkillDataInfo))
            {
                targetEnum = DataType.Skill;
            }
            else if (type == typeof(ActorDataInfo))
            {
                targetEnum = DataType.Actor;
            }
            else if (type == typeof(ItemDataInfo))
            {
                targetEnum = DataType.Item;
            }
            else if (type == typeof(GameEventMessageDataInfo))
            {
                targetEnum = DataType.EventMessage;
            }
            else if (type == typeof(RuneDataInfo))
            {
                targetEnum = DataType.Rune;
            }
            else if (type == typeof(RoomGenerateDataInfo))
            {
                targetEnum = DataType.RoomGenerate;
            }
            else if (type == typeof(ArchitectureDataInfo))
            {
                targetEnum = DataType.ArchitectureParam;
            }
            else if (type == typeof(StateDataInfo))
            {
                targetEnum = DataType.State;
            }
            
            return targetEnum;
        }
        public static string RenameGroupByEnumType(DataType type)
        {
            switch (type)
            {
                case DataType.None:
                   return "无类型新建数据组";
  
                case DataType.Buff:
                    return "Buff新建数据组";
           
                case DataType.Skill:
                    return "技能新建数据组";
                  
                case DataType.Actor:
                    return "角色新建数据组";
                   
                case DataType.Item:
                    return "物品新建数据组";
                case DataType.EventMessage:
                    return "事件数据新建数据组";
                case DataType.Rune:
                    return "符文新建数据组";
                case DataType.RoomGenerate:
                    return "房间生成新建数据组";
                case DataType.ArchitectureParam:
                    return "原型参数值新建数据组";
                case DataType.SkillPoint:
                    return "技能点配置新建数据组";
                case DataType.State:
                    return "状态新建数据组";
                default:
                    return "自定义新建数据组";
            }
        }
        public static string RenamePackByEnumType(DataType type)
        {
            switch (type)
            {
                case DataType.None:
                    return "无类型新建数据包";

                case DataType.Buff:
                    return "Buff新建数据包";

                case DataType.Skill:
                    return "技能新建数据包";

                case DataType.Actor:
                    return "角色技能数据包";

                case DataType.Item:
                    return "物品新建数据包";
                case DataType.EventMessage:
                    return "事件数据新建数据包";
                case DataType.Rune:
                    return "符文新建数据包";
                case DataType.RoomGenerate:
                    return "房间生成新建数据包";
                case DataType.ArchitectureParam:
                    return "原型参数值新建数据包";
                case DataType.SkillPoint:
                    return "技能点配置新建数据包";
                case DataType.State:
                    return "状态新建数据包";
                default:
                    return "自定义新建数据包";
            }
        }
        public static string RenamePackWantWantGroup(DataType type)
        {
            switch (type)
            {
                case DataType.None:
                    return "拖入或者自动搜集再筛选自定义数据组SO文件";

                case DataType.Buff:
                    return "拖入或者自动搜集再筛选Buff数据组SO文件";

                case DataType.Skill:
                    return "拖入或者自动搜集再筛选技能数据组SO文件";

                case DataType.Actor:
                    return "拖入或者自动搜集再筛选角色数据组SO文件";

                case DataType.Item:
                    return "拖入或者自动搜集再筛选 物品数据 组SO文件";
                case DataType.EventMessage:
                    return "拖入或者自动搜集再筛选 事件信息 数据组SO文件";
                case DataType.Rune:
                    return "拖入或者自动搜集再筛选 符文 数据组SO文件";
                case DataType.RoomGenerate:
                    return "拖入或者自动搜集再筛选  房间生成/走廊生成  数据组SO文件";
                case DataType.ArchitectureParam:
                    return "拖入或者自动搜集再筛选 原型参数 数据组SO文件";
                case DataType.SkillPoint:
                    return "拖入或者自动搜集再筛选 技能点 数据组SO文件";
                case DataType.State:
                    return "拖入或者自动搜集再筛选 状态 数据组SO文件";
                default:
                    return "自定义新建数据包";
            }
        }
        public static Type TargetPackType(DataType pack, Type selfDefine)
        {
            Type targetType = default;
            if (typeof(ISoDataPack).IsAssignableFrom(selfDefine))
            {
                targetType = selfDefine;
            }
            switch (pack)
            {
                case DataType.None:
                    targetType = typeof(BuffDataPack);
                    break;
                case DataType.Buff:
                    targetType = typeof(BuffDataPack);
                    break;
                case DataType.Skill:
                    targetType = typeof(SkillDataPack);
                    break;
                case DataType.Actor:
                    targetType = typeof(ActorDataPack);
                    break;
                case DataType.Item:
                    targetType = typeof(ItemDataPack);
                    break;
                case DataType.EventMessage:
                    targetType = typeof(GameEventMessageDataPack);
                    break;
                case DataType.Rune:
                    targetType = typeof(RuneDataPack);
                    break;
                case DataType.RoomGenerate:
                    targetType = typeof(RoomGenerateDataPack);
                    break;
                case DataType.ArchitectureParam:
                    targetType = typeof(ArchitectureDataPack);
                    break;
                case DataType.SkillPoint:
                    targetType = typeof(SkillPointDataPack);
                    break;
                case DataType.State:
                    targetType = typeof(StateDataPack);
                    break;
            }
            return targetType;
        }
        public static Type TargetGroupType(DataType group,Type selfDefine)
        {
            Type targetType = default;
            if (typeof(ISoDataGroup).IsAssignableFrom(selfDefine))
            {
                targetType = selfDefine;
            }
            switch (group)
            {
                case DataType.None:
                    targetType = typeof(BuffDataGroup);
                    break;
                case DataType.Buff:
                    targetType = typeof(BuffDataGroup);
                    break;
                case DataType.Skill:
                    targetType = typeof(SkillDataGroup);
                    break;
                case DataType.Actor:
                    targetType = typeof(ActorDataGroup);
                    break;
                case DataType.Item:
                    targetType = typeof(ItemDataGroup);
                    break;
                case DataType.EventMessage:
                    targetType = typeof(GameEventMessageDataGroup);
                    break;
                case DataType.Rune:
                    targetType = typeof(RuneDataGroup);
                    break;
                case DataType.RoomGenerate:
                    targetType = typeof(RoomGenerateDataGroup);
                    break;
                case DataType.ArchitectureParam:
                    targetType = typeof(ArchitectureDataGroup);
                    break;
                case DataType.SkillPoint:
                    targetType = typeof(SkillPointDataGroup);
                    break;
                case DataType.State:
                    targetType = typeof(StateDataGroup);
                    break;
            }
            return targetType;
        }
        public static bool HasNull(params UnityEngine.Object[] objects)
        {
            if (objects == null) return true;
            foreach(var i in objects)
            {
                if (i == null  ) return true;
            }
            return false;
        }
        public static bool AllIsOk(params UnityEngine.Object[] objects)
        {
            if (objects == null) return false;
            foreach (var i in objects)
            {
                if (i == null  ) return false;
            }
            return true;
        }
        public static ISoDataGroup SearchGroup(Type infoType)
        {
            var all = AssetDatabase.FindAssets("t:ScriptableObject");
           
            foreach (var i in all)
            {
                GUID id = default; GUID.TryParse(i, out id);
                Type type = AssetDatabase.GetMainAssetTypeFromGUID(id);

                if (typeof(ISoDataGroup).IsAssignableFrom(type))
                {
                    string path = AssetDatabase.GUIDToAssetPath(id);
                    UnityEngine.Object ob = AssetDatabase.LoadAssetAtPath(path, type);
                    if (ob == null) continue;
                    ISoDataGroup dataGroup = ob as ISoDataGroup;
                    if (dataGroup.getSoType() == infoType)
                    {
                        return dataGroup;
                    }
                }
            }
            return default;
        }
        public static ISoDataPack SearchPack(Type infoType)
        {
            var all = AssetDatabase.FindAssets("t:ScriptableObject");

            foreach (var i in all)
            {
                GUID id = default; GUID.TryParse(i, out id);
                Type type = AssetDatabase.GetMainAssetTypeFromGUID(id);

                if (typeof(ISoDataPack).IsAssignableFrom(type))
                {
                    string path = AssetDatabase.GUIDToAssetPath(id);
                    UnityEngine.Object ob = AssetDatabase.LoadAssetAtPath(path, type);
                    if (ob == null) continue;
                    ISoDataPack dataPack = ob as ISoDataPack;
                    if (dataPack.getSoType() == infoType)
                    {
                        return dataPack;
                    }
                }
            }
            return default;
        }
    }
}
