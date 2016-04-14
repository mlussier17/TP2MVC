using Sql_Express_Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TP2MVC.Models
{
    public class Connection
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }

    public class Connections : SqlExpressWrapper<Connection>
    {
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

    }
}