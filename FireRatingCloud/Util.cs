#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
#endregion // Namespaces

namespace FireRatingCloud
{
  class Util : FireRating.Util
  {
    #region HTTP Access
    /// <summary>
    /// Define a project identifier for the 
    /// given Revit document.
    /// </summary>
    public static string GetProjectIdentifier(
      Document doc )
    {
      SHA256 hasher = SHA256Managed.Create();

      string key = System.Environment.MachineName
        + ":" + doc.PathName;

      byte[] hashValue = hasher.ComputeHash( GetBytes(
        key ) );

      string hashb64 = Convert.ToBase64String(
        hashValue );

      return hashb64.Replace( '/', '_' );
    }
    #endregion // HTTP Access

    #region Shared Parameter Definition
    // Shared parameter definition constants.

    public const string SharedParameterGroupName = "API Parameters";
    public const string SharedParameterName = "API FireRating";
    public const string SharedParameterFilePath = "C:/tmp/SharedParams.txt";

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
        stream = new StreamWriter( SharedParameterFilePath );
        stream.Close();

        app.SharedParametersFilename = SharedParameterFilePath;
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

    public static bool GetSharedParamGuid(
      Application app,
      out Guid paramGuid )
    {
      paramGuid = Util.SharedParamGuid( app,
        Util.SharedParameterGroupName,
        Util.SharedParameterName );

      return !paramGuid.Equals( Guid.Empty );
    }
    #endregion // Shared Parameter Definition

    #region Timestamp
    static DateTime _1970_01_01
      = new DateTime( 1970, 1, 1 );

    /// <summary>
    /// Converts a given DateTime into a Unix 
    /// timestamp, i.e., number of seconds since 
    /// 1970-01-01.
    /// </summary>
    public static int ToUnixTimestamp( DateTime a )
    {
      return (int) Math.Truncate(
        a.ToUniversalTime().Subtract( _1970_01_01 )
            .TotalSeconds );
    }

    /// <summary>
    /// Gets a Unix timestamp representing 
    /// the current moment, i.e., the number 
    /// of seconds since 1970-01-01.
    /// </summary>
    public static int UnixTimestamp()
    {
      return (int) Math.Truncate(
        DateTime.UtcNow.Subtract( _1970_01_01 )
          .TotalSeconds );
    }
    #endregion // Timestamp
  }
}
