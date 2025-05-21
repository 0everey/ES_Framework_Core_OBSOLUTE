
using ES;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace ES
{
    public class ES_PoolMaster : SerializedMonoBehaviour
    {
        #region 声明
        public static ES_PoolMaster Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindAnyObjectByType<ES_PoolMaster>() as ES_PoolMaster;
                };
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        private static ES_PoolMaster _instance;
        #endregion

        [Title("基本功能(为GameObject而生)", TitleAlignment = TitleAlignments.Centered)]
        [LabelText("默认初次创建容量")] public int defaultCreateNum = 10;
        [LabelText("每次扩容的默认容量")] public int defaultExpandNumForOnce = 5;
        [LabelText("基本游戏对象池"), NonSerialized]
        public SerializedDictionary<GameObject, Queue<GameObject>> GameObjectPool = new SerializedDictionary<GameObject, Queue<GameObject>>();

        [InfoBox("每次加载场景都生成的", InfoMessageType = InfoMessageType.Warning)]
        [LabelText("常规预制件"), AssetsOnly]
        public List<GameObject> Normalprefabs = new List<GameObject>();



        [NonSerialized]
        public SerializedDictionary<string, List<GameObject>> TargetSceneCreateGameObject = new SerializedDictionary<string, List<GameObject>>();

        [Title("高级功能<场景相关>", "---和场景有关的自动创建的预制件", TitleAlignment = TitleAlignments.Centered)]
        [LabelText("使用&配置")]
        public bool ShowIt_TargetScene = false;
        [InfoBox("特定场景自动创建对象池", InfoMessageType = InfoMessageType.Warning)]
        [LabelText("场景专属初始化预制件列"), ShowIfGroup("SceneTarget", VisibleIf = "@ShowIt_TargetScene")]
        public List<TargetScenePrefab> targetScenePrefabs = new List<TargetScenePrefab>();
        [LabelText("自动寻父变换")]
        [InfoBox("预制件将要--存放的父变换", InfoMessageType = InfoMessageType.Warning), ShowIfGroup("SceneTarget")]
        public List<FindParentTrans> FindParent_PreList = new List<FindParentTrans>();
        private Dictionary<GameObject, FindParentTrans> FindParent = new Dictionary<GameObject, FindParentTrans>();

        [Title("高级功能<调度相关>", "---根据场景帧率自动更新对象池", TitleAlignment = TitleAlignments.Centered)]
        [LabelText("使用&配置")]
        public bool ShowIt_AutoCreate = false;
        [InfoBox("帧率稳定时自动修补对象池", InfoMessageType = InfoMessageType.Warning)]
        [LabelText("容许帧率"), ShowIfGroup("AutoCreate", VisibleIf = "@ShowIt_AutoCreate")]
        public float TargetFrame = 100;
        [LabelText("检测间隔"), ShowIfGroup("AutoCreate")]
        public float detectTimeDis = 1;
        [ShowInInspector, ReadOnly, ShowIfGroup("AutoCreate"), LabelText("已经积累的帧率数")] private float detectTimeHasGo = 0;
        private Queue<GameObject> lastUseQueue = new Queue<GameObject>();
        private HashSet<GameObject> lastUseSet = new HashSet<GameObject>();
        private void Awake()
        {
            if (_instance == null || _instance == this)
            {
                _instance = this;
            }
            else
            {
                DestroyImmediate(gameObject);
                return;
            }
            DontDestroyOnLoad(transform.root.gameObject);
            foreach (var i in FindParent_PreList)
            {
                FindParent.Add(i.prefab, i);
            }
            SceneManager.sceneLoaded += OnSceneLoad;
            SceneManager.sceneUnloaded += OnSceneUnLoad; ;
        }
        private void Update()
        {
            if (ShowIt_AutoCreate)
            {
                float frameSpeed = 1 / Time.deltaTime;
                TargetFrame = Mathf.Lerp(TargetFrame, frameSpeed, 0.001f);
                if (frameSpeed < TargetFrame * 1.1f) return;
                detectTimeHasGo += Time.deltaTime;
                if (detectTimeHasGo > detectTimeDis)
                {
                    detectTimeHasGo = 0;
                    if (lastUseSet != null)
                    {
                        foreach (var i in lastUseSet)
                        {
                            lastUseQueue.Enqueue(i);
                        }
                        lastUseSet.Clear();
                    }

                    if (lastUseQueue.Count > 0)
                    {
                        GameObject last = lastUseQueue.Dequeue();
                        if (last != null && GameObjectPool.ContainsKey(last))
                        {
                            var qq = GameObjectPool[last];
                            if (qq.Count < defaultCreateNum)
                            {
                                ExpandPool(last);
                            }
                        }
                    }
                }
            }
        }
        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            foreach (var i in Normalprefabs)
            {
                if (i == null) continue;
                CreatePool(i);
            }
            foreach (var i in targetScenePrefabs)
            {
                if (i == null) continue;
                if (i.sceneName == scene.name)
                {
                    foreach (var ii in i.prefabs)
                    {
                        if (ii == null) continue;
                        CreatePool(ii, i.defaultCreate);
                    }
                }
            }
        }
        private void OnSceneUnLoad(Scene scene)
        {
            ClearPool();
        }

        public void CreatePool(GameObject key, int? num = null)
        {
            if (GameObjectPool.ContainsKey(key)) ExpandPool(key, num);
            else GameObjectPool.Add(key, new Queue<GameObject>(num ?? defaultCreateNum));
            ExpandPool(key, num);
        }
        public void ExpandPool(GameObject key, int? num = null)
        {
            int num_ = num ?? defaultExpandNumForOnce;
            if (!GameObjectPool.ContainsKey(key)) { CreatePool(key); }
            Transform parent = transform;
            if (FindParent.ContainsKey(key))
            {
                var use = FindParent[key];
                GameObject theP = null;
                if (use.targetParent != null) theP = use.targetParent.gameObject;
                else theP = GameObject.Find(use.findParentName);
                if (theP == null && use.autoCreate)
                {
                    theP = new GameObject();
                    theP.name = use.findParentName;
                }
                if (theP != null)
                {
                    use.targetParent = theP.transform;
                    parent = theP.transform;
                }

            }
            for (int i = 0; i < num_; i++)
            {
                var queue = GameObjectPool[key];
                GameObject g = Instantiate(key, parent);
                g.SetActive(false);
                queue.Enqueue(g);
            }
        }
        public GameObject GetInPool(GameObject key, Vector3 pos = default)
        {
            if (!GameObjectPool.ContainsKey(key)) { CreatePool(key); }
            var queue = GameObjectPool[key];
            if (queue.Count <= 0) ExpandPool(key);
            GameObject g = queue.Dequeue();
            if (g == null) return GetInPool(key, pos);
            g.transform.position = pos;
            g.SetActive(true);
            lastUseSet.Add(key);
            return g;
        }
        public void PushToPool(GameObject key, GameObject who)
        {

            if (!GameObjectPool.ContainsKey(key)) { CreatePool(key); }
            var queue = GameObjectPool[key];
            queue.Enqueue(who);
            who.SetActive(false);
        }
        public void ClearPool()
        {
            foreach (var i in GameObjectPool)
            {
                while (i.Value.Count > 0)
                {
                    var aObject = i.Value.Dequeue();
                    if (aObject != null)
                        Destroy(aObject);
                }
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                var gg = transform.GetChild(i);
                if (gg != null)
                {
                    Destroy(gg.gameObject);
                }
            }
        }



        #region 支持类
        [Serializable, TypeRegistryItem("场景专属生成预制件")]
        public class TargetScenePrefab
        {
            [LabelText("已Build场景名"), ValueDropdown("Names", AppendNextDrawer = true, FlattenTreeView = true)]
            public string sceneName = "场景名";
            [LabelText("预制件列"), AssetsOnly]
            public List<GameObject> prefabs = new List<GameObject>();
            [LabelText("覆盖初始化生成数量")]
            public int defaultCreate = 10;
#if UNITY_EDITOR
            public static List<string> Names()
            {
                List<string> ns = new List<string>();
                var ss = EditorBuildSettings.scenes;
                foreach (var i in ss)
                {
                    int indexXIE = i.path.LastIndexOf('/') + 1;
                    int indexLast = i.path.LastIndexOf(".unity");
                    if (indexXIE >= 0 && indexLast >= 0)
                        ns.Add(i.path.Substring(indexXIE, indexLast - indexXIE));
                }
                return ns;
            }
#endif
            [Button("加载场景测试")]
            private void LoadTest()
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        [Serializable, TypeRegistryItem("寻场景父变换")]
        public class FindParentTrans
        {
            [LabelText("预制件")] public GameObject prefab;
            [LabelText("已寻得变换")] public Transform targetParent;
            [LabelText("查询父名")] public string findParentName = "父对象名";
            [LabelText("自动创建")] public bool autoCreate = true;
        }
        #endregion


      
    }
    #region 万能对象池
    public interface IPoolable
    {
        void OnBePushedToPool();
        bool IsRecycled { get; set; }
    }
    public interface IPoolablebSelfControl : IPoolable
    {
        void TryAutoPushToPool();
    }
    public interface IPool<T>
    {
        T GetInPool();
        bool PushToPool(T obj);
    }

    public abstract class Pool<T> : IPool<T>
    {
        public int CurCount
        {
            get { return mObjectStack.Count; }
        }


        protected IFactory<T> mFactory;

        public void SetFactoryDirectly(IFactory<T> factory)
        {
            mFactory = factory;
        }

        public void SetHowToCreate(Func<T> factoryMethod)
        {
            mFactory = new ESFactory_CustomFunction<T>(factoryMethod);
        }

        /// <summary>
        /// 存储相关数据的栈
        /// </summary>
        protected readonly Stack<T> mObjectStack = new Stack<T>();

        public void Clear(Action<T> onClearItem = null)
        {
            if (onClearItem != null)
            {
                foreach (var poolObject in mObjectStack)
                {
                    onClearItem(poolObject);
                }
            }

            mObjectStack.Clear();
        }

        /// <summary>
        /// default is 5
        /// </summary>
        protected int mMaxCount = 12;

        public virtual T GetInPool()
        {
            return mObjectStack.Count == 0
                ? mFactory.Create()
                : mObjectStack.Pop();
        }

        public abstract bool PushToPool(T obj);
    }

    public class ESSimpleObjectPool<T> : Pool<T>
    {
        readonly Action<T> mResetMethod;

        public ESSimpleObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null, int initCount = 0)
        {
            mFactory = new ESFactory_CustomFunction<T>(factoryMethod);
            mResetMethod = resetMethod;

            for (var i = 0; i < initCount; i++)
            {
                mObjectStack.Push(mFactory.Create());
            }
        }

        public override bool PushToPool(T obj)
        {
            mResetMethod?.Invoke(obj);

            mObjectStack.Push(obj);

            return true;
        }
    }
    #endregion
}