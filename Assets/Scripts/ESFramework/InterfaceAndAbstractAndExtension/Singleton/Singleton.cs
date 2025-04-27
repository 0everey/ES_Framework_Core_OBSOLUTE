using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES
{
    
    public abstract class Singleton<T> : DomainCore where T : MonoBehaviour
    {
        [LabelText("不销毁")]public bool DontDestroy = true;
        public static T Instance
        {
            get {
                if (_instance != null) return _instance;
                T t = Object.FindAnyObjectByType<T>();
                if (t != null)
                {
                    _instance = t;
                    return t;
                }
                Debug.LogError($"单例类{typeof(T).Name}场景中不存在");
                GameObject g = GameObject.FindGameObjectWithTag("Manager");
                if (g == null) {
                    g = new GameObject();
                    g.name = $"临时的---单例类{typeof(T).Name}";
                }
                return _instance=g.AddComponent<T>();
                
            }
            set { if (value != null) { _instance = value; }; }
        }

        private static T _instance;
       protected override void Awake()
        {
            //Debug.Log("awake");
            if (_instance == null) {
                _instance = this as T;
                if (_instance != null)
                {
                    BroadCastRegester();
                   if (DontDestroy) DontDestroyOnLoad(transform.root.gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            } 
            
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        
    }

    public abstract class Singleton_<T> 
    {
        [LabelText("不销毁")] public bool DontDestroy = true;
        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                Debug.LogError($"单例类{typeof(T).Name}场景中不存在");
                return default;

            }
            set { if (value != null) { _instance = value; }; }
        }

        private static T _instance;
       
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
