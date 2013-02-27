using System;
using System.Text;
using System.Collections.Generic;


namespace OccupOSMonitor.Models {

    public class AppUser : Entity {
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }
        public virtual System.DateTime createdAt { get; set; }
        public virtual System.DateTime updatedAt { get; set; }
        public virtual System.Nullable<int> creatorId { get; set; }
        public virtual System.Nullable<int> updaterId { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
    }
}
