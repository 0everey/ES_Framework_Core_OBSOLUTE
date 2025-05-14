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
    private void LateUpdate()
    {
        if(Follow!=null)
        transform.position = Follow.position;
        transform.rotation = Follow.rotation;
    }
}
