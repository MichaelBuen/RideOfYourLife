using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Models;
using DomainMapping.SessionMapper;
using Dto;
using Domain;

using ServiceStack.ServiceInterface;




namespace Marshaller.TheServices
{
    public class ReservationService : ServiceStack.ServiceInterface.Service
    {

        public NHibernate.ISession DomainSession { get; set; }

        //[ServiceStack.ServiceInterface.Cors.EnableCors]
        //[ServiceStack.ServiceInterface.Authenticate]
        public ReservationViewDto Get(ReservationViewDto request)
        {
            var dto = Reservation.GetViewable(this.DomainSession, request);
            
            return dto;
        }


        //[ServiceStack.ServiceInterface.Cors.EnableCors]
        // [ServiceStack.ServiceInterface.Authenticate]
        public ReservationResultDto Post(ReservationPersistDto reservationPersistDto)
        {            
            var reservationId = Reservation.Reserve(this.DomainSession, reservationPersistDto);
            
            return new ReservationResultDto {ReservationId = reservationId};


            // var ra = new ReservationAdditional(); // compile error. DDD: we cannot create a child entity by itself, it must be created from aggregate root            
        }
    }

 
 
}