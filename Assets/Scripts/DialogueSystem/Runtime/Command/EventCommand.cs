using System.Linq;
using DialogueSystem.Runtime.Interaction;
using DialogueSystem.Utility;

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
            if (_events is not { Length: > 0 }) return;
            try
            {
                _events.First(e => e.EventName == _eventName).OnDialogueEvent?.Invoke();
            }
            catch
            {
                LogHandler.Alert($"Event with name \"{_eventName}\" not found.");
            }
        }
    }
}