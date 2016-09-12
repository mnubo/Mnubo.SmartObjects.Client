using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Mnubo.SmartObjects.Client.Models
{
    /// <summary>
    /// Give information about the request sent, this is a particular answer for an event batch processing.
    /// </summary>
    public class EventResult
    {

        /// <summary>
        /// Result of the request.
        /// </summary>
        public enum ResultStates
        {
            /// <summary>
            /// every was well with the request.
            /// </summary>
            success,
            /// <summary>
            /// the requested event_id already exists.
            /// </summary>
            conflict,
            /// <summary>
            /// Something was wrong with the request.
            /// </summary>
            error
        };

        /// <summary>
        /// return the GuID id of the event.
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; }

        /// <summary>
        /// true if the event referring object device ID already exists.
        /// </summary>
        [JsonProperty("objectExists")]
        public bool ObjectExists { get; }

        /// <summary>
        /// return the result of the request, this can be 'success' or 'error'.
        /// </summary>
        /// 
        [JsonProperty("result")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultStates Result { get; }

        /// <summary>
        /// return a message in the request, just if this apply.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; }

        /// <summary>
        /// Allow create a new result set
        /// </summary>
        /// <param name="id">Id of the request.</param>
        /// <param name="result">result of the request, take values of 'success' or 'error'.</param>
        /// <param name="message">message of the request.</param>
        public EventResult(Guid id, bool objectExists, ResultStates result, string message)
        {
            Id = id;
            ObjectExists = objectExists;
            Result = result;
            Message = message;
        }

        /// <summary>
        /// Overriding Equals
        /// </summary>
        public override bool Equals(System.Object obj)
        {
            if(obj ==null)
            {
                return false;
            }

            EventResult other = obj as EventResult;
            if((System.Object)other == null)
            {
                return false;
            }

            return (Id == other.Id) && (ObjectExists == other.ObjectExists) && (Result == other.Result) && (Message == other.Message);
        }

        /// <summary>
        /// Compare to another EventResult
        /// </summary>
        public bool Equals(EventResult other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return (Id == other.Id) && (ObjectExists == other.ObjectExists) && (Result == other.Result) && (Message == other.Message);
        }


        /// <summary>
        /// Overriding GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + ObjectExists.GetHashCode();
            hash = hash * 23 + Result.GetHashCode();
            hash = hash * 23 + Message.GetHashCode();

            return hash;
        }
    }
}