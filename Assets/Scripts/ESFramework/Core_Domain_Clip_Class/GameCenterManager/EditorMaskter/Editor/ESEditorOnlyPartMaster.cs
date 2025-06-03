using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ES
{
    //必须纯编辑器环境才能用
    public class ESEditorOnlyPartMaster : SingletonAsMono<ESEditorOnlyPartMaster>
    {
        #region 图标支持
        [LabelText("图标管理")]
        public ESIconControl ICON = new ESIconControl();
        [Serializable]
        public class ESIconControl
        {
            [LabelText("初始偏移")] public float startOffset = -20;
            [LabelText("每次偏移")] public float offsetPerTime = -20;
            [LabelText("宽度")] public float width = 20;
            [SerializeReference, LabelText("应用图标")]
            public List<HierachySelector> HierachySelectorS = new List<HierachySelector>();

        }
        [Serializable]
        public abstract class HierachySelector
        {
            public abstract bool IsAppliable(GameObject g);
            [SerializeReference, LabelText("图标"), PropertyOrder(1), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_02")] public ContentGetter contentGetter = new ContentGetter_SystemIcon_English() { };

        }
        #region 筛选器
        [Serializable, TypeRegistryItem("依赖标签筛选")]
        public class IconSelector_DependTag : HierachySelector
        {
            [LabelText("标签筛选")]
            public PointerForString_Tag tag = new PointerForString_Tag() { tagName = "Player" };
            public override bool IsAppliable(GameObject g)
            {
                if (g.EX_NotNullAndUse()?.CompareTag(tag.tagName) ?? false) return true;
                return false;
            }
        }
        [Serializable, TypeRegistryItem("依赖层级筛选")]
        public class IconSelector_DependLayer : HierachySelector
        {
            [LabelText("层级筛选")]
            public LayerMask mask;
            public override bool IsAppliable(GameObject g)
            {
                return g.EX_IsInLayerMask(mask);
            }
        }
        [Serializable, TypeRegistryItem("脚本筛选(类型名)")]
        public class IconSelector_DependCompo_ClassName : HierachySelector
        {
            [LabelText("类型名")]
            public string typeName;
            public override bool IsAppliable(GameObject g)
            {
                var all = g.GetComponents<Component>();
                foreach (var i in all)
                {
                    if (i.GetType().Name == typeName) return true;
                }
                return false;
            }
        }
        [Serializable, TypeRegistryItem("脚本筛选(Type数据相等)")]
        public class IconSelector_DependCompo_SoInfo : HierachySelector
        {
            [LabelText("类型数据")]
            public ESTypeSelecter types;
            public override bool IsAppliable(GameObject g)
            {
                if (types == null) return false;
                var all = g.GetComponents<Component>();
                foreach (var i in all)
                {
                    if (types.TypeList.Contains(i.GetType())) return true;
                }
                return false;
            }
        }
        [Serializable, TypeRegistryItem("脚本筛选(Type数据可继承)")]
        public class IconSelector_DependCompo_SoInfo_Assign : HierachySelector
        {
            [LabelText("类型数据")]
            public ESTypeSelecter types;
            public override bool IsAppliable(GameObject g)
            {
                if (types == null) return false;
                var all = g.GetComponents<Component>();
                foreach (var i in all)
                {
                    foreach (var type in types.TypeList)
                    {
                        if (type.IsAssignableFrom(i.GetType())) return true;
                    }
                }
                return false;
            }
        }
        #endregion
        #region 内容源(content)
        [Serializable]
        public abstract class ContentGetter
        {
            [ColorPalette,LabelText("颜色"),GUIColor("white")]
            public Color color=Color.white;
            [LabelText("缩放")]
            public float scale = 1;
            public abstract GUIContent GetContent();
        }
        [Serializable, TypeRegistryItem("GUI内容-系统图标(英文名)")]
        public class ContentGetter_SystemIcon_English : ContentGetter
        {
            [LabelText("系统图标名"), ValueDropdown("GetNames")]
            public string contentName = "d_GameObject Icon";
            public List<string> GetNames()
            {
                return UnityEditorIcons.UnityEditorIconNames.AllEnglish;
            }
            public override GUIContent GetContent()
            {
                return EditorGUIUtility.IconContent(contentName);
            }
        }
        [Serializable, TypeRegistryItem("GUI内容-系统图标(中文名)")]
        public class ContentGetter_SystemIcon_Chinese : ContentGetter
        {
            [LabelText("系统图标名"), ValueDropdown("GetNames")]
            public string contentName = "游戏对象图标";
            public List<string> GetNames()
            {
                return UnityEditorIcons.UnityEditorIconNames.AllChinese.Keys.ToList();
            }
            public override GUIContent GetContent()
            {
                return EditorGUIUtility.IconContent(UnityEditorIcons.UnityEditorIconNames.AllChinese[contentName]);
            }
        }
        [Serializable, TypeRegistryItem("GUI内容-引用纹理")]
        public class ContentGetter_ReferenceIcon : ContentGetter
        {
            [LabelText("图标资源")]public Texture texture;
            public override GUIContent GetContent()
            {
                return new GUIContent(texture);
            }
        }
        [Serializable, TypeRegistryItem("GUI内容-文本")]
        public class ContentGetter_Label : ContentGetter
        {
            [LabelText("字符串")] public string content="**";
            public override GUIContent GetContent()
            {
                return new GUIContent(content);
            }
        }
        /* [Serializable, TypeRegistryItem("GUI内容-Odin图标(中文名)")]
         public class ContentGetter_OdinIcon_Chinese : ContentGetter
         {
             [LabelText("系统图标名"), ValueDropdown("GetNames")]
             public string contentName = "游戏对象图标";
             public List<string> GetNames()
             {
                 return .sta UnityEditorIcons.UnityEditorIconNames.AllChinese.Keys.ToList();
             }
             public override GUIContent GetContent()
             {
                 return EditorGUIUtility.IconContent(UnityEditorIcons.UnityEditorIconNames.AllChinese[contentName]);
             }
         }*/
    }
    #endregion
    #region 静态获取
    public static class UnityEditorIcons
    {
        #region 属性
        // 基本图标
        public static GUIContent GameObjectIcon => EditorGUIUtility.IconContent("GameObject Icon");
        public static GUIContent TransformIcon => EditorGUIUtility.IconContent("Transform Icon");
        public static GUIContent CameraIcon => EditorGUIUtility.IconContent("Camera Icon");
        public static GUIContent LightIcon => EditorGUIUtility.IconContent("Light Icon");
        public static GUIContent MeshRendererIcon => EditorGUIUtility.IconContent("MeshRenderer Icon");

        // 物理系统
        public static GUIContent RigidbodyIcon => EditorGUIUtility.IconContent("Rigidbody Icon");
        public static GUIContent ColliderIcon => EditorGUIUtility.IconContent("Collider Icon");
        public static GUIContent BoxColliderIcon => EditorGUIUtility.IconContent("BoxCollider Icon");
        public static GUIContent SphereColliderIcon => EditorGUIUtility.IconContent("SphereCollider Icon");

        // UI系统
        public static GUIContent CanvasIcon => EditorGUIUtility.IconContent("Canvas Icon");
        public static GUIContent ImageIcon => EditorGUIUtility.IconContent("Image Icon");
        public static GUIContent TextIcon => EditorGUIUtility.IconContent("Text Icon");
        public static GUIContent ButtonIcon => EditorGUIUtility.IconContent("Button Icon");

        // 2D系统
        public static GUIContent SpriteRendererIcon => EditorGUIUtility.IconContent("SpriteRenderer Icon");
        public static GUIContent TilemapIcon => EditorGUIUtility.IconContent("Tilemap Icon");
        public static GUIContent TileIcon => EditorGUIUtility.IconContent("Tile Icon");

        // 动画系统
        public static GUIContent AnimatorIcon => EditorGUIUtility.IconContent("Animator Icon");
        public static GUIContent AnimationIcon => EditorGUIUtility.IconContent("Animation Icon");
        public static GUIContent AvatarIcon => EditorGUIUtility.IconContent("Avatar Icon");

        // 音频系统
        public static GUIContent AudioSourceIcon => EditorGUIUtility.IconContent("AudioSource Icon");
        public static GUIContent AudioListenerIcon => EditorGUIUtility.IconContent("AudioListener Icon");

        // 特效系统
        public static GUIContent ParticleSystemIcon => EditorGUIUtility.IconContent("ParticleSystem Icon");
        public static GUIContent TrailRendererIcon => EditorGUIUtility.IconContent("TrailRenderer Icon");
        public static GUIContent LineRendererIcon => EditorGUIUtility.IconContent("LineRenderer Icon");

        // 资源类型
        public static GUIContent PrefabIcon => EditorGUIUtility.IconContent("Prefab Icon");
        public static GUIContent MaterialIcon => EditorGUIUtility.IconContent("Material Icon");
        public static GUIContent TextureIcon => EditorGUIUtility.IconContent("Texture Icon");
        public static GUIContent MeshIcon => EditorGUIUtility.IconContent("Mesh Icon");
        public static GUIContent ScriptableObjectIcon => EditorGUIUtility.IconContent("ScriptableObject Icon");

        // 编辑器UI图标
        public static GUIContent RefreshIcon => EditorGUIUtility.IconContent("Refresh");
        public static GUIContent SettingsIcon => EditorGUIUtility.IconContent("Settings");
        public static GUIContent SearchIcon => EditorGUIUtility.IconContent("Search");
        public static GUIContent ViewToolOrbit => EditorGUIUtility.IconContent("ViewToolOrbit");

        // 控制台图标
        public static GUIContent ConsoleErrorIcon => EditorGUIUtility.IconContent("console.erroricon");
        public static GUIContent ConsoleWarningIcon => EditorGUIUtility.IconContent("console.warnicon");
        public static GUIContent ConsoleInfoIcon => EditorGUIUtility.IconContent("console.infoicon");
        public static GUIContent ConsoleLogIcon => EditorGUIUtility.IconContent("console.logIcon");

        // 版本控制
        public static GUIContent VersionControl => EditorGUIUtility.IconContent("VersionControl");
        public static GUIContent VersionControlUpdated => EditorGUIUtility.IconContent("VersionControl/Updated");
        public static GUIContent VersionControlAdded => EditorGUIUtility.IconContent("VersionControl/Added");

        // 其他常用图标
        public static GUIContent FolderIcon => EditorGUIUtility.IconContent("Folder Icon");
        public static GUIContent SceneAssetIcon => EditorGUIUtility.IconContent("SceneAsset Icon");
        public static GUIContent PlayButton => EditorGUIUtility.IconContent("PlayButton");
        public static GUIContent PauseButton => EditorGUIUtility.IconContent("PauseButton");
        public static GUIContent StepButton => EditorGUIUtility.IconContent("StepButton");

        // 深色/浅色主题适配
        public static GUIContent GetGameObjectIcon(bool darkTheme = true) =>
            EditorGUIUtility.IconContent(darkTheme ? "d_GameObject Icon" : "sv_GameObject Icon");

        public static GUIContent GetFolderIcon(bool darkTheme = true) =>
            EditorGUIUtility.IconContent(darkTheme ? "d_Folder Icon" : "sv_Folder Icon");
        #endregion
        #region 静态字典
        public static class UnityEditorIconNames
        {
            public static readonly List<string> AllEnglish = new List<string>
    {
        // 基本对象图标
        "GameObject Icon",
        "Transform Icon",
        "Camera Icon",
        "Light Icon",
        "MeshRenderer Icon",
        
        // 物理系统
        "Rigidbody Icon",
        "Collider Icon",
        "BoxCollider Icon",
        "SphereCollider Icon",
        "CapsuleCollider Icon",
        "MeshCollider Icon",
        "WheelCollider Icon",
        "TerrainCollider Icon",
        "Cloth Icon",
        "HingeJoint Icon",
        
        // UI系统
        "Canvas Icon",
        "RectTransform Icon",
        "CanvasRenderer Icon",
        "Image Icon",
        "Text Icon",
        "Button Icon",
        "InputField Icon",
        "Slider Icon",
        "Scrollbar Icon",
        "Dropdown Icon",
        "Toggle Icon",
        
        // 2D系统
        "SpriteRenderer Icon",
        "Tilemap Icon",
        "Tile Icon",
        "SpriteMask Icon",
        "SortingGroup Icon",
        
        // 动画系统
        "Animator Icon",
        "Animation Icon",
        "Avatar Icon",
        "AvatarMask Icon",
        "RuntimeAnimatorController Icon",
        
        // 音频系统
        "AudioSource Icon",
        "AudioListener Icon",
        "AudioClip Icon",
        "AudioReverbZone Icon",
        
        // 特效系统
        "ParticleSystem Icon",
        "TrailRenderer Icon",
        "LineRenderer Icon",
        "LensFlare Icon",
        
        // 资源类型
        "Prefab Icon",
        "Material Icon",
        "Texture Icon",
        "Mesh Icon",
        "ScriptableObject Icon",
        "Shader Icon",
        "ComputeShader Icon",
        "PhysicMaterial Icon",
        
        // 编辑器UI图标
        "Refresh",
        "Settings",
        "Search",
        "ViewToolOrbit",
        "ViewToolMove",
        "ViewToolZoom",
        "ViewToolFPS",
        
        // 控制台图标
        "console.erroricon",
        "console.warnicon",
        "console.infoicon",
        "console.logIcon",
        
        // 版本控制
        "VersionControl",
        "VersionControl/Updated",
        "VersionControl/Added",
        "VersionControl/Deleted",
        "VersionControl/Conflicted",
        "VersionControl/Local",
        
        // 其他常用图标
        "Folder Icon",
        "SceneAsset Icon",
        "PlayButton",
        "PauseButton",
        "StepButton",
        "StopButton",
        "RecordButton",
        "PlayButtonProfile",
        "PreMatCube",
        "PreMatSphere",
        "PreMatCylinder",
        "PreMatQuad",
        
        // 深色主题前缀(d_)和浅色主题前缀(sv_)
        "d_GameObject Icon",
        "sv_GameObject Icon",
        "d_Folder Icon",
        "sv_Folder Icon",
        "d_SceneAsset Icon",
        "sv_SceneAsset Icon",
        
        // 编辑器窗口图标
        "UnityEditor.AnimationWindow",
        "UnityEditor.ConsoleWindow",
        "UnityEditor.GameView",
        "UnityEditor.HierarchyWindow",
        "UnityEditor.InspectorWindow",
        "UnityEditor.ProjectWindow",
        "UnityEditor.SceneView",
        
        // 工具栏图标
        "Toolbar Plus",
        "Toolbar Minus",
        "Toolbar Plus More",
        "Toolbar Minus More",
        "FilterByLabel",
        "FilterByType",
        
        // 脚本图标
        "cs Script Icon",
        "dll Script Icon",
        "js Script Icon",
        "boo Script Icon",
        
        // 平台相关
        "BuildSettings.Standalone",
        "BuildSettings.Android",
        "BuildSettings.iPhone",
        "BuildSettings.WebGL",
        "BuildSettings.WindowsStoreApps",
        
        // 其他
        "SaveActive",
        "TreeEditor.Trash",
        "TreeEditor.Refresh",
        "Clipboard",
        "Profiler.CPU",
        "Profiler.Memory",
        "Profiler.Rendering",
        "Profiler.Open",
        "Profiler.Record"
    };

            public static readonly Dictionary<string, string> AllChinese = new Dictionary<string, string>
            {
        // 基础对象
        {"游戏对象图标", "GameObject Icon"},
        {"变换组件图标", "Transform Icon"},
        {"相机图标", "Camera Icon"},
        {"灯光图标", "Light Icon"},
        
        // 物理系统
        {"刚体图标", "Rigidbody Icon"},
        {"碰撞体图标", "Collider Icon"},
        {"盒型碰撞体图标", "BoxCollider Icon"},
        {"球形碰撞体图标", "SphereCollider Icon"},
        {"胶囊碰撞体图标", "CapsuleCollider Icon"},
        
        // UI系统
        {"画布图标", "Canvas Icon"},
        {"图片组件图标", "Image Icon"},
        {"文本组件图标", "Text Icon"},
        {"按钮组件图标", "Button Icon"},
        
        // 2D系统
        {"精灵渲染器图标", "SpriteRenderer Icon"},
        {"瓦片地图图标", "Tilemap Icon"},
        {"瓦片图标", "Tile Icon"},
        
        // 动画系统
        {"动画控制器图标", "Animator Icon"},
        {"动画组件图标", "Animation Icon"},
        {"角色化身图标", "Avatar Icon"},
        
        // 音频系统
        {"音频源图标", "AudioSource Icon"},
        {"音频监听器图标", "AudioListener Icon"},
        
        // 特效系统
        {"粒子系统图标", "ParticleSystem Icon"},
        {"轨迹渲染器图标", "TrailRenderer Icon"},
        {"线条渲染器图标", "LineRenderer Icon"},
        
        // 资源类型
        {"预制体图标", "Prefab Icon"},
        {"材质图标", "Material Icon"},
        {"纹理图标", "Texture Icon"},
        {"网格图标", "Mesh Icon"},
        {"可编程对象图标", "ScriptableObject Icon"},
        
        // 编辑器UI
        {"刷新图标", "Refresh"},
        {"设置图标", "Settings"},
        {"搜索图标", "Search"},
        {"视图旋转工具图标", "ViewToolOrbit"},
        
        // 控制台
        {"错误图标", "console.erroricon"},
        {"警告图标", "console.warnicon"},
        {"信息图标", "console.infoicon"},
        {"日志图标", "console.logIcon"},
        
        // 版本控制
        {"版本控制图标", "VersionControl"},
        {"已更新图标", "VersionControl/Updated"},
        {"已添加图标", "VersionControl/Added"},
        
        // 其他常用
        {"文件夹图标", "Folder Icon"},
        {"场景资源图标", "SceneAsset Icon"},
        {"播放按钮", "PlayButton"},
        {"暂停按钮", "PauseButton"},
        {"停止按钮", "StopButton"},
        
        // 主题相关
        {"深色游戏对象图标", "d_GameObject Icon"},
        {"浅色游戏对象图标", "sv_GameObject Icon"},
        {"深色文件夹图标", "d_Folder Icon"},
        {"浅色文件夹图标", "sv_Folder Icon"},
        
        // 编辑器窗口
        {"动画窗口图标", "UnityEditor.AnimationWindow"},
        {"控制台窗口图标", "UnityEditor.ConsoleWindow"},
        {"游戏视图图标", "UnityEditor.GameView"},
        {"层级窗口图标", "UnityEditor.HierarchyWindow"},
        
        // 脚本类型
        {"C#脚本图标", "cs Script Icon"},
        {"DLL图标", "dll Script Icon"},
        {"JavaScript图标", "js Script Icon"},
        
        // 平台
        {"PC平台图标", "BuildSettings.Standalone"},
        {"安卓平台图标", "BuildSettings.Android"},
        {"iOS平台图标", "BuildSettings.iPhone"},
        {"WebGL平台图标", "BuildSettings.WebGL"},
        
        // 分析器
        {"CPU分析器图标", "Profiler.CPU"},
        {"内存分析器图标", "Profiler.Memory"},
        {"渲染分析器图标", "Profiler.Rendering"}
    };

        }
        #endregion
    }

    #endregion

    #endregion
}


