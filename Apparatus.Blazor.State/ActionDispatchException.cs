namespace Apparatus.Blazor.State
{
    /// <summary>
    /// Exception thrown when an action dispatch fails.
    /// </summary>
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
