using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEditor;

namespace AutoMenu
{
    [RequireComponent(typeof(UIDocument))]
    public class AutoSettings : MonoBehaviour
    {
        private readonly string ResolutionName = "Resolution";
        private readonly string FullscreenName = "Fullscreen";
        private readonly string QualityName = "Quality";
        private readonly string VSyncName = "VSync";
        private readonly string ControlsName = "ControlsOverrides";

        [SerializeField] private StyleSheet _autoSettingsStyleSheet;
        [SerializeField] private List<SettingsPanel> _panels;
        private UIDocument _uiDocument;
        private VisualElement _backElement;
        private VisualElement _instancedSettingsRoot;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _uiDocument = GetComponent<UIDocument>();

            if(_autoSettingsStyleSheet) _uiDocument.rootVisualElement.panel.visualTree.styleSheets.Add(_autoSettingsStyleSheet);
            
            AppendRootContainer();
            AppendSettingsPanels("auto-settings-container");
        }

        private void AppendRootContainer()
        {
            var defaultSettingsAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.cooli2.auto-menu/Runtime/StaticFiles/Documents/DefaultSettingsPage.uxml");
            var defaultSettingsElement = defaultSettingsAsset.Instantiate();
            defaultSettingsElement.AddToClassList("unity-ui-document__root");
            defaultSettingsElement.Q<Button>("auto-settings-back-button").clicked += BackCallback;
            _uiDocument.rootVisualElement.panel.visualTree.Add(defaultSettingsElement);
            _instancedSettingsRoot = defaultSettingsElement;
            _instancedSettingsRoot.Hide();
        }

        private void AppendSettingsPanels(string containerId)
        {
            Assert.IsFalse(string.IsNullOrEmpty(containerId), "Buttons Container Id is Null or Empty.");

            var settingsContainer = _uiDocument.rootVisualElement.panel.visualTree.Q<VisualElement>(containerId);

            foreach (var panel in _panels)
            {
                var tab = new Tab(panel.PanelName);
                tab.AddToClassList("auto-settings-tab");
                var scrollView = new ScrollView(ScrollViewMode.Vertical);
                tab.Add(scrollView);
                switch (panel.PanelType)
                {
                    case SettingsPanelType.GenericPanel:
                        AddGenericPanelFields(panel, scrollView);
                        break;
                    case SettingsPanelType.GraphicsPanel:
                        AddGraphicsPanelFields(panel, scrollView);
                        break;
                    case SettingsPanelType.AudioPanel:
                        AddAudioPanelFields(panel, scrollView);
                        break;
                    case SettingsPanelType.ControlsPanel:
                        AddControlsPanelFields(panel, scrollView);
                        break;
                }
                settingsContainer.Add(tab);
            }
        }

        private void AddGenericPanelFields(SettingsPanel panel, VisualElement tab)
        {
            foreach (var parameter in panel.GenericParameters)
            {
                tab.Add(RenderGenericParameter(parameter));
            }
        }

