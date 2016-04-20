using System;
using System.Collections.Generic;
using Mnubo.SmartObjects.Client.Models;
using System.Threading.Tasks;
using System.Net.Http;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class EventClient : IEventClient
    {
        private HttpClient client;

        internal EventClient(HttpClient client)
        {
            this.client = client;
        }

        #region Sync calls
        public void Post(List<Event> events)
        {
            SyncTaskExecutor(PostAsync(events));
        }
        #endregion

        #region Async Calls
        public Task PostAsync(List<Event> events)
        {
            return Task.Factory.StartNew(() =>
            {
                if(events == null || events.Count == 0)
                {
                    throw new ArgumentException("Event list cannot be empty or null.");
                }
                try
                {
                    client.sendAsyncRequest(HttpMethod.Post,
                        string.Format("events"), EventSerializer.SerializeEvents(events)).Wait();
                }
                catch (AggregateException aggreEx)
                {
                    throw aggreEx.InnerException;
                }
            });
        }
        #endregion

        private void SyncTaskExecutor(Task task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException aggreEx)
            {
                throw aggreEx.InnerException;
            }
        }
    }
}