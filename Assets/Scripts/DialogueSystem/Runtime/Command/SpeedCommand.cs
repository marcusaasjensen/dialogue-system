using DialogueSystem.Runtime.UI;

namespace DialogueSystem.Runtime.Command
{
    public class SpeedCommand : DialogueCommand
    { 
        private readonly float _newSpeed;
        private readonly TextTyper _textTyper;

        public SpeedCommand(int startPosition, bool mustExecute, float speed, TextTyper typer) : base(startPosition, mustExecute)
        {
            _newSpeed = speed;
            _textTyper = typer;
        }

        public override void Execute() => _textTyper.ChangePace(_newSpeed);
    }
}