using System;
using System.Text;
using System.Collections.Generic;


namespace OccupOSMonitorNew.Models
{

    public class AppUser {
        public  string Username { get; set; }
        public  string Email { get; set; }
        public  string Password { get; set; }
        public  System.DateTime createdAt { get; set; }
        public  System.DateTime updatedAt { get; set; }
        public  System.Nullable<int> creatorId { get; set; }
        public  System.Nullable<int> updaterId { get; set; }
        public  string FirstName { get; set; }
        public  string LastName { get; set; }
    }
}
