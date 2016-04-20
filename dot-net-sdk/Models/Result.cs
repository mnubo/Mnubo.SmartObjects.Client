namespace Mnubo.SmartObjects.Client.Models
{
    public class Result
    {
        public enum ResultStates
        {
            Success,
            Error
        };

        /// <summary>
        /// return the id of the result request.
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
        public Result(string id, ResultStates result, string message)
        {
            this.ResourceId = id;
            this.ResultState = result;
            this.Message = message;
        }
    }
}