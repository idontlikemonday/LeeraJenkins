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
    
    public partial class GamePlayer
    {
        public long Id { get; set; }
        public long GameRegistrationId { get; set; }
        public Nullable<long> PlayerId { get; set; }
        public bool IsHost { get; set; }
        public int PlayerNum { get; set; }
    
        public virtual GameRegistration GameRegistration { get; set; }
        public virtual Player Player { get; set; }
    }
}
