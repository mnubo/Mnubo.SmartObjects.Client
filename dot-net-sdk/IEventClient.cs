using System.Collections.Generic;
using Mnubo.SmartObjects.Client.Models;
using System.Threading.Tasks;
using System;

namespace Mnubo.SmartObjects.Client
{
    /// <summary>
    /// Event client interface
    /// </summary>
    public interface IEventClient
    {
        /// <summary>
        /// Allows sending events to several objects in batch mode. In this case, the device id is taken
        /// directly from each event. 
        /// The API returns an individual result for each event in the same order as they have been
        /// sent.  
        /// </summary>
        /// <param name="events">list the events to send</param>
        /// <returns>Result of each element in the batch</returns> 
        IEnumerable<EventResult> Send(IEnumerable<Event> events);

        [Obsolete("Please use `Send(events)` instead. Will be removed in a future version.")]
        IEnumerable<EventResult> Post(IEnumerable<Event> events);

        /// <summary>
        /// Allow validate if an event exists.
        /// </summary>
        /// <param name="eventId">EventId to validate.</param>
        /// <returns>true if the event exists or false if not.</returns>
        bool EventExists(Guid eventId);

        /// <summary>
        /// Allow validate if a list of events exist.
        /// </summary>
        /// <param name="eventIds">list of eventIds to validate. ["eventA", "eventB" ]</param>
        /// <returns>the dictionary of eventIds with an existing boolean, true if it exists or false if not. {"eventA":true},{"eventB":false}</returns>
        IDictionary<string, bool> EventsExist(IList<Guid> eventIds);

        /// <summary>
        /// Allows sending events to several objects in batch mode. In this case, the device id is taken
        /// directly from each event.
        /// The API returns an individual result for each event in the same order as they have been
        /// sent.
        /// </summary>
        /// <param name="events">list the events to send</param>
        /// <returns>Result of each element in the batch</returns> 
        Task<IEnumerable<EventResult>> SendAsync(IEnumerable<Event> events);

        [Obsolete("Please use `SendAsync(events)` instead. Will be removed in a future version.")]
        Task<IEnumerable<EventResult>> PostAsync(IEnumerable<Event> events);

        /// <summary>
        /// Allow validate if an event exists in async mode.
        /// </summary>
        /// <param name="eventId">EventId to validate.</param>
        /// <returns>true if the event exists or false if not.</returns>
        Task<bool> EventExistsAsync(Guid eventId);

        /// <summary>
        /// Allow validate if a list of events exist in async mode.
        /// </summary>
        /// <param name="eventIds">list of eventIds to validate. ["eventA", "eventB" ]</param>
        /// <returns>the dictionary of eventIds with an existing boolean, true if it exists or false if not. {"eventA":true},{"eventB":false}</returns>
        Task<IDictionary<string, bool>> EventsExistAsync(IList<Guid> eventIds);
    }
}