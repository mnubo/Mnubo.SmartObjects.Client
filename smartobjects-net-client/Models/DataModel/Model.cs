using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    /// <summary>
    ///  Model is a representation of you mnubo data model
    /// </summary>
    public sealed class Model
    {
        /// <summary>
        ///  All event types in the model
        /// </summary>
        public List<EventType> EventTypes { get; }

        /// <summary>
        ///  All object types in the model
        /// </summary>
        public List<ObjectType> ObjectTypes { get; }

        /// <summary>
        ///  All timeseries in the model
        /// </summary>
        public List<Timeseries> Timeseries { get; }

        /// <summary>
        ///  All object attributes in the model
        /// </summary>
        public List<ObjectAttribute> ObjectAttributes { get; }

        /// <summary>
        ///  All owner attributes in the model
        /// </summary>
        public List<OwnerAttribute> OwnerAttributes { get; }

        /// <summary>
        ///  All sessionizer in the model
        /// </summary>
        public List<Sessionizer> Sessionizers { get; }

        /// <summary>
        ///  All orphans in the model
        ///  Object attribute not bound to any object types
        ///  Timeseries not bound to any event types`
        /// </summary>
        public Orphans Orphans { get; }

        /// <summary>
        ///  Constructor to build a Datamodel instance
        /// </summary>
        /// <param name="eventTypes">See <see cref="EventTypes" /></param>
        /// <param name="objectTypes">See <see cref="ObjectTypes" /></param>
        /// <param name="timeseries">See <see cref="Timeseries" /></param>
        /// <param name="objectAttributes">See <see cref="ObjectAttributes" /></param>
        /// <param name="ownerAttributes">See <see cref="OwnerAttributes" /></param>
        /// <param name="sessionizers">See <see cref="Sessionizers" /></param>
        /// <param name="orphans">See <see cref="Orphans" /></param>
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