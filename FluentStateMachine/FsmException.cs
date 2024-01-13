using System;

namespace FluentStateMachine;

public class FsmException(string message, Exception inner) : Exception(message, inner)
{
    public FsmException() : this(null, null) { }

    public FsmException(string message) : this(message, null) { }
}
