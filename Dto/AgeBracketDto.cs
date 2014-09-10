using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dto
{

    
    public class AgeBracketDtos 
    {
        public int[] Ids { get; set; }

        public string LanguageCultureCode { get; set; }

        public AgeBracketDtos() : this(new int[] { }) { } // We are solving the billion dollar mistake here: http://www.infoq.com/presentations/Null-References-The-Billion-Dollar-Mistake-Tony-Hoare

        public AgeBracketDtos(params int[] ids)
        {
            this.Ids = ids; 
        }
    }


    

    
    public class AgeBracketDto 
    {
        public int AgeBracketId { get; set; }
        public string AgeIndicator { get; set; }
        public string AgeBracketDescription { get; set; }        
    }

}
