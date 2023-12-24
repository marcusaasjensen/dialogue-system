namespace DialogueSystem.Data
{
    public class CommandData
    {
        public DialogueCommandType Type;
        public int StartPosition;
        public int EndPosition;
        public bool MustExecute;
        public bool[] BoolValues;
        public float[] FloatValues;
        public string StringValue;
        public TextAnimationType TextAnimValue;
        public Emotion EmotionValue;
    }
    
    public enum DialogueCommandType
    {
        Pause, //ignorable
        TextSpeedChange, //ignorable
        AnimStart, //ignorable
        AnimEnd, //ignorable
        DisplayedEmotion, //must execute //TODO: adapt with reactions of other characters
        MusicStart, //must execute
        MusicEnd, //must execute
        SoundEffect, //must execute
        CameraShake, //ignorable
        Animation
    }

    public enum TextAnimationType
    {
        None,
        Shake,
        Wave,
        Wobble,
        FadeInOut,
        Flicker,
        Spiral
    }
}