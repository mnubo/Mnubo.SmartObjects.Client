using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    /// <summary>
    ///  Model is a representation of you mnubo data model.
    /// </summary>
    public sealed class Model
    {
        public List<EventType> EventTypes { get; }
        public List<ObjectType> ObjectTypes { get; }
        public List<Timeseries> Timeseries { get; }
        public List<ObjectAttribute> ObjectAttributes { get; }
        public List<OwnerAttribute> OwnerAttributes { get; }
        public List<Sessionizer> Sessionizers { get; }
        public Orphans Orphans { get; }
        public Model(
            List<EventType> eventTypes,
            List<ObjectType> objectTypes,
            List<Timeseries> timeseries,
            List<ObjectAttribute> objectAttributes,
            List<OwnerAttribute> ownerAttributes,
            List<Sessionizer> sessionizers,
            Orphans orphans
        ) 
        {
            this.EventTypes = eventTypes;
            this.ObjectTypes = objectTypes;
            this.Timeseries = timeseries;
            this.ObjectAttributes = objectAttributes;
            this.OwnerAttributes = ownerAttributes;
            this.Sessionizers = sessionizers;
            this.Orphans = orphans;
        }
    }
}