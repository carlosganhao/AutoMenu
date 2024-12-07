using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AutoMenu
{
    public class BoolParameter : GenericParameter<int, RadioButtonGroup>
    {
        public List<string> GetOptionValuesFunc;

        public BoolParameter(string name, List<string> getOptionValuesFunc, Action<RadioButtonGroup> startupFunc, EventCallback<ChangeEvent<int>> onChangeEvent)
        {
            this.Name = name;
            this.StartupFunc = startupFunc;
            this.GetOptionValuesFunc = getOptionValuesFunc;
            this.OnChangeEvent = onChangeEvent;
        }

        public BoolParameter(string name, Action<RadioButtonGroup> startupFunc, EventCallback<ChangeEvent<int>> onChangeEvent)
        {
            this.Name = name;
            this.StartupFunc = startupFunc;
            this.GetOptionValuesFunc = new List<string>() {"No", "Yes"};
            this.OnChangeEvent = onChangeEvent;
        }

        public override RadioButtonGroup RenderElement()
        {
            var result = new RadioButtonGroup(this.Name, this.GetOptionValuesFunc);

            this.StartupFunc(result);
            result.RegisterValueChangedCallback(this.OnChangeEvent);
            result.AddToClassList("auto-settings-parameter");
            result.AddToClassList("auto-settings-bool-field");

            return result;
        }
    }
}
