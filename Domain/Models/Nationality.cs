using System;
using System.Collections.Generic;
using System.Linq;
using Dto;
using NHibernate;
using NHibernate.Linq;


namespace Domain.Models
{
    public class Nationality
    {
        public virtual int NationalityId { get; protected internal set; }

        public virtual DateTimeOffset CreatedOn { get; set; }

        public static IEnumerable<NationalityDto> GetNationalities(ISession session, NationalityDtos request)
        {

            string languageCultureCode = request.LanguageCultureCode;


            using (var tx = session.BeginTransaction().SetLanguage(session, request.LanguageCultureCode))
            {

                Func<int, NationalityLocalization> requestNatLocal = id =>
                                                                     session.Get<NationalityLocalization>(new NationalityLocalizationCompositeId
                                                                         {
                                                                             NationalityId = id,
                                                                             LanguageCultureCode =
                                                                                 request.LanguageCultureCode
                                                                         });


                var nationalityLocalizations = session.Query<NationalityLocalization>();

                if (!request.Ids.Any() || request.Ids.Count() > 1)
                {
                    
                    var q = from nl in nationalityLocalizations
                            where nl.LanguageCultureCode == languageCultureCode
                            select new NationalityDto() { NationalityId = nl.Nationality.NationalityId, NationalityName = nl.NationalityName };

                    if (request.Ids.Count() > 1)
                        q = q.Where(x => request.Ids.Contains(x.NationalityId));


                    q = q.Cacheable();


                    return q;
                }
                else
                {
                    // cache-friendly retrieval

                    int nationalityId = request.Ids[0];


                    var l = requestNatLocal(nationalityId);


                    var list = new[] { new NationalityDto { NationalityId = nationalityId, NationalityName = l.NationalityName } };

                    return list;

                } 
            }
            
        }
    }



    [Serializable]
    public class NationalityLocalizationCompositeId
    {
        public virtual int NationalityId { get; set; }
        public virtual string LanguageCultureCode { get; set; }



        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var a = obj as NationalityLocalizationCompositeId;

            if (a == null)
                return false;

            if (a.NationalityId == this.NationalityId && a.LanguageCultureCode == this.LanguageCultureCode)
                return true;

            return false;
        }


        public override int GetHashCode()
        {
            return (this.NationalityId + "|" + this.LanguageCultureCode).GetHashCode();
        }
    }


    public class NationalityLocalization
    {
        NationalityLocalizationCompositeId _pk = new NationalityLocalizationCompositeId();
        protected internal virtual NationalityLocalizationCompositeId NationalityLocalizationCompositeId
        {
            get { return _pk; }
            set { _pk = value; }
        }

        Nationality _nationality;
        public virtual Nationality Nationality
        {
            get { return _nationality; }
            set
            {
                _nationality = value;
                _pk.NationalityId = _nationality.NationalityId;
            }
        }

        public virtual string LanguageCultureCode
        {
            get { return _pk.LanguageCultureCode; }
            set { _pk.LanguageCultureCode = value; }
        }



        public virtual string NationalityName { get; protected internal set; }


        // A guide for the user, so he/she could know the source language of the untranslated string came from
        public virtual string ActualLanguageCultureCode { get; protected internal set; }


    }

}
