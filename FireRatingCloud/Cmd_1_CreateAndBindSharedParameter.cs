#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion // Namespaces

namespace FireRatingCloud
{
  /// <summary>
  /// Create and bind shared parameter.
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class Cmd_1_CreateAndBindSharedParameter
    : IExternalCommand
  {
    // What element type are we interested in? The standard 
    // SDK FireRating sample uses BuiltInCategory.OST_Doors. 

    static public BuiltInCategory Target = BuiltInCategory.OST_Doors;

    // Previous tests:
    //
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
    //
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
      // make the shared parameter user visible, if
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
}
