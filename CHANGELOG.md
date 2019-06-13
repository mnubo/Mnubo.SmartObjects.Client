<a name='2.1.0'></a>

# [2.1.0](https://github.com/mnubo/smartobjects-net-client/compare/2.0.2...2.1.0) (2019-06-13)


* Support for new model operations: AddRelation / RemoveRelation 
<a name='2.0.2'></a>

# [2.0.2](https://github.com/mnubo/smartobjects-net-client/compare/2.0.1...2.0.2) (2019-05-10)


* Activated exponential backoff config now retry on 502 and 503
* More options to customize Exponential Backoff configuration
<a name='2.0.1'></a>

# [2.0.1](https://github.com/mnubo/smartobjects-net-client/compare/2.0.0...2.0.1) (2019-01-08)


* Removes new line from the version header (previous version did not work on Windows)
<a name='2.0.0'></a>

# [2.0.0](https://github.com/mnubo/smartobjects-net-client/compare/1.4.1...2.0.0) (2018-03-22)


* the hostname is now configurable to any arbitrary value
<a name='1.4.1'></a>

# [1.4.1](https://github.com/mnubo/smartobjects-net-client/compare/1.3.0...1.4.1) (2018-01-24)


* Support modeler API
<a name='1.3.0'></a>

# [1.3.0](https://github.com/mnubo/smartobjects-net-client/compare/1.2.1...1.3.0) (2017-08-18)


* Support initialization with a static access token
<a name='1.2.1'></a>

# [1.2.1](https://github.com/mnubo/smartobjects-net-client/compare/1.2.0...1.2.1) (2017-07-05)


- Opt in exponential backoff for responses with 503 status code  
<a name='1.2.0'></a>

# [1.2.0](https://github.com/mnubo/smartobjects-net-client/compare/1.1.14...1.2.0) (2017-06-22)


- Better testing
- Deprecated DateTime on RegistrationDate (SmartObject and Owner) and in favor of DateTimeOffset which has support for timezone
<a name='1.1.14'></a>

# [1.1.14](https://github.com/mnubo/smartobjects-net-client/compare/1.1.13...1.1.14) (2017-05-03)


- Added model service to get the data model

The model exposes all of the following:

	- Event types
	- Timeseries
	- Object types
	- Object attributes
	- Owner attributes
	- Sessionizers
- Added batch claim/unclaim
- Support body on claim/unclaim
<a name='1.1.13'></a>

# [1.1.13](https://github.com/mnubo/smartobjects-net-client/compare/v1.1.12...1.1.13) (2016-11-30)


### Features

* **POST /api/v3/owners/{username}/objects/{device}/unclaim**: support for unclaim object
