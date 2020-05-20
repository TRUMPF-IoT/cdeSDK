<!--
SPDX-FileCopyrightText: 2013-2020 TRUMPF Laser GmbH, authors: C-Labs

SPDX-License-Identifier: MPL-2.0
-->

#Http Intercepter Samples
This folder (source\repos\CDE\src\Samples\HttpInterceptor) contains a set of samples provided to demonstrate the use of a C-DEngine Http Interceptor. This feature of C-DEngine enables a plugin to tap into the web-server built into C-DEngine.



## AppHostTest
This is an application host for hosting any of the plugin samples.


## CDMyHelloHttpInterceptor
The "Hello Http Interceptor" plugin sample creates two http interceptors:

- sinkHelloHttpInterceptor - Set up an http interceptor to monitor the URL "/HelloHTTPInterceptor"
- sinkRelay - Sets up an http interceptor to monitor the URL "/relay.html" and returns a simple HTML page.


## RESTServer/CDMyRestApiSampleMileRecordHolder

A C-DEngine plugin that demonstrates the use of http interceptor to implement a REST server. In this case, the data that is served up is information for running who have run the fastest mile. Data is taken from Wikipedia. This sample shows how a user can be validated, and the can use the returned access token to make subsequent calls to the server. There is another sample, RESTClient, which provides the client-side part of the connection.


## RESTClient
The RESTClient sample is a Windows Forms application for calling into the CDMyRestApiSampleMileRecordHolder REST server sample. To run this program:

1. Build the project for the CDMyRestApiSampleMileRecordHolder plugin.
2. Launching the AppHostTest program, which should load the sample REST server plugin.
3. Start the REST Client sample.
4. Enter the URL for the AppHostTest.
5. Click "Login", which sends a login request and - if successful - gets an access token.
6. Click "GET" to send a query that includes the parameters in the "Input Parameters" text field.



