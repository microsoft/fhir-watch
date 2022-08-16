using Microsoft.AspNetCore.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FhirBlaze.SharedComponents
{
    public partial class Treeview
    {
        [Parameter]
        public Branch Trunk { get; set; }

        private void SpanToggle(EventArgs e, Branch item)
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

        private bool IsDate(object obj)
        {
            return DateTime.TryParse(obj as string, out var dateTime) || obj.GetType() == typeof(DateTimeOffset);
        }

        private bool OverridesToString(object obj)
        {
            try
            {
                MethodInfo methodInfo = obj.GetType().GetMethod("ToString", new[] { typeof(string) });

                if (methodInfo == null)
                    return false;

                return methodInfo.DeclaringType != methodInfo.GetBaseDefinition().DeclaringType;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool IsList(object obj)
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

        private bool IsPrimitive(object obj)
        {
            if (obj == null) // questionable..
                return true;

            var type = obj.GetType();

            return obj is string || obj is decimal || type.IsEnum || obj.GetType().IsPrimitive;
        }
    }
}
