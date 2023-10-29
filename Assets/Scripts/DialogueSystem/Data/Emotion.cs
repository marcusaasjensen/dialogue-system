using System.Collections.Generic;

namespace DialogueSystem.Data
{
    public enum Emotion
    {
        Default,
        Hidden,
        Happy,
        Neutral,
        Angry,
        Scared,
        Fearful,
        Sad,
        Disgusted,
        Annoyed,
        Surprised,
        Curious,
        Warm,
        Evil
    }

    public static class EmotionMapper
    {
        private static readonly Dictionary<string, Emotion> EmotionDictionary = new()
        {
            { "default", Emotion.Default },
            { "angry", Emotion.Angry },
            { "annoyed", Emotion.Annoyed },
            { "curious", Emotion.Curious }
        };
        
        public static Emotion GetEmotionByLabel(string label) => EmotionDictionary[label];
    }
}