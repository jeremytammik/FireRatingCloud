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
[assembly: AssemblyCopyright( "Copyright 2015 © Jeremy Tammik Autodesk Inc." )]
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
//
[assembly: AssemblyVersion( "2016.0.0.10" )]
[assembly: AssemblyFileVersion( "2016.0.0.10" )]
