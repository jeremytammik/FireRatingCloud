using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireRatingCloud
{
  class Util
  {
    public const string SharedParameterGroupName = "API Parameters";
    public const string SharedParameterName = "API FireRating";

    const string _shared_param_file_path = "C:/tmp/SharedParams.txt";

    /// <summary>
    /// Get shared parameters file.
    /// </summary>
    public static DefinitionFile GetSharedParamsFile(
      Application app )
    {
      string sharedParamsFileName = app
        .SharedParametersFilename;

      if( 0 == sharedParamsFileName.Length )
      {

        StreamWriter stream;
        stream = new StreamWriter( _shared_param_file_path );
        stream.Close();

        app.SharedParametersFilename = _shared_param_file_path;
        sharedParamsFileName = app.SharedParametersFilename;
      }

      // Get the current file object and return it
      
      DefinitionFile sharedParametersFile = app
        .OpenSharedParameterFile();

      return sharedParametersFile;
    }

    /// <summary>
    /// Return all element instances for a given 
    /// category, identified either by a built-in 
    /// category or by a category name.
    /// </summary>
    public static FilteredElementCollector GetTargetInstances(
      Document doc,
      object targetCategory )
    {
      FilteredElementCollector collector 
        = new FilteredElementCollector( doc );

      bool isName = targetCategory.GetType().Equals( 
        typeof( string ) );

      if( isName )
      {
        Category cat = doc.Settings.Categories
          .get_Item( targetCategory as string );

        collector.OfCategoryId( cat.Id );
      }
      else
      {
        collector.WhereElementIsNotElementType();

        collector.OfCategory( (BuiltInCategory) targetCategory );

        //var model_elements
        //  = from e in collector
        //    where ( null != e.Category && e.Category.HasMaterialQuantities )
        //    select e;

        //elements = model_elements.ToList<Element>();
      }
      return collector;
    }

    /// <summary>
    /// Return GUID for a given shared parameter group and name.
    /// </summary>
    /// <param name="app">Revit application</param>
    /// <param name="defGroup">Definition group name</param>
    /// <param name="defName">Definition name</param>
    /// <returns>GUID</returns>
    public static Guid SharedParamGuid( 
      Application app, 
      string defGroup, 
      string defName )
    {
      DefinitionFile file = app.OpenSharedParameterFile();
      DefinitionGroup group = file.Groups.get_Item( defGroup );
      Definition definition = group.Definitions.get_Item( defName );
      ExternalDefinition externalDefinition = definition as ExternalDefinition;
      return externalDefinition.GUID;
    }

  }
}
