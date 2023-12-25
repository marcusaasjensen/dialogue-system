using DialogueSystem.Data;
using DialogueSystem.Runtime.Audio;
using DialogueSystem.Runtime.UI;

namespace DialogueSystem.Runtime.Command
{
    public class StateCommand : DialogueCommand
    {
        private readonly Emotion _emotion;
        private readonly NarrativeUI _narrativeUI;
        private readonly CharacterSpeaker _characterSpeaker;
        private readonly CharacterData _characterData;
        private readonly bool _hideCharacter;
        
        public StateCommand(int position, bool mustExecute, Emotion emotion, NarrativeUI ui, CharacterSpeaker speaker, CharacterData character, bool hide) : base(position, mustExecute)
        {
            _emotion = emotion;
            _narrativeUI = ui;
            _characterSpeaker = speaker;
            _characterData = character;
            _hideCharacter = hide;
        }

        public override void Execute()
        {
            var characterState = _characterData.GetState(_emotion);

            _narrativeUI.DisplayCharacter(characterState.CharacterFace, _hideCharacter);
            _characterSpeaker.React(characterState.ReactionSound);
            _characterSpeaker.ChangePitch(characterState.SpeakingSoundPitch);
        }
    }
}