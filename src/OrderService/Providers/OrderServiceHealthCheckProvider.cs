using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.InterFaces;
using OrderService.DBContext;

namespace OrderService.Providers
{
    public class OrderServiceHealthCheckProvider : IOrderServiceHealthCheck
    {
        public string HealthCheck()
        {
            try
            {
                using (var dbContext = new OrderDBContext())
                {
                    var dbConnection = dbContext.Database.Connection;
                    dbConnection.Open();
                    dbConnection.Close();
                }
            }
            catch (Exception e)
            {
                return "Status : UNHEALTHY, OrderService cant connect to the database!";
            }

            return "Status : HEALTHY, OrderService is running and healthy!";
        }
    }
    
}
