using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireRatingCloud
{
  class DoorData
  {
    public string UniqueId { get; set; }
    public string ProjectId { get; set; }
    public string Level { get; set; }
    public string Tag { get; set; }
    public double FireRating { get; set; }

    public DoorData(
      Element door,
      string project_id,
      Guid paramGuid )
    {
      Document doc = door.Document;

      UniqueId = door.UniqueId;
      ProjectId = project_id;

      Level = doc.GetElement( door.LevelId ).Name;

      Tag = door.get_Parameter( 
        BuiltInParameter.ALL_MODEL_MARK ).AsString();

      FireRating = door.get_Parameter( paramGuid )
        .AsDouble();
    }
  }
}
