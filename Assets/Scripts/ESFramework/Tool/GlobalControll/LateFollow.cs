using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateFollow : MonoBehaviour
{
    public Transform Follow;
    void Start()
    {
        
    }


    void Update()
    {

    }
    [Button("测试")]
    private void LateUpdate()
    {
        if(Follow!=null)
        transform.position = Follow.position;
        transform.rotation = Follow.rotation;
    }
}
