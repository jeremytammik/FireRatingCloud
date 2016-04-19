#region Namespaces
using Autodesk.Revit.DB;
using System;
#endregion // Namespaces

namespace FireRatingCloud
{
  class DoorData : FireRating.DoorData
  {
    /// <summary>
    /// Access the Revit Element ALL_MODEL_MARK 
    /// built-in parameter.
    /// </summary>
    public const BuiltInParameter BipMark
      = BuiltInParameter.ALL_MODEL_MARK;

    /// <summary>
    /// DoorData constructor to generate the data to
    /// serialise for the REST POST or PUT request.
    /// </summary>
    public DoorData(
      Element door,
      string project_id_arg,
      Guid paramGuid )
    {
      Document doc = door.Document;

      _id = door.UniqueId;

      project_id = project_id_arg;

      level = doc.GetElement( door.LevelId ).Name;

      tag = door.get_Parameter( BipMark ).AsString();

      firerating = door.get_Parameter( paramGuid )
        .AsDouble();

      modified = Util.UnixTimestamp();
    }
  }
}
