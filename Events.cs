//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Calendar
{
    using System;
    using System.Collections.Generic;
    
    public partial class Events
    {
        public int Id { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string Name { get; set; }
        public int TypeID { get; set; }
        public string Note { get; set; }
    
        public virtual EventsTypes EventsTypes { get; set; }
    }
}