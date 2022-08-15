using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace FhirBlaze.SharedComponents
{
    public partial class Treeview<Tvalue>
    {
        [Parameter]
        public List<Tvalue> DataSource { get; set; }
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public string ParentId { get; set; }
        [Parameter]
        public string HasChildren { get; set; }
        [Parameter]
        public string Text { get; set; }
        [Parameter]
        public string Expanded { get; set; }

        public List<Tvalue> AllItems;
        public Dictionary<int, bool> _caretDown = new Dictionary<int, bool>();
        public Dictionary<int, string> _caretcss = new Dictionary<int, string>();
        public Dictionary<int, string> _nestedcss = new Dictionary<int, string>();

        protected override Task OnInitializedAsync()
        {
            //asigning to its new instance to avoid exceptions.
            AllItems = new List<Tvalue>();
            AllItems = DataSource.ToArray().ToList();

            if (AllItems == null)
                return base.OnInitializedAsync();

            foreach (var item in AllItems)
            {
                var _id = Convert.ToInt32(GetPropertyValue(item, Id));

                //initializing fields with default value.
                _caretDown.Add(_id, true);
                _caretcss.Add(_id, "caret");
                _nestedcss.Add(_id, "nested");
            }

            return base.OnInitializedAsync();
        }

        protected override Task OnParametersSetAsync()
        {
            // This will check if the first item in the
            // list/collection has a "parentId" then remove the "parentId" from it.
            // Because we use the first item as a reference in the razor file, so it must not have "parentId".

            var Parem = AllItems.First();

            switch (GetPropertyType(Parem, ParentId))
            {
                case "Int32":
                    if (Convert.ToInt32(GetPropertyValue(Parem, ParentId)) > 0)
                        SetPropertyValue(Parem, ParentId, 0);
                    break;
                case "String":
                    if (GetPropertyValue(Parem, ParentId) != "")
                        SetPropertyValue(Parem, ParentId, "");
                    break;
                default:
                    break;
            }

            return base.OnParametersSetAsync();
        }

        private void SpanToggle(EventArgs e, Tvalue item)
        {
            var _clckdItemid = Convert.ToInt32(GetPropertyValue(item, Id));

            _caretcss[_clckdItemid] = _caretDown[_clckdItemid] ? "caret caret-down" : "caret";
            _nestedcss[_clckdItemid] = _caretDown[_clckdItemid] ? "active" : "nested";
            _caretDown[_clckdItemid] = !_caretDown[_clckdItemid];
        }

        #region reflection methodes to get your property type, propert value and also set property value
        private static string GetPropertyValue(Tvalue item, string Property)
        {
            if (item != null)
                return item.GetType().GetProperty(Property).GetValue(item, null).ToString();

            return "";
        }

        private static void SetPropertyValue<T>(Tvalue item, string Property, T value)
        {
            if (item != null)
                item.GetType().GetProperty(Property).SetValue(item, value);
        }

        private static string GetPropertyType(Tvalue item, string Property)
        {
            if (item != null)
                return item.GetType().GetProperty(Property).PropertyType.Name;

            return null;
        }
        #endregion
    }
}
