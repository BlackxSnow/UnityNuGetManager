using UnityEngine.UIElements;

namespace UnityNuGetManager.Extensions
{
    public static class VisualElementExtensions
    {
        public static void SetDisplay(this VisualElement element, bool isDisplayed)
        {
            element.style.display = new StyleEnum<DisplayStyle>(isDisplayed ? DisplayStyle.Flex : DisplayStyle.None);
        }
    }
}