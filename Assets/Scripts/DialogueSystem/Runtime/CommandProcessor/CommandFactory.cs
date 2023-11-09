using DialogueSystem.Data;
using DialogueSystem.Runtime.Audio;
using DialogueSystem.Runtime.UI;

namespace DialogueSystem.Runtime.CommandProcessor
{
    public static class CommandFactory
    {
        public static PauseCommand CreatePauseCommand(CommandData data, TextTyper typer) => new(data.Position, data.MustExecute, data.FloatValue, typer);
        public static SpeedCommand CreateSpeedCommand(CommandData data, TextTyper typer) => new(data.Position, data.MustExecute, data.FloatValue, typer);
        public static StateCommand CreateStateCommand(CommandData data, Emotion emotion, NarrativeUI ui, CharacterSpeaker speaker, CharacterData character) => new(data.Position, data.MustExecute, emotion, ui, speaker, character);
    }
}