using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementExtensionMethods
{
    public static void Hide(this VisualElement el)
    {
        el.style.display = DisplayStyle.None;
    }

    public static void Show(this VisualElement el)
    {
        el.style.display = DisplayStyle.Flex;
    }
}
