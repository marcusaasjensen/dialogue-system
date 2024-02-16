namespace DialogueSystem.Data
{
    public class CommandData
    {
        public DialogueCommandType Type { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }
        public bool MustExecute { get; set; }
        public bool[] BoolValues { get; set; }
        public float[] FloatValues { get; set; }
        public string StringValue { get; set; }
        public TextAnimationType TextAnimValue { get; set; }
        public Emotion EmotionValue { get; set; }
    }
    
    public enum DialogueCommandType
    {
        Pause, //ignorable
        Speed, //ignorable
        DisplayedEmotion, //must execute //TODO: adapt with reactions of other characters
        MusicStart, //must execute
        MusicEnd, //must execute
        SoundEffect, //must execute
        Animation, //must execute
        Event //must execute
    }

    public enum TextAnimationType
    {
        None,
        Shake,
        Wave,
        Wobble,
        PingPong,
        Flicker,
        Spiral,
        Jitter
    }
}