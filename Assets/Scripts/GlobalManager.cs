using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SaveData
{
    public List<int> stars;

    public SaveData(List<int> p_stars)
    {
        stars = p_stars;
    }
}

//Global variables, save file management, etc
//Put this at the starting game area
public class GlobalManager : MonoBehaviour
{
    public static bool saveValid = false; //is there data loaded?
    //LevelBaseScript has the maxLevel count
    //public static int maxLevels = 6;
    public static List<int> stars;
    public static GlobalManager instance;

    #if UNITY_EDITOR
    public static bool debug = true; //makes it easier to access levels
#endif

    private void Awake()
    {
        //keep the framerate at a reasonable level
        //we don't need to be running at absurdly fast speeds

        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

        LoadFile();
    }

    public static void setStars(int level, int starCount)
    {
        if (stars == null)
        {
            stars = new List<int>();
        }
        if (level-1 > stars.Count)
        {
            stars.Add(0);
        }
        //Debug.Log((level - 1) + " " + stars.Count);
        stars[level-1] = starCount;
    }

    public static int getStars(int level)
    {
        if (stars == null)
        {
            stars = new List<int>();
        }
        if (level-1 > stars.Count)
        {
            stars.Add(0);
        }
        //Debug.Log((level - 1) + " " + stars.Count);
        return stars[level-1];
    }

    public void NewFile()
    {
        stars = new List<int>();
        for (int i = 0; i < LevelBaseScript.getMaxLevels(); i++)
        {
            stars.Add(0);
        }

        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        SaveData data = new SaveData(stars);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    public void SaveFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        SaveData data = new SaveData(stars);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            //make a new file!
            NewFile();
            return;

            //Debug.LogError("File not found");
            //return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        SaveData data = (SaveData)bf.Deserialize(file);
        file.Close();

        stars = data.stars;

        while (stars.Count < LevelBaseScript.getMaxLevels())
        {
              stars.Add(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
