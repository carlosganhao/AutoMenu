using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEditor.UIElements;

namespace AutoMenu.Editor
{
    [CustomPropertyDrawer(typeof(SettingsPanel))]
    public class SettingsPanelDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var drawerXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.cooli2.auto-menu/Editor/StaticFiles/Documents/Properties/SettingsPanelDrawer.uxml");
            var drawerElement = drawerXML.Instantiate();

            var _panelTypeElement = drawerElement.Q<EnumField>("panel-type");
            var _controlSchemeElement = drawerElement.Q<DropdownField>("control-scheme");

            var _graphicsSettingsSection = drawerElement.Q<VisualElement>("graphics-settings-section");
            var _audioSettingsSection = drawerElement.Q<VisualElement>("audio-settings-section");
            var _controlsSettingsSection = drawerElement.Q<VisualElement>("controls-settings-section");
            var _genericSettingsSection = drawerElement.Q<VisualElement>("generic-settings-section");

            _panelTypeElement.RegisterValueChangedCallback(OnPanelTypeChanged);
            _controlSchemeElement.choices = GetControlSchemes();

            return drawerElement;

            List<string> GetControlSchemes()
            {
                var schemes = InputSystem.actions.controlSchemes;
                List<string> result = new ();

                foreach(var controlScheme in schemes)
                {
                    result.Add(controlScheme.bindingGroup);
                }

                return result;
            }

            void OnPanelTypeChanged(ChangeEvent<System.Enum> evt)
            {
                SettingsPanelType panelType = (SettingsPanelType) evt.newValue;
                ToggleSettingsTypeProperties(panelType);
            }

            void ToggleSettingsTypeProperties(SettingsPanelType panelType)
            {
                _graphicsSettingsSection.Hide();
                _audioSettingsSection.Hide();
                _controlsSettingsSection.Hide();

                switch (panelType)
                {
                    case SettingsPanelType.GraphicsPanel:
                        _graphicsSettingsSection.Show();
                        break;
                    case SettingsPanelType.AudioPanel:
                        _audioSettingsSection.Show();
                        break;
                    case SettingsPanelType.ControlsPanel:
                        _controlsSettingsSection.Show();
                        break;
                }
            }
        }
    }
}