using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public enum shapeType
{
    square,
    circle,
    triangle,
    invTriangle,
    octa,
    diamond
}
public class ParametricModifier : MonoBehaviour
{
    public string signName;
    public float width;
    public float height;
    public Material mat;
    public GameObject square;
    public GameObject circle;
    public GameObject triangle;
    public GameObject invTriangle;
    public GameObject diamond;
    public GameObject octa;
    public GameObject currShap;


    public Dropdown signDD;
    public int ID;
    private Dictionary<int, sign> signDict;
    string imagePath = Path.Combine(Application.streamingAssetsPath, "image");
    string dictionaryPath = Path.Combine(Application.streamingAssetsPath, "SignDictionary.txt");

    //constants
    private string folderPath = "Signs/";

    // Custom class defined within the Unity script
    private struct sign
    {
        public string signName;
        public float width;
        public float height;
        public int ID;
        public shapeType st;
        public Texture2D texture;
        public sign(int ID, string signName, shapeType st, float width, float height,  Texture2D texture) 
        {
            this.signName = signName;
            this.width = width;
            this.height = height;
            this.ID = ID;
            this.st = st;
            this.texture = texture;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Running parametricModifier");
        signDict = new Dictionary<int, sign>();
        loadDict();
        randSign();
        signDD.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDemention(float width, float height) {
        this.width = width;
        this.height = height;
        Vector3 scale = new Vector3(this.width, this.height, currShap.gameObject.transform.localScale.z);
        currShap.gameObject.transform.localScale = scale;
    }
    public void setDementionInch(float width, float height)
    {
        this.width = width/39.37f;
        this.height = height/39.37f;
        Vector3 scale = new Vector3(this.width, this.height, currShap.gameObject.transform.localScale.z);
        currShap.gameObject.transform.localScale = scale;
    }
    public void setTexture(Texture2D tx) {

        currShap.GetComponent<Renderer>().material.mainTexture = tx;
    }
    public int getID() {
        return this.ID;
    }
    public void switchToID(int ID) {
        this.ID = ID;
        sign currSign = signDict[ID];
        shapeType st = currSign.st;
        if (currShap != null) {
            currShap.SetActive(false);
        }
        switch (st) {
            case shapeType.square:
                
                currShap = square;
                
                break;
            case shapeType.diamond:
                
                currShap = diamond;
                
                break;
            case shapeType.circle:
                
                currShap = circle;
                break;
            case shapeType.invTriangle:
                currShap = invTriangle;
                break;
            case shapeType.octa:
                currShap = octa;
                break;
        }
        currShap.SetActive(true);
        setDementionInch(currSign.width, currSign.height);
        setTexture(currSign.texture);
    }
    public int randomID() {
        List<int> keys = new List<int>(signDict.Keys);
        int randID;
        if (keys.Count == 1) {
            return keys[0];
        }
        while (true)
        {
            // Get a random index
            int randomIndex = Random.Range(0, keys.Count);

            // Get the number at the random index
            randID = keys[randomIndex];
            
            if (randID != this.ID)
            {
                break;
            }
        }
        Debug.Log("Random ID: " + randID);
        return randID;
    }
    public void randSign() {

        int randID = randomID();
        
        switchToID(randID);
    }
    public void loadDict() {
        Debug.Log("Loading dictionary******************");
        TextAsset textAsset = Resources.Load<TextAsset>("SignDictionary"); // Replace "myFile" with your file name (without extension)
        StreamReader reader = new StreamReader(dictionaryPath);
        List<Dropdown.OptionData> signOptions = new List<Dropdown.OptionData>();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            Debug.Log(line);
            if (line.Length != 0 && line[0] == '#')
            {
                continue;
            }
            
            string[] words = line.Split(' ');

            if (words.Length < 5)
            {
                Debug.Log("Not enought dimension entered, you entered: " + words.Length);
                break;
            }
            string IDstring = words[0];

            Debug.Log(IDstring);
            IDstring = IDstring.Trim();
            int ID = int.Parse(IDstring);
            string signName = words[1];
            Debug.Log("Adding an option" + signName);
            signOptions.Add(new Dropdown.OptionData(IDstring + " " + signName));
            shapeType st;
            System.Enum.TryParse(words[2], out st);
            float width = float.Parse(words[3]);
            float height = float.Parse(words[4]);
            //Texture2D texture = Resources.Load<Texture2D>(folderPath + signName);
            byte[] pngBytes = System.IO.File.ReadAllBytes(Path.Combine(imagePath, signName + ".png"));
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(pngBytes);
            sign newSign = new sign(ID, signName, st, width, height, texture);
            signDict.Add(ID, newSign);
        }
        signDD.ClearOptions();
        Debug.Log("++++++++++There are " + signOptions.Count + "Signs++++++++++");
        // Set the new options list to the dropdown
        signDD.AddOptions(signOptions);

        // Optionally, set the initial value to the first option
        signDD.value = 0;
    }
    public GameObject getCurrShape() {
        return currShap;
    }
    public string[] getAllSignName()
    {
        //TextAsset[] resources = Resources.LoadAll<TextAsset>(randGenConfigPath);
        // Load all assets from the "Resources" folder and its subfolders
        //Object[] resources = Resources.LoadAll("");

        string[] files = Directory.GetFiles(imagePath, "*.ong");

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
        string option = signDD.options[index].text.ToString();
        string[] words = option.Split(' ');
        string IDstring = words[0];

        Debug.Log(IDstring);
        IDstring = IDstring.Trim();
        int ID = int.Parse(IDstring);
        switchToID(ID);
    }

}
