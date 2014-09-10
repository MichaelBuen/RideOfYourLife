using System;
using System.Collections.Generic;
using System.Linq;
using Dto;
using NHibernate;
using System.Globalization;

namespace Domain.Models
{
    public class Reservation
    {
        public virtual int ReservationId { get; protected internal set; }

        public virtual Title Title { get; protected internal set; } // Not Required but always has a default value, i.e., Mr

        public virtual string GuestName { get; protected internal set; }

        public virtual string EmailAddress { get; protected internal set; }
        
        public virtual string ContactNumber { get; protected internal set; }

        public virtual string FaxNumber { get; protected internal set; }

        public virtual Nationality Nationality { get; protected internal set; } // Required

        public virtual DateTime? DateOfRide { get; protected internal set; }

        public virtual DateTimeOffset CreatedOn { get; protected internal set; }


        //List<ReservationAdditional> _reservationAdditionals = new List<ReservationAdditional>();
        //public virtual IList<ReservationAdditional> ReservationAdditionals { get { return this._reservationAdditionals; } }


        IList<ReservationAdditional> _reservationAdditionals = new List<ReservationAdditional>();
        public virtual IList<ReservationAdditional> ReservationAdditionals
        {
            get { return _reservationAdditionals; }
            protected internal set { _reservationAdditionals = value; }
        }


        public virtual void AddAdditionalReservation(ReservationAdditional additional)
        {
            // additional.Reservation = this;
            this.ReservationAdditionals.Add(additional);
        }



        public static Reservation Reserve(ISession session, ReservationPersistDto reservationPersist)
        {
            // We can do validation, business rules and business logic here

            using (var tx = session.BeginTransaction())
            {

                var r = new Reservation();

                r.GuestName = reservationPersist.GuestName;

                r.Title = new Title { TitleId = reservationPersist.TitleId }; // Mr.
                // session.Load<Title>(reservationPersist.TitleId); // this method of assigning stub id works too just like the above

                r.EmailAddress = reservationPersist.EmailAddress;
                r.ContactNumber = reservationPersist.ContactNumber ?? "";
                r.FaxNumber = reservationPersist.FaxNumber ?? "";
                r.Nationality = new Nationality { NationalityId = reservationPersist.NationalityId };
                r.DateOfRide = reservationPersist.DateOfRide;
                r.CreatedOn = DateTime.UtcNow;


                foreach (var additional in reservationPersist.ReservationAdditionals)
                {
                    var a = new ReservationAdditional
                        {
                            Reservation = r,
                            GuestName = additional.GuestName,
                            AgeBracket = new AgeBracket { AgeBracketId = (AgeBracketId)additional.AgeBracketId },
                            Nationality = new Nationality { NationalityId = additional.NationalityId }
                        };

                    r.ReservationAdditionals.Add(a);

                }




                session.Save(r);
                tx.Commit();


                return r;
            }

        }




        public static ReservationViewDto GetViewable(ISession session, ReservationViewDto request)
        {
            using (var tx = session.BeginTransaction().SetLanguage(session, request.LanguageCultureCode))
            {

                var reservation = session.Get<Reservation>(request.ReservationId);

                string languageCultureCode = request.LanguageCultureCode;

                var dto = new ReservationViewDto
                    {
                        ReservationId = request.ReservationId,
                        LanguageCultureCode = request.LanguageCultureCode,

                        Title =
                            session.Get<TitleLocalization>(
                                new TitleLocalizationCompositeId
                                {
                                    TitleId = reservation.Title.TitleId,
                                    LanguageCultureCode = languageCultureCode
                                }).TitleAbbreviation,

                        GuestName = reservation.GuestName,

                        EmailAddress = reservation.EmailAddress,

                        FaxNumber = reservation.FaxNumber,

                        ContactNumber = reservation.ContactNumber,

                        DateOfRide = reservation.DateOfRide == null ? "" : reservation.DateOfRide.Value.ToString("D",new CultureInfo(request.LanguageCultureCode)),
                            


                        Nationality =
                            session.Get<NationalityLocalization>(
                                new NationalityLocalizationCompositeId
                                    {
                                        NationalityId = reservation.Nationality.NationalityId,
                                        LanguageCultureCode = languageCultureCode
                                    }).NationalityName,




                        ReservationAdditionals =
                            reservation.ReservationAdditionals.Select(x =>
                                new ReservationAdditionalViewDto
                                {
                                    GuestName = x.GuestName,

                                    AgeBracket =
                                        x.AgeBracket.AgeBracketId == AgeBracketId.Adult ?

                                        session.Get<AgeBracketLocalization>(
                                            new AgeBracketLocalizationCompositeId { AgeBracketId = x.AgeBracket.AgeBracketId, LanguageCultureCode = languageCultureCode }
                                        ).AgeBracketDescription

                                        :

                                        // no one will live in a thousand years, just testing the culture settings
                                        string.Format(new CultureInfo(languageCultureCode), "{0:n0}-{1:n0}", x.AgeBracket.MinimumAge, x.AgeBracket.MinimumAge),

                                    Nationality =
                                        session.Get<NationalityLocalization>(
                                            new NationalityLocalizationCompositeId { NationalityId = x.Nationality.NationalityId, LanguageCultureCode = languageCultureCode}
                                            ).NationalityName
                                })
                    };


                
                return dto;

                #region -- This is a test view
                //var dto = new ReservationViewDto
                //    {
                //        Id = request.Id,
                //        LanguageCultureCode = request.LanguageCultureCode,

                //        Title =
                //            "",

                //        DateOfRide = "blah",
                //        ReservationViewResultDtos = new[]
                //            {
                //                new ReservationAdditionalViewDto
                //                    {
                //                        GuestName = "Laibin",
                //                        AgeBracket = "Adult",
                //                        Nationality = "Filipino"
                //                    }
                //            }
                //    };
                #endregion

            }
        }
    } // class Reservation


    public class ReservationAdditional
    {
        // We wanted DDD and we wanted to enforce accessing everything through aggregate root, we should use Inverse=false, and therein lies the problem;
        // Inverse=false inserts child entities with their parentid initially set to null, then are updated only after their parent is added,
        // aside from this is a performance issue, it's worrying to have an orphaned entity
        // Unfortunately, bi-directional (Inverse=true) mapping is inevitable in NHibernate
        // http://blog.jonathanoliver.com/nhibernate-inverse-and-object-associations/
        // protected internal shall at least enforce adding a child entity through aggregate root as we cannot set/update the child's parent outside the domain if the access is protected 
        public virtual Reservation Reservation { get; protected internal set; } // we put this on top, to emphasize that this domain model is not an aggregate root



        public virtual int ReservationAdditionalId { get; protected internal set; }

        public virtual string GuestName { get; protected internal set; }

        public virtual AgeBracket AgeBracket { get; protected internal set; } // Not Required but always has a default value, i.e., Adult

        public virtual Nationality Nationality { get; protected internal set; } // Required


        // protected internal constructor shall at least enforce creating child entity through aggregate root
        // http://thatextramile.be/blog/2009/10/why-nhibernate-entities-need-a-public-or-protected-parameterless-constructor/
        protected internal ReservationAdditional() { }
    }
}
