using DialogueSystem.Runtime.UI;

namespace DialogueSystem.Runtime.CommandProcessor
{
    public class SpeedCommand : DialogueCommand
    { 
        private readonly float _newSpeed;
        private readonly TextTyper _textTyper;

        public SpeedCommand(int position, bool mustExecute, float speed, TextTyper typer) : base(position, mustExecute)
        {
            _newSpeed = speed;
            _textTyper = typer;
        }

        public override void Execute() => _textTyper.ChangePace(_newSpeed);
    }
}