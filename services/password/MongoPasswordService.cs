using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PeyulErp.Exceptions;
using PeyulErp.Models;
using PeyulErp.Settings;

namespace PeyulErp.Services
{
    public class MongoPasswordService : IPasswordService
    {
        private readonly IMongoCollection<UserPassword> _userPasswordCollection;
        private readonly FilterDefinitionBuilder<UserPassword> _filterBuilder = Builders<UserPassword>.Filter;
        private readonly MongoDbSettings _settings;

        public MongoPasswordService(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;
            var database = client.GetDatabase(_settings.DatabaseName);
            _userPasswordCollection = database.GetCollection<UserPassword>(_settings.UserPasswordCollectionName);
        }

        public async Task<bool> DeletePassword(Guid userId)
        {
            var filter = _filterBuilder.Eq(p => p.UserId, userId);

            try
            {
                await _userPasswordCollection.DeleteOneAsync(filter);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserPassword> GetByUserId(Guid userId)
        {
            return (await _userPasswordCollection.FindAsync(_filterBuilder.Eq(p => p.UserId, userId))).FirstOrDefault();
        }

        public async Task IncrementFailedAttempts(Guid UserId)
        {
            var filter = _filterBuilder.Eq(p => p.UserId, UserId);
            var existingPass = (await _userPasswordCollection.FindAsync(filter)).FirstOrDefault();

            if(existingPass != null)
            {
                var update = Builders<UserPassword>.Update.Set(p => p.FailedAttempts, existingPass.FailedAttempts + 1);
                await _userPasswordCollection.UpdateOneAsync(filter, update);
            }            
        }

        public async Task<bool> ResetPasswordAttempts(Guid userId)
        {
            var filter = _filterBuilder.Eq(p => p.UserId, userId);
            var update = Builders<UserPassword>.Update.Set(p => p.FailedAttempts, 0);

            try
            {
                await _userPasswordCollection.UpdateOneAsync(filter, update);
                return true;
            } catch
            {
                return false;
            }            
        }

        public async Task Upsert(User user, bool forceReset = false)
        {
            var filter = _filterBuilder.Eq(p => p.UserId, user.Id);
            var existingUserPassword = _userPasswordCollection.Find(filter).FirstOrDefault();

            UserPassword newUserPassword;

            if (existingUserPassword != null)
            {
                if (user.Password == existingUserPassword.CPassword || user.Password == existingUserPassword.LPassword)
                {
                    throw new PasswordException("Password already used");
                }

                newUserPassword = existingUserPassword with
                {
                    UpdateDate = DateTime.UtcNow,
                    LPassword = existingUserPassword.CPassword,
                    CPassword = user.Password,
                    ForceReset = forceReset
                };
            }
            else
            {
                newUserPassword = new UserPassword
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    CPassword = user.Password,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    ForceReset = true
                };
            }

            await _userPasswordCollection.ReplaceOneAsync(filter: filter, options: new ReplaceOptions { IsUpsert = true }, replacement: newUserPassword);
        }


    }
}