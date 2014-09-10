using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dto
{

    
    public class TitleDtos 
    {
        public int[] Ids { get; set; }

        public string LanguageCultureCode { get; set; }

        public TitleDtos() : this(new int[] { }) { } // We are solving the billion dollar mistake here: http://www.infoq.com/presentations/Null-References-The-Billion-Dollar-Mistake-Tony-Hoare

        public TitleDtos(params int[] ids)
        {
            this.Ids = ids; 
        }
    }


    

    
    public class TitleDto 
    {
        public int TitleId { get; set; }
        public string Abbrev { get; set; }        
    }

}
