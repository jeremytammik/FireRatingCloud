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
      UIApplication uiapp = commandData.Application;

      if ( !App.Subscribed && 0 == DbAccessor.Timestamp )
      {
        DbAccessor.SetTimestamp();
      }

      DbAccessor.ToggleSubscription( uiapp );

      return Result.Succeeded;
    }
  }
}
