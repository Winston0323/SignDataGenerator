using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataViewer : MonoBehaviour
{
    public Vector3 faceDir;
    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        faceDir = transform.forward;
    }
}
