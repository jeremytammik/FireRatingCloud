using FireRating;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoorData = FireRating.DoorData;

namespace FireRatingClient
{
  public partial class Form1 : Form
  {
    const string _collection_name = "doors";

    public Form1()
    {
      InitializeComponent();
      List<DoorData> puertas = Util.Get( _collection_name );
      FOLV_Puertas.SetObjects( puertas );
    }

    private void ExportData( DoorData dd )
    {
      HttpStatusCode sc;
      string jsonResponse, errorMessage;

      //Debug.Print( dd._id.ToString() );

      sc = Util.Put( out jsonResponse,
        out errorMessage,
        "doors/" + dd._id, dd );

      //Debug.Print( jsonResponse );
    }

    private void Refresh_Click(
      object sender, 
      EventArgs e )
    {
      List<DoorData> puertas = Util.Get( _collection_name );
      FOLV_Puertas.SetObjects( puertas );
    }

    private void FOLV_Puertas_CellEditFinished( 
      object sender, 
      BrightIdeasSoftware.CellEditEventArgs e )
    {
      ExportData( e.RowObject as DoorData );
    }
  }
}
