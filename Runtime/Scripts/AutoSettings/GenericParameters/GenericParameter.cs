using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace AutoMenu
{
    public abstract class GenericParameter<TValue, TElement> where TElement : VisualElement
    {
        public string Name;
        public Action<TElement> StartupFunc;
        public EventCallback<ChangeEvent<TValue>> OnChangeEvent;

        public abstract TElement RenderElement();

        public static implicit operator VisualElement(GenericParameter<TValue, TElement> g) => g.RenderElement();
    }
}
