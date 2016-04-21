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
            return users.GetUserName(Id);
        }

    }

    public class Connections : SqlExpressWrapper<Connection>
    {
        public Connections(Object connectionString)
            : base(connectionString)
        {
            SetCache(true, "SELECT * FROM Connection ORDER BY UserId");
        }

        public List<Object> GetJsonConnectionList()
        {
            List<Object> json_ThreadList = new List<Object>();

            foreach (Connection con in ToList())
            {
                json_ThreadList.Add(
                    new
                    {
                        Id = con.Id,
                        UserId = con.UserId,
                        StartDate = con.StartDate,
                        EndDate = con.EndDate
                    });
            }

            return json_ThreadList;
        }

        public List<Object> GetJsonConnectionList(int userID)
        {
            List<Object> json_ThreadList = new List<Object>();

            foreach (Connection con in ToList())
            {
                if (con.UserId == userID)
                {
                    
                    json_ThreadList.Add(
                        new
                        {
                            Id = con.Id,
                            UserId = con.UserId,
                            StartDate = con.StartDate,
                            EndDate = con.EndDate
                        });
                }
            }

            return json_ThreadList;
        }

    }
}