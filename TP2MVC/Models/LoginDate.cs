using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TP2MVC.Models
{
    public class LoginDate
    {
        public int UserID { get; set; }
        public DateTime Date { get; set; }
        public int nbLogon { get; set; }

        public LoginDate(int UserID_, DateTime Date_, int nbLogon_)
        {
            UserID = UserID_;
            Date = Date_;
            nbLogon = nbLogon_;
        }
    }
}