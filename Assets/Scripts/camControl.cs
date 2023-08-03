using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
public enum Mode
{
    Director,
    Manual,
    RandGen
}

public enum folderType
{
    test,
    train,
    valid
}
public class camControl : MonoBehaviour
{

    [Header("-----Player settings-----")]
    public float mouseSense = 100f;
    public float xRotation = 0;
    public GameObject player;
    public float moveSpeed = 1f;
    public float scrollInput;
    public float scrollFactor = 10.0f;

    [Header("-----Mode-----")]
    public Mode currMode;
    //object creator
    [Header("-----Manual Creator-----")]
    public bool show;
    public float crtDist;
    public GameObject crtrep;
    public GameObject prefab;
    public Text distance;
    private LineRenderer lineRenderer;

    [Header("-----Screenshot-----")]
    public string screenshotFileName = "screenshot";
    public Text ssText;

    [Header("-----Target camera-----")]
    public Camera cam;

    [Header("-----Random Creator-----")]
    private GameObject randObj;
    public float XLength;
    public float YLength;
    public Vector3 screenPos;
    public bool bbTurn;
    private int testTime;
    private int trainTime;
    private int validTime;
    private int totalTime;
    private int currIter;
    private folderType currType;
    public bool normalize;
    [Header("-----bounding box-----")]
    public bool repStart;
    public Image sltBox;
    public Sprite point;
    public Vector4 result;
    public float randObjDist;

    [Header("-----Input Field-----")]
    public InputField testNumField;
    public InputField trainNumField;
    public InputField validNumField;
    public InputField distanceMin;
    public InputField distanceMax;
    public InputField rotXmin;
    public InputField rotXmax;
    public InputField rotYmin;
    public InputField rotYmax;
    public InputField rotZmin;
    public InputField rotZmax;
    public InputField screenWidthIP;
    public InputField screenHeightIP;
    public Dropdown configDD;
    public Toggle normalizeToggle;
    public Button setScreenBtn;
    [Header("-----HUD-----")]
    public Canvas canvas;
    public GameObject inputField;
    public GameObject leftUpField;
    public GameObject ScreenshotPrompt;

    string configFolderPath = Path.Combine(Application.streamingAssetsPath, "config");
    public string randGenConfigPath = "RandomGeneratorPreset/";
    [Header("-----Screen Properties-----")]
    public int screenWidth;
    public int screenHeight;

