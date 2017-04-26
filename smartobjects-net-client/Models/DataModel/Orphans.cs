using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    /// <summary>
    /// </summary>
    public sealed class Orphans
    {
        public List<Timeseries> Timeseries { get; }
        public List<ObjectAttribute> ObjectAttributes { get; }
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