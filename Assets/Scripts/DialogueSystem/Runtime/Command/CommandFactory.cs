using DialogueSystem.Data;
using DialogueSystem.Runtime.Audio;
using DialogueSystem.Runtime.Interaction;
using DialogueSystem.Runtime.UI;
using UnityEngine;

namespace DialogueSystem.Runtime.Command
{
    public static class CommandFactory
    {
        public static PauseCommand CreatePauseCommand(CommandData data, TextTyper typer) => new(data.StartPosition, data.MustExecute, data.FloatValues[0], typer);
        public static SpeedCommand CreateSpeedCommand(CommandData data, TextTyper typer) => new(data.StartPosition, data.MustExecute, data.FloatValues[0], typer);
        public static StateCommand CreateStateCommand(CommandData data, NarrativeUI ui, CharacterSpeaker speaker, CharacterData character, bool hide) => new(data.StartPosition, data.MustExecute, data.EmotionValue, ui, speaker, character, hide);
        public static MusicCommand CreateMusicCommand(CommandData data, bool stopMusic) => new(data.StartPosition, data.MustExecute, data.StringValue, stopMusic);
        public static SoundEffectCommand CreateSoundEffectCommand(CommandData data) => new(data.StartPosition, data.MustExecute, data.StringValue);
        public static AnimationCommand CreateAnimationCommand(CommandData data, TextAnimator textAnimator) => new(data.StartPosition, data.EndPosition, data.MustExecute, textAnimator, data.TextAnimValue, data.FloatValues?[0], data.FloatValues?[1], data.BoolValues?[0]);
        public static DialogueCommand CreateEventCommand(CommandData data, DialogueMonoBehaviour.DialogueEvent[] events) => new EventCommand(data.StartPosition, data.MustExecute, data.StringValue, events);
    }
}