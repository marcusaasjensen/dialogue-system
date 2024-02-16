using System.Linq;
using DialogueSystem.Runtime.Interaction;

namespace DialogueSystem.Runtime.Command
{
    public class EventCommand : DialogueCommand
    {
        private readonly string _eventName;
        private readonly DialogueMonoBehaviour.DialogueEvent[] _events;
        
        public EventCommand(int startPosition, bool mustExecute, string eventName, DialogueMonoBehaviour.DialogueEvent[] events) : base(startPosition, mustExecute)
        {
            _eventName = eventName;
            _events = events;
        }

        public override void Execute()
        {
            if (_events is { Length: > 0 })
            {
                _events.First(e => e.EventName == _eventName).onDialogueEvent?.Invoke();
            }
        }
    }
}