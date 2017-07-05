# Mnubo's SmartObjects platform .NET client

[![Build status](https://travis-ci.org/mnubo/smartobjects-net-client.svg?branch=master)](https://travis-ci.org/mnubo/smartobjects-net-client)
[![NuGet](https://img.shields.io/nuget/v/Mnubo.SmartObjects.Client.svg?maxAge=2592000)](https://www.nuget.org/packages/Mnubo.SmartObjects.Client/)

Introduction
============

This is a .NET implementation of the [API documentation](https://sop.mtl.mnubo.com/apps/doc/?i=t).


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

Development
===========

## Linux

```bash
# you need to install mono & nuget
# to install packages
nuget restore smartobjects-net-client.sln
# to run tests
nuget install NUnit.Runners -Version 3.5.0 -OutputDirectory testrunner
xbuild /p:Configuration=Release smartobjects-net-client.sln
mono ./testrunner/NUnit.ConsoleRunner.*/tools/nunit3-console.exe ./smartobjects-net-client-test/bin/Release/Mnubo.SmartObjects.Client.Test.dll
```


Download source code
---------------------

Download the source code and include it in your .NET Application project.


Usage
=====

To get a client instance use the `ClientFactory`  Class.

```c#
    ClientConfig config = new ClientConfig.Builder() {
        Environment = Environments.Sandbox,
        ConsumerKey = "theConsumerKey",
        ConsumerSecret = "theConsumerSecret",
        // To enable exponential backoff retries
        ExponentialBackoffConfig = new ExponentialBackoffConfig.On(5, 500)
    };


    var client = ClientFactory.Create(config);
```

By default, all requests are compressed using `gzip`, this can be disabled by setting the `CompressionEnabled` of the configuration to `false`.

Working with owners
-------------------

You can use the `client.Owners` synchronous methods:

```c#
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

    //Claim an object while overriding some properties
    client.Owners.Claim("theUserName", "theDeviceId", new Dictionary<string, object>()
        {
            "x_timestamp", "2017-04-26T07:38:36+00:00"
        }
    );

    //Batch claim objects with or without overriding some properties
    client.Owners.BatchClaim(new List<ClaimOrUnclaim> () {
        new ClaimOrUnclaim("theUserName", "theDeviceId", new Dictionary<string, object>() {
                "x_timestamp", "2017-04-26T07:38:36+00:00"
            }
        ),
        new ClaimOrUnclaim("theUserName2", "theDeviceId2")
    });

    //Unclaim an object
    client.Owners.Unclaim("theUserName", "theDeviceId");

    //Unclaim an object while overriding some properties
    client.Owners.Unclaim("theUserName", "theDeviceId", new Dictionary<string, object>()
        {
            "x_timestamp", "2017-04-26T07:38:36+00:00"
        }
    );

    //Batch unclaim objects with or without overriding some properties
    client.Owners.BatchUnclaim(new List<ClaimOrUnclaim> () {
        new ClaimOrUnclaim("theUserName", "theDeviceId", new Dictionary<string, object>() {
                "x_timestamp", "2017-04-26T07:38:36+00:00"
            }
        ),
        new ClaimOrUnclaim("theUserName2", "theDeviceId2")
    });

    //Check if an owner is provisioned
    bool doesExists = client.Owners.OwnerExists("theUsername");

    //Check if a batch of owners are provisioned
    IDictionary<string, bool> existResults = client.Owners.OwnersExist(new List<string>() { "theUsernameA", "theUsernameB" });
```

Or you can use the `client.Owners` asynchronous methods:

```c#
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

    //Claim an object while overriding some properties
    Task task = client.Owners.ClaimAsync("theUserName", "theDeviceId", new Dictionary<string, object>()
        {
            "x_timestamp", "2017-04-26T07:38:36+00:00"
        }
    );

    //Batch claim objects with or without overriding some properties
    Task<Result> task = client.Owners.BatchClaimAsync(new List<ClaimOrUnclaim> () {
        new ClaimOrUnclaim("theUserName", "theDeviceId", new Dictionary<string, object>() {
                "x_timestamp", "2017-04-26T07:38:36+00:00"
            }
        ),
        new ClaimOrUnclaim("theUserName2", "theDeviceId2")
    });

    //Unclaim an object
    Task task = client.Owners.UnclaimAsync("theUserName", "theDeviceId");

    //Unclaim an object while overriding some properties
    Task task = client.Owners.UnclaimAsync("theUserName", "theDeviceId", new Dictionary<string, object>()
        {
            "x_timestamp", "2017-04-26T07:38:36+00:00"
        }
    );

    //Batch unclaim objects with or without overriding some properties
    Task<Result> task = client.Owners.BatchUnclaimAsync(new List<ClaimOrUnclaim> () {
        new ClaimOrUnclaim("theUserName", "theDeviceId", new Dictionary<string, object>() {
                "x_timestamp", "2017-04-26T07:38:36+00:00"
            }
        ),
        new ClaimOrUnclaim("theUserName2", "theDeviceId2")
    });

    //Check if an owner is provisioned
    Task<bool> doesExists = client.Owners.OwnerExistsAsync("theUsername");

    //Check if a batch of owners are provisioned
    Task<IDictionary<string, bool>> existResults = client.Owners.OwnersExistAsync(new List<string>() { "theUsernameA", "theUsernameB" });
```


Working with objects
--------------------

You can use the `client.Objects` synchronous methods:
 
```c#
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
    IDictionary<string, bool> existResults = client.Objects.ObjectsExist(new List<string>() { "theDeviceIdA", "theDeviceIdB" });
```

Or you can use the `client.Objects` asynchronous methods:

```c#
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
    Task<IDictionary<string, bool>> existResults = client.Objects.ObjectsExistAsync(new List<string>() { "theDeviceIdA", "theDeviceIdB" });
```


Working with events
-------------------

You can use the `client.Events` synchronous methods:

```c#
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
    IEnumerable<EventResult> results = client.Events.Send(new List<Event>() { event1, event2 });
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
    IDictionary<string, bool> existResults = client.Events.EventsExist(new List<guid>() { Guid.Parse("0254f4df-30e3-47eb-bb67-48df1c91430a"), Guid.Parse("fcc3b165-45a6-42f9-80dd-dce27e753dea") });
```

Or you can use the `client.Events` asynchronous methods:

```c#
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
    Task<IEnumerable<EventResult>> results = client.Events.Send(new List<Event>() { event1, event2 });
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
    Task<IDictionary<string, bool>> existResults = client.Events.EventsExist(new List<guid>() { Guid.Parse("0254f4df-30e3-47eb-bb67-48df1c91430a"), Guid.Parse("fcc3b165-45a6-42f9-80dd-dce27e753dea") });
```


Working with Restitution (Searches)
-------------------

You can use the `client.Restitution` synchronous methods:

```c#
    //To get all Datasets associated to the Namespace:
    List<DataSet> datasets = client.Restitution.GetDataSets();

    //Perform a search with the search API:
    string query = "{\"from\":\"owner\",\"select\":[{\"value\":\"username\"},{\"value\":\"x_registration_date\"}],\"where\":{\"username\":{\"EQ\":\"USERNAME TO SEARCH\"}}}";
    ResultSet result = client.Restitution.Search(query);
```

Or you can use the `client.Restitution` asynchronous methods:

```c#
    //To get all Datasets associated to the Namespace:
    Task<List<DataSet>> datasets = client.Restitution.GetDataSetsAsync();

    //Perform a search with the search API:
    string query = "{\"from\":\"owner\",\"select\":[{\"value\":\"username\"},{\"value\":\"x_registration_date\"}],\"where\":{\"username\":{\"EQ\":\"USERNAME TO SEARCH\"}}}";
    Task<ResultSet> result = client.Restitution.SearchAsync(query);
```

Please take a look in the documentation of Mnubo to get more detail about how to use the Search API.


Working with the model
-------------------

You can use the `client.Model` synchronous or asynchronous methods:

```c#
    Model model = client.Model.Export();

    //async
    Task<Model> modelAsync = client.Model.ExportAsync();
```

More information available here: https://smartobjects.mnubo.com/apps/doc/api_search.html?highlight=export#get-api-v3-model-export


References
==========

[nuget](https://www.nuget.org/)

[mnubo documentation](https://sop.mtl.mnubo.com/apps/doc/?i=t)

[Microsoft .NET](https://www.microsoft.com/net/default.aspx)
