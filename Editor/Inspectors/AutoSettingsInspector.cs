using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace AutoMenu.Editor
{
    [CustomEditor(typeof(AutoSettings))]
    public class AutoSettingsInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var drawerXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.cooli2.auto-menu/Editor/StaticFiles/Documents/Inspectors/AutoSettingsInspector.uxml");
            var drawerElement = drawerXML.Instantiate();
            drawerElement.AddToClassList("auto-settings-editor");
            drawerElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.cooli2.auto-menu/Editor/StaticFiles/Documents/Unity Styles/AutoMenuEditorStyles.uss"));

            return drawerElement;
        }
    }
}