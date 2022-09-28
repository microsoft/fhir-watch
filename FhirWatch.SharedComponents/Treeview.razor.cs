using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Graph;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace FhirWatch.SharedComponents
{
    public partial class Treeview
    {
        [Parameter]
        public Branch Trunk { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static void SpanToggle(EventArgs e, Branch item)
        {
            item.Expanded = !item.Expanded;
        }
    }

    public class Branch
    {
        public int Id { get; set; }
        public int LayerId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public List<Branch> Branches { get; set; }
        public bool Expanded { get; set; } = false;
        public bool IsObj { get; set; } = false;

        public Branch() { }
        public Branch(object obj, string name, int id, int layerId = 0)
        {
            Id = id;
            LayerId = layerId;

            // if the object is a primitive or otherwise human readable, set value
            if (obj == null)
            {
                Value = null;
                Branches = null;
                return;
            }
            else if (IsPrimitive(obj) || OverridesToString(obj))
            {
                Value = obj.ToString();
                Branches = null;
            }
            else if (IsDate(obj))
            {
                Value = DateTime.TryParse(obj as string, out DateTime dateTime) ? dateTime.ToLongDateString() : "!error parsing date!";
                Branches = null;
            }
            else
            {
                Value = null;

                // if list of things, convert things to branches
                if (IsList(obj))
                {
                    var list = ((IEnumerable)obj).Cast<object>();
                    //var list = obj as List<object>;

                    // TODO: fix IDs
                    Branches = list.Where(o => o != null).Select((o, i) => new Branch(o, $"[{i}]", i, layerId++)).Where(b => !string.IsNullOrEmpty(b.Name)).ToList();
                }
                else
                {
                    // otherwise convert properties to branches
                    var props = obj.GetType().GetProperties().ToDictionary(pi => pi.Name, pi => pi.GetValue(obj));

                    Branches = props.Where(kvp => kvp.Value != null).Select((kvp, i) => new Branch(kvp.Value, kvp.Key, i, layerId++)).Where(b => !string.IsNullOrEmpty(b.Name)).ToList();
                    IsObj = true;
                }
            }

            Name = name;
        }
        public Branch(JToken jToken, string name, int id, int layerId = 0)
        {
            Id = id;
            LayerId = layerId;


            if (jToken == null)
                return;

            var parent = jToken.Parent as JProperty;
            Name = name ?? parent?.Name ?? $"[{id}]";

            switch (jToken.Type)
            {
                case JTokenType.None:
                case JTokenType.Null:
                case JTokenType.Undefined:
                    Value = null;
                    Branches = null;
                    break;
                case JTokenType.String:
                case JTokenType.Uri:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.Boolean:
                    Value = jToken.ToString();
                    Branches = null;
                    break;
                case JTokenType.Date:
                case JTokenType.TimeSpan:
                    Value = jToken.ToString();
                    Branches = null;
                    break;
                case JTokenType.Property:
                    var prop = jToken as JProperty;
                    Name = prop.Name;

                    if (prop.Value is JArray)
                    {
                        Value = null;
                        Branches = prop.Value.Children().Select((c, i) => new Branch(c, null, i, layerId + 1)).ToList();
                    }
                    else
                    {
                        Value = prop.Value.ToString();
                        Branches = null;
                    }
                    break;
                case JTokenType.Object:
                    Value = null;
                    Branches = jToken.Children().Select((c, i) => new Branch(c, null, i, layerId + 1)).ToList();
                    IsObj = true;
                    break;
                case JTokenType.Array:
                    Value = null;
                    Branches = jToken.Children().Select((c, i) => new Branch(c, null, i, layerId + 1)).ToList();
                    break;
                case JTokenType.Constructor:
                case JTokenType.Comment:
                case JTokenType.Raw:
                case JTokenType.Guid:
                case JTokenType.Bytes:
                default:
                    throw new NotImplementedException();
            }
        }

        private static bool IsDate(object obj)
        {
            return DateTime.TryParse(obj as string, out _) || obj.GetType() == typeof(DateTimeOffset);
        }

        private static bool OverridesToString(object obj)
        {
            MethodInfo methodInfo = obj.GetType().GetMethod("ToString", new[] { typeof(string) });

            if (methodInfo == null)
                return false;

            return methodInfo.DeclaringType != methodInfo.GetBaseDefinition().DeclaringType;
        }
        private static bool IsList(object obj)
        {
            try
            {
                return obj.GetType().GetGenericTypeDefinition() == typeof(List<>);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsPrimitive(object obj)
        {
            if (obj == null) // questionable..
                return true;

            var type = obj.GetType();

            return obj is string || obj is decimal || type.IsEnum || obj.GetType().IsPrimitive;
        }
    }
}
