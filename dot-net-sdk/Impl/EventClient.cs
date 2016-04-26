using System;
using System.Collections.Generic;
using Mnubo.SmartObjects.Client.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class EventClient : IEventClient
    {
        private readonly HttpClient client;

        internal EventClient(HttpClient client)
        {
            this.client = client;
        }

        #region Sync calls
        public void Post(IEnumerable<Event> events)
        {
            ClientUtils.WaitTask(PostAsync(events));
        }
        #endregion

        #region Async Calls
        public async Task PostAsync(IEnumerable<Event> events)
        {
            if (events == null || events.Count() == 0)
            {
                throw new ArgumentException("Event list cannot be empty or null.");
            }
            await client.sendAsyncRequest(
                    HttpMethod.Post,
                    string.Format("events"),
                    EventSerializer.SerializeEvents(events));
        }
        #endregion
    }
}