using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAccessor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Mesh originalMesh = GetComponent<MeshFilter>().sharedMesh; // Get the original mesh from the MeshFilter component
        Mesh newMesh = new Mesh();
        newMesh.vertices = originalMesh.vertices; // Copy vertices
        newMesh.triangles = originalMesh.triangles; // Copy triangles
        newMesh.normals = originalMesh.normals; // Copy normals
        newMesh.uv = originalMesh.uv; // Copy UVs
        newMesh.MarkDynamic(); // Mark the mesh as dynamic
        GetComponent<MeshFilter>().mesh = newMesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
