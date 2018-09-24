using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatBot.Models
{
    public class Patient
    {

        public long ID { get; set; }
        public string Name { get; set; }
        public string  FirstName{ get; set; }
        public string LastName { get; set; }
        public string SSN { get; set; }
        public string ZipCode { get; set; }
        public string birthDate { get; set; }
        public string birthCC { get; set; }
        public string PatWorkPhone { get; set; }
        public string PatHomePhone { get; set; }
    }
}