using System;
using System.Collections.Generic;
using System.Diagnostics;
using Domain.Models;
using Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainMapping.SessionMapper;

using System.Linq;
using NHibernate.Linq;

using System.Diagnostics;


using ServiceStack.Text; // use for .Dump()ing JSON


namespace Domain.Test
{
    [TestClass]
    public class TheUnitTest
    {

        static TheUnitTest()
        {
            
        }

        // [TestMethod]
        public void TestMethod1()
        {
            var os = Mapper.SessionFactory.OpenSession();            
        }



        [TestMethod]
        public void List_All_Titles()
        {
            using (var session = Mapper.SessionFactory.OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, "zh"))
            {
                var list = from t in session.Query<TitleLocalization>().Fetch(x => x.Title)
                           where t.LanguageCultureCode == "zh"
                           select t;

                string s = list.ToList().Dump();

                Debug.WriteLine(s);


            }
        }


        

       
        [TestMethod]
        public void Test_TextResource()
        {
            string languageCultureCode = "en";

            using (var session = Mapper.SessionFactory.OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCultureCode))
            {
                var list = from t in session.Query<TextResource>()                                                      
                           select t;

                string s = list.ToList().Dump();

                Debug.WriteLine(s);

            }
        }

        [TestMethod]
        public void Test_TextResource_Localization()
        {
            string languageCultureCode = "en";

            using (var session = Mapper.SessionFactory.OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCultureCode))
            {
                var list = from t in session.Query<TextResourceLocalization>().Fetch(x => x.TextResource)
                           select t;

                string s = list.ToList().Dump();

                Debug.WriteLine(s);

            }
        }

        [TestMethod]
        public void Test_Titles_GetAllTitlesDto()
        {
            string languageCultureCode = "en";

            using (var session = Mapper.SessionFactory.OpenSession())            
            {
                var q = session.Query<TitleLocalization>();
                var list = Title.GetTitles(session, new TitleDtos { LanguageCultureCode = "en"});
                Debug.WriteLine(list.Dump());
            }
        }


        [TestMethod]
        public void Test_Titles_GetAllTitlesDtoSelectedNumbers()
        {
            string languageCultureCode = "en";

            using (var session = Mapper.SessionFactory.OpenSession())            
            {
                var q = session.Query<TitleLocalization>();
                var list = Title.GetTitles(session, new TitleDtos { Ids = new[] { 1, 3 }, LanguageCultureCode = languageCultureCode});
                Debug.WriteLine(list.Dump());
            }
        }


        [TestMethod]
        public void Test_Titles_GetOneTitleDto()
        {
            string languageCultureCode = "en";

            using (var session = Mapper.SessionFactory.OpenSession())
            {
                var q = session.Query<TitleLocalization>();
                var list = Title.GetTitles(session, new TitleDtos { Ids = new[] { 1 }, LanguageCultureCode = languageCultureCode});

                Debug.WriteLine(list.Dump());
            }
        }


        [TestMethod]
        public void Test_Titles_Add()
        {
            string languageCultureCode = "en";

            using (var session = Mapper.SessionFactory.OpenSession())                      
            {
                TitleLocalization tl = Title.MakeTitle(session, languageCultureCode, ("UT-" + Guid.NewGuid()).Substring(0,10), "Doctor");   
                                
            }
        }



        [TestMethod]
        public void Test_Title_ChangeAbbrev()
        {
            string languageCultureCode = "en";
            using (var session = Mapper.SessionFactory.OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCultureCode))
            {
                var title = session.Load<Title>(1);
                var tl =
                    session.Load<TitleLocalization>(new TitleLocalizationCompositeId
                    {
                        TitleId = title.TitleId,
                        LanguageCultureCode = languageCultureCode
                    });


                title.ChangeTheAbbrev(tl, languageCultureCode, ("Mr.-" + Guid.NewGuid()).Substring(0,10) );

                session.Save(tl);

                tx.Commit();
            }
        }



        [TestMethod]
        public void Test_Reservation_ReserveFromDto()
        {

            var reservationDto = new ReservationPersistDto
                {
                    TitleId = 1, // "Mr.
                    GuestName = "Wolverine-" + Guid.NewGuid(),
                    EmailAddress = "logan@marvel.com",
                    ContactNumber = "520",
                    NationalityId = 1, // American
                    DateOfRide = new DateTime(2076, 11, 05),

                    ReservationAdditionals = new List<ReservationAdditionalDto>
                        {
                            new ReservationAdditionalDto { AgeBracketId = 1, GuestName = "Storm", NationalityId = 1},
                            new ReservationAdditionalDto { AgeBracketId = 2, GuestName = "Spiderman", NationalityId = 2},
                            new ReservationAdditionalDto { AgeBracketId = 3, GuestName = "Thor", NationalityId = 3}
                        }
                };

            using (var session = Mapper.SessionFactory.OpenSession())
            {
                var reservationId = Reservation.Reserve(session, reservationDto);


                Assert.IsTrue(reservationId > 0);

                var ageBracket = session.Load<AgeBracket>(AgeBracketId.YoungTeenager);

                Assert.AreEqual(11, ageBracket.MinimumAge); // just check if stub dto ain't get overwritten
            }
        }

        
    }
}
