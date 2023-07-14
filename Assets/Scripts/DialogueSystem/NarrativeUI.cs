using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NarrativeUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Space, Header("Buttons")] 
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private Button dialogueOptionButtonPrefab;
    [SerializeField] private Button nextMessageButton;
    [SerializeField] private Vector2 buttonOffset;
    
    [FormerlySerializedAs("speakingCharacterImage")]
    [Space, Header("Rendering")]
    [SerializeField, CanBeNull] private Image speakingCharacterSprite;

    [Space, Header("Default Values"), SerializeField]
    private Speaker defaultSpeaker;

    [SerializeField] private int punctuationDelayMultiplier = 10;

    private IEnumerator _currentMessageShowing;

    private static readonly char[] Punctuation = {'.', ',', '!', '?', ':', ';'};
    private static readonly char[] CharactersToIgnore = {' '};

    public event Action OnMessageEnd;

    public List<Button> DisplayDialogueOptionButtons(List<DialogueOption> options)
    {
        nextMessageButton.gameObject.SetActive(false);
        
        var buttonList = new List<Button>();

        var buttonRect = dialogueOptionButtonPrefab.GetComponent<RectTransform>().rect;

        var firstButtonXPosition = (buttonRect.width * (1 - options.Count) - options.Count * buttonOffset.x + buttonOffset.x) / 2;
        
        foreach(var option in options)
        {
            var newOptionButton = Instantiate(dialogueOptionButtonPrefab, buttonsParent);
            var buttonPosition = newOptionButton.GetComponent<RectTransform>();

            var currentIndex = options.IndexOf(option);
            
            buttonPosition.localPosition = new Vector3(firstButtonXPosition + currentIndex * (buttonRect.width + buttonOffset.x), buttonOffset.y,0);
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

        var defaultSpeakerBehaviour = speaker.narrativeBehaviours[0] ?? defaultSpeaker.narrativeBehaviours[0];
        
        var speakerBehaviour =
            speaker.narrativeBehaviours.Find(emotion => emotion.emotionLabel == message.EmotionDisplayed)
            ?? defaultSpeakerBehaviour;
        
        SetupSpeakerSprite(speakerBehaviour.characterFace);
        
        if(_currentMessageShowing != null)
            StopCoroutine(_currentMessageShowing);

        _currentMessageShowing = ShowLetterByLetter(message.Content, speakerBehaviour.speakingRhythm, speakerBehaviour.speakingSound);
        StartCoroutine(_currentMessageShowing);
    }

    private void SetupSpeakerSprite(Sprite sprite)
    {
        if (speakingCharacterSprite == null) return;
        
        if(sprite == null)
            speakingCharacterSprite.gameObject.SetActive(false);
        else
        {
            speakingCharacterSprite.gameObject.SetActive(true);
            speakingCharacterSprite.sprite = sprite;
        }
    } 
    
    private IEnumerator ShowLetterByLetter(string message, float delayBetweenLetters, AudioClip speakerSound)
    {
        
        messageText.text = "";
        float currentDelay = 0;
        foreach (var c in message)
        {
            yield return new WaitForSeconds(currentDelay);
            currentDelay = delayBetweenLetters;

            if (CharactersToIgnore.Contains(c))
            {
                messageText.text += c;
                continue;
            }
            
            if(Punctuation.Contains(c))
                currentDelay = delayBetweenLetters * punctuationDelayMultiplier;

            AudioManager.Instance.PlaySound(speakerSound);
            messageText.text += c;
        }

        EndMessage();
    }
    public bool IsShowingCurrentMessage() => _currentMessageShowing != null;

    public void ShowAllMessage(Message currentMessage)
    {
        messageText.text = currentMessage.Content;
        EndMessage();
    }

    private void EndMessage()
    {
        StopCoroutine(_currentMessageShowing);
        _currentMessageShowing = null;
        OnMessageEnd?.Invoke();
    }

    public void EnableNarrationUI() => nextMessageButton.gameObject.SetActive(true);
    public void DisableNarrationUI() => nextMessageButton.gameObject.SetActive(false);
    public void CloseDialogue() => this.gameObject.SetActive(false);
}
