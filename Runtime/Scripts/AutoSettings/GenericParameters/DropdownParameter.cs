using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AutoMenu
{
    public class DropdownParameter : GenericParameter<string, DropdownField>
    {
        public List<string> GetDropdownValuesFunc;

        public DropdownParameter(string name, List<string> getValuesFunc, Action<DropdownField> startupFunc, EventCallback<ChangeEvent<string>> onChangeEvent)
        {
            this.Name = name;
            this.StartupFunc = startupFunc;
            this.GetDropdownValuesFunc = getValuesFunc;
            this.OnChangeEvent = onChangeEvent;
        }

        public override DropdownField RenderElement()
        {
            var result = new DropdownField(this.Name, this.GetDropdownValuesFunc, 0);

            this.StartupFunc(result);
            result.RegisterValueChangedCallback(this.OnChangeEvent);
            result.AddToClassList("auto-settings-parameter");
            result.AddToClassList("auto-settings-dropdown-field");

            return result;
        }
    }
}
