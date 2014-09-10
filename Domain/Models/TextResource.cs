using System;
using Dto;
using NHibernate;


namespace Domain.Models
{
    public enum TextResourceId
    {
        PersonalInformation = 1,
        CompulsoryField = 2,
        Title = 3,
        GuestName = 4,
        
        ReservationImportant = 5, // IMPORTANT!!

        

        EmailAddress = 6,
        ContactableTelephoneOrMobileNo = 7,
        FaxNo = 8,
        Nationality = 9,
        DateOfRide = 10,
        Age = 11,



        AppName = 12, // The Singapore TEST
        ReservationForm = 13,


        TermsAndConditions = 14,

        CorrespondenceEmailAddress = 15,

        ReservationImportantMessage = 16, // Pls furnish complete e-mail address so that our reply could reach you

        Children = 17,

        ReservationDetails = 18,

        ReservationDetailsOtherMessage = 19,

        AddGuest = 20,

        Reserve = 21,
        
        RequiredWarning = 22,

        ReservationId = 23,

        MakeNewReservation = 24,

        TermsAndConditionsList = 25,

        BookingWarning = 26

    }


    public class TextResource
    {
        public virtual TextResourceId TextResourceId { get; protected internal set; }
        
        public virtual bool IsMarkdown { get; protected internal set; }

        public virtual DateTimeOffset CreatedOn { get; protected internal set; }

        public static ReservationResourceDto GetReservationResource(ISession session, ReservationResourceDto request)
        {
            using (var tx = session.BeginTransaction().SetLanguage(session, request.LanguageCultureCode))
            {

                Func<TextResourceId, string> getResourceText =
                    id => session.Get<TextResourceLocalization>(new TextResourceLocalizationCompositeId
                        {
                            TextResourceId = id,
                            LanguageCultureCode = request.LanguageCultureCode
                        }).TextResourceValue;


              

                return new ReservationResourceDto
                    {
                        AppName = getResourceText(TextResourceId.AppName),

                        ReservationId = getResourceText(TextResourceId.ReservationId),

                        ReservationForm = getResourceText(TextResourceId.ReservationForm),

                        Title = getResourceText(TextResourceId.Title),
                        GuestName = getResourceText(TextResourceId.GuestName),

                        PersonalInformation = getResourceText(TextResourceId.PersonalInformation),

                        CompulsoryField = getResourceText(TextResourceId.CompulsoryField),

                        ReservationImportant = getResourceText(TextResourceId.ReservationImportant),
                        ReservationImportantMessage = getResourceText(TextResourceId.ReservationImportantMessage),

                        EmailAddress = getResourceText(TextResourceId.EmailAddress),

                        CorrespondenceEmailAddress = getResourceText(TextResourceId.CorrespondenceEmailAddress),

                        ContactableTelephoneOrMobileNumber = getResourceText(TextResourceId.ContactableTelephoneOrMobileNo),

                        FaxNumber = getResourceText(TextResourceId.FaxNo),

                        Nationality = getResourceText(TextResourceId.Nationality),

                        DateOfRide = getResourceText(TextResourceId.DateOfRide),

                        ReservationDetails = getResourceText(TextResourceId.ReservationDetails),

                        ReservationDetailsOtherMessage = getResourceText(TextResourceId.ReservationDetailsOtherMessage),

                        Age = getResourceText(TextResourceId.Age),

                        Children = getResourceText(TextResourceId.Children),

                        AddGuest = getResourceText(TextResourceId.AddGuest),

                        Reserve = getResourceText(TextResourceId.Reserve),


                        RequiredWarning = getResourceText(TextResourceId.RequiredWarning),

                        MakeNewReservation = getResourceText(TextResourceId.MakeNewReservation),

                        TermsAndConditions = getResourceText(TextResourceId.TermsAndConditions),

                        TermsAndConditionsList = getResourceText(TextResourceId.TermsAndConditionsList),

                        BookingWarning = getResourceText(TextResourceId.BookingWarning),
                    }; 
            }
            
        } // GetReservationResource

    }//TextResource class


    [Serializable]
    public class TextResourceLocalizationCompositeId
    {
        public virtual TextResourceId TextResourceId { get; set; }
        public virtual string LanguageCultureCode { get; set; }



        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var t = obj as TextResourceLocalizationCompositeId;

            if (t == null)
                return false;

            if (t.TextResourceId == this.TextResourceId && t.LanguageCultureCode == this.LanguageCultureCode)
                return true;

            return false;
        }


        public override int GetHashCode()
        {
            return (this.TextResourceId + "|" + this.LanguageCultureCode).GetHashCode();
        }
    }


    public class TextResourceLocalization
    {
        TextResourceLocalizationCompositeId _pk = new TextResourceLocalizationCompositeId();
        protected internal virtual TextResourceLocalizationCompositeId TextResourceLocalizationCompositeId
        {
            get { return _pk; }
            set { _pk = value; }
        }

        TextResource _textResource;
        public virtual TextResource TextResource
        {
            get { return _textResource; }
            protected internal set
            {
                _textResource = value;
                _pk.TextResourceId = _textResource.TextResourceId;
            }
        }

        public virtual string LanguageCultureCode
        {
            get { return _pk.LanguageCultureCode; }
            protected internal set { _pk.LanguageCultureCode = value; }
        }



        public virtual string TextResourceValue { get; protected internal set; }
        

        // A guide for the user, so he/she could know the source language of the untranslated string came from
        public virtual string ActualLanguageCultureCode { get; protected internal set; }


    }

}
