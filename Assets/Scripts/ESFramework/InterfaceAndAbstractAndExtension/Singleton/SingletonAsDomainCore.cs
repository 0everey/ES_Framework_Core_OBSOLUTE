using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES
{
    
    public abstract class SingletonAsCore<This> : BaseCore where This : MonoBehaviour
    {
        [LabelText("不销毁")]public bool DontDestroy = true;
        public static This Instance
        {
            get {
                if (_instance != null) return _instance;
                This t = Object.FindAnyObjectByType<This>();
                if (t != null)
                {
                    _instance = t;
                    return t;
                }
                Debug.LogError($"单例类{typeof(This).Name}场景中不存在");
                GameObject g = GameObject.FindGameObjectWithTag("Manager");
                if (g == null) {
                    g = new GameObject();
                    g.name = $"临时的---单例类{typeof(This).Name}";
                }
                return _instance=g.AddComponent<This>();
                
            }
            set { if (value != null) { _instance = value; }; }
        }

        private static This _instance;
       protected override void Awake()
        {
            //Debug.Log("awake");
            if (_instance == null) {
                _instance = this as This;
                if (_instance != null)
                {
                    AwakeBroadCastRegester();
                   if (DontDestroy) DontDestroyOnLoad(transform.root.gameObject);
                }
            }
            else
            {
                DestroyImmediate(gameObject);
            } 
            
        }
        // Start is called before the first frame update
        // Update is called once per frame
        
    }
    public abstract class SingletonAsMono<This> : MonoBehaviour where This : MonoBehaviour
    {
        [LabelText("不销毁")] public bool DontDestroy = true;
        public static This Instance
        {
            get
            {
                if (_instance != null) return _instance;
                This t = Object.FindAnyObjectByType<This>();
                if (t != null)
                {
                    _instance = t;
                    return t;
                }
                Debug.LogError($"单例类{typeof(This).Name}场景中不存在");
                GameObject g = GameObject.FindGameObjectWithTag("Manager");
                if (g == null)
                {
                    g = new GameObject();
                    g.name = $"临时的---单例类{typeof(This).Name}";
                }
                return _instance = g.AddComponent<This>();

            }
            set { if (value != null) { _instance = value; }; }
        }

        private static This _instance;
        protected  void Awake()
        {
            //Debug.Log("awake");
            if (_instance == null)
            {
                _instance = this as This;
                if (_instance != null)
                {
                    if (DontDestroy) DontDestroyOnLoad(transform.root.gameObject);
                }
            }
            else
            {
                DestroyImmediate(gameObject);
            }

        }
    }
    public abstract class SingletonAsNormalClass<This> 
    {
        [LabelText("不销毁")] public bool DontDestroy = true;
        public static This Instance
        {
            get
            {
                if (_instance != null) return _instance;
                Debug.LogError($"单例普通类{typeof(This).Name}中不存在");
                return default;

            }
            set { if (value != null) { _instance = value; }; }
        }
        private static This _instance;
        // Start is called before the first frame update
    }
}
