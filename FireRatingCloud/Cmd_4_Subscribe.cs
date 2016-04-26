#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace FireRatingCloud
{
  [Transaction( TransactionMode.ReadOnly )]
  class Cmd_4_Subscribe : IExternalCommand
  {
     public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      UIApplication uiapp = commandData.Application;
      Document doc = uiapp.ActiveUIDocument.Document;

      // Determine custom project identifier.

      string project_id = Util.GetProjectIdentifier( doc );

      if ( !App.Subscribed && 0 == DbAccessor.Timestamp )
      {
        DbAccessor.Init( project_id );
      }

      DbAccessor.ToggleSubscription( uiapp );

      return Result.Succeeded;
    }
  }
}
