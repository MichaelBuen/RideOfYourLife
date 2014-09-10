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
    public class NationalityMapping : ClassMapping<Nationality>
    {
        public NationalityMapping()
        {
            Cache(x => x.Usage(CacheUsage.ReadWrite));

            Id(x => x.NationalityId, x => x.Generator(Generators.Identity));
            
            Property(x => x.CreatedOn);
        }
    }

    public class NationalityLocalizationMapping : ClassMapping<NationalityLocalization>
    {
        const string SaveDml =
@"merge NationalityLocalization as dst
using( values(?,?,?,?) )
    as src(NationalityName, ActualLanguageCultureCode, NationalityId, LanguageCultureCode)
on
    src.NationalityId = dst.NationalityId and src.LanguageCultureCode = dst.LanguageCultureCode

when matched then
    update set dst.TextResourceValue = src.TextResourceValue

when not matched then
    insert (NationalityId, LanguageCultureCode, NationalityName)
    values (src.NationalityId, src.LanguageCultureCode, src.NationalityName)
;
";


        public NationalityLocalizationMapping()
        {
            // Table faker. Giving the ORM an illusion that all Titles has a corresponding translation. When there's no translation, it defaults to English.
            // This proves useful when "updating" an entity which doesn't have a translation yet
            Table("dbo.GetNationalityLocalization(:lf.LanguageCultureCode)");


            Cache(x => x.Usage(CacheUsage.ReadWrite));

            ComponentAsId(key => key.NationalityLocalizationCompositeId, idMapper =>
            {
                idMapper.Property(x => x.NationalityId);
                idMapper.Property(x => x.LanguageCultureCode);
            });

            ManyToOne(x => x.Nationality, mapping =>
            {
                mapping.Column("NationalityId");
                mapping.Insert(false);
                mapping.Update(false);
            });

            Property(x => x.LanguageCultureCode, mapping =>
            {
                mapping.Column("LanguageCultureCode");
                mapping.Insert(false);
                mapping.Update(false);
            });

            Property(x => x.NationalityName);

            Property(x => x.ActualLanguageCultureCode);

            SqlInsert(SaveDml);
            SqlUpdate(SaveDml);

        }
    }
}
