using System;
using System.Collections.Generic;

namespace States
{
    public abstract class State
    {
        public StateMachine Parent { get; set; }
        public abstract Type[] Transitions { get; }

        private readonly Dictionary<Type, State> _transitions = new();

        public virtual void Enter(State prevState, params object[] args)
        {
        }

        public virtual void Service()
        {
        }

        public virtual void Exit(State nextState, params object[] args)
        {
        }

        public State AddTransition(State state)
        {
            var type = state.GetType();
            if (HasTransition(type))
                throw new ArgumentException($"{GetType().Name} has already transition to {type.Name}");

            _transitions.Add(type, state);
            return this;
        }

        public bool HasTransition(Type state)
        {
            return _transitions.ContainsKey(state);
        }

        public bool HasTransition(State state)
        {
            return _transitions.ContainsValue(state);
        }
    }
}