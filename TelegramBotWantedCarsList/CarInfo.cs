using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotWantedCarsList
{
    public class CarInfo
    {
        public string ID { get; set; }
        public string OVD { get; set; }
        public string BRAND { get; set; }
        public string COLOR { get; set; }
        public string VEHICLENUMBER { get; set; }
        public string BODYNUMBER { get; set; }
        public string CHASSISNUMBER { get; set; }
        public string ENGINENUMBER { get; set; }
        public string THEFT_DATA { get; set; }
        public string INSERT_DATE { get; set; }
    }

    public class CarInfoRoot
    {
        public Dictionary<string, CarInfo> Files { get; set; }
    }
}
