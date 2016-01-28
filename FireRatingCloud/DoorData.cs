#region Namespaces
using Autodesk.Revit.DB;
using System;
#endregion // Namespaces

namespace FireRatingCloud
{
  class DoorData : FireRating.DoorData
  {
    /// <summary>
    /// Access Revit Element ALL_MODEL_MARK parameter.
    /// </summary>
    public const BuiltInParameter BipMark 
      = BuiltInParameter.ALL_MODEL_MARK;

    /// <summary>
    /// Constructor from BIM to serialise for
    /// the REST POST or PUT request.
    /// </summary>
    /// <param name="door"></param>
    /// <param name="project_id"></param>
    /// <param name="paramGuid"></param>
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
    }
  }
}
