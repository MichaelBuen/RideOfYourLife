using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Cfg;

using DomainMapping.Mappings;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Type;

namespace DomainMapping.SessionMapper
{
    public static class Mapper
    {

        static NHibernate.ISessionFactory _sessionFactory = Mapper.GetSessionFactory();

        public static NHibernate.ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }

        static NHibernate.ISessionFactory GetSessionFactory()
        {
            var mapper = new NHibernate.Mapping.ByCode.ModelMapper();


            //mapper.AddMappings(new[]
            //    {
            //        typeof(TitleMapping), 
            //        typeof(TitleLocalizationMapping), 
                    
            //        typeof(TextResourceMapping), 
            //        typeof(TextResourceLocalizationMapping),
                    
            //        typeof(NationalityMapping),
            //        typeof(NationalityLocalizationMapping),

            //        typeof(AgeBracketMapping),
            //        typeof(AgeBracketLocalizationMapping),

            //        typeof(ReservationMapping),
            //        typeof(ReservationAdditionalMapping)
            //    });


            mapper.AddMappings
                (
                    typeof(Mapper).Assembly.GetTypes()
                        .Where(x => x.BaseType.IsGenericType
                                    && x.BaseType.GetGenericTypeDefinition() == typeof(NHibernate.Mapping.ByCode.Conformist.ClassMapping<>))
                );


            var cfg = new NHibernate.Cfg.Configuration();
            cfg.DataBaseIntegration(c =>
                {
                    c.Driver<NHibernate.Driver.SqlClientDriver>();
                    c.Dialect<NHibernate.Dialect.MsSql2008Dialect>();
                    c.ConnectionString = "Server=.;Database=RideOfYourLife;Trusted_Connection=True";

                    c.LogSqlInConsole = true;
                    c.LogFormattedSql = true;                    
                });

            var domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            cfg.AddMapping(domainMapping);

            
            var filterDef = new NHibernate.Engine.FilterDefinition("lf", /*default condition*/ null,
                                                                   new Dictionary<string, IType>
                                                                       {
                                                                           { "LanguageCultureCode", NHibernateUtil.String}
                                                                       }, useManyToOne: false);
            
            cfg.AddFilterDefinition(filterDef);


            cfg.Cache(x =>
            {
                x.Provider<NHibernate.Caches.SysCache.SysCacheProvider>();

                // http://stackoverflow.com/questions/2365234/how-does-query-caching-improves-performance-in-nhibernate

                // Need to be explicitly turned on so the .Cacheable directive on Linq will work:                    
                x.UseQueryCache = true;
            });



            var sf = cfg.BuildSessionFactory();


            //using (var file = new System.IO.FileStream(@"c:\x\ddl.txt",
            //       System.IO.FileMode.Create,
            //       System.IO.FileAccess.ReadWrite))
            //using (var sw = new System.IO.StreamWriter(file))
            //{
            //    new SchemaUpdate(cfg)
            //        .Execute(sw.Write, false);
            //}

            return sf;
        }
    }

    


}
