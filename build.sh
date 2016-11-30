#!/bin/bash

if [ "$TRAVIS_PULL_REQUEST" = "false" ]; then
    xbuild /p:Configuration=Release smartobjects-net-client.sln && \
    mono ./testrunner/NUnit.ConsoleRunner.*/tools/nunit3-console.exe ./smartobjects-net-client-test/bin/Release/Mnubo.SmartObjects.Client.Test.dll && \
    mono ./testrunner/NUnit.ConsoleRunner.*/tools/nunit3-console.exe ./smartobjects-net-client-ittest/bin/Release/Mnubo.SmartObjects.Client.ITTest.dll
else
    xbuild /p:Configuration=Release smartobjects-net-client.sln && \
    mono ./testrunner/NUnit.ConsoleRunner.*/tools/nunit3-console.exe ./smartobjects-net-client-test/bin/Release/Mnubo.SmartObjects.Client.Test.dll
fi
