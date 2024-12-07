using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AutoMenu
{
    [Serializable]
    public class MenuButton
    {
        #region Common Button Properties
        public string Text;
        public ButtonType ButtonType;
        public List<string> ExtraClasses;
        #endregion

        #region Generic Button Properties
        public UnityEvent Event;
        #region Condition Buttion Properties
        public UnityEvent Condition;
        #endregion
        #endregion

        #region Play Button Properties
        public int SceneIdToPlay;
        #endregion

        #region Settings Button Properties
        public bool UseCustomSettings;
        public UnityEvent<UIDocument, VisualElement> OnLoadCustomSettings;
        #endregion

        #region Quit Button Properties
        public bool ShowWarning;
        #endregion
    }
}