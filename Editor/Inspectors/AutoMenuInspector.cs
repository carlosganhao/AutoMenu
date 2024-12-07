using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace AutoMenu.Editor
{
    [CustomEditor(typeof(AutoMenu))]
    public class AutoMenuInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var drawerXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.cooli2.auto-menu/Editor/StaticFiles/Documents/Inspectors/AutoMenuInspector.uxml");
            var drawerElement = drawerXML.Instantiate();
            drawerElement.AddToClassList("auto-menu-editor");
            drawerElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.cooli2.auto-menu/Editor/StaticFiles/Documents/Unity Styles/AutoMenuEditorStyles.uss"));

            var _useCustomPageToggle = drawerElement.Q<Toggle>("auto-menu-custom-page-toggle");

            var _customPageGroup = drawerElement.Q<VisualElement>("auto-menu-custom-page-group");
            var _containerIdField = drawerElement.Q<VisualElement>("auto-menu-id-field");

            _useCustomPageToggle.RegisterValueChangedCallback(OnCustomPageToggleChanged);

            return drawerElement;

            void OnCustomPageToggleChanged(ChangeEvent<bool> evt)
            {
                if(evt.newValue)
                {
                    _customPageGroup.Hide();
                    _containerIdField.Show();
                }
                else
                {
                    _customPageGroup.Show();
                    _containerIdField.Hide();
                }
            }
        }
    }
}