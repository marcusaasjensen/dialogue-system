using DialogueSystem.Runtime.UI;

namespace DialogueSystem.Runtime.Command
{
    public class PauseCommand : DialogueCommand
    {
        private readonly float _pauseDuration;
        private readonly TextTyper _textTyper;

        public PauseCommand(int position, bool mustExecute, float pause, TextTyper typer) : base(position, mustExecute)
        {
            _pauseDuration = pause;
            _textTyper = typer;
        }

        public override void Execute() => _textTyper.Pause(_pauseDuration);
    }
}