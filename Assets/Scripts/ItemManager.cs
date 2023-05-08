using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Vector3 sizeVec;


    // Start is called before the first frame update
    void Start()
    {
        sizeVec = GetComponent<Collider>().bounds.size;
        float resizeFactor = 3.0f / sizeVec.y;
        transform.localScale = new Vector3(resizeFactor, resizeFactor, resizeFactor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
