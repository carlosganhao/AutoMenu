using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace AutoMenu.Editor
{
    [CustomPropertyDrawer(typeof(SerializedGenericParameter))]
    public class SerializedGenericParameterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var drawerXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.cooli2.auto-menu/Editor/StaticFiles/Documents/Properties/SerializedGenericParameterDrawer.uxml");
            var drawerElement = drawerXML.Instantiate();

            var _parameterTypeElement = drawerElement.Q<EnumField>("parameter-type");

            var _groupParameterSection = drawerElement.Q<VisualElement>("group-parameter-section");
            var _sliderParameterSection = drawerElement.Q<VisualElement>("slider-parameter-section");

            _parameterTypeElement.RegisterValueChangedCallback(OnParameterTypeChanged);

            return drawerElement;

            void OnParameterTypeChanged(ChangeEvent<System.Enum> changeEvent) 
            {
                ParameterType parameterType = (ParameterType) changeEvent.newValue;
                ToggleParameterTypeProperties(parameterType);
            }

            void ToggleParameterTypeProperties(ParameterType buttonType)
            {
                _groupParameterSection.Hide();
                _sliderParameterSection.Hide();

                switch (buttonType)
                {
                    case ParameterType.Bool:
                    case ParameterType.Dropdown:
                        _groupParameterSection.Show();
                        break;
                    case ParameterType.Slider:
                        _sliderParameterSection.Show();
                        break;
                }
            }
        }
    }
}