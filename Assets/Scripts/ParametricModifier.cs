using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public enum shapeType
{
    square,
    circle,
    triangle,
    invTriangle,
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
    public GameObject currShap;
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
        
        signDict = new Dictionary<int, sign>();
        loadDict();

        currShap = square;
        width = currShap.gameObject.transform.localScale.x;
        height = currShap.gameObject.transform.localScale.y;

        randSign();
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
        switch (st) {
            case shapeType.square:
                currShap.SetActive(false);
                currShap = square;
                currShap.SetActive(true);
                break;
            case shapeType.diamond:
                currShap.SetActive(false);
                currShap = diamond;
                currShap.SetActive(true);
                break;
            case shapeType.circle:
                currShap.SetActive(false);
                currShap = circle;
                currShap.SetActive(true);
                break;
            case shapeType.invTriangle:
                currShap.SetActive(false);
                currShap = invTriangle;
                currShap.SetActive(true);
                break;
        }
        setDementionInch(currSign.width, currSign.height);
        setTexture(currSign.texture);
    }
    public int randomID() {
        List<int> keys = new List<int>(signDict.Keys);
        int randID;
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
        Debug.Log("Loading dictionary");
        TextAsset textAsset = Resources.Load<TextAsset>("SignDictionary"); // Replace "myFile" with your file name (without extension)
        StreamReader reader = new StreamReader(dictionaryPath);
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
                return;
            }
            string IDstring = words[0];

            Debug.Log(IDstring);
            IDstring = IDstring.Trim();
            int ID = int.Parse(IDstring);
            string signName = words[1];
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
    }
}
