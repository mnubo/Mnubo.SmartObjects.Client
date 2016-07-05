# Mnubo's SmartObjects platform .NET client

Introduction
============

This is a .NET implemenation of the [API documentation](https://sop.mtl.mnubo.com/apps/doc/?i=t).

Architecture
============

Use class `ClientFactory` to get a client instance for SmartObjects.  Then, use the appropriate method to instantiate an owner, object, or event API instance for ingestion and Restitution for searching.

Use classes `Owner`, `SmartObject`, and `Event` to build the entities. There are two ways to build these entities as seen below.

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

You can use the `client.Owners` methods:

```
    Owner owner = new Owner.Builder() {
        Username = "theUsername",
        Password = "thePassword"
    };

    client.Owners.Create(owner);

    client.Owners.Delete("theUsername");

	client.Owners.IsOwnerExist("theUsername");

	client.Owners.OwnersExist(new List<string>() { "theUsernameA", "theUsernameB" });
```

Working with objects
--------------------

You can use the `client.Objects` methods:

```
    SmartObject smartObject = new SmartObject.Builder() {
        DeviceId = "theDeviceId",
        ObjectType = "theObjectType"
    };

    client.Objects.Create(smartObject);

    client.Owners.Claim("theUserName", "theDeviceId");

	client.Objects.IsObjectExist("theDeviceId");

	client.Objects.ObjectsExist(new List<string>() { "theDeviceIdA", "theDeviceIdB" });
```

Working with events
-------------------

You can use the `client.Events` methods:

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

    client.Events.Post(new List<Event>() { event1, event2 });

	client.Events.IsEventExist(Guid.Parse("83250b2e-28b0-4d9f-82cd-d7bad2230d4b"));

	client.Events.EventsExist(new List<guid>() { Guid.Parse("0254f4df-30e3-47eb-bb67-48df1c91430a"), Guid.Parse("fcc3b165-45a6-42f9-80dd-dce27e753dea") });
```

Working with Restitution (Searchs)
-------------------

You can use the `client.Restitution` methods:

To get all Datasets associated to the Namespace:
```
    client.Restitution.GetDataSets();

```

Or search to use the search API:

```
    string query = "{\"from\":\"owner\",\"select\":[{\"value\":\"username\"},{\"value\":\"x_registration_date\"}],\"where\":{\"username\":{\"EQ\":\"USERNAME TO SEARCH\"}}}";

    ResultSet result = client.Restitution.Search(query);
```

Please take a look in the documentation of Mnubo to get more detail about how to use the Search API.

References
==========

[nuget](https://www.nuget.org/)

[mnubo .NET sdk](http://git-lab1.mtl.mnubo.com/mauro/mnubo-dotnet-sdk/)

[mnubo documentation](https://sop.mtl.mnubo.com/apps/doc/?i=t)

[Microsoft .NET](https://www.microsoft.com/net/default.aspx)
