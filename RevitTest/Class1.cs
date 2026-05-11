
//using Autodesk.Revit.DB;
//using Autodesk.Revit.DB.Events;
//using Autodesk.Revit.UI;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Xml.Linq;

//namespace RevitTest
//{
//    public class App : IExternalApplication
//    {
//        public Result OnStartup(UIControlledApplication application)
//        {

//            application.ControlledApplication.DocumentChanged += OnDocumentChanged;
//            return Result.Succeeded;
//        }

//        public Result OnShutdown(UIControlledApplication application)
//        {
//            application.ControlledApplication.DocumentChanged -= OnDocumentChanged;
//            return Result.Succeeded;
//        }

//        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
//        {
//            TaskDialog.Show("Title", "OnDocumentChanged working");
//            Document doc = e.GetDocument();

//            var modifiedIds = e.GetModifiedElementIds();
//            var addedIds = e.GetAddedElementIds();
//            var deletedIds = e.GetDeletedElementIds();

//            var result = new
//            {
//                Time = DateTime.Now,

//                Added = ProcessElements(doc, addedIds, "Added"),

//                Modified = ProcessElements(doc, modifiedIds, "Modified"),

//                Deleted = deletedIds.Select(id => new
//                {
//                    Id = id.Value,
//                    Status = "Deleted"
//                }).ToList()
//            };

//            WriteJson(result);
//        }

//        public List<object> ProcessElements(Document doc, ICollection<ElementId> Ids, string WhatChanged)
//        {
//            List<object> changedElements = new List<object>();

//            foreach (var id in Ids)
//            {
//                Element element = doc.GetElement(id);

//                if (element == null)
//                {
//                    continue;
//                }
//                var data = GetElementsData(element);
//                changedElements.Add(data);
//            }

//            return changedElements;
//        }

//        public Dictionary<string, Object> GetElementsData(Element element)
//        {
//            TaskDialog.Show("Title", "GetElementsData working");
//            var data = new Dictionary<string, object>();

//            Type t = element.GetType();

//            data["Id"] = element.Id.Value;
//            data["Category"] = element.Category;
//            data["ClassName"] = t.Name;

//           PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

//            foreach (var prop in properties)
//            {
//                try
//                {
//                    if (!prop.CanRead)
//                    {
//                        continue;
//                    }

//                    if (prop.GetIndexParameters().Length > 0)
//                    {
//                        continue;
//                    }

//                    Object Value = prop.GetValue(element);

//                    if (Value == null)
//                    {
//                        data[prop.Name] = null;
//                        continue;
//                    }

//                    //if (Value == typeof(string) || Value == typeof(double) || Value == typeof(decimal) || Value == typeof(bool))
//                    //{
//                    //    data[prop.Name] = ConvertToSafeValue(Value);
//                    //}
//                    //else
//                    //{
//                    //}
//                    var inner = GetInsideIt(prop, element);

//                    foreach (var kv in inner)
//                    {
//                        data[kv.Key] = kv.Value;
//                    }

//                }
//                catch
//                {
//                    data[prop.Name] = "None";
//                }

//            }
//            //data["Parameters"] = GetAllParameters(element);
//            return data;
//        }

//        public Dictionary<string, Object> GetInsideIt(PropertyInfo prop, Element element)
//        {
//            var result = new Dictionary<string, object>();
//            object value = prop.GetValue(element);

//            if (value is ParameterSet paramSet)
//            {
//                foreach (Parameter param in paramSet)
//                {
//                    try
//                    {
//                        result[param.Definition.Name] = GetParameterValue(param);
//                    }
//                    catch
//                    {
//                        result[param.Definition.Name] = "None";
//                    }
//                }
//            }
//            else if (value is Document doc)
//            {
//                foreach (var p in doc.GetType().GetProperties())
//                {
//                    try
//                    {
//                        result[p.Name] = p.GetValue(doc);
//                    }
//                    catch
//                    {
//                        result[p.Name] = "None";
//                    }
//                }
//            } 
//            else if (value is Location loc)
//            {
//                foreach (var l in loc.GetType().GetProperties())
//                {
//                    try
//                    {
//                        result[l.Name] = l.GetValue(loc);
//                    }
//                    catch
//                    {
//                        result[l.Name] = "None";
//                    }
//                }
//            }
//            return result;
//        }

