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
        // Construction of an object must goes through DDD language, example: Reservation.Reserve
        protected internal Reservation() { }



        protected internal virtual int ReservationId { get; set; }

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



        public static int Reserve(ISession session, ReservationPersistDto reservationPersist)
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


                return r.ReservationId;
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

        // protected internal constructor shall at least enforce creating child entity through aggregate root
        // http://thatextramile.be/blog/2009/10/why-nhibernate-entities-need-a-public-or-protected-parameterless-constructor/
        protected internal ReservationAdditional() { }
    


        
        // We wanted DDD and we wanted to enforce accessing everything through aggregate root, we should use Inverse=false, and therein lies the problem;
        // Inverse=false inserts child entities with their parentid initially set to null, then are updated only after their parent is added,
        // aside from this is a performance issue, it's worrying to have an orphaned entity.
        // Unfortunately, bi-directional (Inverse=true) mapping is inevitable in NHibernate
        // http://blog.jonathanoliver.com/nhibernate-inverse-and-object-associations/
        // protected internal shall at least enforce adding a child entity through aggregate root as we cannot set/update the child's parent outside the domain if the access is protected 


        /// <summary>
        ///  !!!ALERT!!! Please don't access me directly, access me from aggregate root.
        ///  Please be DDD
        /// </summary>
        /// Sadly this doesn't work when the class is referenced from project. Would work if referenced as DLL:
        // [System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]        
        public virtual Reservation Reservation { get; protected internal set; } // we put this on top, to emphasize that this domain model is not an aggregate root


        // Ideally, there should be no Reservation reference in Value objects such as ReservationAdditional
        // However, NHibernate can't facilitate that.
        // And also, making the whole Reservation reference as protected internal is not allowed, this doesn't work:
        // protected internal virtual Reservation Reservation { get; set; } 


        


        /*         
           When Eric Evans talks about "entities have identity, Value Objects do not", he's not talking about an ID column in the database - he's talking about identity as a *concept.*
           VOs have no conceptual identity. That doesn't mean that they shouldn't have persistence identity. Don't let persistence implementation cloud your understanding of Entities vs VOs. 
         
           -- http://stackoverflow.com/questions/949320/ddd-value-objects-and-orm/955218#955218          
        */
        // ReservationAdditional is a Value Object, it has no conceptual identity, but has persistence identity.
        // We can enforce ReservationAdditional object not to have conceptual identity, by making the getter internal too
        protected internal virtual int ReservationAdditionalId { get; set; }


        public virtual string GuestName { get; protected internal set; }

        public virtual AgeBracket AgeBracket { get; protected internal set; } // Not Required but always has a default value, i.e., Adult

        public virtual Nationality Nationality { get; protected internal set; } // Required


        // See difference of Entity vs Value. When an entity becomes a Value object
    
    
    }



}
