namespace DialogueSystem.Data
{
    public class CommandData
    {
        public DialogueCommandType Type;
        public int Position;
        public bool MustExecute;
        
        public float FloatValue;
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
        Interaction, //must execute
        MusicStart, //must execute
        MusicEnd, //must execute
        SoundEffect, //must execute
        CameraShake //ignorable
    }

    public enum TextAnimationType
    {
        None,
        Shake,
        Wave
    }
}