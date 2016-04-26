#region Namespaces
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
      if( !App.Subscribed
        && 0 == DbUpdater.Timestamp )
      {
        DbUpdater.SetTimestamp();
      }

      DbUpdater.ToggleSubscription(
        commandData.Application );

      return Result.Succeeded;
    }
  }
}
