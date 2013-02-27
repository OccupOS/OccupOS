using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using OccupOSMonitor.Models;
using NHibernate.Linq;


namespace OccupOSMonitor.Models {


    public class SensorMetadataMap : ClassMapping<SensorMetadata> {

        public SensorMetadataMap() {
            Table("SensorMetadata");
            Schema("dbo");
            Lazy(false);
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.ExternalId, map => { map.NotNullable(true); map.Unique(true); });
            Property(x => x.SensorName);
            Property(x => x.RoomId, map => map.NotNullable(true));
            Property(x => x.FloorNr);
            Property(x => x.GeoLongitude);
            Property(x => x.GeoLatidude);
            Property(x => x.UpdatedAt, map => map.NotNullable(true));
            Property(x => x.CreatedAt, map => map.NotNullable(true));
            Property(x => x.UpdaterId);
            Property(x => x.CreatorId);
            ManyToOne(x => x.HwControllerMetadata, map => { map.Column("HwControllerMetadataId"); map.PropertyRef("Id"); });

            Bag<SensorData>(x => x.SensorDatas, colmap => { colmap.Key(x => x.Column("SensorMetadataId")); }, map => { map.OneToMany(x => x.Class(typeof(SensorData))); });
        }
    }
}
