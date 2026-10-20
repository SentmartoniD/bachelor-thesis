using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticleService.Interfaces;
using ArticleService.DBContext;

namespace ArticleService.Providers
{
    public class ArticleServiceHealthCheckProvider : IArticleServiceHealthCheck
    {
        public string HealthCheck()
        {
            try
            {
                using (var dbContext = new ArticleDBContext()) {
                    var dbConnection = dbContext.Database.Connection;
                    dbConnection.Open();
                    dbConnection.Close();
                }
            }
            catch (Exception e) {
                return "Status : UNHEALTHY, ArticleService cant connect to the database!";
            }

            return "Status : HEALTHY, ArticleService is running and healthy!";
        }
    }
}
