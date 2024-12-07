using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace AutoMenu.Editor
{
    [CustomPropertyDrawer(typeof(MenuButton))]
    public class MenuButtonDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var drawerXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.cooli2.auto-menu/Editor/StaticFiles/Documents/Properties/MenuButtonDrawer.uxml");
            var drawerElement = drawerXML.Instantiate();

            var _buttonTypeElement = drawerElement.Q<EnumField>("button-type");
            var _customPageToggle = drawerElement.Q<Toggle>("settings-button-use-custom-toggle");

            var _playButtonSection = drawerElement.Q<VisualElement>("play-button-properties");
            var _settingsButtonSection = drawerElement.Q<VisualElement>("settings-button-properties");
            var _quitButtonSection = drawerElement.Q<VisualElement>("quit-button-properties");
            var _genericButtonSection = drawerElement.Q<VisualElement>("generic-button-properties");

            var _onLoadCustomPageField = drawerElement.Q<VisualElement>("settings-on-load-custom-page-event");

            _buttonTypeElement.RegisterValueChangedCallback(OnButtonTypeChanged);
            _customPageToggle.RegisterValueChangedCallback(OnCustomPageToggleChanged);

            drawerElement.Q<VisualElement>("unity-content-container").style.height = 0;

            return drawerElement;

            void OnButtonTypeChanged(ChangeEvent<System.Enum> changeEvent) 
            {
                ButtonType buttonType = (ButtonType) changeEvent.newValue;
                ToggleButtonTypeProperties(buttonType);
            }

            void ToggleButtonTypeProperties(ButtonType buttonType)
            {
                _playButtonSection.Hide();
                _settingsButtonSection.Hide();
                _quitButtonSection.Hide();
                _genericButtonSection.Hide();

                switch (buttonType)
                {
                    case ButtonType.PlayButton:
                        _playButtonSection.Show();
                        break;
                    case ButtonType.SettingsButton:
                        _settingsButtonSection.Show();
                        break;
                    case ButtonType.QuitButton:
                        _quitButtonSection.Show();
                        break;
                    case ButtonType.GeneralButton:
                        _genericButtonSection.Show();
                        break;
                }
            }

            void OnCustomPageToggleChanged(ChangeEvent<bool> evt)
            {
                if(evt.newValue)
                {
                    _onLoadCustomPageField.Show();
                }
                else
                {
                    _onLoadCustomPageField.Hide();
                }
            }
        }
    }

}