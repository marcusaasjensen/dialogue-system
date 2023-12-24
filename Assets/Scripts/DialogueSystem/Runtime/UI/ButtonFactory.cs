using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogueSystem.Runtime.UI
{
    public class ButtonFactory : MonoBehaviour
    {
        public static Button CreateButton(Button buttonPrefab, Button disabledButtonPrefab, Transform parent, bool isDisabled, string buttonText, UnityAction onClickAction)
        {
            var newButton = Instantiate(isDisabled ? disabledButtonPrefab : buttonPrefab, parent);
            newButton.interactable = !isDisabled;
            
            var optionTextContainer = newButton.transform.GetComponentInChildren<TextMeshProUGUI>();
            optionTextContainer.text = buttonText;

            newButton.onClick.AddListener(onClickAction);

            return newButton;
        }
        
        public static void PlaceButton(RectTransform button, Rect parentRect, int columnIndex, int rowIndex, int numberOfButtonsInRow, Vector2 buttonOffset)
        {
            var buttonRect = button.rect;

            var initialButtonXPosition = (buttonRect.width * (1 - numberOfButtonsInRow) -
                numberOfButtonsInRow * buttonOffset.x + buttonOffset.x) / 2;
            var initialButtonYPosition = parentRect.height / 2 - buttonRect.height / 2;

            var xOffset = columnIndex * (buttonRect.width + buttonOffset.x);
            var yOffset = rowIndex * (buttonRect.height + buttonOffset.y);

            button.localPosition = new Vector3(initialButtonXPosition + xOffset, initialButtonYPosition - yOffset, 0);
        }
    }
}