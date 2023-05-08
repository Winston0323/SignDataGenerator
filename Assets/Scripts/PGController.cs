using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PGController : MonoBehaviour
    
{
    public Vector3 currPosition;
    public Vector3 currRotation;
    public Vector3 currSize;
    public int objectNumber;
    public Button Generator;
    public InputField PosX;
    public InputField PosY;
    public InputField PosZ;
    public InputField RotX;
    public InputField RotY;
    public InputField RotZ;
    public InputField sizeX;
    public InputField sizeY;
    public InputField sizeZ;
    // Start is called before the first frame update
    void Start()
    {
        PosX.text = "0";
        PosY.text = "0";
        PosZ.text = "0";
        RotX.text = "0";
        RotY.text = "0";
        RotZ.text = "0";
        sizeX.text = "1";
        sizeY.text = "1";
        sizeZ.text = "1";
        Generator.onClick.AddListener(CubeGenerator);
    }

    // Update is called once per frame
    void Update()
    {
        
        
        int xVal = int.Parse(PosX.text);
        int yVal = int.Parse(PosY.text);
        int zVal = int.Parse(PosZ.text);
        currPosition = new Vector3(xVal, yVal, zVal);
        int xRotVal = int.Parse(RotX.text);
        int yRotVal = int.Parse(RotY.text);
        int zRotVal = int.Parse(RotZ.text);
        currRotation = new Vector3(xRotVal, yRotVal, zRotVal);
        int xSizeVal = int.Parse(sizeX.text);
        int ySizeVal = int.Parse(sizeY.text);
        int zSizeVal = int.Parse(sizeZ.text);
        currSize = new Vector3(xSizeVal, ySizeVal, zSizeVal);
        
    }

    void CubeGenerator() 
    {
        GameObject currCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        currCube.transform.position = currPosition;
        currCube.transform.rotation = Quaternion.Euler(currRotation);
        currCube.transform.localScale = currSize;
    }
}
