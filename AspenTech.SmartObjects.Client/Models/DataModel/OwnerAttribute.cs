using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace AspenTech.SmartObjects.Client.Models.DataModel
{
    /// <summary>
    ///  An owner attribute of the model
    /// </summary>
    public sealed class OwnerAttribute
    {

        /// <summary>
        ///  Unique identifer of the owner attribute
        /// </summary>
        public string Key  { get; }

        /// <summary>
        ///  Display name of the owner attribute
        /// </summary>
        public string DisplayName  { get; }

        /// <summary>
        ///  Description of the owner attribute
        /// </summary>
        public string Description  { get; }


        /// <summary>
        ///  High level type of the owner attribute
        ///  https://smartobjects.mnubo.com/documentation/references.html#high-level-data-types
        /// </summary>
        public string HighLevelType  { get; }

        /// <summary>
        ///  Container type of the owner attribute
        /// </summary>
        public string ContainerType  { get; }


        /// <summary>
        ///  Constructor to create an owner attribute instance
        /// </summary>
        /// <param name="key">See <see cref="Key" /></param>
        /// <param name="displayName">See <see cref="DisplayName" /></param>
        /// <param name="description">See <see cref="Description" /></param>
        /// <param name="type">See <see cref="HighLevelType" /></param>
        /// <param name="containerType">See <see cref="ContainerType" /></param>
        public OwnerAttribute(string key, string displayName, string description, string type, string containerType) 
        {
            this.Key = key;
            this.DisplayName = displayName;
            this.Description = description;
            this.HighLevelType = type;
            this.ContainerType = containerType;
        }
    }
}