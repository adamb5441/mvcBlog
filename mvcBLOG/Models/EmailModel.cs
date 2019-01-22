using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mvcBLOG.Models
{
    public class EmailModel
    {
        [require, Display(Name = "Name")]
        public string FromName { get; set; }
        [require, Display(Name = "Email"), EmailAddress]
        public string FromEmail { get; set; }
        [require]
        public string Subject { get; set; }
        [require]
        public string Body { get; set; }
    }
}