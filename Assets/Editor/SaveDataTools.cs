using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class SaveDataTools
{
    [MenuItem("Tools/Save Data/Open Save Data Folder")]
    static public void FindSaveData()
    {
        string path = Application.persistentDataPath;
        EditorUtility.RevealInFinder(path);
    }
    [MenuItem("Tools/Save Data/Print Save File Data")]
    static public void PrintSaveData()
    {
        string pathToSaveFile = Path.Combine(Application.persistentDataPath, "Play Time Data.json");
        if (File.Exists(pathToSaveFile))
        {
            string jsonData = File.ReadAllText(pathToSaveFile);
            Debug.Log("Save File Data:\n" + jsonData);
        }
        else
        {
            Debug.LogWarning("No save file found at: " + pathToSaveFile);
        }
    }

    [MenuItem("Tools/Save Data/Delete Save File")]
    static public void DeleteSaveFile()
    {
        string pathToSaveFile = Path.Combine(Application.persistentDataPath, "Play Time Data.json");
        if (File.Exists(pathToSaveFile))
        {
            File.Delete(pathToSaveFile);
            Debug.Log("Deleted save file at: " + pathToSaveFile);
        }
        else
        {
            Debug.LogWarning("No save file found at: " + pathToSaveFile);
        }
    }
}
