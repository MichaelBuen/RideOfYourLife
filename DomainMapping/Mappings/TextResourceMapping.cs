using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using NHibernate.Mapping.ByCode.Conformist;
using Domain;
using NHibernate.Mapping.ByCode;

namespace DomainMapping.Mappings
{
    public class TextResourceMapping : ClassMapping<TextResource>
    {
        public TextResourceMapping()
        {
            Cache(x => x.Usage(CacheUsage.ReadWrite));

            Id(x => x.TextResourceId);

            Property(x => x.IsMarkdown);
            Property(x => x.CreatedOn);
        }
    }

    public class TextResourceLocalizationMapping : ClassMapping<TextResourceLocalization>
    {
        const string SaveDml =
@"merge TextResourceLocalization as dst
using( values(?,?,?,?) )
    as src(TextResourceValue, ActualLanguageCultureCode, TextResourceId, LanguageCultureCode)
on
    src.TextResourceId = dst.TextResourceId and src.LanguageCultureCode = dst.LanguageCultureCode

when matched then
    update set dst.TextResourceValue = src.TextResourceValue

when not matched then
    insert (TextResourceId, LanguageCultureCode, TextResourceValue)
    values (src.TextResourceId, src.LanguageCultureCode, src.TextResourceValue)
;
";


        public TextResourceLocalizationMapping()
        {
            // Table faker. Giving the ORM an illusion that all Titles has a corresponding translation. When there's no translation, it defaults to English.
            // This proves useful when "updating" an entity which doesn't have a translation yet
            Table("dbo.GetTextResourceLocalization(:lf.LanguageCultureCode)");


            Cache(x => x.Usage(CacheUsage.ReadWrite));

            ComponentAsId(key => key.TextResourceLocalizationCompositeId, idMapper =>
            {
                idMapper.Property(x => x.TextResourceId);
                idMapper.Property(x => x.LanguageCultureCode);
            });

            ManyToOne(x => x.TextResource, mapping =>
            {
                mapping.Column("TextResourceId");
                mapping.Insert(false);
                mapping.Update(false);
            });

            Property(x => x.LanguageCultureCode, mapping =>
            {
                mapping.Column("LanguageCultureCode");
                mapping.Insert(false);
                mapping.Update(false);
            });

            Property(x => x.TextResourceValue);            

            Property(x => x.ActualLanguageCultureCode);

            SqlInsert(SaveDml);
            SqlUpdate(SaveDml);

        }
    }
}
