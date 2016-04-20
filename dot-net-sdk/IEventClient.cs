using System.Collections.Generic;
using Mnubo.SmartObjects.Client.Models;
using System.Threading.Tasks;

namespace Mnubo.SmartObjects.Client
{
    /// <summary>
    /// Event client interface
    /// </summary>
    public interface IEventClient
    {
        /// <summary>
        /// Allows post events to several objects. In this case, the device id is taken
        /// directly from each event. Thus, a post will be sent by each event in the list in sync mode.
        /// </summary>
        /// <param name="events">list the events to send</param>
        void Post(List<Event> events);

        /// <summary>
        /// Allows post events to several objects. In this case, the device id is taken
        /// directly from each event. Thus, a post will be sent by each event in the list in async mode
        /// </summary>
        /// <param name="events">list the events to send</param>
        Task PostAsync(List<Event> events);
    }
}