#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Text.RegularExpressions;
#endregion

namespace FireRatingCloud
{
  #region Cmd_1_CreateAndBindSharedParameter
  /// <summary>
  /// Create and bind shared parameter.
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class Cmd_1_CreateAndBindSharedParameter : IExternalCommand
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
        t.Start( "Bind Fire Rating Shared Parameter" );

        Binding binding = app.Create.NewInstanceBinding( catSet );

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
  public class Cmd_2_ExportSharedParameterValues : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      UIApplication uiapp = commandData.Application;
      Application app = uiapp.Application;
      Document doc = uiapp.ActiveUIDocument.Document;

      #region OLD_CODE
      //Category cat = doc.Settings.Categories.get_Item(
      //  Cmd_1_CreateAndBindSharedParameter.Target );

      // Launch Excel (same as in Lab 4_2, so we really
      // should have better created some utils...)

      //X.Application excel = new X.ApplicationClass();
      //if( null == excel )
      //{
      //  Util.ErrorMsg( "Failed to get or start Excel." );
      //  return Result.Failed;
      //}
      //excel.Visible = true;
      //X.Workbook workbook = excel.Workbooks.Add( Missing.Value );
      //X.Worksheet worksheet;
      ////while( 1 < workbook.Sheets.Count )
      ////{
      ////  worksheet = workbook.Sheets.get_Item( 0 ) as X.Worksheet;
      ////  worksheet.Delete();
      ////}
      //worksheet = excel.ActiveSheet as X.Worksheet;
      //worksheet.Name = "Revit " + cat.Name;
      //worksheet.Cells[1, 1] = "ID";
      //worksheet.Cells[1, 2] = "Level";
      //worksheet.Cells[1, 3] = "Tag";
      //worksheet.Cells[1, 4] = Util.SharedParamsDefFireRating;
      //worksheet.get_Range( "A1", "Z1" ).Font.Bold = true;
      #endregion // OLD_CODE

      // Get shared parameter GUID.

      Guid paramGuid = Util.SharedParamGuid( app,
        Util.SharedParameterGroupName,
        Util.SharedParameterName );

      if( paramGuid.Equals( Guid.Empty ) )
      {
        message = "Shared parameter GUID not found.";
        return Result.Failed;
      }

      // Loop through all elements of the given category 
      // and export the parameter value for each.

      FilteredElementCollector collector
        = Util.GetTargetInstances( doc,
          Cmd_1_CreateAndBindSharedParameter.Target );

      int n = collector.Count<Element>();

      string[] records = new string[n];

      //int row = 2;

      int i = 0;

      foreach( Element e in collector )
      {
        #region OLD_CODE
        //worksheet.Cells[row, 1] = e.Id.IntegerValue; // ID

        ////worksheet.Cells[row, 2] = e.Level.Name; // Level // 2013
        //worksheet.Cells[row, 2] = doc.GetElement( e.LevelId ).Name; // Level // 2014

        //// Tag:

        //Parameter tagParameter = e.get_Parameter( BuiltInParameter.ALL_MODEL_MARK );
        //if( null != tagParameter )
        //{
        //  worksheet.Cells[row, 3] = tagParameter.AsString();
        //}

        //// FireRating:

        //Parameter parameter = e.get_Parameter( paramGuid );
        //if( null != parameter )
        //{
        //  worksheet.Cells[row, 4] = parameter.AsDouble();
        //}
        //++row;
        #endregion // OLD_CODE

        records[i++] = string.Format( "[{0},{1},{2},{3}]",
          e.UniqueId,
          doc.GetElement( e.LevelId ).Name,
          e.get_Parameter( BuiltInParameter.ALL_MODEL_MARK ).AsString(),
          e.get_Parameter( paramGuid ).AsDouble() );
      }

      string json = "[" + string.Join( ",", records ) + "]";

      Debug.Print( json );

      // [[194b64e6-8132-4497-ae66-74904f7a7710-0004b28a,Level 1,1,0]]

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
  public class Cmd_3_ImportSharedParameterValues : IExternalCommand
  {
    //StringSplitOptions _opt = new StringSplitOptions();

