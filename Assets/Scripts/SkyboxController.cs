using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class SkyboxController : MonoBehaviour
{
    public string materialFolder;
    private List<Material> skyboxes;
    public int sbIndex = 0;
    public Text frameIndex;
    public int arraySize;
    // Start is called before the first frame update
    void Start()
    {
        //skyboxes = Resources.LoadAll<Material>("Skybox");
        skyboxes = new List<Material>();
    }

    // Update is called once per frame
    void Update()
    {
        //
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (sbIndex < skyboxes.Count - 1)  sbIndex += 1;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (sbIndex > 0) sbIndex -= 1;
        }
        RenderSettings.skybox = skyboxes[sbIndex];
        frameIndex.text = "Frame: " + sbIndex.ToString();
        arraySize = skyboxes.Count;
    }
    public void addSkybox(Material skybox) {
        skyboxes.Add(skybox);
    }
}
