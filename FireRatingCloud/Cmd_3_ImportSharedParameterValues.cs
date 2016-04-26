#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion // Namespaces

namespace FireRatingCloud
{
  /// <summary>
  /// Import updated FireRating parameter values 
  /// from external database.
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class Cmd_3_ImportSharedParameterValues
    : IExternalCommand
  {
    /// <summary>
    /// Test retrieving only recently modified records.
    /// </summary>
    static uint _test_timestamp = 0;

    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      UIApplication uiapp = commandData.Application;
      Application app = uiapp.Application;
      Document doc = uiapp.ActiveUIDocument.Document;

      // Determine custom project identifier.

      string project_id = Util.GetProjectIdentifier( doc );

      // Retrieve all doors referencing this project, 
      // optionally modified after the given timestamp.

      List<FireRating.DoorData> doors
        = DbAccessor.GetDoorRecords(
          project_id, _test_timestamp );

      Result rc = Result.Succeeded;

      if ( null != doors && 0 < doors.Count )
      {
        rc = BimUpdater.UpdateBim( doc, doors, ref message )
          ? Result.Succeeded
          : Result.Failed;
      }
      return rc;
    }
  }
}
