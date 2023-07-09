using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button dialogueOptionButtonPrefab;
    [SerializeField] private Button nextMessageButton;
    [SerializeField] private Vector3 buttonOffset;
    [SerializeField, CanBeNull] private Renderer speakingCharacterImage;

    public void ShowMessage(Speaker speaker, Message message)
    {
        //nextMessageButton.gameObject.SetActive(true);
        if (message == null) return;
        
        speakerNameText.text = message.Speaker;
        messageText.text = message.Content;
        
        if (speaker == null) return;
        
        var speakerEmotion = speaker.emotions.Find(emotion => emotion.emotionLabel == message.EmotionDisplayed);
        speakingCharacterImage = speakerEmotion?.characterFace;
    }

    public void EnableNarrationUI()
    {
        nextMessageButton.gameObject.SetActive(true);
    }

    public List<Button> DisplayDialogueOptionButtons(List<DialogueOption> options)
    {
        nextMessageButton.gameObject.SetActive(false);
        
        var buttonList = new List<Button>();

        foreach(var option in options)
        {
            var newOptionButton = Instantiate(dialogueOptionButtonPrefab, transform);
            var buttonPosition = newOptionButton.GetComponent<RectTransform>();
            buttonPosition.position += buttonOffset * options.IndexOf(option);
            newOptionButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = option.Text;
            
            buttonList.Add(newOptionButton);
        }

        return buttonList;
    }

    public void CloseDialogue()
    {
        this.gameObject.SetActive(false);
    }
}