    public GameObject currShape;
    // Start is called before the first frame update
    void Start()
    {
        ResizeScreen(1920, 1080, true);
        PopulateConfigDropdownOptions();
        configDD.onValueChanged.AddListener(OnDropdownValueChanged);
        Cursor.lockState = CursorLockMode.Locked;

        //object representor
        crtDist = 5.0f;
        //create objects
        crtrep = Instantiate(prefab);
        randObj = Instantiate(prefab);
        //create a location
        Vector3 location = this.transform.position + crtDist * this.transform.forward;
        crtrep.transform.position = location;
        randObj.transform.position = location;
        faceCam(cam, randObj);
        randObj.transform.SetParent(cam.gameObject.transform);
        //render a line for manual mode
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        currMode = Mode.Director;
        bbTurn = false;
        normalize = normalizeToggle.isOn;
        string folderPath = Application.dataPath + "/../GeneratedData/";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }
        normalizeToggle.onValueChanged.AddListener(OnToggleValueChanged);
        setScreenBtn.onClick.AddListener(SetScreenBtn);
    }

    // Update is called once per frame
    void Update()
    {

        //float mouseX = Input.GetAxis("Mouse X") * mouseSense * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * mouseSense * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSense * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        if (!repStart)
        {
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            player.transform.Rotate(Vector3.up * mouseX);
            if (Input.GetMouseButtonDown(1))
            {
                randObj.GetComponent<ParametricModifier>().randSign();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                ManualModeInit();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                DirectorModeInit();
            }
        }


        // Get input from WSAD keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.R))
        {
            RandCrtModeInit();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveScreenshot();
        }

        if (currMode == Mode.Manual)//manual object creator mode
        {
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
            if (repStart)
            {

                if (randObj == null)
                {
                    randObj = Instantiate(prefab);
                }
                else
                {
                    randObj.SetActive(true);
                }
                if (!bbTurn)
                {
                    sltBox.gameObject.SetActive(false);
                    //generate random position and rotation
                    randObj.transform.position = randomPosition(); // set the position of the copy to match the original
                    randObjDist = Vector3.Distance(randObj.transform.position, this.transform.position);
                    faceCam(cam, randObj);//make it facing camera
                    randObj.transform.rotation = randObj.transform.rotation * randomRot(); // set the rotation of the copy to match the original
                    hideHUD();
                    SaveScreenshotManualName(randObjDist, randObj.transform.rotation.eulerAngles, new Vector4(0, 0, 0, 0), "screenshot", currType.ToString(), "images");

                    //generate a bounding box
                    currShape = randObj.GetComponent<ParametricModifier>().getCurrShape();
                    result = findBoundMesh(currShape, cam); 
                }
                else
                {
                    sltBox.gameObject.SetActive(true);
                    RectTransform rectTransform = sltBox.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(result.x, result.y);
                    //screenPos.z = 0;
                    rectTransform.position = new Vector2(result.z, result.w);

                    XLength = result.x;
                    YLength = result.y;

                    //save the images
                    hideHUD();
                    //SaveScreenshotManual(distance, randObj.transform.rotation.eulerAngles, "screenshot");

                    SaveScreenshotManualName(randObjDist, randObj.transform.rotation.eulerAngles, result, "screenshot", currType.ToString(), "answers");
                    totalTime--;
                    currIter++;
                }
                bbTurn = !bbTurn;

                if (currIter == testTime)
                {
                    currType = folderType.train;
                }
                else if ((currIter == testTime + trainTime))
                {
                    currType = folderType.valid;

                }
            }
            else
            {//stop repeating screenshot
                activateHUD();
                Cursor.lockState = CursorLockMode.None;//unlock cursor
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    repStart = true;
                    testTime = int.Parse(testNumField.text);
                    trainTime = int.Parse(trainNumField.text);
                    validTime = int.Parse(validNumField.text);
                    totalTime = testTime + trainTime + validTime;
                    currIter = 0;
                    currType = folderType.test;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }

            if (totalTime <= 0)
            {
                repStart = false;

            }
        }
        else
        {//director mode
            DirectorModeInit();

        }
        //sltBox.GetComponent.z = canvas.transform.position.z;
    }
    void hideHUD()
    {
        inputField.SetActive(false);
        leftUpField.SetActive(false);
        ScreenshotPrompt.SetActive(false);
    }
    void activateHUD()
    {
        inputField.SetActive(true);
        leftUpField.SetActive(true);
        ScreenshotPrompt.SetActive(true);
    }
    void RandCrtModeInit()
    {
        currMode = Mode.RandGen;

        Cursor.lockState = CursorLockMode.None;//unlock cursor
        inputField.SetActive(true);//active user input

        //activate correct representation
        crtrep.SetActive(false);
        randObj.SetActive(true);
    }
    void ManualModeInit()
    {
        currMode = Mode.Manual;
        inputField.SetActive(false);
        randObj.SetActive(false);
        sltBox.gameObject.SetActive(false);
        crtrep.SetActive(true);
    }
    void DirectorModeInit()
    {
        currMode = Mode.Director;
        sltBox.gameObject.SetActive(false);
        randObj.SetActive(false);
        inputField.SetActive(false);
        crtrep.SetActive(false);
        lineRenderer.SetPosition(1, this.transform.position + new Vector3(0, -1, 0));
    }
    void SaveScreenshot()
    {
        string folderPath = Application.dataPath + "/../Screenshots/";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }
        string fileName = "Screenshot" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
        string filePath = folderPath + fileName;
        ScreenCapture.CaptureScreenshot(filePath);
        ssText.text = fileName + " saved to: " + filePath;
    }
    string SaveScreenshotManualName(float distance, Vector3 rot, Vector4 result, string title, string folder, string folderType)
    {
        string folderPath = Application.dataPath + "/../GeneratedData/" + folder + "/";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }
        string folderTypePath = folderPath + folderType + "/";
        if (!System.IO.Directory.Exists(folderTypePath))
        {
            System.IO.Directory.CreateDirectory(folderTypePath);
        }
        string fileName = title + "_" + distance + "_" + rot + ".jpg";
        string filePath = folderTypePath + fileName;
        Debug.Log(filePath);
        saveBoundingBoxTxt(folderPath, fileName.Substring(0, fileName.Length - 4), result, randObj.GetComponent<ParametricModifier>().getID(), normalize);
        ScreenCapture.CaptureScreenshot(filePath);
        ssText.text = fileName + " saved to: " + filePath;
        return fileName;
    }
    void saveBoundingBoxTxt(string folderPath, string fileName, Vector4 result, int index, bool normalize)
    {
        string folderLabelPath = folderPath + "/labels/";
        if (!System.IO.Directory.Exists(folderLabelPath))
        {
            System.IO.Directory.CreateDirectory(folderLabelPath);
        }

        string fileNamePath = folderLabelPath + fileName + ".txt";
        float width = result.x;
        float height = result.y;
        float x = result.z;
        float y = Screen.height - result.w ;
        string content = index.ToString() + " " + x + " " + y + " " + width + " " + height;

        if (normalize)
        {
            width = result.x / (float)Screen.width;
            height = result.y / (float)Screen.height;
            x = result.z / (float)Screen.width;
            y = 1 - (result.w  / (float)Screen.height);
            content = index.ToString() + " " + x + " " + y + " " + width + " " + height ;
        }

        File.WriteAllText(fileNamePath, content);

        Debug.Log("File written: " + fileName);

    }
    Vector3 randomPosition()
    {
        Camera cameraToUse = GetComponent<Camera>();
        float randomX = UnityEngine.Random.Range(0.2f, 0.8f);
        float randomY = UnityEngine.Random.Range(0.2f, 0.8f);
        float randomZ = UnityEngine.Random.Range(float.Parse(distanceMin.text), float.Parse(distanceMax.text));
        //Debug.Log(int.Parse(distanceMin.text) + " " + int.Parse(distanceMax.text));

        Vector3 viewportPosition = new Vector3(randomX, randomY, randomZ);

        // Convert viewport coordinates to world coordinates within the camera's viewing frustum
        Vector3 worldPosition = cameraToUse.ViewportToWorldPoint(viewportPosition);

        return worldPosition;
    }
    void faceCam(Camera camera, GameObject gameObject)
    {
        Vector3 dir = camera.transform.forward;
        gameObject.transform.rotation = Quaternion.LookRotation(-dir);

    }
    Quaternion randomRot()
    {
        float Xmin = float.Parse(rotXmin.text);
        float Xmax = float.Parse(rotXmax.text);
        float Ymin = float.Parse(rotYmin.text);
        float Ymax = float.Parse(rotYmax.text);
        float Zmin = float.Parse(rotZmin.text);
        float Zmax = float.Parse(rotZmax.text);
        Vector3 rot = new Vector3(UnityEngine.Random.Range(Xmin, Xmax), UnityEngine.Random.Range(Ymin, Ymax), UnityEngine.Random.Range(Zmin, Zmax));
        //Vector3 rot = new Vector3(0, 0, 0);
        Quaternion rotQ = Quaternion.Euler(rot);
        return rotQ;
    }

    Vector4 findBoundMesh(GameObject gameObj, Camera camera)
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
            //vertices[i] = randObj.transform.localToWorldMatrix *  vertices[i]  ;
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
        return new Vector4(width, height, centerX, centerY);
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
    void loadConfig(string fileName)
    {
        Debug.Log("Loading config");
        StreamReader reader = new StreamReader(Path.Combine(configFolderPath, fileName));

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            // Process each line here
            Debug.Log(line);
            string[] words = line.Split(' ');
            switch (words[0])
            {
                case "Test":
                    testNumField.text = words[1];
                    break;
                case "Train":
                    trainNumField.text = words[1];
                    break;
                case "Valid":
                    validNumField.text = words[1];
                    break;
                case "Distance":
                    distanceMin.text = words[1];
                    distanceMax.text = words[2];
                    break;
                case "RotationX":
                    rotXmin.text = words[1];
                    rotXmax.text = words[2];
                    break;
                case "RotationY":
                    rotYmin.text = words[1];
                    rotYmax.text = words[2];
                    break;
                case "RotationZ":
                    rotZmin.text = words[1];
                    rotZmax.text = words[2];
                    break;
                case "Normalize":

                    normalizeToggle.isOn = bool.Parse(words[1]);
                    break;
                case "Screen":
                    
                    screenWidthIP.text = words[1];
                    screenHeightIP.text = words[2];
                    screenWidth = int.Parse(words[1]);
                    screenHeight = int.Parse(words[2]);
                    ResizeScreen(screenWidth, screenHeight, false);
                    break;

                default:
                    Debug.Log("Error key readed: \"" + words[0] + "\"");
                    break;
            }
        }

        

        
        
        Debug.Log("Finish Loading");
    }
    void PopulateConfigDropdownOptions()
    {
        // Clear existing options (if any)
        configDD.ClearOptions();

        // Create a list to store the options
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        // Add new options to the list
        string[] fileNames = getAllConfigName();
        foreach (string fileName in fileNames)
        {
            options.Add(new Dropdown.OptionData(fileName));
        }

        // Set the new options list to the dropdown
        configDD.AddOptions(options);

        // Optionally, set the initial value to the first option
        configDD.value = 0;
        loadConfig(configDD.options[0].text);
    }
    public string[] getAllConfigName()
    {
        //TextAsset[] resources = Resources.LoadAll<TextAsset>(randGenConfigPath);
        // Load all assets from the "Resources" folder and its subfolders
        //Object[] resources = Resources.LoadAll("");
        
        string[] files = Directory.GetFiles(configFolderPath, "*.txt");
        
        // Create an array to store the file names
        string[] fileNames = new string[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            // Get the name of each asset and store it in the fileNames array
            fileNames[i] = Path.GetFileName(files[i]);
        }

        // Now you have all file names in the 'fileNames' array
        foreach (string fileName in fileNames)
        {
            Debug.Log("File name found: " + fileName);
        }
        return fileNames;
    }
    void OnDropdownValueChanged(int index)
    {
        loadConfig(configDD.options[index].text);
    }
    void OnToggleValueChanged(bool isOn)
    {
        // Display the status of the toggle
        Debug.Log("Toggle value change");
        normalize = isOn;
    }
    // Call this method to resize the screen
    public void ResizeScreen(int screenWidth, int screenHeight, bool fullScreen)
    {
        // Change the screen resolution
        Screen.SetResolution(screenWidth, screenHeight, fullScreen);

        // Calculate the new aspect ratio
        float targetAspect = (float)screenWidth / screenHeight;
        cam.aspect = targetAspect;

        // You might also want to handle UI scaling here if you have a canvas in your scene.
        // Adjusting the canvas scaler's settings can help the UI elements fit the new resolution.
    }
    private void SetScreenBtn()
    {
        screenWidth = int.Parse(screenWidthIP.text);
        screenHeight = int.Parse(screenHeightIP.text);
        ResizeScreen(screenWidth, screenHeight, false);
    }
}