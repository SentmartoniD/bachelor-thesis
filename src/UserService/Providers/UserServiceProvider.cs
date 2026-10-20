using AutoMapper;
using ClassLibrary.WcfModels;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Interfaces;
using UserService.DBContext;
using UserService.Mapper;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Serilog;
using Serilog.Sinks.File;
using System.ServiceModel;

namespace UserService.Providers
{
    public class UserServiceProvider : IWcfUserService
    {
        private readonly IMapper _mapper;
        private readonly UserDBContext _dbContext;
        private readonly string _secretKey;
      /*  private ILogger auditLogger;
        private ILogger exceptionLogger;*/
        private readonly string binFolderPath;
        private readonly string exceptionLoggerPath;
        private readonly string auditLoggerPath;

        public UserServiceProvider()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();
            _dbContext = new UserDBContext();
            _secretKey = "r2ff3f3f3f33ff33ff222dwjidwjiwdijdwijwdijwdijijwdiwd2h22dd12d2";
             binFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
             exceptionLoggerPath = Path.Combine(binFolderPath, "UserServieExceptionLog.txt");
             auditLoggerPath = Path.Combine(binFolderPath, "UserServieAuditLog.txt");
        }

        public UserDtoRegistration AddUser(UserDtoRegistration newUser)
        {
            var auditLogger = new LoggerConfiguration()
                .WriteTo.File(auditLoggerPath, outputTemplate: 
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}")
                .CreateLogger();
            var exceptionLogger = new LoggerConfiguration()
                .WriteTo.File(exceptionLoggerPath, outputTemplate: 
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            if (_dbContext.Users.Any(x => x.Email == newUser.Email))
            {
                exceptionLogger.Error("Email already in use!");
                Log.CloseAndFlush();
                throw new FaultException("Email already in use!");
            }
            if (_dbContext.Users.Any(x => x.Username == newUser.Username))
            {
                exceptionLogger.Error("Username already in use!");
                Log.CloseAndFlush();
                throw new FaultException("Username already in use!");
            }

            WcfUser user = _mapper.Map<WcfUser>(newUser);
            user.Password = GetHashValueInString(newUser.Password);
            User u = _mapper.Map<User>(user);

            _dbContext.Users.Add(u);
            _dbContext.SaveChanges();

            auditLogger.Information("Successfully added user to the database!", DateTime.Now);
            Log.CloseAndFlush();

            return _mapper.Map<UserDtoRegistration>(user);
        }

        public UserDtoApprovedVerified GetUserByEmail(string email)
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
                IQueryable<User> query = _dbContext.Users;
                query = query.Where(e => e.Email.Contains(email));
                User user = query.ToList<User>()[0];
                WcfUser wcfUser = _mapper.Map<WcfUser>(user);
                auditLogger.Information("Successfully retrieved user by email!", DateTime.Now);
                Log.CloseAndFlush();
                return _mapper.Map<UserDtoApprovedVerified>(wcfUser);
            }
            catch (FaultException e) {
                exceptionLogger.Error(e.Message);
                Log.CloseAndFlush();
                throw e;
            }
            
        }

        public UserDtoRegistration GetUserById(int id)
        {
            return _mapper.Map<UserDtoRegistration>(_mapper.Map<WcfUser>(_dbContext.Users.Find(id)));
        }

        public List<UserDtoApprovedVerified> GetUsers()
        {
            List<WcfUser> users = _mapper.Map<List<WcfUser>>(_dbContext.Users.ToList());
            return _mapper.Map<List<UserDtoApprovedVerified>>(users);
        }

        public string LoginUser(UserDtoLogin userDto)
        {
            if (!_dbContext.Users.Any(u => u.Email == userDto.Email))
                throw new Exception("There is no user with that email!");

            IQueryable<User> query = _dbContext.Users;
            query = query.Where(e => e.Email.Contains(userDto.Email));
            User user = query.ToList<User>()[0];

            bool verified = false;
            if (user.Verified == Status.YES)
                verified = true;

            //if (BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password, false, BCrypt.Net.HashType.SHA256 ))//Uporedjujemo hes pasvorda iz baze i unetog pasvorda
            if (GetHashValueInString(userDto.Password) == user.Password)
            {
                if (user.Approved == Status.UNDEFINED)
                    throw new Exception("The user hasnt been approved yet!");
                if (user.Approved == Status.NO)
                    throw new Exception("The user registration has been denied!");

                List<Claim> claims = new List<Claim>();
                //Mozemo dodati Claimove u token, oni ce biti vidljivi u tokenu i mozemo ih koristiti za autorizaciju
                if (user.UserType == "admin")
                    claims.Add(new Claim(ClaimTypes.Role, "admin")); //Add user type to claim
                if (user.UserType == "buyer")
                    claims.Add(new Claim(ClaimTypes.Role, "buyer")); //Add user type to claim
                if (user.UserType == "seller")
                    claims.Add(new Claim(ClaimTypes.Role, "seller")); //Add user type to claim
                //mozemo izmisliti i mi neki nas claim
                claims.Add(new Claim("verified", verified.ToString()));

                //Kreiramo kredencijale za potpisivanje tokena. Token mora biti potpisan privatnim kljucem
                //kako bi se sprecile njegove neovlascene izmene
                SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.HmacSha256Signature);
                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:44337", //url servera koji je izdao token
                    claims: claims, //claimovi
                    expires: DateTime.Now.AddMinutes(50), //vazenje tokena u minutama
                    signingCredentials: signinCredentials //kredencijali za potpis
                );
                string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                return tokenString;
            }
            else
            {
                throw new Exception("There is no user with that password!");
            }
        }

        public UserDtoRegistration UpdateUser(int id, UserDtoRegistration userDtoRegistration)
        {
            if (_dbContext.Users.Any(u => u.Email == userDtoRegistration.Email))
                throw new Exception("Email already in use!");
            if (_dbContext.Users.Any(u => u.Username == userDtoRegistration.Username))
                throw new Exception("Username already in use!");

            var user = _dbContext.Users.Find(userDtoRegistration.UserId);

            if (user == null)
                throw new Exception("There is no user ith that id!");

            if (user != null)
            {
                if (userDtoRegistration.Username != null && userDtoRegistration.Username != string.Empty)
                    user.Username = userDtoRegistration.Username;
                if (userDtoRegistration.Email != null && userDtoRegistration.Email != string.Empty)
                    user.Email = userDtoRegistration.Email;
                if (userDtoRegistration.FirstName != null && userDtoRegistration.FirstName != string.Empty)
                    user.FirstName = userDtoRegistration.FirstName;
                if (userDtoRegistration.LastName != null && userDtoRegistration.LastName != string.Empty)
                    user.LastName = userDtoRegistration.LastName;
                if (userDtoRegistration.DateOfBirth != null && userDtoRegistration.DateOfBirth != string.Empty)
                    user.DateOfBirth = userDtoRegistration.DateOfBirth;
                if (userDtoRegistration.Address != null && userDtoRegistration.Address != string.Empty)
                    user.Address = userDtoRegistration.Address;
                if (userDtoRegistration.UserType != null && userDtoRegistration.UserType != string.Empty)
                    user.UserType = userDtoRegistration.UserType;
                if (userDtoRegistration.ImageFile != null)
                    user.ImageFile = userDtoRegistration.ImageFile;
                if (userDtoRegistration.Password != null && userDtoRegistration.Password != string.Empty)
                    user.Password = GetHashValueInString(userDtoRegistration.Password);
            }

            _dbContext.SaveChanges();


            return _mapper.Map<UserDtoRegistration>(user);
        }

        public UserDtoStatus UpdateUserStatus(UserDtoStatus userDtoStatus)
        {
            var user = _dbContext.Users.Find(userDtoStatus.UserId);

            if (user == null)
                throw new Exception("There is no user with that id!");

            if (userDtoStatus.Status == "approved")
            {
                user.Approved = Status.YES;
            }
            if (userDtoStatus.Status == "deny")
            {
                user.Approved = Status.NO;
            }
            if (userDtoStatus.Status == "verify")
            {
                user.Verified = Status.YES;
            }
            if (userDtoStatus.Status == "unverify")
            {
                user.Verified = Status.NO;
            }

            _dbContext.SaveChanges();

            return userDtoStatus;
        }

        public bool UploadImageMS(MemoryStream ms, string email)
        {
            bool rez = true;
            if (ms != null)
            {
                User user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

                using (ms)
                {
                    user.ImageFile = ms.ToArray();
                }

                _dbContext.SaveChanges();
            }

            return rez;
        }

        private string GetHashValueInString(string inputPassword)
        {
            string hashString;
            byte[] bytes = Encoding.UTF8.GetBytes(inputPassword);

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] hashBytes = sha256Hash.ComputeHash(bytes);
                hashString = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            }

            return hashString;
        }

    }
}
