//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Session1_TPRedo
{
    using System;
    using System.Collections.Generic;
    
    public partial class Resource_Allocation
    {
        public int allocId { get; set; }
        public int resIdFK { get; set; }
        public int skillIdFK { get; set; }
    
        public virtual Resource Resource { get; set; }
        public virtual Skill Skill { get; set; }
    }
}
