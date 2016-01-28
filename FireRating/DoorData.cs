#region Namespaces
using System;
#endregion // Namespaces

namespace FireRating
{
  public class DoorData
  {
    public string _id { get; set; }
    public string project_id { get; set; }
    public string level { get; set; }
    public string tag { get; set; }
    public double firerating { get; set; }


    /// <summary>
    /// Constructor to populate instance by 
    /// deserialising the REST GET response.
    /// </summary>
    public DoorData()
    {
    }
  }
}
