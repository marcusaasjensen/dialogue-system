namespace DialogueSystem.Runtime.Command
{
    public class NullCommand : DialogueCommand
    {
        public NullCommand(int position, bool mustExecute) : base(position, mustExecute) { }

        public override void Execute()
        {
            // Do nothing
        }
    }
}