using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    public sealed class Orphans
    {
        /// <summary>
        /// List of all timeseries that are not bound to an event type
        /// </summary>
        public List<Timeseries> Timeseries { get; }
        

        /// <summary>
        /// List of all object attributes that are not bound to an object type
        /// </summary>
        public List<ObjectAttribute> ObjectAttributes { get; }
        

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="timeseries">See <see cref="Timeseries" /></param>
        /// <param name="objectAttributes">See <see cref="ObjectAttributes" /></param>
        public Orphans(
            List<Timeseries> timeseries,
            List<ObjectAttribute> objectAttributes
        ) 
        {
            this.Timeseries = timeseries;
            this.ObjectAttributes = objectAttributes;
        }
    }
}