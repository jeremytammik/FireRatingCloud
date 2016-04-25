#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace FireRatingCloud
{
  [Transaction( TransactionMode.ReadOnly )]
  class Cmd_0_About : IExternalCommand
  {
    const string _description
      = "Demonstrate round-trip editing of shared prameter "
      + "values in a Revit model on any mobile device with "
      + "need for installation of any additional software "
      + "whatsoever. How can this be achieved? "
      + "A Revit add-in exports the data to a cloud-based "
      + "MongoDB NoSQL database. Tthe database cn be edited "
      + "from anywhere, whether mobile device or Windows "
      + "client. Modified vlues are saved back to the "
      + "cloud database. The Revit add-in picks up the "
      + "changes and updates the BIM in real-time. All of "
      + "the components used are completely open source, "
      + "except Revit itself.\r\n\r\n"
      + "Jeremy Tammik, Autodesk Inc.";

    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      Util.InfoMsg2( 
        "Cloud-based FireRating Database",
        _description );
      
      return Result.Succeeded;
    }
  }
}
