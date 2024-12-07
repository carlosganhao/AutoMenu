using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace AutoMenu
{
    [RequireComponent(typeof(UIDocument))]
    public class AutoMenu : MonoBehaviour
    {
        public bool UseCustomPage = false;
        public string ContainerId = "menu-buttons-container";

        [SerializeField] private string _titleText = "Dummy Title";
        [SerializeField] private string _footerText = "Made by ANON";
        [SerializeField] private StyleSheet _autoMenuStyleSheet;
        [SerializeField] private List<MenuButton> _buttons;
        private UIDocument _uiDocument;
        private AutoSettings _autoSettings;
        private VisualElement _quitWarningElement;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _uiDocument = GetComponent<UIDocument>();
            _autoSettings = GetComponent<AutoSettings>();

            if(_autoMenuStyleSheet) _uiDocument.rootVisualElement.panel.visualTree.styleSheets.Add(_autoMenuStyleSheet);
            if(!UseCustomPage) PrepareDefaultPage();

            AppendMenuButtons(ContainerId);
        }

        void OnValidate()
        {
            if(!UseCustomPage && TryGetComponent<UIDocument>(out UIDocument uiDocument))
            {
                uiDocument.visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.cooli2.auto-menu/Runtime/StaticFiles/Documents/DefaultMenuPage.uxml");
                ContainerId = "menu-buttons-container";
            }

            if(!TryGetComponent<AutoSettings>(out _) && _buttons.Any((b) => b.ButtonType == ButtonType.SettingsButton && !b.UseCustomSettings))
            {
                _autoSettings = gameObject.AddComponent<AutoSettings>();
            }
        }

        private void PrepareDefaultPage()
        {
            _uiDocument.rootVisualElement.Q<Label>("logo-text").text = _titleText;
            _uiDocument.rootVisualElement.Q<Label>("footer-text").text = _footerText;
        }

        private void AppendMenuButtons(string containerId)
        {
            Assert.IsFalse(string.IsNullOrEmpty(containerId), "Buttons Container Id is Null or Empty.");

            VisualElement menuButtonsContainer = _uiDocument.rootVisualElement.Q(containerId);
            foreach (var button in _buttons)
            {
                var buttonEl = new Button() {
                    text = button.Text,
                    viewDataKey = $"auto-menu-${button.Text.ToLowerInvariant().Replace(' ', '-')}"
                };
                buttonEl.clicked += AddButtonSpecificCallback(button);

                button.ExtraClasses.AddRange(new string[] {"auto-menu-button", GetTypeSpecificClass(button.ButtonType), $"auto-menu-button-${button.Text.ToLowerInvariant().Replace(' ', '-')}"});
                foreach (var styleClass in button.ExtraClasses)
                {
                    buttonEl.AddToClassList(styleClass);
                }

                menuButtonsContainer.Add(buttonEl);
            }

            Action AddButtonSpecificCallback(MenuButton button) => button.ButtonType switch
            {
                ButtonType.GeneralButton => button.Event.Invoke,
                ButtonType.PlayButton => () => SceneManager.LoadScene(button.SceneIdToPlay),
                ButtonType.SettingsButton => () => {
                    if(button.UseCustomSettings)
                    {
                        button.OnLoadCustomSettings.Invoke(_uiDocument, _uiDocument.rootVisualElement);
                    }
                    else
                    {
                        _autoSettings.ShowSettings(_uiDocument.rootVisualElement);
                    }
                },
                ButtonType.QuitButton => () => {
                    if(button.ShowWarning)
                    {
                        ShowQuitWarning();
                    }
                    else
                    {
                        CloseGame();
                    }
                },
                _ => throw new ArgumentException("ButtonType outside of possible values"),
            };
        }

        private string GetTypeSpecificClass(ButtonType type) => type switch {
            ButtonType.PlayButton => "auto-menu-play-button",
            ButtonType.GeneralButton => "auto-menu-general-button",
            ButtonType.SettingsButton => "auto-menu-settings-button",
            ButtonType.QuitButton => "auto-menu-quit-button",
            _ => "auto-menu-missing-type-button",
        };

        private void ShowQuitWarning()
        {
            _quitWarningElement = new VisualElement();
            _quitWarningElement.name = "auto-menu-quit-warning-overlay";
            _quitWarningElement.AddToClassList("auto-menu-overlay");

            var quitWarningDialog = new VisualElement();
            quitWarningDialog.name = "auto-menu-quit-warning";
            quitWarningDialog.AddToClassList("auto-menu-dialog");

            var warningText = new Label("Are you sure you want to quit?");
            warningText.AddToClassList("auto-menu-quit-warning-text");
            var buttonContainer = new VisualElement();
            buttonContainer.AddToClassList("auto-menu-quit-warning-button-container");
            var cancelButton = new Button(() => {
                _uiDocument.rootVisualElement.panel.visualTree.Remove(_quitWarningElement);
            });
            cancelButton.text = "Cancel";
            cancelButton.AddToClassList("auto-menu-button");
            cancelButton.AddToClassList("auto-menu-cancel-button");
            var confirmButton = new Button(CloseGame);
            confirmButton.text = "Confirm";
            confirmButton.AddToClassList("auto-menu-button");
            confirmButton.AddToClassList("auto-menu-confirm-button");

            buttonContainer.Add(cancelButton);
            buttonContainer.Add(confirmButton);
            quitWarningDialog.Add(warningText);
            quitWarningDialog.Add(buttonContainer);
            _quitWarningElement.Add(quitWarningDialog);

            _uiDocument.rootVisualElement.panel.visualTree.Add(_quitWarningElement);
        }

        private void CloseGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
            #else
                Application.Quit();
            #endif
        }
    }
}