using System;

namespace Faktory.Core.Exceptions;

public class InvalidExitCodeException(string message) : Exception(message);