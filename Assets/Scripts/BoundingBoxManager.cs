using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoundingBoxManager : MonoBehaviour
{
    public Sprite point;
    public Vector4 result;
    public GameObject targetObject;
    [Header("-----Image of the bounding box-----")]
    public GameObject sltBox_prefab;
    public GameObject sltBoxGameObject;
    public Image sltBox;
    [Header("-----Your main camera-----")]
    public Camera cam;
    [Header("-----Canvas for bounding box to show-----")]
    public Canvas canvas;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponent<Camera>();
        Debug.Log("Here creating a select box prefab");
        sltBoxGameObject = Instantiate(sltBox_prefab, canvas.transform);
        sltBox = sltBoxGameObject.GetComponent<Image>();
        //canvas = FindObjectOfType<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {

        //result = findBoundMesh(targetObject, cam);
        Debug.Log("result:"+ result);
        if (sltBox.gameObject.activeSelf) {
            updateLocation();
        }
    }

    public Vector4 findBoundMesh(GameObject gameObj, Camera camera)
    {
        //get the mesh data of object 
        Mesh mesh = gameObj.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;//getting vertices
        //set min max record to max and min
        float minX = Mathf.Infinity, minY = Mathf.Infinity, maxX = -Mathf.Infinity, maxY = -Mathf.Infinity;
        Debug.Log(vertices.Length);
        //looping through all verteices
        for (int i = 0; i < vertices.Length; i++)
        {
            //transfrom the vertices into screen space
            Vector3 screenPoint = camera.WorldToScreenPoint(gameObj.transform.TransformPoint(vertices[i]));
            CreateMySprite(screenPoint);
            //find minimum and maximum value on x and y directions
            if (screenPoint.x < minX) minX = screenPoint.x;
            if (screenPoint.y < minY) minY = screenPoint.y;
            if (screenPoint.x > maxX) maxX = screenPoint.x;
            if (screenPoint.y > maxY) maxY = screenPoint.y;
        }

        //calculate width and height by minus the minimum from max
        float width = maxX - minX;
        float height = maxY - minY;
        //calculate center from width and height
        float centerX = minX + width / 2.0f;
        float centerY = minY + height / 2.0f;
        //return result
        this.result = new Vector4(width, height, centerX, centerY);
        return result;
    }
    void CreateMySprite(Vector3 position)
    {
        // Create a new empty GameObject
        GameObject newSprite = new GameObject("My Sprite");

        // Add the SpriteRenderer component and assign the sprite
        SpriteRenderer renderer = newSprite.AddComponent<SpriteRenderer>();
        renderer.sprite = point;

        // Position the sprite at the given position
        newSprite.transform.position = position;
        newSprite.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
    }
    public void showbox() {
        sltBox.gameObject.SetActive(true);
    }
    public void updateLocation() {
        RectTransform rectTransform = sltBox.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(result.x, result.y);
        rectTransform.position = new Vector2(result.z, result.w);
    }
    public void hideBox()
    {
        if(sltBox != null)
            sltBox.gameObject.SetActive(false);
    }
}
