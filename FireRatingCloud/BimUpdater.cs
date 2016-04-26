#region Namespaces
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using System.Collections.Generic;
using System.Diagnostics;
#endregion

namespace FireRatingCloud
{
  class BimUpdater : IExternalEventHandler
  {
    /// <summary>
    /// Current document project id.
    /// Todo: update this when switching Revit documents.
    /// </summary>
    static string _project_id = null;

    public BimUpdater( UIApplication uiapp )
    {
      _project_id = Util.GetProjectIdentifier( 
        uiapp.ActiveUIDocument.Document );
    }

    /// <summary>
    /// Execute method invoked by Revit via the 
    /// external event as a reaction to a call 
    /// to its Raise method.
    /// </summary>
    public void Execute( UIApplication a )
    {
      uint timestamp_before_bim_update 
        = Util.UnixTimestamp();

      Document doc = a.ActiveUIDocument.Document;
      string error_message = null;

      bool rc = Cmd_3_ImportSharedParameterValues
        .UpdateBimFromDb( doc, DbAccessor.Timestamp,
          ref error_message );

      if( rc )
      {
        DbAccessor.Timestamp = timestamp_before_bim_update;
      }
      else
      {
        throw new SystemException( error_message );
      }
    }

    /// <summary>
    /// Required IExternalEventHandler interface 
    /// method returning a descriptive name.
    /// </summary>
    public string GetName()
    {
      return App.Caption + " " + GetType().Name;
    }
  }
}
