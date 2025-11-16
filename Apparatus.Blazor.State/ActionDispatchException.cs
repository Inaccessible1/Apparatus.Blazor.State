using System.Diagnostics.CodeAnalysis;

namespace Apparatus.Blazor.State
{
    /// <summary>
    /// Exception thrown when an action dispatch fails.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ActionDispatchException : Exception
    {
        public ActionDispatchException(string message) : base(message)
        {
        }

        public ActionDispatchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
