﻿namespace DialogueSystem.Runtime.CommandProcessor
{
    public abstract class DialogueCommand
    {
        public int StartPosition { get; }
        public int EndPosition { get; }
        public bool MustExecute { get; }

        protected DialogueCommand(int startPosition, bool mustExecute)
        {
            StartPosition = startPosition;
            MustExecute = mustExecute;
        }
        
        protected DialogueCommand(int startPosition, int endPosition, bool mustExecute) : this(startPosition, mustExecute) => EndPosition = endPosition;
        
        public abstract void Execute();
    }
}