using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace WebApplication2.Connection
{
    public class ConnectionFactory
    {
        string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
       
    }
}