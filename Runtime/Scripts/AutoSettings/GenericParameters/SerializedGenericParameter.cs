using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

namespace AutoMenu
{
    [Serializable]
    public class SerializedGenericParameter
    {
        public string Name;
        public string PrefsKey;
        public ParameterType Type;
        public List<string> Options;
        public float MinValue;
        public float MaxValue;
        public int DefaultValue;
        public UnityEvent<VisualElement> OnStartup;
        public UnityEvent<object> OnValueChanged;
    }
}
