using Sql_Express_Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace TP2MVC.Models
{
    public class Connection
    {
        [ScriptIgnore(ApplyToOverrides = true)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public String GetUserName()
        {
            Users users = (Users)HttpRuntime.Cache["Users"];
            return users.GetUserName(UserId);
        }

    }

    public class Connections : SqlExpressWrapper<Connection>
    {
        public Connections(Object connectionString)
            : base(connectionString)
        {
            SetCache(true, "SELECT * FROM Connections ORDER BY UserId ASC, StartDate DESC");
        }

        public List<LoginDate> GetJsonConnectionList(int userID, int nbDays = 1, DateTime? date = null)
        {
            if (date.HasValue) date = DateTime.Now;

            List<LoginDate> json_DateList = new List<LoginDate>();
            Dictionary<DateTime, int> dic = new Dictionary<DateTime, int>(); 


            foreach (Connection con in ToList())
            {
                if (con.UserId == userID)
                {
                    int currentCount;
                    if (dic.TryGetValue(con.StartDate.Date, out currentCount))
                    {
                        dic[con.StartDate.Date] = currentCount + 1;
                    }
                    else
                    {
                        dic[con.StartDate.Date] = 1;
                    }
                }
            }

            foreach (DateTime key in dic.Keys)
            {
                json_DateList.Add(new LoginDate(userID, key, dic[key]));
            }

            return json_DateList;
        }
    }
}