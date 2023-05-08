using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastCreator : MonoBehaviour
{
    public float distance;
    // Start is called before the first frame update
    void Start()
    {
        distance = 10.0f;
        GameObject currCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 location = this.transform.position + distance * this.transform.forward;
        currCube.transform.position = location;
        currCube.transform.parent = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 location = this.transform.position + distance * this.transform.forward;


    }
}
