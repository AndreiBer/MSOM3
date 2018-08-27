using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xvue.Framework.API.Commands;
using Xvue.Framework.API.Services.Log;
using Xvue.Framework.API.ViewModels.Base;
using Xvue.Framework.API.DataModels.Base;
using System.Collections.Generic;

namespace Xvue.Framework.API.ViewModels.StateMachine
{


    [ComVisibleAttribute(false)]
    public abstract class ViewModelStateMachineBase<TState, TInput> : ViewModelSerializedPlugin 
        where TInput : IComparable
        where TState : IComparable
    {
        static readonly TraceSwitch _stateMachineTrace = new TraceSwitch("StateMachineTrace", "State Machine Tracing","4");
        #region localVariables
        StateMachineState<TState, TInput> _currentState = null;
        #endregion localVariables
        public StateMachineState<TState, TInput> CurrentState
        {
            get { return _currentState; }
            private set {
                if (!_currentState.Equals(value) )
                {
                    if (_stateMachineTrace.TraceVerbose)
                        PrintTraceMessage(DisplayName + ", old state " + _currentState + " changed to " + value, _stateMachineTrace.DisplayName);
                    _currentState = value;
                    OnPropertyChanged("CurrentState");
                    if (uiDispatcher != null)
                    {
                        uiDispatcher.BeginInvoke(new Action(() =>
                        {
                            CommandManager.InvalidateRequerySuggested();
                        }));
                    }
                }
            }
        }

        readonly Dictionary<TState, StateMachineState<TState, TInput>> States = new Dictionary<TState, StateMachineState<TState, TInput>>();

        public void AddTransition(TState state, TInput input, TState nextState, DelegateStateMachineTransitionCustomFunction<TInput> function)
        {
            if (States.ContainsKey(state))
            {
                States[state].AddTransition(input, nextState, function);
            }
            else
            {
                StateMachineState<TState, TInput> newState = new StateMachineState<TState, TInput>(state, input, nextState, function);
                States.Add(state, newState);
            }
            if(!States.ContainsKey(nextState))
            {
                States.Add(nextState, new StateMachineState<TState, TInput>(nextState));
            }
        }

        #region ICoreServiceStateMachine Members

        public event DelegateStateChangedEventHandler<TState> ChangedStateEvent;

        #endregion
        private System.Windows.Threading.Dispatcher uiDispatcher=null;
        public bool StartMachine(TState initialState)
        {
            uiDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            if(_currentState == null && States.ContainsKey(initialState) )
            {
                _currentState = States[initialState];
                return true;
            }

            return false;
        }

        private void OnChangedState(TState oldState, TState newState)
        {
            if (ChangedStateEvent != null)
                ChangedStateEvent(oldState, newState);
        }

        protected virtual bool CanAcceptInput(TInput input)
        {
            StateMachineTransition<TState, TInput> transition=null;
            if (CurrentState != null)
            {
                if (States.ContainsKey(CurrentState.State))
                {
                    transition = States[CurrentState.State].FindTransitionForInput(input);
                }
                if (transition != null)
                    return true;
            }
            return false;
        }

        DelegateCommand<TInput> commandAcceptInput;
        public ICommand CommandAcceptInput
        {
            get
            {
                if (commandAcceptInput == null)
                {
                    commandAcceptInput = new DelegateCommand<TInput>(_acceptInputGoToState, CanAcceptInput);
                }
                return commandAcceptInput;
            }
        }

        void _acceptInputGoToState(TInput input)
        {
            StateMachineTransition<TState, TInput> trans = null;
            StateMachineState<TState, TInput> oldStateTemp = null;                        
            if (_stateMachineTrace.TraceVerbose)
                PrintTraceMessage(DisplayName + " incoming input " + input + " in state " + _currentState.ToString(), _stateMachineTrace.DisplayName);
            if (CanAcceptInput(input))
            {
                lock (_currentState)
                {
                    oldStateTemp = _currentState;
                    if (CurrentState != null)
                    {
                        trans = CurrentState.FindTransitionForInput(input);
                        if (trans != null)
                        {
                            if (trans.Run(input))
                            {
                                if (States.ContainsKey(trans.NextState))
                                {
                                    CurrentState = States[trans.NextState];
                                }
                                else
                                {
                                    Log.AddEventLog(0, DisplayName, "State: " + trans.NextState + " does not exist in the state list");
                                }
                            }
                            if (CurrentState != null)
                            {
                                OnChangedState(oldStateTemp.State, _currentState.State);
                            }
                            else
                            {
                                CurrentState = oldStateTemp;
                            }
                        }
                    }
                }
            }
            else
            {
                if (_stateMachineTrace.TraceInfo)
                    PrintTraceMessage(DisplayName + " Failed to accept input " + input + " in state " + _currentState.ToString(), _stateMachineTrace.DisplayName);
            }
        }
    }
}
