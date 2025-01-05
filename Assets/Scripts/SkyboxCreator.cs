using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

public class SkyboxCreator : MonoBehaviour
{
    public string basePath;
    public string ImageTexturePath = "Assets/Resources/SkyboxTex";
    string skyboxPanoramaFolderPath = "Assets\\StreamingAssets\\Skybox";
    public string[] folderNames;

    void Start()
    {
        #if UNITY_EDITOR
        string[] folderNames = Directory.GetDirectories(basePath);
        int j = 1;
        foreach (string folderName in folderNames)
        {
            string skyBoxName = Path.GetFileName(folderName).Substring(Path.GetFileName(folderName).LastIndexOf('\\') + 1);
            Debug.Log(skyBoxName);
            Texture2D[] skyboxTextures = new Texture2D[6];
            string[] fileNames = { "nx.jpg", "px.jpg", "py.jpg", "ny.jpg", "pz.jpg", "nz.jpg" };
            string texFldName = j.ToString();
            Debug.Log(ImageTexturePath + texFldName);
            
            if (AssetDatabase.IsValidFolder(ImageTexturePath + "/" + texFldName)) {

            }
            else
            {
                AssetDatabase.CreateFolder(ImageTexturePath, texFldName);
            }
            
            for (int i = 0; i < 6; i++)
            {
                string filePath = Path.Combine(basePath, folderName, fileNames[i]);
                byte[] fileData = File.ReadAllBytes(filePath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                skyboxTextures[i] = tex;
                // Save the texture asset to disk
                // Create a new folder in the Assets directory
                
                string texPath = ImageTexturePath +"/"+ texFldName+ "/" + fileNames[i] + ".asset";
                AssetDatabase.CreateAsset(tex, texPath);
            }

            Material skyboxMaterial = new Material(Shader.Find("Skybox/6 Sided"));
            skyboxMaterial.name = folderName;
            skyboxMaterial.SetTexture("_FrontTex", skyboxTextures[4]);
            skyboxMaterial.SetTexture("_BackTex", skyboxTextures[5]);
            skyboxMaterial.SetTexture("_LeftTex", skyboxTextures[1]);
            skyboxMaterial.SetTexture("_RightTex", skyboxTextures[0]);
            skyboxMaterial.SetTexture("_UpTex", skyboxTextures[2]);
            skyboxMaterial.SetTexture("_DownTex", skyboxTextures[3]);

            string assetPath = "Assets/Resources/Road/" + skyBoxName.ToString().PadLeft(2, '0') + ".mat";
            AssetDatabase.CreateAsset(skyboxMaterial, assetPath);
            j++;
        }
        #endif
    }
}