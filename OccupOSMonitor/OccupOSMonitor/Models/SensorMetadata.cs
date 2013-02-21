// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SensorMetadata.cs" company="UCL">
// Open Source
// </copyright>
// <summary>
//   Defines the SensorMetadata type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace OccupOSMonitor.Models {
    /// <summary>
    /// The sensor metadata.
    /// </summary>
    public class SensorMetadata : Entity {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [NHibernate.Validator.Constraints.NotNullNotEmpty]
        public virtual Guid Id { get; protected set; }
    }
}