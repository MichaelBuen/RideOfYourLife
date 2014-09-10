using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    
    public class ReservationViewDto // return ReservationViewDto
    {
        public int ReservationId { get; set; }

        public string LanguageCultureCode { get; set; }

        public string Title { get; set; }
        public string GuestName { get; set; }
        public string EmailAddress { get; set; }
        public string ContactNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Nationality { get; set; }

        public string DateOfRide { get; set; }


        public IEnumerable<ReservationAdditionalViewDto> ReservationAdditionals { get; set; }
       
    }


    public class ReservationAdditionalViewDto
    {
        public string GuestName { get; set; }
        public string AgeBracket { get; set; }
        public string Nationality { get; set; }
    }




    public class ReservationPersistDto // returns ReservationResultDto
    {
        public int ReservationId { get; set; }
        public int TitleId { get; set; }
        public string GuestName { get; set; }
        public string EmailAddress { get; set; }
        public string FaxNumber { get; set; }
        public string ContactNumber { get; set; }
        public int NationalityId { get; set; }
        public DateTime? DateOfRide { get; set; }

        public IList<ReservationAdditionalDto> ReservationAdditionals { get; set; }        
    }

    public class ReservationAdditionalDto
    {
        public string GuestName { get; set; }
        public int AgeBracketId { get; set; }
        public int NationalityId { get; set; }
    }

    public class ReservationResultDto
    {
        public int ReservationId { get; set; }
        public string OtherMessage { get; set; }
    }

}
