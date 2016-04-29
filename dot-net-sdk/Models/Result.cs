namespace Mnubo.SmartObjects.Client.Models
{
    /// <summary>
    /// Give information about the request sent, this is a particular answer for batch processing.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Result of the request.
        /// </summary>
        public enum ResultStates
        {
            /// <summary>
            /// every was well with the request.
            /// </summary>
            Success,
            /// <summary>
            /// Something was wrong with the request.
            /// </summary>
            Error
        };

        /// <summary>
        /// return the id of the resource, device id in object's cases and "username" in owner's cases.
        /// </summary>
        public string ResourceId { get; }

        /// <summary>
        /// return the result of the request, this can be 'success' or 'error'.
        /// </summary>
        public ResultStates ResultState { get; }

        /// <summary>
        /// return a message in the request, just if this apply.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Allow create a new result set
        /// </summary>
        /// <param name="id">Id of the request.</param>
        /// <param name="result">result of the request, take values of 'success' or 'error'.</param>
        /// <param name="message">message of the request.</param>
        public Result(string resourceId, ResultStates result, string message)
        {
            ResourceId = resourceId;
            ResultState = result;
            Message = message;
        }
    }
}