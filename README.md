# Mnubo's SmartObjects platform .NET client

Introduction
============

This is a .NET implemenation of the [API documentation](https://sop.mtl.mnubo.com/apps/doc/?i=t).

Architecture
============

Use class `ClientFactory` to get a client instance for SmartObjects.  Then, use the appropriate method to instantiate an owner, object, or event API instance for ingestion.

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
```

References
==========

[nuget](https://www.nuget.org/)

[mnubo .NET sdk](http://git-lab1.mtl.mnubo.com/mauro/mnubo-dotnet-sdk/)

[mnubo documentation](https://sop.mtl.mnubo.com/apps/doc/?i=t)

[Microsoft .NET](https://www.microsoft.com/net/default.aspx)
