#region Namespaces
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using FireRating;
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

      //Util.Log( dd._id.ToString() );

      HttpStatusCode sc = Util.Put( 
        out jsonResponse, out errorMessage,
        "doors/" + dd._id, dd );

      //Util.Log( jsonResponse );
    }

    void OnDoorsCellEditFinished(
      object sender,
      BrightIdeasSoftware.CellEditEventArgs e )
    {
      ExportData( e.RowObject as DoorData );
    }
  }
}
