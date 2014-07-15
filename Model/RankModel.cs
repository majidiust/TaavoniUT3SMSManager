using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaavoniUT3SMSManager.Model
{
    public class RankModel
    {
        public double Point { get; set; }
        public Guid UserId { get; set; }
        public String UserName { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public int Rank { get; set; }
    }
}
