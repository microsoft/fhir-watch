﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FhirWatch.PatientModule.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FhirWatch.PatientModule.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;@odata.etag&quot;: &quot;W/\&quot;9042015\&quot;&quot;,
        ///  &quot;customertypecode@OData.Community.Display.V1.FormattedValue&quot;: &quot;Default Value&quot;,
        ///  &quot;customertypecode&quot;: 1,
        ///  &quot;address1_latitude@OData.Community.Display.V1.FormattedValue&quot;: &quot;32.94799&quot;,
        ///  &quot;address1_latitude&quot;: 32.947991129353447,
        ///  &quot;birthdate@OData.Community.Display.V1.FormattedValue&quot;: &quot;12/31/2014&quot;,
        ///  &quot;birthdate&quot;: &quot;2014-12-31&quot;,
        ///  &quot;merged@OData.Community.Display.V1.FormattedValue&quot;: &quot;No&quot;,
        ///  &quot;merged&quot;: false,
        ///  &quot;gendercode@OData.Community.Display.V1.FormattedValue&quot;: &quot;Ma [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DVPatientData {
            get {
                return ResourceManager.GetString("DVPatientData", resourceCulture);
            }
        }
    }
}