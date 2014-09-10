using System;
using System.Collections.Generic;
using Dto;
using NHibernate;


namespace Domain.Models
{
    public enum AgeBracketId
    {
        Adult = 1, 
        Kid = 2, // 1 to 10 years old
        YoungTeenager = 3, // 11 to 16 years old
    }


    public class AgeBracket
    {
        public virtual AgeBracketId AgeBracketId { get; protected internal set; }

        public virtual int MinimumAge { get; protected internal set; }

        public virtual int MaximumAge { get; protected internal set; }

        public virtual DateTimeOffset CreatedOn { get; protected set; }


        //public static IEnumerable<AgeBracketDto> GetAgeBrackets(AgeBracketDtos request,
        //    Func<AgeBracketLocalization> adultGet,
        //    Func<AgeBracketId, AgeBracket> youngPeopleGet) 


        public static IEnumerable<AgeBracketDto> GetAgeBrackets(ISession session, AgeBracketDtos request)
        {
            
            var adult = session.Load<AgeBracketLocalization>(
                new AgeBracketLocalizationCompositeId
                    {
                        AgeBracketId = AgeBracketId.Adult,
                        LanguageCultureCode = request.LanguageCultureCode
                    });

            Func<AgeBracketId, AgeBracket> youngPeopleGet = id => session.Load<AgeBracket>(id);

            yield return new AgeBracketDto {AgeBracketId = (int) AgeBracketId.Adult, AgeIndicator = adult.AgeBracketDescription};

            var kid = youngPeopleGet(AgeBracketId.Kid);
            yield return new AgeBracketDto { AgeBracketId = (int)AgeBracketId.Kid, AgeIndicator = kid.MinimumAge + "-" + kid.MaximumAge };


            var youngTeenager = youngPeopleGet(AgeBracketId.YoungTeenager);
            yield return new AgeBracketDto { AgeBracketId = (int)AgeBracketId.YoungTeenager, AgeIndicator = youngTeenager.MinimumAge + "-" + youngTeenager.MaximumAge };

            



        }
    }



    [Serializable]
    public class AgeBracketLocalizationCompositeId
    {
        public virtual AgeBracketId AgeBracketId { get; set; }
        public virtual string LanguageCultureCode { get; set; }



        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var a = obj as AgeBracketLocalizationCompositeId;

            if (a == null)
                return false;

            if (a.AgeBracketId == this.AgeBracketId && a.LanguageCultureCode == this.LanguageCultureCode)
                return true;

            return false;
        }


        public override int GetHashCode()
        {
            return (this.AgeBracketId + "|" + this.LanguageCultureCode).GetHashCode();
        }
    }


    public class AgeBracketLocalization
    {
        AgeBracketLocalizationCompositeId _pk = new AgeBracketLocalizationCompositeId();
        protected internal virtual AgeBracketLocalizationCompositeId AgeBracketLocalizationCompositeId
        {
            get { return _pk; }
            set { _pk = value; }
        }

        AgeBracket _textResource;
        public virtual AgeBracket AgeBracket
        {
            get { return _textResource; }
            set
            {
                _textResource = value;
                _pk.AgeBracketId = _textResource.AgeBracketId;
            }
        }

        public virtual string LanguageCultureCode
        {
            get { return _pk.LanguageCultureCode; }
            set { _pk.LanguageCultureCode = value; }
        }



        public virtual string AgeBracketDescription { get; protected internal set; }


        // A guide for the user, so he/she could know the source language of the untranslated string came from
        public virtual string ActualLanguageCultureCode { get; protected internal set; }


    }

}
