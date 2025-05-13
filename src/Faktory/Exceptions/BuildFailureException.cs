using System;

namespace Faktory.Core.Exceptions;

public class BuildFailureException(string message) : Exception(message);