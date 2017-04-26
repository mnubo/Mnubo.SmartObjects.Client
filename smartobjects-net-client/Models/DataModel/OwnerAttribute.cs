using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    /// <summary>
    /// </summary>
    public sealed class OwnerAttribute
    {
        public string Key  { get; }
        public string DisplayName  { get; }
        public string Description  { get; }
        public string HighLevelType  { get; }
        public string ContainerType  { get; }
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