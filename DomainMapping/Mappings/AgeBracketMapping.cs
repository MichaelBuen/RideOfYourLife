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
    public class AgeBracketMapping : ClassMapping<AgeBracket>
    {
        public AgeBracketMapping()
        {
            Cache(x => x.Usage(CacheUsage.ReadWrite));

            Id(x => x.AgeBracketId);

            Property(x => x.MinimumAge);
            Property(x => x.MaximumAge);
            Property(x => x.CreatedOn);
        }
    }

    public class AgeBracketLocalizationMapping : ClassMapping<AgeBracketLocalization>
    {
        const string SaveDml =
@"merge AgeBracketLocalization as dst
using( values(?,?,?,?) )
    as src(AgeBracketDescription, ActualLanguageCultureCode, AgeBracketId, LanguageCultureCode)
on
    src.AgeBracketId = dst.AgeBracketId and src.LanguageCultureCode = dst.LanguageCultureCode

when matched then
    update set dst.TextResourceValue = src.TextResourceValue

when not matched then
    insert (AgeBracketId, LanguageCultureCode, AgeBracketDescription)
    values (src.AgeBracketId, src.LanguageCultureCode, src.AgeBracketDescription)
;
";


        public AgeBracketLocalizationMapping()
        {
            // Table faker. Giving the ORM an illusion that all Titles has a corresponding translation. When there's no translation, it defaults to English.
            // This proves useful when "updating" an entity which doesn't have a translation yet
            Table("dbo.GetAgeBracketLocalization(:lf.LanguageCultureCode)");


            Cache(x => x.Usage(CacheUsage.ReadWrite));

            ComponentAsId(key => key.AgeBracketLocalizationCompositeId, idMapper =>
            {
                idMapper.Property(x => x.AgeBracketId);
                idMapper.Property(x => x.LanguageCultureCode);
            });

            ManyToOne(x => x.AgeBracket, mapping =>
            {
                mapping.Column("AgeBracketId");
                mapping.Insert(false);
                mapping.Update(false);
            });

            Property(x => x.LanguageCultureCode, mapping =>
            {
                mapping.Column("LanguageCultureCode");
                mapping.Insert(false);
                mapping.Update(false);
            });

            Property(x => x.AgeBracketDescription);

            Property(x => x.ActualLanguageCultureCode);

            SqlInsert(SaveDml);
            SqlUpdate(SaveDml);

        }
    }
}
