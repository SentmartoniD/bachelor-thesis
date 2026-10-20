using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Interfaces;
using UserService.DBContext;
using System.IO;
using Serilog;

namespace UserService.Providers
{
    public class UserServiceHealthCheckProvider : IUserServiceHealthCheck
    {
        private readonly string binFolderPath;
        private readonly string exceptionLoggerPath;

        public UserServiceHealthCheckProvider()
        {
            binFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
            exceptionLoggerPath = Path.Combine(binFolderPath, "UserServieExceptionLog.txt");
        }

        public string HealthCheck()
        {
            var exceptionLogger = new LoggerConfiguration()
                .WriteTo.File(exceptionLoggerPath, outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            try
            {
                using (var dbContext = new UserDBContext())
                {
                    var dbConnection = dbContext.Database.Connection;
                    dbConnection.Open();
                    dbConnection.Close();
                }
            }
            catch (Exception e)
            {
                exceptionLogger.Error(e.Message);
                Log.CloseAndFlush();
                return "Status : UNHEALTHY, UserService cant connect to the database!";
            }

            return "Status : HEALTHY, UserService is running and healthy!";
        }
    }
}
