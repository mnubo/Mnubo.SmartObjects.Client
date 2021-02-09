using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace AspenTech.SmartObjects.Client.Models
{
    /// <summary>
    /// One result of a result set from a <see cref="IEventClient.Send"/> call
    /// </summary>
    public class EventResult
    {

        /// <summary>
        /// Result of the request
        /// </summary>
        public enum ResultStates
        {
            /// <summary>
            /// Everything went well with the request
            /// </summary>
            success,
            /// <summary>
            /// The provided event_id already exists
            /// </summary>
            conflict,
            /// <summary>
            /// Something was wrong with the request
            /// </summary>
            error
        };

        /// <summary>
        /// The GuID id of the event
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; }

        /// <summary>
        /// True if the event referring object device ID already exists
        /// </summary>
        [JsonProperty("objectExists")]
        public bool ObjectExists { get; }

        /// <summary>
        /// Status of this event result
        /// </summary>
        ///
        [JsonProperty("result")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultStates Result { get; }

        /// <summary>
        /// Status as a human readable message
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; }

        /// <summary>
        /// Constructor for a new EventResult
        /// </summary>
        /// <param name="id">See <see cref="Id" /></param>
        /// <param name="objectExists">See <see cref="ObjectExists" /></param>
        /// <param name="result">See <see cref="Result" /></param>
        /// <param name="message">See <see cref="Message" /></param>
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
