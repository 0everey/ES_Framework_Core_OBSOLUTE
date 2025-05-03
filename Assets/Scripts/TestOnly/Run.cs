using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        transform.position = default;
    }
    private void OnDisable()
    {
        transform.position = Vector3.one;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left;
    }
}
