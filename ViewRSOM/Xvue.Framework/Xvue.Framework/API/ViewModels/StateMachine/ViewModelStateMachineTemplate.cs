using System;
using System.Collections.Generic;

namespace Xvue.Framework.API.ViewModels.StateMachine
{

    public delegate void DelegateStateChangedEventHandler<in T>(T oldState, T newState);
    public delegate bool DelegateStateMachineTransitionCustomFunction<in TInput>(TInput input);

    public class StateMachineState<TState, TInput> 
        where TInput : IComparable
        where TState : IComparable
    {
        public override string ToString()
        {
            return Description;
        }

        public TState State { get; private set; }

        public string Description { get; private set; }
        
        readonly Dictionary<TInput, StateMachineTransition<TState, TInput>> _allowedTransitions;    

        public StateMachineState(TState stateObject)
        {
            State = stateObject;
            Description = stateObject.ToString();
            _allowedTransitions = new Dictionary<TInput, StateMachineTransition<TState, TInput>>();
        }

        public StateMachineState(TState stateObject, TInput input, TState nextState, DelegateStateMachineTransitionCustomFunction<TInput> function)
            :this(stateObject)
        {
            AddTransition(input, nextState, function);
        }

        internal StateMachineTransition<TState, TInput> FindTransitionForInput(TInput input)
        {
            if( _allowedTransitions.ContainsKey(input) )
            {
                StateMachineTransition<TState, TInput> transition = _allowedTransitions[input];
                return transition;
            }
            return null;
        }
        internal void AddTransition(TInput input, TState nextState, DelegateStateMachineTransitionCustomFunction<TInput> function)
        {
            StateMachineTransition<TState, TInput> newTransition = new StateMachineTransition<TState,TInput>(input, nextState, function);
            _allowedTransitions.Add(input, newTransition);
        }
    }
    public class StateMachineTransition<TState, TInput> 
        where TInput : IComparable
        where TState : IComparable
    {
        private TInput _input;        
        internal TState NextState { get; private set; }
        internal string Description { get; private set; }

        public override string ToString()
        {
            return Description;
        }

        private event DelegateStateMachineTransitionCustomFunction<TInput> transitionCustomFunction;
        public StateMachineTransition(TInput input, TState nextState, DelegateStateMachineTransitionCustomFunction<TInput> function)
        {
            _input = input;
            NextState = nextState;
            Description = input.ToString() + "->" + nextState.ToString();
            if (function != null)
                transitionCustomFunction += function;
            //if( function != null)
            //    TransitionCustomFunction += function;
        }

        internal bool AllowInput(TInput input)
        {
            return _input.Equals(input);
        }

        internal bool Run(TInput input)
        {
            if (transitionCustomFunction != null)
            {
                return transitionCustomFunction(input);
            }
            else
                return true; //if no function is specified, transition will always occur
        }
    }
}
