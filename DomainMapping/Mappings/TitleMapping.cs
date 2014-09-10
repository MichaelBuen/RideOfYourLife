using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using NHibernate.Bytecode.CodeDom;
using NHibernate.Mapping.ByCode.Conformist;
using Domain;
using NHibernate.Mapping.ByCode;

namespace DomainMapping.Mappings
{
    public class TitleMapping : ClassMapping<Title>
    {
        public TitleMapping()
        {
            Cache(x => x.Usage(CacheUsage.ReadWrite));
            
            Id(x => x.TitleId, x => x.Generator(Generators.Identity) );
            Property(x => x.CreatedOn);
        }
    }

    public class TitleLocalizationMapping : ClassMapping<TitleLocalization>
    {
        const string SaveDml =
@"merge TitleLocalization as dst
using( values(?,?,?,?,?) )
    as src(TitleAbbreviation, TitleDescription, ActualLanguageCultureCode, TitleId, LanguageCultureCode)
on
    src.TitleId = dst.TitleId and src.LanguageCultureCode = dst.LanguageCultureCode

when matched then
    update set dst.TitleAbbreviation = src.TitleAbbreviation, dst.TitleDescription = src.TitleDescription

when not matched then
    insert (TitleId, LanguageCultureCode, TitleAbbreviation, TitleDescription)
    values (src.TitleId, src.LanguageCultureCode, src.TitleAbbreviation, src.TitleDescription)
;
";


        public TitleLocalizationMapping()
        {
            Cache(x => x.Usage(CacheUsage.ReadWrite));


            // Table faker. Giving the ORM an illusion that all Titles has a corresponding translation. When there's no translation, it defaults to English.
            // This proves useful when "updating" an entity which doesn't have a translation yet
            Table("dbo.GetTitleLocalization(:lf.LanguageCultureCode)");
           


            ComponentAsId(key => key.TitleLocalizationCompositeId, idMapper =>
                {
                    idMapper.Property(x => x.TitleId);
                    idMapper.Property(x => x.LanguageCultureCode);
                });

            ManyToOne(x => x.Title, mapping =>
                {
                    mapping.Column("TitleId");
                    mapping.Insert(false);
                    mapping.Update(false);
                });

            Property(x => x.LanguageCultureCode, mapping =>
                {
                    mapping.Column("LanguageCultureCode");
                    mapping.Insert(false);
                    mapping.Update(false);
                });

            Property(x => x.TitleAbbreviation);
            Property(x => x.TitleDescription);

            Property(x => x.ActualLanguageCultureCode);

            SqlInsert(SaveDml);
            SqlUpdate(SaveDml);
            
        }
    }
}
