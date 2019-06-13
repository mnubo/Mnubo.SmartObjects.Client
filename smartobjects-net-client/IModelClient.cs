using Mnubo.SmartObjects.Client.Models.DataModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client
{
    /// <summary>
    /// IModelClient gives you access to the data model in the target environment.
    ///
    /// The target environment is determined by the configuration you use:
    ///  - sandbox: sandbox url, consumer key and consumer secret
    ///  - production: production url, consumer key and consumer secret
    ///
    ///  https://smartobjects.mnubo.com/documentation/api_modeler.html
    /// </summary>
    public interface IModelClient
    {
        /// <summary>
        /// Export the model of the current zone
        /// </summary>
        /// <returns>Model</returns>
        Model Export();

        /// <summary>
        /// Access to operations only available in the sandbox environment.
        /// </summary>
        /// <returns>SandboxOnlyOps</returns>
        SandboxOnlyOps SandboxOps { get; }

        /// <summary>
        /// All timeseries in the target environment.
        /// <see cref="Mnubo.SmartObjects.Client.Models.DataModel.Timeseries" />
        /// </summary>
        /// <returns>List{Timeseries}</returns>
        List<Timeseries> GetTimeseries();

        /// <see cref="Mnubo.SmartObjects.Client.IModelClient.GetTimeseries" />
        Task<List<Timeseries>> GetTimeseriesAsync();

        /// <summary>
        /// All object attributes in the target environment.
        /// <see cref="Mnubo.SmartObjects.Client.Models.DataModel.ObjectAttribute" />
        /// </summary>
        /// <returns>List{ObjectAttributes}</returns>
        List<ObjectAttribute> GetObjectAttributes();

        /// <see cref="Mnubo.SmartObjects.Client.IModelClient.GetObjectAttributes" />
        Task<List<ObjectAttribute>> GetObjectAttributesAsync();

        /// <summary>
        /// All owner attributes in the target environment.
        /// <see cref="Mnubo.SmartObjects.Client.Models.DataModel.OwnerAttribute" />
        /// </summary>
        /// <returns>List{OwnerAttributes}</returns>
        List<OwnerAttribute> GetOwnerAttributes();

        /// <see cref="Mnubo.SmartObjects.Client.IModelClient.GetOwnerAttributes" />
        Task<List<OwnerAttribute>> GetOwnerAttributesAsync();

        /// <summary>
        /// All object types in the target environment.
        /// <see cref="Mnubo.SmartObjects.Client.Models.DataModel.ObjectType" />
        /// </summary>
        /// <returns>List{ObjectTypes}</returns>
        List<ObjectType> GetObjectTypes();

        /// <see cref="Mnubo.SmartObjects.Client.IModelClient.GetObjectTypes" />
        Task<List<ObjectType>> GetObjectTypesAsync();

        /// <summary>
        /// All event types in the target environment.
        /// <see cref="Mnubo.SmartObjects.Client.Models.DataModel.EventType" />
        /// </summary>
        /// <returns>List{EventTypes}</returns>
        List<EventType> GetEventTypes();

        /// <see cref="Mnubo.SmartObjects.Client.IModelClient.GetEventTypes" />
        Task<List<EventType>> GetEventTypesAsync();

        /// <summary>
        /// Export model of the current zone asynchronously
        /// </summary>
        /// <returns>Model</returns>
        Task<Model> ExportAsync();
    }

    /// <summary>
    /// Updates operations are only available in sandbox. If you call methods on this interface when your
    /// client is configured to hit the production environment, you'll get undefined behaviour:
    ///  - 404 Not Found
    ///  - Bad Request
    ///  - etc.
    /// </summary>
    public interface SandboxOnlyOps {
        /// <summary>
        /// Sandbox timeseries operations
        /// </summary>
        /// <returns>SandboxEntityOps{Timeseries}</returns>
        SandboxEntityOps<Timeseries> TimeseriesOps { get; }
        /// <summary>
        /// Sandbox object attributes operations
        /// </summary>
        /// <returns>SandboxEntityOps{ObjectAttribute}</returns>
        SandboxEntityOps<ObjectAttribute> ObjectAttributesOps { get; }
        /// <summary>
        /// Sandbox owner attributes operations
        /// </summary>
        /// <returns>SandboxEntityOps{OwnerAttribute}</returns>
        SandboxEntityOps<OwnerAttribute> OwnerAttributesOps { get; }

        /// <summary>
        /// Sandbox object types operations
        /// </summary>
        /// <returns>SandboxTypeOps{ObjectType}</returns>
        SandboxTypeOps<ObjectType> ObjectTypesOps { get; }

        /// <summary>
        /// Sandbox event types operations
        /// </summary>
        /// <returns>SandboxTypeOps{EventType}</returns>
        SandboxTypeOps<EventType> EventTypesOps { get; }

        /// <summary>
        /// Reset the sandbox data model
        /// </summary>
        ResetOps ResetOps { get; }

    }

    /// <summary>
    /// Create operations for the following types:
    ///  - Object Attribute
    ///  - Owner Attribute
    ///  - Timeseries
    ///  - Event Types
    ///  - Object Types
    /// </summary>
    public interface CreateOps<A> {
        /// <summary>
        /// Creates multiple instances of A
        /// </summary>
        /// <param name="value">Multiple instance of A</param>
        void Create(List<A> value);

        /// <see cref="Mnubo.SmartObjects.Client.CreateOps{A}.Create" />
        Task CreateAsync(List<A> value);

        /// <summary>
        /// Creates one instance of A
        /// </summary>
        /// <param name="value">One instance of A</param>
        void CreateOne(A value);

        /// <see cref="Mnubo.SmartObjects.Client.CreateOps{A}.CreateOne" />
        Task CreateOneAsync(A value);
    }

    /// <summary>
    /// Update operations for the following types:
    ///  - Object Attribute
    ///  - Owner Attribute
    ///  - Timeseries
    ///  - Event Types
    ///  - Object Types
    /// </summary>
    public interface UpdateOps<A> {
        /// <summary>
        /// Update an instance of A that has the matching key
        /// </summary>
        /// <param name="key">Key of the instance A to update</param>
        /// <param name="update">The update payload</param>
        void Update(string key, A update);

        /// <see cref="Mnubo.SmartObjects.Client.UpdateOps{A}.Update" />
        Task UpdateAsync(string key, A update);
    }

    /// <summary>
    /// Operations to deploy one of the following entity type:
    ///  - Object Attribute
    ///  - Owner Attribute
    ///  - Timeseries
    /// </summary>
    public interface DeployOps {
        /// <summary>
        /// Initiate the deploy process of an instance A that has the matching key
        ///
        /// <see cref="Mnubo.SmartObjects.Client.DeployOps.ApplyDeployCode" />
        /// </summary>
        /// <param name="key">Key of the instance A to initiate deploy</param>
        /// <returns>string</returns>
        string GenerateDeployCode(string key);

        /// <see cref="Mnubo.SmartObjects.Client.DeployOps.GenerateDeployCode" />
        Task<string> GenerateDeployCodeAsync(string key);

        /// <summary>
        /// Completes the deploy process of an instance A with the matching key
        ///
        /// <see cref="Mnubo.SmartObjects.Client.DeployOps.GenerateDeployCode" />
        /// </summary>
        /// <param name="key">Key of the instance A to deploy</param>
        /// <param name="code">Code generated for the instance A to deploy</param>
        void ApplyDeployCode(string key, string code);

        /// <see cref="Mnubo.SmartObjects.Client.DeployOps.ApplyDeployCode" />
        Task ApplyDeployCodeAsync(string key, string code);

        /// <summary>
        /// Runs the complete deploy process of an instance A with the matching key.
        /// @param key of the instance of type A to deploy
        ///
        /// <see cref="Mnubo.SmartObjects.Client.DeployOps.ApplyDeployCode" />
        /// <see cref="Mnubo.SmartObjects.Client.DeployOps.GenerateDeployCode" />
        /// </summary>
        /// <param name="key">Key of the instance A to deploy</param>
        void Deploy(string key);

        /// <see cref="Mnubo.SmartObjects.Client.DeployOps.Deploy" />
        Task DeployAsync(string key);
    }

    /// <summary>
    /// Operations related to the reset process of the sandbox data model
    /// </summary>
    public interface ResetOps {
        /// <summary>
        /// Initiate the reset process of the sandbox data model
        /// <see cref="Mnubo.SmartObjects.Client.ResetOps.ApplyResetCode" />
        /// </summary>
        /// <returns>string</returns>
        string GenerateResetCode();

        /// <see cref="Mnubo.SmartObjects.Client.ResetOps.GenerateResetCode" />
        Task<string> GenerateResetCodeAsync();

        /// <summary>
        /// Completes the reset process of the sandbox data model
        /// <see cref="Mnubo.SmartObjects.Client.ResetOps.GenerateResetCode" />
        /// </summary>
        /// <param name="code">Code generated for RESET</param>
        void ApplyResetCode(string code);

        /// <see cref="Mnubo.SmartObjects.Client.ResetOps.ApplyResetCode" />
        Task ApplyResetCodeAsync(string code);

        /// <summary>
        /// Runs the complete reset process of the sandbox data model
        /// <see cref="Mnubo.SmartObjects.Client.ResetOps.ApplyResetCode" />
        /// <see cref="Mnubo.SmartObjects.Client.ResetOps.GenerateResetCode" />
        /// </summary>
        void Reset();

        /// <see cref="Mnubo.SmartObjects.Client.ResetOps.Reset" />
        Task ResetAsync();
    }

    /// <summary>
    /// Restricted updates operations specialized to one of the following entity type:
    ///  - Object Attribute
    ///  - Owner Attribute
    ///  - Timeseries
    /// </summary>
    public interface SandboxEntityOps<A> : CreateOps<A>, UpdateOps<UpdateEntity>, DeployOps { }

    /// <summary>
    /// Restricted updates operations specialized to one of the following entity type:
    ///  - Object Types
    ///  - Event Types
    /// </summary>
    public interface SandboxTypeOps<A> : CreateOps<A>, UpdateOps<A> {
        /// <summary>
        /// Delete an instance A that has the matching key
        /// @param key of the instance of type A to delete
        /// </summary>
        /// <param name="key">Key of the instance A to delete</param>
        void Delete(string key);

        /// <see cref="Mnubo.SmartObjects.Client.SandboxTypeOps{A}.Delete" />
        Task DeleteAsync(string key);

        /// <summary>
        /// Add a relation to the entity identified by `entityKey`.
        ///
        /// - Object Types => `entityKey` is an object attribute key
        /// - Event Types => `entityKey` is a timeseries key
        ///
        /// </summary>
        /// <param name="key">key identifier of the instance of type A</param>
        /// <param name="entityKey">entityKey identifier of the instance to add a relation to</param>
        void AddRelation(string key, string entityKey);

        /// <see cref="Mnubo.SmartObjects.Client.SandboxTypeOps{A}.AddRelation" />
        Task AddRelationAsync(string key, string entityKey);

        /// <summary>
        /// Remove a relation to the entity identified by `entityKey`.
        ///
        /// - Object Types => `entityKey` is an object attribute key
        /// - Event Types => `entityKey` is a timeseries key
        ///
        /// </summary>
        /// <param name="key">key identifier of the instance of type A</param>
        /// <param name="entityKey">entityKey identifier of the instance to remove a relation to</param>
        void RemoveRelation(string key, string entityKey);

        /// <see cref="Mnubo.SmartObjects.Client.SandboxTypeOps{A}.RemoveRelation" />
        Task RemoveRelationAsync(string key, string entityKey);
    }

    /// <summary>
    ///  An entity to model the update of the entities
    /// </summary>
    public sealed class UpdateEntity
    {

        /// <summary>
        ///  Updated display name
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        ///  Updated description
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///  Constructor to create an timeseries instance
        /// </summary>
        /// <param name="displayName">See <see cref="DisplayName" /></param>
        /// <param name="description">See <see cref="Description" /></param>
        public UpdateEntity(string displayName, string description)
        {
            this.DisplayName = displayName;
            this.Description = description;
        }
    }
}
