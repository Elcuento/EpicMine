using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class DeveloperManagerEditor : EditorWindow
{
    [MenuItem("Scenes/Entry")]
    public static void OpenSceneEntry()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        EditorSceneManager.OpenScene("Assets/EpicMine/Scenes/EntryPoint.unity");
    }

    [MenuItem("Scenes/Mine")]
    public static void OpenSceneMine()
    {
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        EditorSceneManager.OpenScene("Assets/EpicMine/Scenes/Mine.unity");
    }

    [MenuItem("Scenes/PvpMine")]
    public static void OpenScenePvpMine()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        EditorSceneManager.OpenScene("Assets/EpicMine/Scenes/PvpMine.unity");
    }

    [MenuItem("Scenes/Village")]
    public static void OpenSceneVillage()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        EditorSceneManager.OpenScene("Assets/EpicMine/Scenes/Village.unity");
    }

    [MenuItem("Scenes/Tiers")]
    public static void OpenSceneTiers()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene("Assets/EpicMine/Scenes/Tiers.unity");
    }

    [MenuItem("Scenes/AutoMiner")]
    public static void OpenSceneAutoMiner()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene("Assets/EpicMine/Scenes/AutoMiner.unity");
    }

    [MenuItem("Utils/Clear Prefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();


        Debug.Log("Prefs clear");
    }

    [MenuItem("Utils/Clear Application folder")]
    public static void ClearApplicationFolder()
    {
        var files = Directory.GetFiles(Application.persistentDataPath);
        foreach (var file in files)
        {
           File.Delete($"{file}");
        }

        Debug.Log("data clear");
    }

}
