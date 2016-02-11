#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Net;
#endregion // Namespaces

namespace FireRatingCloud
{
  /// <summary>
  /// Export all target element ids and their
  /// FireRating parameter values to external database.
  /// </summary>
  [Transaction( TransactionMode.ReadOnly )]
  public class Cmd_2a_ExportSharedParameterValues
    : IExternalCommand
  {
    #region Project
#if NEED_PROJECT_DOCUMENT
    /// <summary>
    /// Retrieve the project identification information 
    /// to store in the external database and return it
    /// as a dictionary in a JSON formatted string.
    /// </summary>
    string GetProjectDataJson(
      //string project_id, 
      Document doc )
    {
      string path = doc.PathName.Replace( '\\', '/' );

      BasicFileInfo file_info = BasicFileInfo.Extract( path );
      DocumentVersion doc_version = file_info.GetDocumentVersion();
      ModelPath model_path = doc.GetWorksharingCentralModelPath();

      string central_server_path = null != model_path
        ? model_path.CentralServerPath
        : string.Empty;

      // Hand-written JSON formatting.

      string s = string.Format(
        //"\"_id\": \"{0}\","
        "\"projectinfo_uid\": \"{0}\","
        + "\"versionguid\": \"{1}\","
        + "\"numberofsaves\": {2},"
        + "\"title\": \"{3}\","
        + "\"centralserverpath\": \"{4}\","
        + "\"path\": \"{5}\","
        + "\"computername\": \"{6}\","
        + "\"jid\": \"{7}\"",
        //project_id,
        doc.ProjectInformation.UniqueId,
        doc_version.VersionGUID,
        doc_version.NumberOfSaves,
        doc.Title,
        central_server_path,
        path,
        System.Environment.MachineName,
        Util.GetProjectIdentifier( doc ) );

      return "{" + s + "}";

    #region Use JavaScriptSerializer
#if USE_JavaScriptSerializer
      // Use JavaScriptSerializer to format JSON data.

      ProjectData project_data = new ProjectData()
      {
        projectinfo_uid = doc.ProjectInformation.UniqueId,
        versionguid = doc_version.VersionGUID.ToString(),
        numberofsaves = doc_version.NumberOfSaves,
        title = doc.Title,
        centralserverpath = central_server_path,
        path = path,
        computername = System.Environment.MachineName
      };

      return new JavaScriptSerializer().Serialize(
        project_data );
#endif // USE_JavaScriptSerializer
    #endregion // Use JavaScriptSerializer
    }
#endif // NEED_PROJECT_DOCUMENT
    #endregion // Project

    #region Obsolete code
    /// <summary>
    /// Retrieve the door instance data to store in 
    /// the external database and return it as a
    /// dictionary-like object. 
    /// Obsolete, replaced by DoorData constructor.
    /// </summary>
    object GetDoorData(
      Element door,
      string project_id,
      Guid paramGuid )
    {
      Document doc = door.Document;

      string levelName = doc.GetElement(
        door.LevelId ).Name;

      string tagValue = door.get_Parameter(
        BuiltInParameter.ALL_MODEL_MARK ).AsString();

      double fireratingValue = door.get_Parameter(
        paramGuid ).AsDouble();

      object data = new
      {
        _id = door.UniqueId,
        project_id = project_id,
        level = levelName,
        tag = tagValue,
        firerating = fireratingValue
      };

      return data;
    }
    #endregion // Obsolete code

    public static Result ExecuteOneByOne(
      FilteredElementCollector doors,
      Guid paramGuid,
      string project_id,
      ref string message )
    {
      #region Project
#if NEED_PROJECT_DOCUMENT
      // Post project data.

      string project_id = string.Empty;
      object obj;
      Hashtable d;

      // curl -i -X POST -H 'Content-Type: application/json' -d '{ 
      //   "projectinfo_uid": "8764c510-57b7-44c3-bddf-266d86c26380-0000c160", 
      //   "versionguid": "f498e8b1-7311-4409-a669-2fd290356bb4", 
      //   "numberofsaves": 271, 
      //   "title": "rac_basic_sample_project.rvt", 
      //   "centralserverpath": "", 
      //   "path": "C:/Program Files/Autodesk/Revit 2016/Samples/rac_basic_sample_project.rvt", 
      //   "computername": "JEREMYTAMMIB1D2" }' 
      //   http://localhost:3001/api/v1/projects

      string json = GetProjectDataJson( doc );

      Debug.Print( json );

      string jsonResponse = string.Empty;

      try
      {
        jsonResponse = Util.QueryOrUpsert(
          "projects/jid/" + jid, string.Empty, "GET" );
      }
      catch( System.Net.WebException ex )
      {
      }

      if( 0 == jsonResponse.Length )
      {
        jsonResponse = Util.QueryOrUpsert(
          "projects", json, "POST" );
      }
      else
      {
        jsonResponse = Util.QueryOrUpsert(
          "projects/" + project_id, json, "PUT" );
      }

      Debug.Print( jsonResponse );

      obj = JsonParser.JsonDecode( jsonResponse );

      if( null != obj )
      {
        d = obj as Hashtable;
        project_id = d["_id"] as string;
      }

      //if( null != project_id )
      //{
      //  jsonResponse = Util.QueryOrUpsert(
      //    "projects/" + project_id, json, "PUT" );

      //  Debug.Assert( 
      //    jsonResponse.Equals( "Accepted" ), 
      //    "expected successful db update response" );

      //  if( !jsonResponse.Equals( "Accepted" ) )
      //  {
      //    project_id = null;
      //  }
      //}
      //else
      //{
      //  jsonResponse = Util.QueryOrUpsert(
      //    "projects", json, "POST" );

      //  Debug.Print( jsonResponse );

      //  obj = JsonParser.JsonDecode( jsonResponse );

      //  if( null != obj )
      //  {
      //    d = obj as Hashtable;
      //    project_id = d["_id"] as string;
      //  }
      //}

      if( !string.IsNullOrEmpty( project_id ) )
      {
#endif // NEED_PROJECT_DOCUMENT
      #endregion // Project

      // Loop through the selected doors and export 
      // their shared parameter value one by one.

      DoorData doorData;
      HttpStatusCode sc;
      string jsonResponse, errorMessage;
      Result rc = Result.Succeeded;

      //collector.Select<Element, string>( 
      //  d => Util.Put( "doors/" + d.UniqueId, 
      //    new DoorData( d, project_id, paramGuid ) ) );

      foreach( Element e in doors )
      {
        //Debug.Print( e.Id.IntegerValue.ToString() );

        doorData = new DoorData( e,
          project_id, paramGuid );

        sc = Util.Put( out jsonResponse, 
          out errorMessage, 
          "doors/" + e.UniqueId, doorData );

        if( 0 == (int) sc )
        {
          message = errorMessage;
          rc = Result.Failed;
          break;
        }

        //Debug.Print( jsonResponse );
      }
      return rc;
    }

    public static Result ExecuteBatch(
      FilteredElementCollector doors,
      Guid paramGuid,
      string project_id,
      ref string message )
    {
      // Loop through the selected doors and export 
      // their shared parameter value in one single 
      // batch call.

      int n = doors.Count<Element>();

      List<FireRating.DoorData> doorData 
        = new List<FireRating.DoorData>( n );
      
      HttpStatusCode sc;
      string jsonResponse, errorMessage;
      Result rc = Result.Succeeded;

      foreach( Element e in doors )
      {
        //Debug.Print( e.Id.IntegerValue.ToString() );

        doorData.Add( new DoorData( e,
          project_id, paramGuid ) );

        //Debug.Print( jsonResponse );
      }

      string query = "doors/project/" + project_id;

      string content = Util.Delete( query );

      sc = Util.PostBatch( out jsonResponse,
        out errorMessage, "doors", doorData );

      if( 0 == (int) sc )
      {
        message = errorMessage;
        rc = Result.Failed;
      }
      return rc;
    }

    public static Result ExecuteMain(
      bool useBatch,
      ExternalCommandData commandData,
      ref string message )
    {
      UIApplication uiapp = commandData.Application;
      Application app = uiapp.Application;
      Document doc = uiapp.ActiveUIDocument.Document;

      // Get shared parameter GUID.

      Guid paramGuid;
      if( !Util.GetSharedParamGuid( app, out paramGuid ) )
      {
        message = "Shared parameter GUID not found.";
        return Result.Failed;
      }

      // Determine custom project identifier.

      string project_id = Util.GetProjectIdentifier( doc );

      // Loop through all elements of the given target
      // category and export the shared parameter value 
      // specified by paramGuid for each.

      //FilteredElementCollector collector
      //  = Util.GetTargetInstances( doc,
      //    Cmd_1_CreateAndBindSharedParameter.Target );

      FilteredElementCollector collector
        = new FilteredElementCollector( doc )
          .OfClass( typeof( FamilyInstance ) )
          .OfCategory( BuiltInCategory.OST_Doors );

      int n = collector.Count<Element>();

      Debug.Print( "Exporting {0} elements {1}.", n,
        (useBatch ? "in batch" : "one by one" ) );

      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();

      Result rc = useBatch
        ? ExecuteBatch( collector, paramGuid, project_id, ref message )
        : ExecuteOneByOne( collector, paramGuid, project_id, ref message );

      stopwatch.Stop();

      Debug.Print(
        "{0} milliseconds to export {1} elements: {2}.",
        stopwatch.ElapsedMilliseconds, n, rc );

      return rc;
    }

    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      return ExecuteMain( false, 
        commandData, ref message );
    }
  }
}