//        public Object GetParameterValue(Parameter param)
//        {
//            string valueString = param.AsValueString();

//            if (!string.IsNullOrEmpty(valueString))
//            {
//                return valueString;
//            }

//            switch (param.StorageType)
//            {
//                case StorageType.String:
//                    return param.AsString();
//                case StorageType.Integer:
//                    return param.AsInteger();
//                case StorageType.Double:
//                    return param.AsDouble();
//                case StorageType.ElementId:
//                    return param.AsElementId().Value;
//                default:
//                    return "Unsupported Type";
//            }
//        }
//        private object ConvertToSafeValue(object value)
//        {
//            if (value == null)
//                return null;

//            // Primitive types
//            if (value is string ||
//                value is int ||
//                value is double ||
//                value is bool)
//            {
//                return value;
//            }

//            // ElementId
//            if (value is ElementId eid)
//            {
//                return eid.Value;
//            }

//            // Category
//            if (value is Category cat)
//            {
//                return cat.Name;
//            }

//            // XYZ
//            if (value is XYZ xyz)
//            {
//                return new
//                {
//                    X = xyz.X,
//                    Y = xyz.Y,
//                    Z = xyz.Z
//                };
//            }

//            // Element
//            if (value is Element el)
//            {
//                return $"Element:{el.Id.Value}";
//            }

//            // Fallback
//            return value.ToString();
//        }
//        private void WriteJson(object data)
//        {
//            string folder = @"C:\RevitUnitySync";

//            if (!Directory.Exists(folder))
//                Directory.CreateDirectory(folder);

//            string filePath = Path.Combine(
//                folder,
//                $"changes_{DateTime.Now:yyyyMMdd_HHmmss_fff}.json"
//            );

//            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

//            File.WriteAllText(filePath, json);
//        }

//    }
//}



////using Autodesk.Revit.DB;
////using Autodesk.Revit.DB.Events;
////using Autodesk.Revit.UI;
////using Newtonsoft.Json;
////using System;
////using System.Collections.Generic;
////using System.IO;
////using System.Linq;
////using System.Reflection;

////namespace RevitTest
////{
////    public class App : IExternalApplication
////    {
////        public Result OnStartup(UIControlledApplication application)
////        {
////            application.ControlledApplication.DocumentChanged += OnDocumentChanged;
////            return Result.Succeeded;
////        }

////        public Result OnShutdown(UIControlledApplication application)
////        {
////            application.ControlledApplication.DocumentChanged -= OnDocumentChanged;
////            return Result.Succeeded;
////        }

////        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
////        {
////            System.Diagnostics.Debug.WriteLine("OnDocumentChanged working");

////            Document doc = e.GetDocument();

////            var modifiedIds = e.GetModifiedElementIds();
////            var addedIds = e.GetAddedElementIds();
////            var deletedIds = e.GetDeletedElementIds();

////            var result = new
////            {
////                Time = DateTime.Now,

////                Added = ProcessElements(doc, addedIds, "Added"),
////                Modified = ProcessElements(doc, modifiedIds, "Modified"),

////                Deleted = deletedIds.Select(id => new
////                {
////                    Id = id.Value,
////                    Status = "Deleted"
////                }).ToList()
////            };

////            WriteJson(result);
////        }

////        public List<object> ProcessElements(Document doc, ICollection<ElementId> Ids, string WhatChanged)
////        {
////            List<object> changedElements = new List<object>();

////            foreach (var id in Ids)
////            {
////                Element element = doc.GetElement(id);

////                if (element == null)
////                    continue;

