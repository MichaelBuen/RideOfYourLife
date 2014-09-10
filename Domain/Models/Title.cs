using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Dto;
using NHibernate;
using NHibernate.Linq;

[assembly: InternalsVisibleTo("DomainMapping")]



namespace Domain.Models
{
    public class Title
    {
        public virtual int TitleId { get; protected internal set; }
        public virtual DateTimeOffset CreatedOn { get; protected set; }

        public static TitleLocalization MakeTitle(ISession session, string languageCultureCode, string titleAbbreviation, string titleDescription)
        {

            using (var tx = session.BeginTransaction().SetLanguage(session, languageCultureCode))
            {
                var t = new Title {CreatedOn = DateTime.UtcNow};
                session.Save(t);


                var l = new TitleLocalization
                    {
                        LanguageCultureCode = languageCultureCode,
                        Title = t,
                        TitleAbbreviation = titleAbbreviation,
                        TitleDescription = titleDescription
                    };


                session.Save(l);

                tx.Commit(); ;

                return l;
            }


        }

      
        public static IEnumerable<TitleDto> GetTitles(ISession session, TitleDtos request)
        {


            using (var tx = session.BeginTransaction().SetLanguage(session, request.LanguageCultureCode))
            {

                var titleLocalizations = session.Query<TitleLocalization>();

                if (!request.Ids.Any() || request.Ids.Count() > 1)
                {
                    var q = from tl in titleLocalizations
                            where tl.LanguageCultureCode == request.LanguageCultureCode
                            select new TitleDto { TitleId = tl.Title.TitleId, Abbrev = tl.TitleAbbreviation };

                    if (request.Ids.Count() > 1)
                        q = q.Where(x => request.Ids.Contains(x.TitleId));


                    q = q.Cacheable();


                    return q;
                }
                else
                {
                    // cache-friendly retrieval

                    int titleId = request.Ids[0];


                    var t = session.Get<Title>(titleId);


                    var l = 
                        session.Get<TitleLocalization>(new TitleLocalizationCompositeId
                            {
                                TitleId = titleId,
                                LanguageCultureCode = request.LanguageCultureCode
                            });



                    var list = new[] { new TitleDto { TitleId = titleId, Abbrev = l.TitleAbbreviation } };

                    return list.AsQueryable();

                } 
            }
            
        }
    }

    [Serializable]
    public class TitleLocalizationCompositeId
    {
        public virtual int TitleId { get; set; }
        public virtual string LanguageCultureCode { get; set; }



        public override bool Equals(object obj)
        {
            if (obj == null) 
                return false;

            var t = obj as TitleLocalizationCompositeId;
            
            if (t == null) 
                return false;

            if (t.TitleId == this.TitleId && t.LanguageCultureCode == this.LanguageCultureCode)
                return true;

            return false;
        }


        public override int GetHashCode()
        {
            return (this.TitleId + "|" + this.LanguageCultureCode).GetHashCode();
        }
    }


    public class TitleLocalization
    {
        TitleLocalizationCompositeId _pk = new TitleLocalizationCompositeId();
        protected internal virtual TitleLocalizationCompositeId TitleLocalizationCompositeId 
        { 
            get { return _pk;  }
            set { _pk = value; }
        }

        Title _title;
        public virtual Title Title
        {
            get { return _title; }
            protected internal set
            {
                _title = value;
                _pk.TitleId = _title.TitleId;
            }
        }

        public virtual string LanguageCultureCode 
        {
            get { return _pk.LanguageCultureCode;  }
            protected internal set { _pk.LanguageCultureCode = value; }
        }



        public virtual string TitleAbbreviation { get; protected internal set; }
        public virtual string TitleDescription { get; protected internal set; }


        // A guide for the user, so he/she could know the source language of the untranslated string came from
        public virtual string ActualLanguageCultureCode { get; protected internal set; }


    }



    // http://www.ienablemuch.com/2013/12/pragmatic-ddd.html

    public static class TitleDomainExtension
    {
        public static void ChangeTheAbbrev(this Title t, TitleLocalization tl, string languageCultureCode, string newAbbrev)
        {
            
                tl.TitleAbbreviation = newAbbrev;

        }

        //public static void ChangeTheAbbrevAndDescription(this TitleLocalization tl, NHibernate.ISession session, string languageCultureCode,
        //                                   string newAbbrev, string newDescription)
        //{
        //    using (var tx = session.BeginTransaction().SetLanguage(session, languageCultureCode))
        //    {
                
        //        tl.TitleAbbreviation = newAbbrev;
        //        tl.TitleDescription = newDescription;

        //        session.Save(tl);
        //        session.Flush();

        //        tx.Commit();
        //    }

        //}

    }
}
