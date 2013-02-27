// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Entity.cs" company="UCL">
//   Open Source
// </copyright>
// <summary>
//   The entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace OccupOSMonitor.Models {
    /// <summary>
    /// The entity superclass, since each Entity/Table has an Id field
    /// </summary>
    public abstract class Entity {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [NHibernate.Validator.Constraints.NotNullNotEmpty]
        public virtual int Id { get; protected set; }
    }
}