////                var data = GetElementsData(element);
////                changedElements.Add(data);
////            }

////            return changedElements;
////        }

////        public Dictionary<string, object> GetElementsData(Element element)
////        {
////            System.Diagnostics.Debug.WriteLine("GetElementsData working");

////            var data = new Dictionary<string, object>();

////            Type t = element.GetType();

////            data["Id"] = element.Id.Value;
////            data["Category"] = element.Category?.Name;
////            data["ClassName"] = t.Name;

////            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

////            foreach (var prop in properties)
////            {
////                try
////                {
////                    if (!prop.CanRead)
////                        continue;

////                    if (prop.GetIndexParameters().Length > 0)
////                        continue;

////                    object value = null;

////                    try
////                    {
////                        value = prop.GetValue(element);
////                    }
////                    catch
////                    {
////                        data[prop.Name] = "None";
////                        continue;
////                    }

////                    if (value == null)
////                    {
////                        data[prop.Name] = null;
////                        continue;
////                    }

////                    var inner = GetInsideIt(prop, element);

////                    foreach (var kv in inner)
////                    {
////                        data[kv.Key] = kv.Value;
////                    }
////                }
////                catch
////                {
////                    data[prop.Name] = "None";
////                }
////            }

////            return data;
////        }

////        public Dictionary<string, object> GetInsideIt(PropertyInfo prop, Element element)
////        {
////            var result = new Dictionary<string, object>();

////            object value = null;

////            try
////            {
////                value = prop.GetValue(element);
////            }
////            catch
////            {
////                return result;
////            }

////            if (value is ParameterSet paramSet)
////            {
////                foreach (Parameter param in paramSet)
////                {
////                    try
////                    {
////                        result[param.Definition.Name] = GetParameterValue(param);
////                    }
////                    catch
////                    {
////                        result[param.Definition.Name] = "None";
////                    }
////                }
////            }
////            else if (value is Document doc)
////            {
////                foreach (var p in doc.GetType().GetProperties())
////                {
////                    try
////                    {
////                        result[p.Name] = p.GetValue(doc);
////                    }
////                    catch
////                    {
////                        result[p.Name] = "None";
////                    }
////                }
////            }
////            else if (value is Location loc)
////            {
////                foreach (var l in loc.GetType().GetProperties())
////                {
////                    try
////                    {
////                        result[l.Name] = l.GetValue(loc);
////                    }
////                    catch
////                    {
////                        result[l.Name] = "None";
////                    }
////                }
////            }

////            return result;
////        }

////        public object GetParameterValue(Parameter param)
////        {
////            string valueString = param.AsValueString();

////            if (!string.IsNullOrEmpty(valueString))
////                return valueString;

////            switch (param.StorageType)
////            {
////                case StorageType.String:
////                    return param.AsString();

////                case StorageType.Integer:
////                    return param.AsInteger();

////                case StorageType.Double:
////                    return param.AsDouble();

////                case StorageType.ElementId:
////                    return param.AsElementId().Value;

////                default:
////                    return "Unsupported Type";
////            }
////        }

////        private void WriteJson(object data)
////        {
////            TaskDialog.Show("Title", "WriteJson working");

////            string folder = @"C:\RevitUnitySync";

////            if (!Directory.Exists(folder))
////                Directory.CreateDirectory(folder);

////            string filePath = Path.Combine(
////                folder,
////                $"changes_{DateTime.Now:yyyyMMdd_HHmmss_fff}.json"
////            );

////            TaskDialog.Show("Title", "WriteJson working");
////            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

////            TaskDialog.Show("Title", "WriteJson working");

