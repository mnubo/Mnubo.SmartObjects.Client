# Mnubo's SmartObjects platform .NET client

Introduction
============

This is a .NET implemenation of the [API documentation](https://sop.mtl.mnubo.com/apps/doc/?i=t).

Architecture
============

Use class `ClientFactory` to get a client instance for SmartObjects.  Then, use the appropriate method to instantiate an owner, object, or event API instance for ingestion and Restitution for searching.

Use classes `Owner`, `SmartObject`, and `Event` to build the entities.

This SDK supports both synchronous and asynchronous (non-blocking) calls. As seen in examples below, for async calls, we pass the entity instance to be updated by the callback function.

Client token authentication is supported. User token authentication is not supported yet.

Prerequisites
=============

- SDK requires .NET 4.5.1 or higher.
- SDK has been built using nuget.

Installation & Configuration
============================

Include the mnubo client in your .NET application using Nuget (Coming Soon).

Download source code
---------------------

Download the source code and include it in your .NET Application project.

Usage
=====

To get a client instance use the `ClientFactory`  Class. 

```
    ClientConfig config = new ClientConfig.Builder() {
        Environment = Environments.Sandbox,
        ConsumerKey = "theConsumerKey",
        ConsumerSecret = "theConsumerSecret"
    };

    var client = ClientFactory.Create(config);
```

Working with owners
-------------------

You can use the `client.Owners` synchronous methods:

```
    Owner owner1 = new Owner.Builder() {
        Username = "theUsername1",
        Password = "thePassword"
    };

	Owner owner2 = new Owner.Builder() {
        Username = "theUsername2",
        Password = "thePassword"
    };

	//Create an owner
    client.Owners.Create(owner);

	//Create or update(if already created) a batch of objects
	IEnumerable<Result> results = client.Owners.CreateUpdate(new List<Owner>(){ owner1, owner2 });
	for(Results result in results)
	{
		Console.WriteLine(string.Format("ResourceId={0} Result={1} Message={2}",
			result.ResourceId,
			result.Result,
			result.Message
			))
	}

	//Update owner attributes
	 Owner ownerAttributes = new Owner.Builder() {
        Attributes = new Dictionary<string,object>()
		{
			"name", "value"
		}
    };
	client.Owners.Update(ownerAttributes, "username");

	//Delete an owner
    client.Owners.Delete("theUsername");

	//Claim an object
    client.Owners.Claim("theUserName", "theDeviceId");

	//Check if an owner is provisioned
	bool doesExists = client.Owners.OwnerExists("theUsername");

	//Check if a batch of owners are provisioned
	IEnumerable<IDictionary<string, bool>> existResults = client.Owners.OwnersExist(new List<string>() { "theUsernameA", "theUsernameB" });
```

Or you can use the `client.Owners` asynchronous methods:

```
    Owner owner1 = new Owner.Builder() {
        Username = "theUsername1",
        Password = "thePassword"
    };

	Owner owner2 = new Owner.Builder() {
        Username = "theUsername2",
        Password = "thePassword"
    };

	//Create an owner
    Task task = client.Owners.CreateAsync(owner);

	//Create or update(if already created) a batch of objects
	Task<IEnumerable<Result>> results = client.Owners.CreateUpdateAsync(new List<Owner>(){ owner1, owner2 });
	result.Wait();
	for(Results result in results.Result)
	{
		Console.WriteLine(string.Format("ResourceId={0} Result={1} Message={2}",
			result.ResourceId,
			result.Result,
			result.Message
			))
	}

	//Update owner attributes
	 Owner ownerAttributes = new Owner.Builder() {
        Attributes = new Dictionary<string,object>()
		{
			"name", "value"
		}
    };
	Task task = client.Owners.UpdateAsync(ownerAttributes, "username");

	//Delete an owner
    Task task = client.Owners.DeleteAsync("theUsername");

	//Claim an object
    Task task = client.Owners.ClaimAsync("theUserName", "theDeviceId");

	//Check if an owner is provisioned
	Task<bool> doesExists = client.Owners.OwnerExistsAsync("theUsername");

	//Check if a batch of owners are provisioned
	Task<IEnumerable<IDictionary<string, bool>>> existResults = client.Owners.OwnersExistAsync(new List<string>() { "theUsernameA", "theUsernameB" });
```

Working with objects
--------------------

You can use the `client.Objects` synchronous methods:

```
    SmartObject smartObject1 = new SmartObject.Builder() {
        DeviceId = "theDeviceId1",
        ObjectType = "theObjectType"
    };

	SmartObject smartObject2 = new SmartObject.Builder() {
        DeviceId = "theDeviceId2",
        ObjectType = "theObjectType"
    };

	//Create a single object
    client.Objects.Create(smartObject1);

	//Create or update(if already created) a batch of objects
	IEnumerable<Result> results = client.Objects.CreateUpdate(new List<SmartObject>(){ smartObject1, smartObject2 });
	for(Results result in results)
	{
		Console.WriteLine(string.Format("ResourceId={0} Result={1} Message={2}",
			result.ResourceId,
			result.Result,
			result.Message
			))
	}

	//Update object attributes
	 SmartObject objectAttributes = new SmartObject.Builder() {
        Attributes = new Dictionary<string,object>()
		{
			"name", "value"
		}
    };
	client.Objects.Update(ownerAttributes, "deviceId");

	//Check if an object is provisioned
	bool doesExists = client.Objects.ObjectExists("theDeviceId");

	//Check if a batch of object is provisioned
	IEnumerable<IDictionary<string, bool>> existResults = client.Objects.ObjectsExist(new List<string>() { "theDeviceIdA", "theDeviceIdB" });
```

Or you can use the `client.Objects` asynchronous methods:

```
    SmartObject smartObject1 = new SmartObject.Builder() {
        DeviceId = "theDeviceId1",
        ObjectType = "theObjectType"
    };

	SmartObject smartObject2 = new SmartObject.Builder() {
        DeviceId = "theDeviceId2",
        ObjectType = "theObjectType"
    };

	//Create a single object
    Task task = client.Objects.CreateAsync(smartObject1);

	//Create or update(if already created) a batch of objects
	Task<IEnumerable<Result>> results = client.Objects.CreateUpdateAsync(new List<SmartObject>(){ smartObject1, smartObject2 });
	results.Wait();
	for(Results result in results.Result)
	{
		Console.WriteLine(string.Format("ResourceId={0} Result={1} Message={2}",
			result.ResourceId,
			result.Result,
			result.Message
			))
	}

	//Update object attributes
	 SmartObject objectAttributes = new SmartObject.Builder() {
        Attributes = new Dictionary<string,object>()
		{
			"name", "value"
		}
    };
	Task task = client.Objects.UpdateAsync(ownerAttributes, "deviceId");

	//Check if an object is provisioned
	Task<bool> doesExists = client.Objects.ObjectExistsAsync("theDeviceId");

	//Check if a batch of object is provisioned
	Task<IEnumerable<IDictionary<string, bool>>> existResults = client.Objects.ObjectsExistAsync(new List<string>() { "theDeviceIdA", "theDeviceIdB" });
```

Working with events
-------------------

You can use the `client.Events` synchronous methods:

```
    Event event1 = new Event.Builder() {
        EventType = "theEventType",
        DeviceId = "aDeviceId",
        Timeseries = new Dictionary<string, object>() {
            {"pressure", 1026.92}
        }
    };

    Event event2 = new Event.Builder() {
        EventType = "theEventType",
        DeviceId = "anOtherDeviceId",
        Timeseries = new Dictionary<string, object>() {
            {"pressure", 1013.36}
        }
    };

	//Scan results to get individual result of event
    IEnumerable<EventResult> results = client.Events.Post(new List<Event>() { event1, event2 });
	for(EventResult result in results)
	{
		Console.WriteLine(string.Format("id={0} objectExists={1} result={2} message={3}",
			result.Id,
			result.ObjectExists,
			result.Result,
			result.Message
			))
	}

	//Check if an event ID exists
	bool doesExists = client.Events.EventExists(Guid.Parse("83250b2e-28b0-4d9f-82cd-d7bad2230d4b"));

	//Check if a batch of event IDs exists
	IEnumerable<IDictionary<string, bool>> existResults = client.Events.EventsExist(new List<guid>() { Guid.Parse("0254f4df-30e3-47eb-bb67-48df1c91430a"), Guid.Parse("fcc3b165-45a6-42f9-80dd-dce27e753dea") });
```

Or you can use the `client.Events` asynchronous methods:

```
    Event event1 = new Event.Builder() {
        EventType = "theEventType",
        DeviceId = "aDeviceId",
        Timeseries = new Dictionary<string, object>() {
            {"pressure", 1026.92}
        }
    };

    Event event2 = new Event.Builder() {
        EventType = "theEventType",
        DeviceId = "anOtherDeviceId",
        Timeseries = new Dictionary<string, object>() {
            {"pressure", 1013.36}
        }
    };

	//Scan results to get individual result of event
    Task<IEnumerable<EventResult>> results = client.Events.Post(new List<Event>() { event1, event2 });
	results.Wait();
	for(EventResult result in results.Result)
	{
		Console.WriteLine(string.Format("id={0} objectExists={1} result={2} message={3}",
			result.Id,
			result.ObjectExists,
			result.Result,
			result.Message
			))
	}

	//Check if an event ID exists
	Task<bool> doesExists = client.Events.EventExists(Guid.Parse("83250b2e-28b0-4d9f-82cd-d7bad2230d4b"));

	//Check if a batch of event IDs exists
	Task<IEnumerable<IDictionary<string, bool>>> existResults = client.Events.EventsExist(new List<guid>() { Guid.Parse("0254f4df-30e3-47eb-bb67-48df1c91430a"), Guid.Parse("fcc3b165-45a6-42f9-80dd-dce27e753dea") });
```

Working with Restitution (Searchs)
-------------------

You can use the `client.Restitution` synchronous methods:

```
	//To get all Datasets associated to the Namespace:
    List<DataSet> datasets = client.Restitution.GetDataSets();

	//Perform a search with the search API:
    string query = "{\"from\":\"owner\",\"select\":[{\"value\":\"username\"},{\"value\":\"x_registration_date\"}],\"where\":{\"username\":{\"EQ\":\"USERNAME TO SEARCH\"}}}";
    ResultSet result = client.Restitution.Search(query);
```

Or you can use the `client.Restitution` asynchronous methods:

```
	//To get all Datasets associated to the Namespace:
    Task<List<DataSet>> datasets = client.Restitution.GetDataSetsAsync();

	//Perform a search with the search API:
    string query = "{\"from\":\"owner\",\"select\":[{\"value\":\"username\"},{\"value\":\"x_registration_date\"}],\"where\":{\"username\":{\"EQ\":\"USERNAME TO SEARCH\"}}}";
    Task<ResultSet> result = client.Restitution.SearchAsync(query);
```

Please take a look in the documentation of Mnubo to get more detail about how to use the Search API.

References
==========

[nuget](https://www.nuget.org/)

[mnubo documentation](https://sop.mtl.mnubo.com/apps/doc/?i=t)

[Microsoft .NET](https://www.microsoft.com/net/default.aspx)
