using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle( "FireRatingCloud" )]
[assembly: AssemblyDescription( "Revit add-in multi-project re-implementation of the FireRating SDK sample using a cloud-based mongo DB" )]
[assembly: AssemblyConfiguration( "" )]
[assembly: AssemblyCompany( "Autodesk Inc." )]
[assembly: AssemblyProduct( "FireRatingCloud Revit Add-In" )]
[assembly: AssemblyCopyright( "Copyright 2015-2016 © Jeremy Tammik Autodesk Inc." )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid( "321044f7-b0b2-4b1c-af18-e71a19252be0" )]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
//
// 2015-07-06 2016.0.0.0 extracted base code from FireRating SDK sample and AdnRevitApiLabsXtra
// 2015-07-07 2016.0.0.1 implemented HTTP request to store project data
// 2015-07-08 2016.0.0.2 retrieve project db id and POST door instance data
// 2015-07-08 2016.0.0.3 implement UpsertDocument and use PUT instead of POST for doors
// 2015-07-08 2016.0.0.4 implement QueryOrUpsert and use GET to check for pre-existing project
// 2015-07-08 2016.0.0.5 eliminated project documents completely and use hash id to identify owner project in door document instead
// 2015-07-09 2016.0.0.6 cleanup and publication
// 2015-07-09 2016.0.0.7 switched from local database server to Heroku-hosted cloud-based
// 2015-09-10 2016.0.0.8 implemented Util.Put using RestSharp instead of HttpWebRequest
// 2015-09-13 2016.0.0.9 implemented Util.Get using RestSharp instead of HttpWebRequest
// 2015-09-14 2016.0.0.10 removed obsolete HttpWebRequest QueryOrUpsert method and the .NET references it requires
// 2015-09-14 2016.0.0.11 implemented DoorData class and added call to JsonDeserializer.Deserialize
// 2015-09-14 2016.0.0.11 updated Get to return a list of deserialised DoorData instances
// 2015-09-14 2016.0.0.12 commented out JsonParser code and pass DoorData instance to Put method
// 2015-09-15 2016.0.0.13 cleaned up for publication
// 2015-11-18 2016.0.0.14 readme enhancements and installation instructions for autodesk university
// 2016-01-26 2016.0.0.15 beginning to set up for madrid bim programming http://www.bimprogramming.com
// 2016-01-26 2016.0.0.16 split Commands.cs into three separate modules
// 2016-01-27 2016.0.0.17 added error message in case of node.js web server not running
// 2016-01-27 2016.0.0.18 implemented import of tag aka mark as well as fire rating
// 2016-01-28 2016.0.0.19 split all Revit independent utils into separate module
// 2016-01-28 2016.0.0.20 integrated FireRatingClient
// 2016-02-07 2016.0.0.21 implemented batch upload using C# Delete + PostBatch calling fireratingdb deleteAllForProject + insertBatch
// 2016-02-07 2016.0.0.22 batch upload worked fine locally but not on web server so switched back to one by one upload
// 2016-02-11 2016.0.0.23 batch upload works fine on mongolab too, implemented separate upload commands for batch versus one by one 
// 2016-04-19 2016.0.0.24 implemented unix timestamp and DoorData modified field
// 2016-04-20 2016.0.0.25 implemented test code to query doors modified after specific timestamp only
// 2016-04-25 2017.0.0.0 flat migration to Revit 2017
// 2016-04-25 2017.0.0.1 implemented real-time bim update: external app, external event, ribbon ui, subscribe command and toggle button
// 2016-04-26 2017.0.0.2 converted timestamp to unsigned
// 2016-04-26 2017.0.0.3 moved GetDoorRecords from Cmd_3_ImportSharedParameterValues to DbUpdater
// 2016-04-26 2017.0.0.3 split DbUpdater into separate DbAccessor and BimUpdater classes
// 2016-04-26 2017.0.0.4 moved and renamed UpdateBimFromDb to BimUpdater.UpdateBim
// 2016-04-26 2017.0.0.5 cleanup: all except DbAccessor and BimUpdater is now clear
// 2016-04-26 2017.0.0.6 rewrote UpdateBim to take list of modified doors from DbAccessor or import command
//
[assembly: AssemblyVersion( "2017.0.0.6" )]
[assembly: AssemblyFileVersion("2017.0.0.6")]
