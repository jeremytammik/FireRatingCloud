#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace FireRatingCloud
{
  #region Lab4_3_1_CreateAndBindSharedParam
  /// <summary>
  /// Create and bind shared parameter.
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class Lab4_3_1_CreateAndBindSharedParam : IExternalCommand
  {
    // What element type are we interested in? the standard SDK FireRating
    // sample uses BuiltInCategory.OST_Doors. we also test using
    // BuiltInCategory.OST_Walls to demonstrate that the same technique
    // works with system families just as well as with standard ones.
    //
    // To test attaching shared parameters to inserted DWG files,
    // which generate their own category on the fly, we also identify
    // the category by category name.
    //
    // The last test is for attaching shared parameters to model groups.

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
        // The category we are defining the parameter for

        try
        {
          cat = doc.Settings.Categories.get_Item( Target );
        }
        catch( Exception ex )
        {
          message = "Error obtaining the shared param document category: "
            + ex.Message;
          return Result.Failed;
        }
        if( null == cat )
        {
          message = "Unable to obtain the shared param document category.";
          return Result.Failed;
        }
      }

      // Get the current shared params definition file

      DefinitionFile sharedParamsFile = LabUtils.GetSharedParamsFile( app );
      if( null == sharedParamsFile )
      {
        message = "Error getting the shared params file.";
        return Result.Failed;
      }

      // Get or create the shared params group

      DefinitionGroup sharedParamsGroup = LabUtils.GetOrCreateSharedParamsGroup(
        sharedParamsFile, LabConstants.SharedParamsGroupAPI );
      if( null == sharedParamsGroup )
      {
        message = "Error getting the shared params group.";
        return Result.Failed;
      }

      // Visibility of the new parameter: the 
      // Category.AllowsBoundParameters property 
      // determines whether a category is allowed to
      // have user-visible shared or project parameters. 
      // If it is false, it may not be bound to visible
      // shared parameters using the BindingMap. Note 
      // that non-user-visible parameters can still be 
      // bound to these categories.

      bool visible = cat.AllowsBoundParameters;

      // Get or create the shared params definition

      Definition fireRatingParamDef = LabUtils.GetOrCreateSharedParamsDefinition(
        sharedParamsGroup, ParameterType.Number, LabConstants.SharedParamsDefFireRating, visible );
      if( null == fireRatingParamDef )
      {
        message = "Error in creating shared parameter.";
        return Result.Failed;
      }

      // Create the category set for binding and add the category
      // we are interested in, doors or walls or whatever:

      CategorySet catSet = app.Create.NewCategorySet();
      try
      {
        catSet.Insert( cat );
      }
      catch( Exception )
      {
        message = string.Format(
          "Error adding '{0}' category to parameters binding set.",
          cat.Name );
        return Result.Failed;
      }

      // Bind the parameter.

      try
      {
        using( Transaction t = new Transaction( doc ) )
        {
          t.Start( "Bind Fire Rating Shared Parameter" );

          Binding binding = app.Create.NewInstanceBinding( catSet );

          // We could check if already bound, but looks 
          // like Insert will just ignore it in that case.

          doc.ParameterBindings.Insert( fireRatingParamDef, binding );

          // You can also specify the parameter group here:

          //doc.ParameterBindings.Insert( fireRatingParamDef, binding, BuiltInParameterGroup.PG_GEOMETRY );

          t.Commit();
        }
      }
      catch( Exception ex )
      {
        message = ex.Message;
        return Result.Failed;
      }
      return Result.Succeeded;
    }
  }
  #endregion // Lab4_3_1_CreateAndBindSharedParam

  #region Lab4_3_2_ExportSharedParamToExcel
  /// <summary>
  /// Export all target element ids and their FireRating param values to Excel.
  /// </summary>
  [Transaction( TransactionMode.ReadOnly )]
  public class Lab4_3_2_ExportSharedParamToExcel : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      UIApplication uiapp = commandData.Application;
      Application app = uiapp.Application;
      Document doc = uiapp.ActiveUIDocument.Document;

      Category cat = doc.Settings.Categories.get_Item(
        Lab4_3_1_CreateAndBindSharedParam.Target );

      // Launch Excel (same as in Lab 4_2, so we really 
      // should have better created some utils...)

      X.Application excel = new X.ApplicationClass();
      if( null == excel )
      {
        LabUtils.ErrorMsg( "Failed to get or start Excel." );
        return Result.Failed;
      }
      excel.Visible = true;
      X.Workbook workbook = excel.Workbooks.Add( Missing.Value );
      X.Worksheet worksheet;
      //while( 1 < workbook.Sheets.Count )
      //{
      //  worksheet = workbook.Sheets.get_Item( 0 ) as X.Worksheet;
      //  worksheet.Delete();
      //}
      worksheet = excel.ActiveSheet as X.Worksheet;
      worksheet.Name = "Revit " + cat.Name;
      worksheet.Cells[1, 1] = "ID";
      worksheet.Cells[1, 2] = "Level";
      worksheet.Cells[1, 3] = "Tag";
      worksheet.Cells[1, 4] = LabConstants.SharedParamsDefFireRating;
      worksheet.get_Range( "A1", "Z1" ).Font.Bold = true;

      List<Element> elems = LabUtils.GetTargetInstances( doc,
        Lab4_3_1_CreateAndBindSharedParam.Target );

      // Get Shared param Guid

      Guid paramGuid = LabUtils.SharedParamGUID( app,
        LabConstants.SharedParamsGroupAPI, LabConstants.SharedParamsDefFireRating );

      if( paramGuid.Equals( Guid.Empty ) )
      {
        LabUtils.ErrorMsg( "No Shared param found in the file - aborting..." );
        return Result.Failed;
      }

      // Loop through all elements and export each to an Excel row

      int row = 2;
      foreach( Element e in elems )
      {
        worksheet.Cells[row, 1] = e.Id.IntegerValue; // ID

        //worksheet.Cells[row, 2] = e.Level.Name; // Level // 2013
        worksheet.Cells[row, 2] = doc.GetElement( e.LevelId ).Name; // Level // 2014

        // Tag:

        Parameter tagParameter = e.get_Parameter( BuiltInParameter.ALL_MODEL_MARK );
        if( null != tagParameter )
        {
          worksheet.Cells[row, 3] = tagParameter.AsString();
        }

        // FireRating:

        Parameter parameter = e.get_Parameter( paramGuid );
        if( null != parameter )
        {
          worksheet.Cells[row, 4] = parameter.AsDouble();
        }
        ++row;
      }
      return Result.Succeeded;
    }
  }
  #endregion // Lab4_3_2_ExportSharedParamToExcel
}
