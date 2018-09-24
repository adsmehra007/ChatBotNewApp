using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatBot.Models
{
    public class BOTResponse
    {
        public string RequestToken { get; set; }
        public string ResponseMessage { get; set; }
        public bool status { get; set; }
        public List<Patient> FilteredPatList { get; set; }

    }
}