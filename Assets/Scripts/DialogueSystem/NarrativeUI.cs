using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI messageText;
    
    [Space, Header("Buttons")]
    [SerializeField] private Button dialogueOptionButtonPrefab;
    [SerializeField] private Button nextMessageButton;
    [SerializeField] private Vector3 buttonOffset;
    
    [Space, Header("Rendering")]
    [SerializeField, CanBeNull] private Renderer speakingCharacterImage;

    [Space, Header("Default Values"), SerializeField, NotNull]
    private Speaker defaultSpeaker;

    [SerializeField] private int punctuationDelayMultiplier = 10;

    private IEnumerator _currentMessageShowing;

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

    public void ShowMessage(Speaker speaker, Message message)
    {
        if (message == null) return;
        
        speakerNameText.text = message.Speaker;

        if (speaker == null) speaker = defaultSpeaker;
        
        var speakerBehaviour = 
            speaker.narrativeBehaviours.Find(emotion => emotion.emotionLabel == message.EmotionDisplayed) 
            ??
            defaultSpeaker.narrativeBehaviours[0];
        
        if(_currentMessageShowing != null)
            StopCoroutine(_currentMessageShowing);

        _currentMessageShowing = ShowLetterByLetter(message.Content, speakerBehaviour.speakingRhythm, speakerBehaviour.speakingSound);
        StartCoroutine(_currentMessageShowing);
        
        speakingCharacterImage = speakerBehaviour.characterFace;
    }
    private IEnumerator ShowLetterByLetter(string message, float delayBetweenLetters, AudioClip speakerSound)
    {
        
        messageText.text = "";
        foreach (var c in message)
        {
            var currentDelay = delayBetweenLetters;
            
            switch (c)
            {
                case ' ':
                    messageText.text += c; continue;
                case '.' or '?' or '!' or ',':
                    currentDelay = delayBetweenLetters * punctuationDelayMultiplier;
                    break;
            }

            AudioManager.Instance.PlaySound(speakerSound);
            messageText.text += c;
            yield return new WaitForSeconds(currentDelay);
        }

        _currentMessageShowing = null;
    }
    public bool IsShowingCurrentMessage() => _currentMessageShowing != null;

    public void ShowAllMessage(Message currentMessage)
    {
        StopCoroutine(_currentMessageShowing);
        _currentMessageShowing = null;
        messageText.text = currentMessage.Content;
    }

    public void EnableNarrationUI() => nextMessageButton.gameObject.SetActive(true);
    public void DisableNarrationUI() => nextMessageButton.gameObject.SetActive(false);
    public void CloseDialogue() => this.gameObject.SetActive(false);
}