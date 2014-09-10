using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Models;
using DomainMapping.SessionMapper;
using Dto;
using Domain;

namespace Marshaller.TheServices
{
    public class ReservationResourceService : ServiceStack.ServiceInterface.Service
    {
        public NHibernate.ISession DomainSession { get; set; }

        
        //[ServiceStack.ServiceInterface.Cors.EnableCors]
        // [ServiceStack.ServiceInterface.Authenticate]
        public ReservationResourceDto Get(ReservationResourceDto request)
        {


            var dto = TextResource.GetReservationResource(this.DomainSession, request);
                        
            return dto;
            
        }
    }
}