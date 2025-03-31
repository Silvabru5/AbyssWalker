using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonTextColorHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText; // drag the button's text object here in the inspector
    public Color normalColor = Color.black; // color when not hovered
    public Color hoverColor = Color.white;  // color when hovered

    // this runs when the mouse pointer enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.color = hoverColor;
    }

    // this runs when the mouse pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.color = normalColor;
    }
}
