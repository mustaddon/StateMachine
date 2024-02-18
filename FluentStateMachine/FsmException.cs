using System;

namespace FluentStateMachine;

public class FsmException(string message, Exception innerException = null) : Exception(message, innerException);
