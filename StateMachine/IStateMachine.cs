﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public interface IStateMachine<TState, TEvent> : IStateMachine
    {
        TState Current { get; }

        IEnumerable<TEvent> GetEvents(params object[] data);

        object Trigger(TEvent e, params object[] args);

        bool JumpTo(TState state, params object[] args);
    }

    public interface IStateMachine
    {
        Type GetStateType();

        Type GetEventType();
    }
}
