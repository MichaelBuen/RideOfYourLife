using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dto
{

    
    public class NationalityDtos 
    {
        public int[] Ids { get; set; }

        public string LanguageCultureCode { get; set; }

        public NationalityDtos() : this(new int[] { }) { } // We are solving the billion dollar mistake here: http://www.infoq.com/presentations/Null-References-The-Billion-Dollar-Mistake-Tony-Hoare

        public NationalityDtos(params int[] ids)
        {
            this.Ids = ids; 
        }
    }




    
    public class NationalityDto 
    {
        public int NationalityId { get; set; }
        public string NationalityName { get; set; }        
    }

}
