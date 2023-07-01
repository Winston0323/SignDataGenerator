using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class camControl : MonoBehaviour
{
    public float mouseSense = 100f;
    public float xRotation = 0;
    public GameObject player;
    public float moveSpeed = 1f;
    public float scrollInput;
    public float scrollFactor = 10.0f;

    public enum Mode
    {
        Normal,
        Manual,
        RandGen
    }
    public Mode currMode;
    //object creator
    public bool show;
    public float crtDist;
    public GameObject crtrep;
    public GameObject prefab;
    public Text distance;

    // line render
    private LineRenderer lineRenderer;

    public string screenshotFileName = "screenshot";
    public Text ssText;

    //main camera
    public Camera cam;

    //random generator
    public GameObject randObj;
    public InputField repeatTime;
    public InputField distanceMin;
    public InputField distanceMax;
    public GameObject inputField;
    public int repTime;
    public bool repStart;
    // Start is called before the first frame update
    void Start()
    {
        repeatTime.text = "10";
        distanceMin.text = "5";
        distanceMax.text = "20";
        Cursor.lockState = CursorLockMode.Locked;
        
        //object representor
        crtDist = 10.0f;
        crtrep = Instantiate(prefab);
        Vector3 location = this.transform.position + crtDist * this.transform.forward;
        crtrep.transform.position = location;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        currMode = Mode.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        
        //float mouseX = Input.GetAxis("Mouse X") * mouseSense * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * mouseSense * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSense * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.transform.Rotate(Vector3.up * mouseX);

        // Get input from WSAD keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Cursor.lockState = CursorLockMode.None;
            currMode = Mode.RandGen;
            inputField.SetActive(true);
            crtrep.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            currMode = Mode.Manual;
            repeatTime.gameObject.SetActive(false);
            randObj.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveScreenshot();
        }
        
        if (currMode == Mode.Manual)//manual object creator mode
        {
            
            crtrep.SetActive(true);
            Vector3 crtPos = this.transform.position + crtDist * this.transform.forward;
            lineRenderer.SetPosition(0, this.transform.position + new Vector3(0, -1, 0));
            lineRenderer.SetPosition(1, crtPos);

            // Set the width and color of the line
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material.color = Color.red;

            crtrep.transform.position = crtPos;
            scrollInput = Input.GetAxis("Mouse ScrollWheel"); // get the scroll wheel input
            crtDist = (int)(crtDist + scrollInput * scrollFactor);
            distance.text = "Distance:" + crtDist.ToString();
            if (crtDist <= 2) crtDist = 2;

            if (Input.GetMouseButtonDown(0)) // check if the left mouse button is pressed down
            {
                GameObject copyObject = Instantiate(prefab); // create a new instance of the object
                copyObject.transform.position = randomPosition(); // set the position of the copy to match the original
                copyObject.transform.rotation = randomRot(); // set the rotation of the copy to match the original
                copyObject.transform.localScale = crtrep.transform.localScale; // set the scale of the copy to match the original
            }
        }
        else if (currMode == Mode.RandGen)
        {//random data generator mode
            
            lineRenderer.SetPosition(1, this.transform.position + new Vector3(0, -1, 0));
            if (Input.GetKeyDown(KeyCode.Space)) {
                repStart = true;
                repTime = int.Parse(repeatTime.text);
                Cursor.lockState = CursorLockMode.Locked;
            }
            if (repStart)
            {
                inputField.SetActive(false);
                if (randObj == null)
                {
                    randObj = Instantiate(prefab);
                }
                else {
                    randObj.SetActive(true);
                }
                randObj.transform.position = randomPosition(); // set the position of the copy to match the original
                float distance = Vector3.Distance(randObj.transform.position, this.transform.position);
                randObj.transform.rotation = randomRot(); // set the rotation of the copy to match the original
                SaveScreenshotManual(distance, randObj.transform.rotation.eulerAngles);
                repTime--;
                Debug.Log(repTime);
            }
            else {
                inputField.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }

            if (repTime <= 0)
            {
                repStart = false;
            }
        }
        else
        {//skybox viewer mode
            if (randObj != null)
            {
                randObj.SetActive(false);
            }
            inputField.SetActive(false);
            crtrep.SetActive(false);
            lineRenderer.SetPosition(1, this.transform.position + new Vector3(0, -1, 0));
        }





    }
    void SaveScreenshot()
    {
        string folderPath = Application.dataPath + "/../Screenshots/";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }
        string fileName = "Screenshot" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        string filePath = folderPath + fileName;
        ScreenCapture.CaptureScreenshot(filePath);
        ssText.text = fileName + " saved to: " + filePath;
    }
    void SaveScreenshotManual(float distance, Vector3 rot)
    {
        string folderPath = Application.dataPath + "/../Screenshots/";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }
        string fileName = "Screenshot_" + distance+ "_" + rot + ".png";
        string filePath = folderPath + fileName;
        Debug.Log(filePath);
        ScreenCapture.CaptureScreenshot(filePath);
        ssText.text = fileName + " saved to: " + filePath;
    }
    Vector3 randomPosition()
    {
        Camera cameraToUse = GetComponent<Camera>();
        float randomX = UnityEngine.Random.Range(0.3f, 0.8f);
        float randomY = UnityEngine.Random.Range(0.3f, 0.8f);
        float randomZ = UnityEngine.Random.Range(float.Parse(distanceMin.text), float.Parse(distanceMax.text));
        Debug.Log(int.Parse(distanceMin.text) +" "+ int.Parse(distanceMax.text));

        Vector3 viewportPosition = new Vector3(randomX, randomY, randomZ);

        // Convert viewport coordinates to world coordinates within the camera's viewing frustum
        Vector3 worldPosition = cameraToUse.ViewportToWorldPoint(viewportPosition);

        return worldPosition;
    }
    Quaternion randomRot() 
    {
        Vector3 rot = new Vector3(UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(-60f, 60f), UnityEngine.Random.Range(-30f, 30f)); ;
        Quaternion rotQ = Quaternion.Euler(rot);
        return rotQ;
    }
    
    
}
