# FireRatingCloud
Revit add-in multi-project re-implementation of the FireRating SDK sample using a cloud-based database managed by the
[firerating](https://github.com/jeremytammik/firerating) node.js mongoDB web server.

For more information, please refer to
[The 3D Web Coder](http://the3dwebcoder.typepad.com),
[The Building Coder](http://thebuildingcoder.typepad.com) and
the detailed articles describing the entire implementation and evolution:

- [My first mongo database](http://the3dwebcoder.typepad.com/blog/2015/06/my-first-mongo-database.html)
  - Define the over-all goal and what to store, namely building projects, door instances hosted in them, and each door's fire rating value, based on the venerable old Revit SDK FireRating sample.
- [Implementing relationships](http://the3dwebcoder.typepad.com/blog/2015/07/implementing-mongo-database-relationships.html)
  - Define a more complete schema that includes information about the container projects, i.e., the Revit RVT BIM or building information model project files.
  - Define and maintain the relationships between the door family instances and their container projects.
- [Starting to Implement the FireRating REST API](http://the3dwebcoder.typepad.com/blog/2015/07/starting-to-implement-the-firerating-rest-api.html)
  - Adding a REST API to populate and query the database programmatically.
- Implement and test REST API PUT, POST and DELETE requests.
- Implement a Revit add-in exercising the REST API from C# .NET.
- Re-implement the complete cloud-based Revit FireRating SDK sample functionality.


## Author

Jeremy Tammik,
[The Building Coder](http://thebuildingcoder.typepad.com) and
[The 3D Web Coder](http://the3dwebcoder.typepad.com),
[ADN](http://www.autodesk.com/adn)
[Open](http://www.autodesk.com/adnopen),
[Autodesk Inc.](http://www.autodesk.com)


## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).
Please see the [LICENSE](LICENSE) file for full details.
