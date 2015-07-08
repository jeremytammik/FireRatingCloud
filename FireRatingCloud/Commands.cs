#region Namespaces
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace FireRatingCloud
{
  #region Project data helper class for JavaScriptSerializer
#if USE_JavaScriptSerializer
  /// <summary>
  /// Data holder to use JavaScriptSerializer.
  /// </summary>
  public class ProjectData
  {
    public string projectinfo_uid { get; set; }
    public string versionguid { get; set; }
    public int numberofsaves { get; set; }
    public string title { get; set; }
    public string centralserverpath { get; set; }
    public string path { get; set; }
    public string computername { get; set; }
  }
#endif // USE_JavaScriptSerializer
  #endregion // Project data helper class for JavaScriptSerializer

  #region Cmd_1_CreateAndBindSharedParameter
  /// <summary>
  /// Create and bind shared parameter.
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class Cmd_1_CreateAndBindSharedParameter 
    : IExternalCommand
  {
    // What element type are we interested in? The standard 
    // SDK FireRating sample uses BuiltInCategory.OST_Doors. 
    // We also test using BuiltInCategory.OST_Walls to 
    // demonstrate that the same technique works with system 
    // families just as well as with standard ones.
    //
    // To test attaching shared parameters to inserted 
    // DWG files, which generate their own category on 
    // the fly, we also identify the category by 
    // category name.
    //
    // The last test is for attaching shared parameters 
    // to model groups.

    static public BuiltInCategory Target = BuiltInCategory.OST_Doors;

    //static public BuiltInCategory Target = BuiltInCategory.OST_Walls;
    //static public string Target = "Drawing1.dwg";
    //static public BuiltInCategory Target = BuiltInCategory.OST_IOSModelGroups; // doc.Settings.Categories.get_Item returns null
    //static public string Target = "Model Groups"; // doc.Settings.Categories.get_Item throws an exception SystemInvalidOperationException "Operation is not valid due to the current state of the object."
    //static public BuiltInCategory Target = BuiltInCategory.OST_Lines; // model lines
    //static public BuiltInCategory Target = BuiltInCategory.OST_SWallRectOpening; // Rectangular Straight Wall Openings, case 1260656 [Add Parameters Wall Opening]

    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      UIApplication uiapp = commandData.Application;
      Application app = uiapp.Application;
      Document doc = uiapp.ActiveUIDocument.Document;
      Category cat = null;

      // The category to define the parameter for.

      #region Determine model group category
#if DETERMINE_MODEL_GROUP_CATEGORY
      List<Element> modelGroups = new List<Element>();
      //Filter fType = app.Create.Filter.NewTypeFilter( typeof( Group ) ); // "Binding the parameter to the category Model Groups is not allowed"
      Filter fType = app.Create.Filter.NewTypeFilter( typeof( GroupType ) ); // same result "Binding the parameter to the category Model Groups is not allowed"
      Filter fCategory = app.Create.Filter.NewCategoryFilter( BuiltInCategory.OST_IOSModelGroups );
      Filter f = app.Create.Filter.NewLogicAndFilter( fType, fCategory );
      if ( 0 < doc.get_Elements( f, modelGroups ) )
      {
        cat = modelGroups[0].Category;
      }
#endif // DETERMINE_MODEL_GROUP_CATEGORY
      #endregion // Determine model group category

      if( null == cat )
      {
        cat = doc.Settings.Categories.get_Item( Target );
      }

      // Retrieve shared parameter definition file.

      DefinitionFile sharedParamsFile = Util
        .GetSharedParamsFile( app );

      if( null == sharedParamsFile )
      {
        message = "Error getting the shared params file.";
        return Result.Failed;
      }

      // Get or create the shared parameter group.

      DefinitionGroup sharedParamsGroup
        = sharedParamsFile.Groups.get_Item(
          Util.SharedParameterGroupName );

      if( null == sharedParamsGroup )
      {
        sharedParamsGroup = sharedParamsFile.Groups
          .Create( Util.SharedParameterGroupName );
      }

      // Visibility of the new parameter: the
      // Category.AllowsBoundParameters property
      // determines whether a category is allowed to
      // have user-visible shared or project parameters.
      // If it is false, it may not be bound to visible
      // shared parameters using the BindingMap. Note
      // that non-user-visible parameters can still be
      // bound to these categories. In our case, we
      // make the shared parameter user visibly, if
      // the category allows it.

      bool visible = cat.AllowsBoundParameters;

      // Get or create the shared parameter definition.

      Definition def = sharedParamsGroup.Definitions
        .get_Item( Util.SharedParameterName );

      if( null == def )
      {
        ExternalDefinitionCreationOptions opt
          = new ExternalDefinitionCreationOptions(
            Util.SharedParameterName,
            ParameterType.Number );

        opt.Visible = visible;

        def = sharedParamsGroup.Definitions.Create(
          opt );
      }

      if( null == def )
      {
        message = "Error creating shared parameter.";
        return Result.Failed;
      }

      // Create the category set for binding and
      // add the category of interest to it.

      CategorySet catSet = app.Create.NewCategorySet();

      catSet.Insert( cat );

      // Bind the parameter.

      using( Transaction t = new Transaction( doc ) )
      {
        t.Start( "Bind FireRating Shared Parameter" );

        Binding binding = app.Create.NewInstanceBinding( 
          catSet );

        // We could check if it is already bound; if so,
        // Insert will apparently just ignore it.

        doc.ParameterBindings.Insert( def, binding );

        // You can also specify the parameter group here:

        //doc.ParameterBindings.Insert( def, binding, 
        //  BuiltInParameterGroup.PG_GEOMETRY );

        t.Commit();
      }
      return Result.Succeeded;
    }
  }
  #endregion // Cmd_1_CreateAndBindSharedParameter

  #region Cmd_2_ExportSharedParameterValues
  /// <summary>
  /// Export all target element ids and their
  /// FireRating parameter values to external database.
  /// </summary>
  [Transaction( TransactionMode.ReadOnly )]
  public class Cmd_2_ExportSharedParameterValues 
    : IExternalCommand
  {
    /// <summary>
    /// Retrieve the project identification information 
    /// to store in the external database and return it
    /// as a dictionary in a JSON formatted string.
    /// </summary>
    string GetProjectDataJson( Document doc )
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
        "\"projectinfo_uid\": \"{0}\","
        + "\"versionguid\": \"{1}\","
        + "\"numberofsaves\": {2},"
        + "\"title\": \"{3}\","
        + "\"centralserverpath\": \"{4}\","
        + "\"path\": \"{5}\","
        + "\"computername\": \"{6}\"",
        doc.ProjectInformation.UniqueId,
        doc_version.VersionGUID,
        doc_version.NumberOfSaves,
        doc.Title,
        central_server_path,
        path,
        System.Environment.MachineName );

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

    /// <summary>
    /// Retrieve the door instance data to store in 
    /// the external database and return it as a
    /// dictionary in a JSON formatted string.
    /// </summary>
    string GetDoorDataJson(
      Element door,
      string project_id,
      Guid paramGuid )
    {
      Document doc = door.Document;

      string s = string.Format(
        "\"_id\": \"{0}\","
        + "\"project_id\": \"{1}\","
        + "\"level\": \"{2}\","
        + "\"tag\": \"{3}\","
        + "\"firerating\": {4}",
        door.UniqueId,
        project_id,
        doc.GetElement( door.LevelId ).Name,
        door.get_Parameter( BuiltInParameter.ALL_MODEL_MARK ).AsString(),
        door.get_Parameter( paramGuid ).AsDouble() );

      return "{" + s + "}";
    }

    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      UIApplication uiapp = commandData.Application;
      Application app = uiapp.Application;
      Document doc = uiapp.ActiveUIDocument.Document;

      // Get shared parameter GUID.

      Guid paramGuid = Util.SharedParamGuid( app,
        Util.SharedParameterGroupName,
        Util.SharedParameterName );

      if( paramGuid.Equals( Guid.Empty ) )
      {
        message = "Shared parameter GUID not found.";
        return Result.Failed;
      }

      // Post project data.

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

      // Determine project database id.

      string project_id = Util.GetProjectDbId( doc );

      string jsonResponse;

      if( null != project_id )
      {
        jsonResponse = Util.QueryOrUpsert(
          "projects/" + project_id, json, "PUT" );

        Debug.Assert( 
          jsonResponse.Equals( "Accepted" ), 
          "expected successful db update response" );

        if( !jsonResponse.Equals( "Accepted" ) )
        {
          project_id = null;
        }
      }
      else
      {
        jsonResponse = Util.QueryOrUpsert(
          "projects", json, "POST" );

        Debug.Print( jsonResponse );

        obj = JsonParser.JsonDecode( jsonResponse );

        if( null != obj )
        {
          d = obj as Hashtable;
          project_id = d["_id"] as string;
        }
      }

      if( null != project_id )
      {
        // Loop through all elements of the given target
        // category and export the shared parameter value 
        // specified by paramGuid for each.

        FilteredElementCollector collector
          = Util.GetTargetInstances( doc,
            Cmd_1_CreateAndBindSharedParameter.Target );

        int n = collector.Count<Element>();

        //string[] records = new string[n];

        foreach( Element e in collector )
        {

          //records[i++] = string.Format( "[{0},{1},{2},{3}]",
          //  e.UniqueId,
          //  doc.GetElement( e.LevelId ).Name,
          //  e.get_Parameter( BuiltInParameter.ALL_MODEL_MARK ).AsString(),
          //  e.get_Parameter( paramGuid ).AsDouble() );

          json = GetDoorDataJson( e, project_id,
            paramGuid );

          Debug.Print( json );

          jsonResponse = Util.QueryOrUpsert(
            "doors", json, "POST" );

          //jsonResponse = Util.QueryOrUpsert(
          //  "doors/" + e.UniqueId, json, "PUT" );

          Debug.Print( jsonResponse );
        }

        //json = "[" + string.Join( ",", records ) + "]";
        //Debug.Print( json );
        // [[194b64e6-8132-4497-ae66-74904f7a7710-0004b28a,Level 1,1,0]]
      }
      return Result.Succeeded;
    }
  }
  #endregion // Cmd_2_ExportSharedParameterValues

  #region Cmd_3_ImportSharedParameterValues
  /// <summary>
  /// Import updated FireRating parameter values 
  /// from external database.
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class Cmd_3_ImportSharedParameterValues 
    : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      UIApplication uiapp = commandData.Application;
      Application app = uiapp.Application;
      Document doc = uiapp.ActiveUIDocument.Document;

      Guid paramGuid = Util.SharedParamGuid( app,
        Util.SharedParameterGroupName,
        Util.SharedParameterName );

      if( paramGuid.Equals( Guid.Empty ) )
      {
        message = "Shared parameter GUID not found.";
        return Result.Failed;
      }

      // Determine project database id.

      string project_id = Util.GetProjectDbId( doc );

      if( null != project_id )
      {
        // Get all doors referencing this project.

        string query = "doors?project_id=" + project_id;

        string jsonResponse = Util.QueryOrUpsert( query,
          string.Empty, "GET" );

        object obj = JsonParser.JsonDecode( jsonResponse );

        if( null != obj )
        {
          ArrayList doors = obj as ArrayList;

          if( null != doors && 0 < doors.Count )
          {
            using( Transaction t = new Transaction( doc ) )
            {
              t.Start( "Import Fire Rating Values" );

              // Retrieve element unique id and 
              // FireRating parameter values.

              foreach( object door in doors )
              {
                Hashtable d = door as Hashtable;
                string uid = d["_id"] as string;
                Element e = doc.GetElement( uid );

                if( null == e )
                {
                  message = string.Format(
                    "Error retrieving element for unique id {0}.",
                    uid );

                  return Result.Failed;
                }

                Parameter p = e.get_Parameter( paramGuid );

                if( null == p )
                {
                  message = string.Format(
                    "Error retrieving shared parameter on element with unique id {0}.",
                    uid );

                  return Result.Failed;
                }
                object fire_rating = d["firerating"];

                p.Set( (double) fire_rating );
              }
              t.Commit();
            }
          }
        }
      }
      return Result.Succeeded;
    }
  }
  #endregion // Cmd_3_ImportSharedParameterValues
}
