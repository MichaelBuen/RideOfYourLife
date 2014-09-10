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
    public class TitlesService : ServiceStack.ServiceInterface.Service
    {
        public NHibernate.ISession DomainSession { get; set; }


        //[ServiceStack.ServiceInterface.Cors.EnableCors]
        // [ServiceStack.ServiceInterface.Authenticate]
        public IEnumerable<TitleDto> Get(TitleDtos request)
        {
            var qTitle = Title.GetTitles(this.DomainSession, request);

         

            return qTitle;
            
        }
    }
}