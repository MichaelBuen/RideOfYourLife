using Domain;
using Domain.Models;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainMapping.Mappings
{
    public class ReservationMapping : ClassMapping<Reservation>
    {
        public ReservationMapping()
        {
            Cache(x => x.Usage(CacheUsage.ReadWrite));

            Id(x => x.ReservationId, x => x.Generator(Generators.Identity));

            ManyToOne(x => x.Title, k => k.Column("TitleId"));

            Property(x => x.GuestName);

            Property(x => x.EmailAddress);

            Property(x => x.ContactNumber);

            Property(x => x.FaxNumber);

            ManyToOne(x => x.Nationality, k => k.Column("NationalityId"));

            Property(x => x.DateOfRide);

            Property(x => x.CreatedOn);

            //Bag<ReservationAdditional>(
            //    "_reservationAdditionals",
            //    rel =>
            //        {
            //            rel.Cache(x => x.Usage(CacheUsage.ReadWrite));
            //            rel.Table("ReservationAdditional");
            //            rel.Access(Accessor.Field);
            //            rel.Key(k => k.Column("ReservationId"));
            //            rel.Inverse(true);
            //        },
            //    relType => relType.OneToMany()
            //    );

            Bag(x => x.ReservationAdditionals,               
                rel =>
                {
                    rel.Cache(x => x.Usage(CacheUsage.ReadWrite));                                    
                    rel.Key(k => k.Column("ReservationId"));
                    rel.Inverse(true);
                    rel.Cascade(Cascade.All| Cascade.DeleteOrphans);
                },
                relType => relType.OneToMany()
                );
        }
    }


    public class ReservationAdditionalMapping : ClassMapping<ReservationAdditional>
    {
        public ReservationAdditionalMapping()
        {
            // Should set this, even ReservationAdditional is a child entity. http://ayende.com/blog/4046/nhibernate-beware-of-inadvisably-applied-caching-strategies
            Cache(x => x.Usage(CacheUsage.ReadWrite));

            ManyToOne(x => x.Reservation, x => x.Column("ReservationId"));            

            Id(x => x.ReservationAdditionalId, x => x.Generator(Generators.Identity));

            Property(x => x.GuestName);

            ManyToOne(x => x.AgeBracket, k => k.Column("AgeBracketId"));

            ManyToOne(x => x.Nationality, k => k.Column("NationalityId"));

        }
    }
}
