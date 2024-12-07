using UnityEngine;
using UnityEditor;
using System.IO;

public static class MenuFunctions
{
    [MenuItem("Assets/Create/Auto Menu/Auto Menu Document Template", false, 0)]
    public static void CreateAutoMenuDocumentTemplate()
    {
        CreateTemplate("Documents/Examples/AutoMenuExampleDocument.uxml");
    }

    [MenuItem("Assets/Create/Auto Menu/Auto Settings Document Template", false, 1)]
    public static void CreateAutoSettingsDocumentTemplate()
    {
        CreateTemplate("Documents/Examples/AutoSettingsExampleDocument.uxml");
    }

    [MenuItem("Assets/Create/Auto Menu/Auto Menu Style Template", false, 20)]
    public static void CreateAutoMenuStyleTemplate()
    {
        CreateTemplate("Style Templates/AutoMenuStyleTemplate.uss");
    }

    [MenuItem("Assets/Create/Auto Menu/Auto Settings Style Template", false, 21)]
    public static void CreateAutoSettingsStyleTemplate()
    {
        CreateTemplate("Style Templates/AutoSettingsStyleTemplate.uss");
    }

    [MenuItem("Assets/Create/Auto Menu/Menu Premade Style - Sleek Black", false, 40)]
    public static void CreateAutoMenuPremadeStyle1()
    {
        CreateTemplate("Style Templates/AutoMenuPremade - Sleek Dark.uss");
    }

    [MenuItem("Assets/Create/Auto Menu/Settings Premade Style - Sleek Black", false, 40)]
    public static void CreateAutoSettingsPremadeStyle1()
    {
        CreateTemplate("Style Templates/AutoSettingsPremade - Sleek Dark.uss");
    }

    private static void CreateTemplate(string templateName)
    {
        Object target = Selection.activeObject;
        string absolutePath = AssetDatabase.GetAssetPath(target);
        string folderPath = File.Exists(absolutePath) ? Path.GetDirectoryName(absolutePath) : absolutePath;

        string fileName = templateName.Split('/')[^1];
        string destinationPath = $"{folderPath}/{fileName}";
        if(!AssetDatabase.CopyAsset($"Packages/com.cooli2.auto-menu/Editor/StaticFiles/{templateName}", destinationPath))
            Debug.LogWarning($"Error Generating Template! {templateName}");

        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(destinationPath);
    }
}
