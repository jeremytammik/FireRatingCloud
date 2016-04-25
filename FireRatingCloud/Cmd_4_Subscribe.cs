#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace FireRatingCloud
{
  /// <summary>
  /// Temporary dummy placeholder class.
  /// </summary>
  class DbUpdater
  {
    public static int LastSequence;
    public static void SetTimestamp() { }
    public static void ToggleSubscription( UIApplication a) { }
  }

  [Transaction( TransactionMode.ReadOnly )]
  class Cmd_4_Subscribe : IExternalCommand
  {
     public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      if( !App.Subscribed
        && -1 == DbUpdater.LastSequence )
      {
        DbUpdater.SetTimestamp();
      }

      DbUpdater.ToggleSubscription(
        commandData.Application );

      return Result.Succeeded;
    }
  }
}
