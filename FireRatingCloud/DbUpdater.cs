#region Namespaces
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
#endregion

namespace FireRatingCloud
{
  class DbUpdater : IExternalEventHandler
  {
    static public int Timestamp
    {
      get;
      set;
    }

    /// <summary>
    /// Determine and set the timestamp 
    /// after updating database.
    /// </summary>
    static public int SetTimestamp()
    {
      Timestamp = Util.UnixTimestamp();

      Util.InfoMsg( string.Format(
        "Timestamp set to {0}."
        + "\nChanges from now on will be applied.",
        Timestamp ) );

      return Timestamp;
    }

    /// <summary>
    /// Revit UI application.
    /// </summary>
    UIApplication _uiapp = null;

    /// <summary>
    /// Separate thread running loop to
    /// poll for pending database changes.
    /// </summary>
    static Thread _thread = null;

    public DbUpdater( UIApplication uiapp )
    {
      _uiapp = uiapp;
    }

    /// <summary>
    /// Execute method invoked by Revit via the 
    /// external event as a reaction to a call 
    /// to its Raise method.
    /// </summary>
    public void Execute( UIApplication a )
    {
      Document doc = a.ActiveUIDocument.Document;
      string error_message = null;

      bool rc = Cmd_3_ImportSharedParameterValues
        .UpdateBimFromDb( doc, Timestamp, 
          ref error_message );

      if ( rc )
      {
        SetTimestamp();
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

    /// <summary>
    /// Count total number of checks for
    /// database updates made so far.
    /// </summary>
    static int _nLoopCount = 0;

    /// <summary>
    /// Count total number of checks for
    /// database updates made so far.
    /// </summary>
    static int _nCheckCount = 0;

    /// <summary>
    /// Count total number of database 
    /// updates requested so far.
    /// </summary>
    static int _nUpdatesRequested = 0;

    /// <summary>
    /// Number of milliseconds to wait and relinquish
    /// CPU control before next check for pending
    /// database updates.
    /// </summary>
    static int _timeout = 500;

    // DLL imports from user32.dll to set focus to
    // Revit to force it to forward the external event
    // Raise to actually call the external event 
    // Execute.

    /// <summary>
    /// The GetForegroundWindow function returns a 
    /// handle to the foreground window.
    /// </summary>
    [DllImport( "user32.dll" )]
    static extern IntPtr GetForegroundWindow();

    /// <summary>
    /// Move the window associated with the passed 
    /// handle to the front.
    /// </summary>
    [DllImport( "user32.dll" )]
    static extern bool SetForegroundWindow(
      IntPtr hWnd );

    /// <summary>
    /// Repeatedly check database status and raise 
    /// external event when updates are pending.
    /// Relinquish control and wait for timeout
    /// period between each attempt. Run in a 
    /// separate thread.
    /// </summary>
    static void CheckForPendingDatabaseChanges()
    {
      while( null != App.Event )
      {
        ++_nLoopCount;

        //Debug.Assert( null != _event,
        //"expected non-null external event" );

        //if( null == _event )
        //{
        //  break;
        //}

        if( App.Event.IsPending )
        {
          Util.Log( string.Format(
            "CheckForPendingDatabaseChanges loop {0} - "
            + "database update event is pending",
            _nLoopCount ) );
        }
        else
        {
          //using( JtTimer pt = new JtTimer(
          //  "CheckForPendingDatabaseChanges" ) )
          {
            ++_nCheckCount;

            Util.Log( string.Format(
              "CheckForPendingDatabaseChanges loop {0} - "
              + "check for changes {1}",
              _nLoopCount, _nCheckCount ) );

            //RoomEditorDb rdb = new RoomEditorDb();

            //if( rdb.LastSequenceNumberChanged(
            //  DbUpdater.LastSequence ) )
            {
              App.Event.Raise();

              ++_nUpdatesRequested;

              Util.Log( string.Format(
                "database update pending event raised {0} times",
                _nUpdatesRequested ) );

              // Set focus to Revit for a moment.
              // Otherwise, it may take a while before 
              // Revit forwards the event Raise to the
              // event handler Execute method.

              IntPtr hBefore = GetForegroundWindow();

              SetForegroundWindow(
                ComponentManager.ApplicationWindow );

              SetForegroundWindow( hBefore );
            }
          }
        }

        // Wait a moment and relinquish control before
        // next check for pending database updates.

        Thread.Sleep( _timeout );
      }
    }

    /// <summary>
    /// Toggle subscription to automatic database 
    /// updates. Forward the call to the external 
    /// application that creates the external event,
    /// store it and launch a separate thread checking 
    /// for database updates. When changes are pending,
    /// invoke the external event Raise method.
    /// </summary>
    public static void ToggleSubscription(
      UIApplication uiapp )
    {
      // Todo: stop thread first!

      if( App.ToggleSubscription2( new DbUpdater( uiapp ) ) )
      {
        // Start a new thread to regularly check the
        // database status and raise the external event
        // when updates are pending.

        _thread = new Thread(
          CheckForPendingDatabaseChanges );

        _thread.Start();
      }
      else
      {
        _thread.Abort();
        _thread = null;
      }
    }
  }
}
