using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Models;
using DomainMapping.SessionMapper;
using Dto;
using Domain;

using NHibernate.Linq;

namespace Marshaller.TheServices
{
    
    public class AgeBracketService : ServiceStack.ServiceInterface.Service
    {
        public NHibernate.ISession DomainSession { get; set; }


        //[ServiceStack.ServiceInterface.Cors.EnableCors]
        // [ServiceStack.ServiceInterface.Authenticate]
        public IEnumerable<AgeBracketDto> Get(AgeBracketDtos request)
        {

            //using (var session = DomainMapping.SessionMapper.Mapper.SessionFactory.OpenSession())
            //using (var tx = session.BeginTransaction().SetLanguage(session, "en"))
            //{
            //    var r = session.Get<Reservation>(1);


            //    foreach (var d in r.ReservationAdditionals)
            //    {
            //        Console.WriteLine(d.GuestName);
            //    }


            //}


            
            using (var tx = this.DomainSession.BeginTransaction().SetLanguage(this.DomainSession, request.LanguageCultureCode))
            {




                var result = AgeBracket.GetAgeBrackets(this.DomainSession, request);




                return result;
            }
        }
    }
}