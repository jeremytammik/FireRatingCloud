#region Namespaces
using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion // Namespaces

namespace FireRatingCloud
{
  [Transaction( TransactionMode.ReadOnly )]
  class Cmd_2b_ExportSharedParameterValuesBatch
    : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      return Cmd_2a_ExportSharedParameterValues
        .ExecuteMain( true, commandData, ref message );
    }
  }
}
