//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LeeraJenkins.Db
{
    using System;
    using System.Collections.Generic;
    
    public partial class GameRegistrationUnfinished
    {
        public long Id { get; set; }
        public System.Guid Guid { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Place { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string MaxPlayers { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<long> SheetRowId { get; set; }
        public System.DateTime Created { get; set; }
        public string Host { get; set; }
        public string DateRaw { get; set; }
        public string TimeRaw { get; set; }
    }
}
