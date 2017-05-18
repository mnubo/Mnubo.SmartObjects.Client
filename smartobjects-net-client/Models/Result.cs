namespace Mnubo.SmartObjects.Client.Models
{
    /// <summary>
    /// One result of from a result set from a call to <see cref="IObjectClient.CreateUpdate"/> call
    /// or <see cref="IOwnerClient.CreateUpdate"/>
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Result of the request
        /// </summary>
        public enum ResultStates
        {
            /// <summary>
            /// The request was a success
            /// </summary>
            Success,
            /// <summary>
            /// Something went wrong with the request
            /// </summary>
            Error
        };

        /// <summary>
        /// The unique identifier of the resource: username for owner and device id for object
        /// </summary>
        public string ResourceId { get; }

        /// <summary>
        /// Status of the result
        /// </summary>
        public ResultStates ResultState { get; }

        /// <summary>
        /// Status as a human readable message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Allow create a new result set
        /// </summary>
        /// <param name="resourceId">See <see cref="ResourceId" /></param>
        /// <param name="result">See <see cref="ResultState" /></param>
        /// <param name="message">See <see cref="Message" /></param>
        public Result(string resourceId, ResultStates result, string message)
        {
            ResourceId = resourceId;
            ResultState = result;
            Message = message;
        }

        /// <summary>
        /// Overriding Equals
        /// </summary>
        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Result other = obj as Result;
            if ((System.Object)other == null)
            {
                return false;
            }

            return (ResourceId == other.ResourceId) && (ResultState == other.ResultState) && (Message == other.Message);
        }

        /// <summary>
        /// Compare to another Result
        /// </summary>
        public bool Equals(Result other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return (ResourceId == other.ResourceId) && (ResultState == other.ResultState) && (Message == other.Message);
        }


        /// <summary>
        /// Overriding GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + ResourceId.GetHashCode();
            hash = hash * 23 + ResultState.GetHashCode();
            hash = hash * 23 + Message.GetHashCode();

            return hash;
        }
    }
}