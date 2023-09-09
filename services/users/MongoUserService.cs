using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PeyulErp.Exceptions;
using PeyulErp.Model;
using PeyulErp.Models;
using PeyulErp.Settings;
using PeyulErp.Utility;

namespace PeyulErp.Services
{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    public class MongoUsersService : IUsersService
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IPasswordService _passwordService;
        private readonly MongoDbSettings _settings;
        private readonly SystemSettings _systemSettings;
        private readonly IMailingService _mailingService;
        private readonly FilterDefinitionBuilder<User> _filterBuilder = Builders<User>.Filter;

        public MongoUsersService(
            IMongoClient client,
            IOptions<MongoDbSettings> settings,
            IOptions<SystemSettings> systemSettings,
            IPasswordService passwordService,
            IMailingService mailingService)
        {
            _settings = settings.Value;
            var database = client.GetDatabase(_settings.DatabaseName);
            _usersCollection = database.GetCollection<User>(_settings.UsersCollectionName);
            _mailingService = mailingService;
            _systemSettings = systemSettings.Value;
            _passwordService = passwordService;

            //create default admin user if not exists
            CreateDefaultAdminUser().ConfigureAwait(false);
        }

        //create default admin user if not exists
        public async Task CreateDefaultAdminUser()
        {
            var adminUser = await _usersCollection.Find(_filterBuilder.Eq(u => u.Role, UserRole.Admin)).FirstOrDefaultAsync();
            Console.WriteLine("Admin Email: " + _systemSettings.AdminEmail);

            if (adminUser == null)
            {
                var defaultAdminUser = new User
                {
                    Name = "Admin",
                    Email = _systemSettings.AdminEmail,
                    PhoneNumber = "254722000000",
                    Status = UserStatus.Active,
                    Role = UserRole.Admin,
                    Password = Password.HashPassword(_systemSettings.AdminPassword)
                };

                await CreateAsync(defaultAdminUser, true);
            }
        }

        public async Task<User> CreateAsync(User user, bool isDefaultUser = false)
        {
            if (!isDefaultUser)
            {
                var existingUser = _usersCollection.Find(_filterBuilder.Eq(u => u.Email, user.Email)).FirstOrDefault();

                if (existingUser != null)
                    throw new Exception("User already exists");
            }       

            var randomPassword = isDefaultUser ? user.Password : Password.GenerateRandomPassword();
            var dbUser = user with
            {
                Id = Guid.NewGuid(),
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Status = UserStatus.Active,
                Password = Password.HashPassword(randomPassword)
            };

            await _passwordService.Upsert(dbUser, !isDefaultUser);
            await _usersCollection.InsertOneAsync(dbUser);
            SendWelcomeEmail(dbUser with { Password = randomPassword }).ConfigureAwait(false);

            return dbUser;
        }

        public async Task<bool> DeleteAsync(Guid Id)
        {
            var existingUser = (await _usersCollection.FindAsync(_filterBuilder.Eq(u => u.Id, Id))).FirstOrDefault();

            if (existingUser == null)
            {
                return false;
            }

            await _usersCollection.DeleteOneAsync(_filterBuilder.Eq(u => u.Id, Id));
            await _passwordService.DeletePassword(existingUser.Id);

            return true;
        }

        public async Task<IList<User>> GetAllAsync()
        {
            return await _usersCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return (await _usersCollection.FindAsync(_filterBuilder.Eq(u => u.Email, email))).FirstOrDefault();
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return (await _usersCollection.FindAsync(_filterBuilder.Eq(u => u.Id, id))).FirstOrDefault();
        }

        public async Task<User> UpdateAsync(User user)
        {
            var existingUser = (await _usersCollection.FindAsync(_filterBuilder.Eq(u => u.Id, user.Id))).FirstOrDefault();

            if (existingUser == null)
            {
                return null;
            }

            await _usersCollection.ReplaceOneAsync(_filterBuilder.Eq(u => u.Id, user.Id), user with { UpdateDate = DateTime.UtcNow, Password = existingUser.Password });

            return user;
        }

        public async Task UpdateUserStatusAsync(Guid Id, UserStatus status)
        {
            var existingUser = (await _usersCollection.FindAsync(_filterBuilder.Eq(u => u.Id, Id))).FirstOrDefault();

            if (existingUser == null)
            {
                throw new Exception("User not found");
            }

            await _usersCollection.UpdateOneAsync(_filterBuilder.Eq(u => u.Id, Id), Builders<User>.Update.Set(u => u.Status, status));
        }

        public async Task UpdateUserPassWordAsync(Guid Id, string password, bool forceReset = false)
        {
            var existingUser = (await _usersCollection.FindAsync(_filterBuilder.Eq(u => u.Id, Id))).FirstOrDefault();

            if (existingUser == null)
            {
                throw new Exception("User not found");
            }

            await _passwordService.Upsert(existingUser with { Password = Password.HashPassword(password) }, forceReset);
            await _usersCollection.UpdateOneAsync(_filterBuilder.Eq(u => u.Id, Id), Builders<User>.Update.Set(u => u.Password, Password.HashPassword(password)));
        }

        private async Task SendWelcomeEmail(User user)
        {
            var emailSubject = "Welcome to Peyul ERP";
            var emailBody = $"Hello {user.Name},\n\nWelcome to Peyul ERP. Your account has been created successfully With Password {user.Password}.\n\nPlease consider changing this password after logging in.\n\nRegards,\nPeyul ERP Team";

            var email = new Email {
                ReciverEmail = user.Email,
                Subject = emailSubject,
                Body = emailBody
            };

           await _mailingService.SendMailAsync(email);
        }

        public async Task<bool> ResetUserPasswordAsync(User user)
        {
            var newPassword = Password.GenerateRandomPassword();
            await UpdateUserPassWordAsync(user.Id, newPassword, true);
            SendPasswordResetEmailAsync(user with { Password = newPassword });

            return true;
        }

        private async Task SendPasswordResetEmailAsync(User user)
        {
            var emailSubject = "Peyul ERP Password Reset";
            var emailBody = $"Hello {user.Name},\n\nYour Peyul ERP password has been reset successfully. Your new password is {user.Password}.\n\nPlease consider changing this password after logging in.\n\nRegards,\nPeyul ERP Team";

            var email = new Email
            {
                ReciverName = user.Name,
                ReciverEmail = user.Email,
                Subject = emailSubject,
                Body = emailBody
            };

           await _mailingService.SendMailAsync(email);
        }
    }
}