
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Ev_PoolManager : MonoBehaviour
{

    public static Ev_PoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindAnyObjectByType<Ev_PoolManager>() as Ev_PoolManager;
            };
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
    private static Ev_PoolManager _instance;
    public int defaultNum = 10;
    public int defaultExpandNumOnce = 5;

    public SerializedDictionary<GameObject, Queue<GameObject>> m_poolDictionary = new SerializedDictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        _instance = this;

    }
    public void CreatePool(GameObject key, int? num = null)
    {
        if (m_poolDictionary.ContainsKey(key)) return;
        else m_poolDictionary.Add(key, new Queue<GameObject>(num ?? defaultNum));
        ExpandPool(key, num);
    }
    public void ExpandPool(GameObject key, int? num = null)
    {
        int num_ = num ?? defaultExpandNumOnce;
        if (!m_poolDictionary.ContainsKey(key)) { CreatePool(key); }

        for (int i = 0; i < num_; i++)
        {
            var queue = m_poolDictionary[key];
            GameObject g = Instantiate(key,transform);
            g.SetActive(false);
            queue.Enqueue(g);
        }
    }
    public GameObject GetInPool(GameObject key, Vector3 pos = default)
    {
        if (!m_poolDictionary.ContainsKey(key)) { CreatePool(key); }
        var queue = m_poolDictionary[key];
        if (queue.Count <= 0) ExpandPool(key);
        GameObject g = queue.Dequeue();
        if (g == null) return GetInPool(key,pos);
        g.transform.position = pos;
        g.SetActive(true);
        return g;
    }
    public void PushToPool(GameObject key,GameObject who)
    {
        
        if (!m_poolDictionary.ContainsKey(key)) { CreatePool(key); }
        var queue = m_poolDictionary[key];
        queue.Enqueue(who);
        who.SetActive(false);
    }
}
