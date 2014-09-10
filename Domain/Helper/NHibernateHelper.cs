using NHibernate;



// ReSharper disable CheckNamespace
namespace Domain
// ReSharper restore CheckNamespace
{
    public static class TheNHibernateHelper
    {
        public static ITransaction SetLanguage(this ITransaction tx, NHibernate.ISession session, string languageCultureCode)
        {

            session.EnableFilter("lf").SetParameter("LanguageCultureCode", languageCultureCode);

            return tx;
        }
    }
}
