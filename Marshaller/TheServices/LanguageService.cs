using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using Dto;
using Domain;

using ServiceStack.ServiceInterface;

namespace Marshaller.TheServices
{
    public class LanguageService : ServiceStack.ServiceInterface.Service
    {
        
        //[ServiceStack.ServiceInterface.Cors.EnableCors]
        //[ServiceStack.ServiceInterface.Authenticate]
        public LanguageDto Get(LanguageDto request)
        {

            string language = this.Session["Language"] as string ?? "en";

            var dto = new LanguageDto { LanguageCultureCode = language };

            return dto;


        }


        //[ServiceStack.ServiceInterface.Cors.EnableCors]
        // [ServiceStack.ServiceInterface.Authenticate]
        public LanguageDto Post(LanguageDto changedLanguage)
        {
            this.Session["Language"] = changedLanguage.LanguageCultureCode;

            string language = this.Session["Language"] as string ?? "en";

            var dto = new LanguageDto { LanguageCultureCode = language };

            return dto;
        }
    }

 
 
}