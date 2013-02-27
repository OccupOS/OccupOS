using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using OccupOSMonitor.Models;


namespace OccupOSMonitor.Models {


    public class HwControllerMetadataMap : ClassMapping<HwControllerMetadata> {

        public HwControllerMetadataMap() {
            Table("HwControllerMetadata");
            Schema("dbo");
            Lazy(false);
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.ExternalId, map => { map.NotNullable(true); map.Unique(true); });
            Property(x => x.DepartmentName);
            Property(x => x.BuildingName);
            Property(x => x.UpdatedAt, map => map.NotNullable(true));
            Property(x => x.CreatedAt, map => map.NotNullable(true));
            Property(x => x.UpdaterId);
            Property(x => x.CreatorId);
            Property(x => x.FloorNr);
            Property(x => x.RoomId, map => map.NotNullable(true));
            Bag<SensorData>(x => x.SensorDatas, colmap => { colmap.Key(x => x.Column("IntermediateHwMedadataId")); }, map => { map.OneToMany(x => x.Class(typeof(SensorData))); });
            Bag<SensorMetadata>(x => x.SensorMetadatas, colmap => { colmap.Key(x => x.Column("HwControllerMetadataId")); }, map => { map.OneToMany(x => x.Class(typeof(SensorMetadata))); });
        }
    }
}
