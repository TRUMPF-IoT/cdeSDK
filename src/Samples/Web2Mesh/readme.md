<!--
SPDX-FileCopyrightText: 2013-2020 TRUMPF Laser GmbH, authors: C-Labs

SPDX-License-Identifier: MPL-2.0
-->

#Web-to-Mesh Samples
This folder  (source\repos\CDE\src\Samples\Web2Mesh) holds sample code to illustrate web-to-mesh connection details. Another folder holds a related set of samples for the C-DEngine Http Interceptor. The Web2Mesh samples rely on the RESTClient sample, which is one of the Http Interceptor samples. In addition, the CDMyWebToMeshSample in this set of samples builds on the Http Interceptor sample named CDMyRestApiSampleMileRecordHolder.

The folders in this set of samples includes the following:

1. CDMyWebToMeshSample - A single plugin that provides either (or both) of two separate services. Support is enabled through the setting of environment variable flags.
2. AppHostTest - application host for CDMyWebToMeshSample plugin.


## CDMyWebToMeshSample

A C-DEngine plugin that demonstrates the use of http interceptor to implement a REST server. In this case, the data that is served up is information for running who have run the fastest mile. Data is taken from Wikipedia. This sample shows how a user can be validated, and the can use the returned access token to make subsequent calls to the server. There is another sample, RESTClient, which provides the client-side part of the connection.

This plugin provides either (or both) of two separate but related services, depending on which flags are set:

1. #### CDMyWebToMeshSample_EnableREST= true -- set this flag to enable the Http Interceptor and support for incoming REST requests.

2. #### CDMyWebToMeshSample_EnableMeshDataQuery = true -- set this flag to enable sending of TSM messages to request the data from another plugin.

3. #### CDMyWebToMeshSample_EnableMeshDataResponse = true -- set this flag to enable receiving TSM messages to respond for requests for data from other plugins.

Note: The accompanying AppHostTest sample provides nice user-interface support for the setting of these (and other) necessary settings.


## AppHostTest
This application host provides the process in which the above-mentioned plugin can run. It enables a user to set the three flags needed for the plugin, and also allows for the setting of key host app settings like the port to be used and the service route.


