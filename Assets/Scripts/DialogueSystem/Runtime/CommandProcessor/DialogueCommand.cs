namespace DialogueSystem.Runtime.CommandProcessor
{
    public abstract class DialogueCommand
    {
        public int Position { get; }
        public bool MustExecute { get; }

        protected DialogueCommand(int position, bool mustExecute)
        {
            Position = position;
            MustExecute = mustExecute;
        }
        
        public abstract void Execute();
    }
}