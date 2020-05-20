# Chat Samples

The samples in this folder show how to send telegrams from host to host (or node to node).

## CDMyChatSample

A sample plugin showing a Chat dialog in the NMI. 

## HackChatXAML

A modern Windows UWP App. It shows how to include a plugin in a host application directly.
When it connects to the Cloud you can go to our sample Cloud: https://cloud.C-Labs.com/nmi and login with the ScopeID you have specified in the UWP app.

You can find the plugin "loading" code in line 76 and 77 of the mainpage.xaml.cs

## ChatClientWPF

Similar to the UWP app, this host is using WPF instead of UWP.

## Other hosts

If you want to test a .NET Core host, load the sample "MyNetCoreHost" and add the CDMyChatSample plugin to this project.
The C-DEngine will find any DLL in the bin Folder starting with "CDMy", therefore you do not need to instantiate the Plugin in your host as you have to do with UWP or Phone apps that might not allow dynamic loading of DLLs

