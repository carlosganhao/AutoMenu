using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

namespace AutoMenu
{
    [Serializable]
    public class SettingsPanel
    {
        #region Common Panel Properties
        public string PanelName;
        public SettingsPanelType PanelType;
        #endregion
        #region Generic Panel Properties
        public List<SerializedGenericParameter> GenericParameters;
        #endregion
        #region Graphics Panel Properties
        public bool UseResolution;
        public bool UseFullscreen;
        public bool UseQualityLevel;
        public bool UseVsync;
        #endregion
        #region Audio Panel Properties
        public AudioParameter[] AudioParameters;
        #endregion
        #region Control Panel Properties
        public string ControlScheme;
        #endregion
    }
}
