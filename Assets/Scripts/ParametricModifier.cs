using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum shapeType
{
    square,
    circle,
    triangle
}
public class ParametricModifier : MonoBehaviour
{
    public string signName;
    public float width;
    public float height;
    public Material mat;
    public GameObject square;
    public GameObject circle;
    public GameObject currShap;
    public int ID;
    private Dictionary<int, sign> signDict;
    
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
        TextAsset textAsset = Resources.Load<TextAsset>("SignDictionary"); // Replace "myFile" with your file name (without extension)

        if (textAsset != null)
        {
            string[] lines = textAsset.text.Split('\n');

            foreach (string line in lines)
            {
                string[] words = line.Split(' ');
                if (words.Length != 5) {
                    return;
                }
                string IDstring = words[0];
                //Debug.Log(IDstring);
                IDstring = IDstring.Trim();
                int ID = int.Parse(IDstring);
                string signName = words[1];
                shapeType st;
                System.Enum.TryParse(words[2], out st);
                float width = float.Parse(words[3]);
                float height = float.Parse(words[4]);
                Texture2D texture = Resources.Load<Texture2D>(folderPath + signName);
                sign newSign = new sign(ID, signName, st, width, height, texture);
                signDict.Add(ID, newSign);
            }
        }
        else
        {
            Debug.LogError("Text file not found.");
        }
    }
}
