using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using OccupOSMonitor.Models;


namespace OccupOSMonitor.Models {


    public class SensorDataMap : ClassMapping<SensorData> {

        public SensorDataMap() {
            //Table("SensorData");
            //Schema("dbo");
            //Lazy(false);
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.MeasuredData, map => map.NotNullable(true));
            Property(x => x.MeasuredAt, map => map.NotNullable(true));
            Property(x => x.SendAt);
            Property(x => x.PolledAt);
            Property(x => x.UpdatedAt, map => map.NotNullable(true));
            Property(x => x.CreatedAt, map => map.NotNullable(true));
            ManyToOne(x => x.SensorMetadata, map => { map.Column("SensorMetadataId"); map.PropertyRef("Id"); });

            ManyToOne(x => x.HwControllerMetadata, map => { map.Column("IntermediateHwMedadataId"); map.PropertyRef("Id"); });


        }
    }
}