        private void AddGraphicsPanelFields(SettingsPanel panel, VisualElement tab)
        {
            if (panel.UseResolution)
            {
                var resolutionDropdown = new DropdownParameter(ResolutionName,
                    GetResolutionOptions(),
                    (element) => {
                        var value = GetIntSettingsPrefs(ResolutionName, 0);
                        var selectedResultion = Screen.resolutions[value];
                        element.index = value;
                        Screen.SetResolution(selectedResultion.width, selectedResultion.height, GetIntSettingsPrefs(FullscreenName, 1) == 1);
                    },
                    (evt) => {
                        var element = evt.currentTarget as DropdownField;
                        var changedResolution = Screen.resolutions[element.index];
                        Screen.SetResolution(changedResolution.width, changedResolution.height, GetIntSettingsPrefs(FullscreenName, 1) == 1);
                        SetIntSettingsPrefs(ResolutionName, element.index);
                });

                tab.Add(resolutionDropdown);
            } 

            if (panel.UseFullscreen)
            {
                var fullscreenToggle = new BoolParameter(FullscreenName,
                    (element) => {
                        var value = GetIntSettingsPrefs(FullscreenName, 1);
                        element.value = value;
                        Screen.fullScreen = value == 1;
                    },
                    (evt) => {
                        Screen.fullScreen = evt.newValue == 1;
                        SetIntSettingsPrefs(FullscreenName, evt.newValue);
                });

                tab.Add(fullscreenToggle);
            } 
            
            if (panel.UseQualityLevel)
            {
                var qualityDropdown = new DropdownParameter(QualityName,
                    new List<string>(QualitySettings.names),
                    (element) => {
                        var value = GetIntSettingsPrefs(QualityName, 0);
                        element.index = value;
                        QualitySettings.SetQualityLevel(value);
                    },
                    (evt) => {
                        var element = evt.currentTarget as DropdownField;
                        QualitySettings.SetQualityLevel(element.index);
                        SetIntSettingsPrefs(QualityName, element.index);
                });

                tab.Add(qualityDropdown);
            } 

            if (panel.UseVsync)
            {
                var VsyncToggle = new BoolParameter(VSyncName,
                    (element) => {
                        var value = GetIntSettingsPrefs(VSyncName, 1);
                        element.value = value;
                        QualitySettings.vSyncCount = value;
                    },
                    (evt) => {
                        QualitySettings.vSyncCount = evt.newValue;
                        SetIntSettingsPrefs(VSyncName, evt.newValue);
                });

                tab.Add(VsyncToggle);
            }
            
            AddGenericPanelFields(panel, tab);

            List<string> GetResolutionOptions()
            {
                var resolutions = Screen.resolutions;
                List<string> result = new List<string>();

                for(int i = 0; i < resolutions.Length; i++) {
                    result.Add(resolutions[i].ToString());
                }

                return result;
            }
        }

        private void AddAudioPanelFields(SettingsPanel panel, VisualElement tab)
        {
            foreach (var audioParamenter in panel.AudioParameters)
            {
                var audioSlider = new Slider(audioParamenter.Label, 0, 100);
                audioSlider.fill = true;
                audioSlider.showInputField = true;
                audioSlider.AddToClassList("auto-settings-parameter");
                audioSlider.AddToClassList("auto-settings-slider-field");
                var value = GetFloatSettingsPrefs(audioParamenter.ParameterName, 1);
                var formatedValue = Map(value, 0.0001f, 1, 0, 100);
                audioSlider.value = formatedValue;
                audioParamenter.Mixer.SetFloat(audioParamenter.ParameterName, Mathf.Log10(value) * 20);
                audioSlider.RegisterValueChangedCallback((evt) => {
                    var actualValue = Map(evt.newValue, 0, 100, 0.0001f, 1);
                    audioParamenter.Mixer.SetFloat(audioParamenter.ParameterName, Mathf.Log10(actualValue) * 20);
                    SetFloatSettingsPrefs(audioParamenter.ParameterName, actualValue);
                });
                tab.Add(audioSlider);
            }

            AddGenericPanelFields(panel, tab);

            float Map(float value, float inMin, float inMax, float outMin, float outMax) 
                => outMin + ((outMax - outMin) / (inMax - inMin)) * (value - inMin);
        }

