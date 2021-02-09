using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using AspenTech.SmartObjects.Client.Models;
using Newtonsoft.Json;

namespace AspenTech.SmartObjects.Client.Impl
{
    internal class EventClient : IEventClient
    {
        private readonly HttpClient client;

        internal EventClient(HttpClient client)
        {
            this.client = client;
        }

        #region Sync calls
        public IEnumerable<EventResult> Send(IEnumerable<Event> events)
        {
            return ClientUtils.WaitTask(SendAsync(events));
        }

        public IEnumerable<EventResult> Post(IEnumerable<Event> events)
        {
            return Send(events);
        }

        public bool EventExists(Guid eventId)
        {
            return ClientUtils.WaitTask<bool>(EventExistsAsync(eventId));
        }

        public IDictionary<string, bool> EventsExist(IList<Guid> eventIds)
        {
            return ClientUtils.WaitTask<IDictionary<string, bool>>(EventsExistAsync(eventIds));
        }
        #endregion

        #region Async Calls
        public async Task<IEnumerable<EventResult>> SendAsync(IEnumerable<Event> events)
        {
            if (events == null || events.Count() == 0)
            {
                throw new ArgumentException("Event list cannot be empty or null.");
            }
            var asyncResult = await client.SendAsyncRequestWithResult(
                    HttpMethod.Post,
                    "/api/v3/events?report_results=true",
                    EventSerializer.SerializeEvents(events));

            return JsonConvert.DeserializeObject<IEnumerable<EventResult>>(asyncResult);        
        }

        public async Task<IEnumerable<EventResult>> PostAsync(IEnumerable<Event> events)
        {
            return await SendAsync(events);
        }

        public async Task<bool> EventExistsAsync(Guid eventId)
        {
            if (eventId == null)
            {
                throw new ArgumentException("eventId cannot be blank.");
            }

            var asynResultAsStr = await client.SendAsyncRequestWithResult(
                HttpMethod.Get,
                string.Format("/api/v3/events/exists/{0}", eventId),
                null);

            var asynResult = JsonConvert.DeserializeObject<IDictionary<string, bool>>(asynResultAsStr);

            return asynResult != null && asynResult.Count == 1 && asynResult.ContainsKey(eventId.ToString()) && asynResult[eventId.ToString()];
        }

        public async Task<IDictionary<string, bool>> EventsExistAsync(IList<Guid> eventIds)
        {
            if (eventIds == null)
            {
                throw new ArgumentException("List of eventIds cannot be null.");
            }

            var asynResultAsStr = await client.SendAsyncRequestWithResult(
                HttpMethod.Post,
                "/api/v3/events/exists",
                JsonConvert.SerializeObject(eventIds));

            return ExistResultsDeserializer.DeserializeExistResults(asynResultAsStr);
        }
        #endregion
    }
}