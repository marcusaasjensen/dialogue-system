using DialogueSystem.Data;
using DialogueSystem.Runtime.UI;

namespace DialogueSystem.Runtime.Command
{
    public class AnimationCommand : DialogueCommand
    {
        private readonly TextAnimationType _animationType;
        private readonly TextAnimator _textAnimator;
        private readonly float? _speed;
        private readonly float? _amount;
        private readonly bool? _sync;

        public AnimationCommand(int startPosition, int endPosition, bool mustExecute, TextAnimator textAnimator, TextAnimationType animationType, float? speed, float? amount, bool? sync) : base(startPosition, endPosition, mustExecute)
        {
            _animationType = animationType;
            _textAnimator = textAnimator;
            _speed = speed;
            _amount = amount;
            _sync = sync;
        }

        public override void Execute() => _textAnimator.PlayAnimation(_animationType, StartPosition, EndPosition, _speed, _amount, _sync);
    }
}