        private void AddControlsPanelFields(SettingsPanel panel, VisualElement tab)
        {
            InputSystem.actions.LoadBindingOverridesFromJson(GetStringSettingsPrefs(ControlsName, ""));
            var actions = InputSystem.actions.actionMaps[0].actions;

            foreach (var action in actions)
            {
                var bindingIndex = action.GetBindingIndex(group: panel.ControlScheme);
                var binding = action.bindings[bindingIndex];
                if(binding.isComposite || binding.isPartOfComposite)
                {
                    bindingIndex += binding.isComposite ? 1 : 0;
                    for(var i = bindingIndex; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
                    {
                        tab.Add(CreateRebindField(action, action.bindings[i], i, action.bindings[i].name));
                    }
                }
                else
                {
                    tab.Add(CreateRebindField(action, action.bindings[bindingIndex], bindingIndex, action.name));
                }
            }

            AddGenericPanelFields(panel, tab);

            VisualElement CreateRebindField(InputAction action, InputBinding binding, int bindingIndex, string labelName)
            {
                var container = new VisualElement();
                container.style.flexDirection = FlexDirection.Row;
                container.AddToClassList("auto-settings-parameter");
                container.AddToClassList("auto-settings-control-field");

                var label = new Label(labelName);
                label.AddToClassList("auto-settings-control-field-label");

                var field = new Button();
                field.text = InputControlPath.ToHumanReadableString(binding.effectivePath);
                field.AddToClassList("auto-settings-control-field-space");

                var reset = new Button();
                reset.text = "Reset";
                reset.AddToClassList("auto-settings-control-field-reset");

                field.clicked += Rebind;
                reset.clicked += ResetBind;
                
                container.Add(label);
                container.Add(field);
                container.Add(reset);
                return container;

                void Rebind()
                {
                    field.text = "Press a Key...";

                    action.Disable();
                    var rebindingOperation = action.PerformInteractiveRebinding(bindingIndex);
                        // .WithExpectedControlType(panel.ControlScheme);

                    rebindingOperation.OnComplete(operation => {
                        SetStringSettingsPrefs(ControlsName, InputSystem.actions.SaveBindingOverridesAsJson());
                        field.text = InputControlPath.ToHumanReadableString(operation.selectedControl.path);

                        operation.Dispose();
                        action.Enable();
                    });

                    rebindingOperation.Start();
                }

                void ResetBind()
                {
                    action.RemoveBindingOverride(bindingIndex);
                    SetStringSettingsPrefs(ControlsName, InputSystem.actions.SaveBindingOverridesAsJson());
                    field.text = InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath);
                }
            }
        }
        private VisualElement RenderGenericParameter(SerializedGenericParameter parameter) => parameter.Type switch
        {
            ParameterType.Bool => (
                new BoolParameter(
                    parameter.Name,
                    parameter.Options,
                    (el) => {
                        var value = GetIntSettingsPrefs(parameter.PrefsKey, 0);
                        el.value = value;
                        parameter.OnStartup.Invoke(el);
                    },
                    (evt) => {
                        SetIntSettingsPrefs(parameter.PrefsKey, evt.newValue);
                        parameter.OnValueChanged.Invoke(evt);
                    }
                )
            ).RenderElement(),
            ParameterType.Dropdown => (
                new DropdownParameter(
                    parameter.Name,
                    parameter.Options,
                    (el) => {
                        var value = GetIntSettingsPrefs(parameter.PrefsKey, parameter.DefaultValue);
                        el.index = value;
                        parameter.OnStartup.Invoke(el);
                    },
                    (evt) => {
                        var dropdown = evt.currentTarget as DropdownField;
                        SetIntSettingsPrefs(parameter.PrefsKey, dropdown.index);
                        parameter.OnValueChanged.Invoke(evt);
                    }
                )
            ).RenderElement(),
            ParameterType.Slider => (
                new SliderParameter(
                    parameter.Name,
                    parameter.MinValue,
                    parameter.MaxValue,
                    (el) => {
                        var value = GetFloatSettingsPrefs(parameter.PrefsKey, parameter.DefaultValue);
                        el.value = value;
                        parameter.OnStartup.Invoke(el);
                    },
                    (evt) => {
                        SetFloatSettingsPrefs(parameter.PrefsKey, evt.newValue);
                        parameter.OnValueChanged.Invoke(evt);
                    }
                )
            ).RenderElement(),
            _ => throw new ArgumentException("ParamenterType outside of possible values"),
        };

        #region Public Methods
        public void ShowSettings(VisualElement calleeRootElement)
        {
            calleeRootElement.Hide();
            _instancedSettingsRoot.Show();
            _backElement = calleeRootElement;
        }
        #endregion

        #region Callbacks
        private void BackCallback()
        {
            _instancedSettingsRoot.Hide();
            _backElement.Show();
        }
        #endregion

        #region Player Prefs Setting
        private void SetIntSettingsPrefs(string name, int value)
        {
            PlayerPrefs.SetInt($"Auto-Settings::${name}", value);
        }

        private int GetIntSettingsPrefs(string name, int defaultValue = default(int))
        {
            return PlayerPrefs.GetInt($"Auto-Settings::${name}", defaultValue);
        }

        private void SetFloatSettingsPrefs(string name, float value)
        {
            PlayerPrefs.SetFloat($"Auto-Settings::${name}", value);
        }

        private float GetFloatSettingsPrefs(string name, float defaultValue = default(float))
        {
            return PlayerPrefs.GetFloat($"Auto-Settings::${name}", defaultValue);
        }

        private void SetStringSettingsPrefs(string name, string value)
        {
            PlayerPrefs.SetString($"Auto-Settings::${name}", value);
        }

        private string GetStringSettingsPrefs(string name, string defaultValue = default(string))
        {
            return PlayerPrefs.GetString($"Auto-Settings::${name}", defaultValue);
        }
        #endregion
    }
}
