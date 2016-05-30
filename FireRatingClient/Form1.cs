#region Namespaces
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using FireRating;
using System.Diagnostics;
#endregion // Namespaces

namespace FireRatingClient
{
  public partial class Form1 : Form
  {
    const string _collection_name = "doors";

    public Form1()
    {
      InitializeComponent();
      List<DoorData> doors = Util.Get( _collection_name );

#if DEBUG
      if( null == doors )
      {
        doors = new List<DoorData>( 1 );
        doors.Add( new DoorData() );
        doors[0].firerating = 123;
      }
#endif

      FOLV_doors.SetObjects( doors );
    }

    void Refresh_Click(
      object sender,
      EventArgs e )
    {
      List<DoorData> doors = Util.Get( _collection_name );
      FOLV_doors.SetObjects( doors );
    }

    void ExportData( DoorData dd )
    {
      uint timestamp = Util.UnixTimestamp();

      dd.modified = timestamp;

      string jsonResponse, errorMessage;

      Debug.Print(
        "{0}: set door {1} firerating to {2}",
        timestamp, dd._id, dd.firerating );

      HttpStatusCode sc = Util.Put(
        out jsonResponse, out errorMessage,
        "doors/" + dd._id, dd );

      //Util.Log( jsonResponse );
    }

    void OnDoorsCellEditValidating(
      object sender,
      BrightIdeasSoftware.CellEditEventArgs e )
    {
      if ( !e.Cancel )
      {
        ( (DoorData) e.RowObject ).firerating 
          = double.Parse( e.Control.Text );
      }
    }

    void OnDoorsCellEditFinishing(
      object sender,
      BrightIdeasSoftware.CellEditEventArgs e )
    {
      if ( !e.Cancel )
      {
        ExportData( e.RowObject as DoorData );
      }
    }
  }
}
