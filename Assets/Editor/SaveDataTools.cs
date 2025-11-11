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
    [MenuItem("Tools/Save Data/Preview Save File Data")]
    static public void DisplaySaveDataFile()
    {
        //Use DisplaySaveJSON window to show save file data
        string pathToSaveFile = Path.Combine(Application.persistentDataPath, "Play Time Data.json");
        //Read the file content
        string readData = File.ReadAllText(pathToSaveFile);

        DisplaySaveJSON window = (DisplaySaveJSON)EditorWindow.GetWindow(typeof(DisplaySaveJSON));
        window.content = readData;
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
