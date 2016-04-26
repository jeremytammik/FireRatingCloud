#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace FireRatingCloud
{
  /// <summary>
  /// BIM updater, driven both via external 
  /// command and external event handler.
  /// </summary>
  class BimUpdater : IExternalEventHandler
  {
    /// <summary>
    /// Update the BIM with the given database records.
    /// </summary>
    public static bool UpdateBim(
      Document doc,
      List<FireRating.DoorData> doors,
      ref string error_message )
    {
      Guid paramGuid;

      if ( !Util.GetSharedParamGuid( doc.Application,
        out paramGuid ) )
      {
        error_message = "Shared parameter GUID not found.";
        return false;
      }

      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();

      // Loop through the doors and update   
      // their firerating parameter values.

      if ( null != doors && 0 < doors.Count )
      {
        using ( Transaction t = new Transaction( doc ) )
        {
          t.Start( "Import Fire Rating Values" );

          // Retrieve element unique id and 
          // FireRating parameter values.

          foreach ( FireRating.DoorData d in doors )
          {
            string uid = d._id;
            Element e = doc.GetElement( uid );

            if ( null == e )
            {
              error_message = string.Format(
                "Error retrieving element for "
                + "unique id {0}.", uid );

              return false;
            }

            Parameter p = e.get_Parameter( paramGuid );

            if ( null == p )
            {
              error_message = string.Format(
                "Error retrieving shared parameter on "
                + " element with unique id {0}.", uid );

              return false;
            }
            object fire_rating = d.firerating;

            p.Set( (double) fire_rating );

            p = e.get_Parameter( DoorData.BipMark );

            if ( null == p )
            {
              error_message = string.Format(
                "Error retrieving ALL_MODEL_MARK "
                + "built-in parameter on element with "
                + "unique id {0}.", uid );

              return false;
            }

            p.Set( (string) d.tag );
          }
          t.Commit();
        }
      }

      stopwatch.Stop();

      Util.Log( string.Format(
        "{0} milliseconds to import {1} element{2}.",
        stopwatch.ElapsedMilliseconds, doors.Count,
        Util.PluralSuffix( doors.Count ) ) );

      return true;
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

      Debug.Assert( Util.GetProjectIdentifier( doc )
        .Equals( DbAccessor.ProjectId ),
        "expected same project" );

      string error_message = null;

      bool rc = UpdateBim( doc,
        DbAccessor.ModifiedDoors,
        ref error_message );

      if ( rc )
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