    /// <summary>
    /// Extract a list of strings representing the
    /// elements from the given JSON-formatted list.
    /// Return true on success, false otherwise.
    /// This is totally hard-wired for a single list
    /// of lists, and nothing else!
    /// The first version used Split(',').
    /// That does not work for lists of lists, since
    /// the commas in the internal lists will be used 
    /// as separators jut like in the top-level one!
    /// After that this evolved into the most horrible 
    /// of hacks!
    /// </summary>
    bool GetJsonListRecords(
      string json,
      out string[] elements )
    {
      elements = null;

      json.Trim();

      int n = json.Length;

      if( '[' == json[0] && ']' == json[n - 1] )
      {
        json = json.Substring( 1, n - 2 );
        json.Trim();
        n = json.Length;
        if( '[' == json[0] && ']' == json[n - 1] )
        {
          json = json.Substring( 1, n - 2 );
          json.Trim();
          elements = Regex.Split( json, @"\],\[" );
        }
      }
      return null != elements;
    }

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

      #region OLD_CODE
      // Let user select the Excel file.

      //WinForms.OpenFileDialog dlg = new WinForms.OpenFileDialog();
      //dlg.Title = "Select source Excel file from which to update Revit shared parameters";
      //dlg.Filter = "Excel spreadsheet files (*.xls;*.xlsx)|*.xls;*.xlsx|All files (*)|*";
      //if( WinForms.DialogResult.OK != dlg.ShowDialog() )
      //{
      //  return Result.Cancelled;
      //}

      // Launch/Get Excel via COM Interop:

      //X.Application excel = new X.Application();
      //if( null == excel )
      //{
      //  Util.ErrorMsg( "Failed to get or start Excel." );
      //  return Result.Failed;
      //}
      //excel.Visible = true;
      //X.Workbook workbook = excel.Workbooks.Open( dlg.FileName,
      //  Missing.Value, Missing.Value, Missing.Value,
      //  Missing.Value, Missing.Value, Missing.Value, Missing.Value,
      //  Missing.Value, Missing.Value, Missing.Value, Missing.Value,
      //  Missing.Value, Missing.Value, Missing.Value );
      //X.Worksheet worksheet = workbook.ActiveSheet as X.Worksheet;
      #endregion // OLD_CODE

      string json = "[[194b64e6-8132-4497-ae66-74904f7a7710-0004b28a,Level 1,1,123.45]]";

      string[] records;

      if( !GetJsonListRecords( json, out records ) )
      {
        message = "Error parsing JSON input.";
        return Result.Failed;
      }

      using( Transaction t = new Transaction( doc ) )
      {
        t.Start( "Import Fire Rating Values" );

        // Retrieve element unique id and FireRating parameter values.

        string[] values;

        foreach( string record in records )
        {
          values = record.Split( ',' );

          //if( !GetJsonListElements( record, out values ) )
          //{
          //  message = "Error parsing JSON input.";
          //  return Result.Failed;
          //}

          Element e = doc.GetElement( values[0] );

          if( null == e )
          {
            message = string.Format(
              "Error retrieving element for unique id {0}.",
              values[0] );
            return Result.Failed;
          }

          Parameter p = e.get_Parameter( paramGuid );

          if( null == p )
          {
            message = string.Format(
              "Error retrieving shared parameter on element with unique id {0}.",
              values[0] );
            return Result.Failed;
          }

          p.Set( double.Parse( values[3] ) );
        }

        #region OLD_CODE
        //int id;
        //double fireRatingValue;
        //int row = 2;
        //while( true )
        //{
        //  try
        //  {
        //    // Extract relevant XLS values.

        //    X.Range r = worksheet.Cells[row, 1] as X.Range;
        //    if( null == r.Value2 )
        //    {
        //      break;
        //    }
        //    double d = (double) r.Value2;
        //    id = (int) d;
        //    if( 0 >= id )
        //    {
        //      break;
        //    }
        //    r = worksheet.Cells[row, 4] as X.Range;
        //    fireRatingValue = (double) r.Value2;

        //    // Get document's door element via Id

        //    ElementId elementId = new ElementId( id );
        //    Element door = doc.GetElement( elementId );

        //    // Set the param

        //    if( null != door )
        //    {
        //      //Parameter parameter = door.get_Parameter( Util.SharedParamsDefFireRating );
        //      Parameter parameter = door.get_Parameter( paramGuid );
        //      parameter.Set( fireRatingValue );
        //    }
        //  }
        //  catch( Exception )
        //  {
        //    break;
        //  }
        //  ++row;
        #endregion // OLD_CODE

        t.Commit();
      }
      return Result.Succeeded;
    }
  }
  #endregion // Cmd_3_ImportSharedParameterValues
}
