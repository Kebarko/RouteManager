using System;

namespace KE.MSTS.RouteManager.Exceptions;

/// <summary>
/// Represents errors that occur during reading of the application configuration.
/// </summary>
internal class BadConfigurationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadConfigurationException"/> class.
    /// </summary>
    public BadConfigurationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadConfigurationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BadConfigurationException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadConfigurationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public BadConfigurationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
