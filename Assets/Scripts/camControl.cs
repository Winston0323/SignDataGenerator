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
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        //object representor
        crtDist = 10.0f;
        crtrep = Instantiate(prefab);
        Vector3 location = this.transform.position + crtDist * this.transform.forward;
        crtrep.transform.position = location;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
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

        //object creator
        if (show)
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
                copyObject.transform.position = crtrep.transform.position; // set the position of the copy to match the original
                copyObject.transform.rotation = crtrep.transform.rotation; // set the rotation of the copy to match the original
                copyObject.transform.localScale = crtrep.transform.localScale; // set the scale of the copy to match the original
            }

        }
        else {
            crtrep.SetActive(false);
            lineRenderer.SetPosition(1, this.transform.position + new Vector3(0, -1, 0));

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Take a screenshot
            /*            string currentTimeString = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                        string filePath = Application.dataPath + "/ScreenShot/" + screenshotFileName+ currentTimeString +".png";
                        ScreenCapture.CaptureScreenshot(filePath);
                        Debug.Log("Screenshot saved to: " + filePath);*/
            SaveScreenshot();
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            show = !show;
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
}
