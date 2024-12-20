using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveSystem
{
    private string dataDirPath = "";

    private string dataFileName = "";

    public SaveSystem(string dataDirPath, string dataFileName) 
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public void Save(int data) 
    {
        if (data > Load())
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                string dataToStore = JsonUtility.ToJson(data, true);

                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(data);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
            }
        }
        else
            Debug.Log("Level already saved");
    }
    
    public int Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        int loadData = 0;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                        loadData = Convert.ToInt32(dataToLoad);
                    }
                }
                //loadData = JsonUtility.FromJson<int>(dataToLoad);

            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }

        }
        return loadData;

    }
}
