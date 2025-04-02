using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace States
{
    public class StateMachine : IDisposable
    {
        //[Inject] private readonly DiContainer _container;

        public State CurrentState { get; private set; }

        private bool _strict;

        private int _switchStateCount;
        private readonly Dictionary<Type, State> _states = new();

        private bool _dispose;

        public void Dispose()
        {
            _dispose = true;

            if (CurrentState != null)
            {
                Debug.Log("Dispose state: " + CurrentState);

                CurrentState.Exit(null);
                CurrentState = null;
            }
        }

        public StateMachine Add(State state)
        {
            Type type = state.GetType();
            if (_states.ContainsKey(type))
                throw new ArgumentException("Already state " + type.Name);

            //_container.Inject(state);

            state.Parent = this;
            _states.Add(type, state);

            return this;
        }

        public StateMachine AddTransition<TF, T>()
            where TF : State
            where T : State
        {
            Type fromType = typeof(TF);
            Type toType = typeof(T);

            var from = _states.Values.FirstOrDefault(s => s.GetType() == fromType);
            if (from == null) throw new ArgumentException($"State of type {fromType.FullName} not found");

            var to = _states.Values.FirstOrDefault(s => s.GetType() == toType);
            if (to == null) throw new ArgumentException($"State of type {toType.FullName} not found");

            AddTransition(from.GetType(), to.GetType());
            return this;
        }

        public StateMachine AddTransition(State from, State to)
        {
            // Add states If not exists
            if (!_states.ContainsValue(from)) Add(from);
            if (!_states.ContainsValue(to)) Add(to);

            from.AddTransition(to);
            return this;
        }

        public StateMachine AddTransition(Type from, Type to)
        {
            State fromState;
            if (!_states.TryGetValue(from, out fromState))
                throw new ArgumentException($"State {from} not found");

            State toState;
            if (!_states.TryGetValue(to, out toState))
                throw new ArgumentException($"State {to} not found");

            fromState.AddTransition(toState);
            return this;
        }

        public StateMachine SetStrict(bool strict)
        {
            _strict = strict;
            return this;
        }

        public StateMachine ClearStates()
        {
            if (CurrentState != null)
            {
                CurrentState.Exit(null);
                CurrentState = null;
            }

            _states.Clear();
            return this;
        }

        public void Service()
        {
            CurrentState?.Service();
        }

        public void SwitchState<T>(params object[] args) where T : State
        {
            SwitchState(typeof(T), args);
        }

        public void SwitchState(Type type, params object[] args)
        {
            State state;
            if (!_states.TryGetValue(type, out state))
                throw new ArgumentException($"State {type.Name} not found");

            SwitchState(state, args);
        }

        public void SwitchState(State state, params object[] args)
        {
            if (_dispose)
                return;

            if (!_states.ContainsValue(state))
                throw new ArgumentException($"State {state.GetType().Name} not found");

            // Check transition
            if (_strict && CurrentState != null && !CurrentState.HasTransition(state))
            {
                UpdateTransitions(CurrentState);
                if (!CurrentState.HasTransition(state))
                {
                    // throw new ArgumentException($"Transition from {currentState} to {state} not allowed");
                    Debug.LogError($"Transition from {CurrentState} to {state} not allowed");
                    return;
                }
            }

            int count = ++_switchStateCount;
            State prevState = CurrentState;
            prevState?.Exit(state, args);

            // Exit has SwitchState
            if (_switchStateCount != count)
                return;

            CurrentState = state;
            state.Enter(prevState, args);
        }

        private void UpdateTransitions(State state)
        {
            if (state.Transitions == null)
            {
                Debug.LogWarning($"State '{state}' doesn't have transitions");
                return;
            }

            foreach (Type transition in state.Transitions)
            {
                if (state.HasTransition(transition))
                    continue;

                State other;
                if (!_states.TryGetValue(transition, out other))
                    continue;

                state.AddTransition(other);
            }
        }
    }
}