﻿using DialogueSystem.Data;
using DialogueSystem.Runtime.Audio;
using DialogueSystem.Runtime.UI;

namespace DialogueSystem.Runtime.CommandProcessor
{
    public static class CommandFactory
    {
        public static PauseCommand CreatePauseCommand(CommandData data, TextTyper typer) => new(data.Position, data.MustExecute, data.FloatValue, typer);
        public static SpeedCommand CreateSpeedCommand(CommandData data, TextTyper typer) => new(data.Position, data.MustExecute, data.FloatValue, typer);
        public static StateCommand CreateStateCommand(CommandData data, NarrativeUI ui, CharacterSpeaker speaker, CharacterData character) => new(data.Position, data.MustExecute, data.EmotionValue, ui, speaker, character);
        public static MusicCommand CreateMusicCommand(CommandData data, bool stopMusic = false) => new(data.Position, data.MustExecute, data.StringValue, stopMusic);
        public static SoundEffectCommand CreateSoundEffectCommand(CommandData data) => new(data.Position, data.MustExecute, data.StringValue);
    }
}