////            File.WriteAllText(filePath, json);
////        }
////    }
////}
///

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RevitTest
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentChanged += OnDocumentChanged;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentChanged -= OnDocumentChanged;
            return Result.Succeeded;
        }

        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            Document doc = e.GetDocument();

            var result = new
            {
                Time = DateTime.Now,

                Added = ProcessElements(doc, e.GetAddedElementIds()),

                Modified = ProcessElements(doc, e.GetModifiedElementIds()),

                Deleted = e.GetDeletedElementIds()
                    .Select(x => new
                    {
                        Id = x.Value,
                        Status = "Deleted"
                    })
                    .ToList()
            };

            WriteJson(result);
        }

        public List<object> ProcessElements(Document doc, ICollection<ElementId> ids)
        {
            List<object> elements = new List<object>();

            foreach (var id in ids)
            {
                Element element = doc.GetElement(id);

                if (element == null)
                    continue;

                var visited = new HashSet<object>();

                elements.Add(
                    ExtractObjectData(element, visited, 0)
                );
            }

            return elements;
        }

        private object ExtractObjectData(
            object obj,
            HashSet<object> visited,
            int depth)
        {
            if (obj == null)
                return null;

            if (depth > 7)
                return "Max Depth Reached";

            Type type = obj.GetType();

            // PREVENT CIRCULAR REFERENCES
            if (!type.IsValueType)
            {
                if (visited.Contains(obj))
                    return "Circular Reference";

                visited.Add(obj);
            }

            // SIMPLE TYPES
            if (IsSimple(type))
                return obj;

            // ELEMENT ID
            if (obj is ElementId eid)
                return eid.Value;

            // CATEGORY
            if (obj is Category cat)
                return cat.Name;

            // XYZ
            if (obj is XYZ xyz)
            {
                return new
                {
                    xyz.X,
                    xyz.Y,
                    xyz.Z
                };
            }

            // DOCUMENT SKIP
            if (obj is Document)
                return "Document Skipped";

            //// PARAMETER
            //if (obj is Parameter param)
            //{
            //    return GetParameterValue(param);
            //}

            // COLLECTIONS
            if (obj is IEnumerable enumerable &&
                !(obj is string))
            {
                List<object> list = new List<object>();

                foreach (var item in enumerable)
                {
                    list.Add(
                        ExtractObjectData(item, visited, depth + 1)
                    );
                }

                return list;
            }

            // COMPLEX OBJECT
            Dictionary<string, object> data =
                new Dictionary<string, object>();

            data["Type"] = type.Name;

            PropertyInfo[] props =
                type.GetProperties(
                    BindingFlags.Public |
                    BindingFlags.Instance);

            foreach (var prop in props)
            {
                try
                {
                    if (!prop.CanRead)
                        continue;

                    if (prop.GetIndexParameters().Length > 0)
                        continue;

                    object value = prop.GetValue(obj);

                    data[prop.Name] =
                        ExtractObjectData(
                            value,
                            visited,
                            depth + 1);
                }
                catch (Exception ex)
                {
                    data[prop.Name] =
                        $"Error: {ex.Message}";
                }
            }

            return data;
        }

        private bool IsSimple(Type type)
        {
            return
                type.IsPrimitive ||
                type.IsEnum ||
                type == typeof(string) ||
                type == typeof(decimal) ||
                type == typeof(DateTime);
        }

        public object GetParameterValue(Parameter param)
        {
            string valueString = param.AsValueString();

            if (!string.IsNullOrEmpty(valueString))
                return valueString;

            switch (param.StorageType)
            {
                case StorageType.String:
                    return param.AsString();

                case StorageType.Integer:
                    return param.AsInteger();

                case StorageType.Double:
                    return param.AsDouble();

                case StorageType.ElementId:
                    return param.AsElementId().Value;

                default:
                    return "Unsupported";
            }
        }

        private void WriteJson(object data)
        {
            string folder = @"C:\RevitUnitySync";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string filePath = Path.Combine(
                folder,
                $"changes_{DateTime.Now:yyyyMMdd_HHmmss_fff}.json"
            );

            string json = JsonConvert.SerializeObject(
                data,
                Formatting.Indented);

            File.WriteAllText(filePath, json);
        }
    }
}