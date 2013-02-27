using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using OccupOSMonitor.Models;


namespace OccupOSMonitor.Models {


    public class AppUserMap : ClassMapping<AppUser> {

        public AppUserMap() {
            Schema("dbo");
            Lazy(false);
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.Username, map => { map.NotNullable(true); map.Unique(true); });
            Property(x => x.Email, map => { map.NotNullable(true); map.Unique(true); });
            Property(x => x.Password, map => map.NotNullable(true));
            Property(x => x.createdAt, map => map.NotNullable(true));
            Property(x => x.updatedAt, map => map.NotNullable(true));
            Property(x => x.creatorId);
            Property(x => x.updaterId);
            Property(x => x.FirstName);
            Property(x => x.LastName);
        }
    }
}
