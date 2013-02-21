// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="UCL">
//   Open Source
// </copyright>
// <summary>
//   Defines the User type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Validator.Constraints;

namespace OccupOSMonitor.Models {
    /// <summary>
    /// The user.
    /// </summary>
    public class User : Entity {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [NotNullNotEmpty, Length(Max = 100), Email]
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [NotNullNotEmpty, Length(Max = 100)]
        public virtual string Password { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        [NotNullNotEmpty]
        public virtual string CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        [NotNullNotEmpty]
        public virtual string UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the creator id.
        /// </summary>
        public virtual int? CreatorId { get; set; }

        /// <summary>
        /// Gets or sets the updater id.
        /// </summary>
        public virtual int? UpdaterId { get; set; }
    }
}