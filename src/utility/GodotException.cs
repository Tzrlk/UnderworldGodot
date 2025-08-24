using System;
using System.Linq;
using Godot;

namespace Underworld.Utility;

/// <summary>
/// An exception specifically designed to wrap Godot Errors into something
/// that'll interrupt operation.
/// </summary>
/// <see cref="HandleError"/>
public class GodotException : Exception
{

    /// <summary>
    /// Takes a given godot error and turns it into an exception if it isn't
    /// considered "ok".
    /// </summary>
    /// <param name="error">The Godot error to check.</param>
    /// <param name="ok">An input value that won't trigger an exception.</param>
    /// <param name="alsoOk">Any other input values that won't trigger an exception.</param>
    /// <returns>The input error if ok.</returns>
    /// <exception cref="GodotException">If the error isn't "ok".</exception>
    public static Error HandleError(Error error, Error ok = Error.Ok, params Error[] alsoOk)
        => error == ok || alsoOk.Contains(error) ? error : throw new GodotException(error);

    /// <inheritdoc cref="HandleError(Error, Error, Error[])"/>
    /// <param name="message">Context for the error ocurring.</param> 
    public static Error HandleError(Error error, string message, Error ok = Error.Ok, params Error[] alsoOk)
        => error == ok || alsoOk.Contains(error) ? error : throw new GodotException(error, message);

    /// <summary>
    /// Which Godot Error caused this exception.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// A new exception that wraps the provided Godot Error.
    /// </summary>
    /// <param name="error">The Godot Error that caused this.</param>
    /// <param name="cause">An underlying deeper exception.</param>
    public GodotException(Error error, Exception cause = null)
        : base(Enum.GetName(error), cause) { Error = error; }

    /// <inheritDoc cref="GodotException(Error, Exception?)"/>
    /// <param name="message">A context message to use.</param>
    public GodotException(Error error, string message, Exception cause = null)
        : base($"{message} ({Enum.GetName(error)})", cause) { Error = error; }

}