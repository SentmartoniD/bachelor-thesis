using ClassLibrary.WcfModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Interfaces;
using UserService.DBContext;
using UserService.Mapper;
using ClassLibrary.Models;
using AutoMapper;
using Serilog;
using System.IO;

namespace UserService.Providers
{
    public class InternalUserServiceProvider : IWcfInternalUserService
    {
        private readonly UserDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly string binFolderPath;
        private readonly string exceptionLoggerPath;
        private readonly string auditLoggerPath;

        public InternalUserServiceProvider()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = _mapper = config.CreateMapper();
            _dbContext = new UserDBContext();
            binFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
            exceptionLoggerPath = Path.Combine(binFolderPath, "UserServieExceptionLog.txt");
            auditLoggerPath = Path.Combine(binFolderPath, "UserServieAuditLog.txt");
        }

        public WcfUser GetUserByEmail(string email)
        {
            var auditLogger = new LoggerConfiguration()
                .WriteTo.File(auditLoggerPath, outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}")
                .CreateLogger();
            var exceptionLogger = new LoggerConfiguration()
                .WriteTo.File(exceptionLoggerPath, outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            try
            {
                User user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
                auditLogger.Information("Successfully retrieved user by email!", DateTime.Now);
                Log.CloseAndFlush();
                return _mapper.Map<WcfUser>(user);
            }
            catch(Exception e)
            {
                exceptionLogger.Error(e.Message);
                Log.CloseAndFlush();
                throw e;
            }
        }
    }
    
}
