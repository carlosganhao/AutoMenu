using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AutoMenu
{
    public class SliderParameter : GenericParameter<float, Slider>
    {
        public float MinValue;
        public float MaxValue;

        public SliderParameter(string name, float minValue, float maxValue, Action<Slider> startupFunc, EventCallback<ChangeEvent<float>> onChangeEvent)
        {
            this.Name = name;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.StartupFunc = startupFunc;
            this.OnChangeEvent = onChangeEvent;
        }

        public override Slider RenderElement()
        {
            var result = new Slider(this.Name, start: this.MinValue, end: this.MaxValue);
            result.fill = true;
            result.showInputField = true;

            this.StartupFunc(result);
            result.RegisterValueChangedCallback(this.OnChangeEvent);
            result.AddToClassList("auto-settings-parameter");
            result.AddToClassList("auto-settings-slider-field");

            return result;
        }
    }
}
