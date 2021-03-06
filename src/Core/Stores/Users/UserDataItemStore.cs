﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Abstractions;
using PlatoCore.Models.Users;
using PlatoCore.Repositories.Users;
using PlatoCore.Stores.Abstractions.Users;

namespace PlatoCore.Stores.Users
{
    public class UserDataItemStore : IUserDataItemStore<UserData>
    {

        #region "Private Variables"

        private readonly IUserDataRepository<UserData> _userDataRepository;

        #endregion

        #region ""Constructor"

        public UserDataItemStore(IUserDataRepository<UserData> userDataRepository)
        {
            _userDataRepository = userDataRepository;
        }

        #endregion

        #region "IMplementation"

        public async Task<T> GetAsync<T>(int userId, string key) where T : class
        {
            var result = await GetByUserIdAndKeyAsync(userId, key);
            if (result != null)
            {
                // If the generic type implements ISerializable use
                // the the DeserializeAsync method on the generic type
                // Allows us to implement our own Serialize and Deserialize
                // methods on the type if we want to override the default
                // Serializable base class virtual methods
                var serializable = typeof(T) as ISerializable;
                if (serializable != null)
                {
                    return await serializable.DeserializeGenericTypeAsync<T>(result.Value);
                }
                return await result.Value.DeserializeAsync<T>();
            }

            return default(T);
        }

        public async Task<T> UpdateAsync<T>(int userId, string key, ISerializable value) where T : class
        {

            // Get or create setting
            var data = await GetByUserIdAndKeyAsync(userId, key)
                       ?? new UserData()
                       {
                           UserId = userId,
                           Key = key
                       };

            // Serialize value
            data.Value = await value.SerializeAsync();

            var updatedData = await _userDataRepository.InsertUpdateAsync(data);
            if (updatedData != null)
            {
                return await GetAsync<T>(userId, updatedData.Key);
            }

            return default(T);
        }

        public Task<bool> DeleteAsync(int userId, string key)
        {
            // TODO
            throw new NotImplementedException();
        }

        #endregion

        #region "Private Methods"

        async Task<UserData> GetByUserIdAndKeyAsync(int userId, string key)
        {
            var allKeys = await GetAllKeys(userId);
            return allKeys?.FirstOrDefault(s => s.Key == key);
        }

        async Task<IEnumerable<UserData>> GetAllKeys(int userId)
        {
            var output = new List<UserData>();
            var entries = await _userDataRepository.SelectByUserIdAsync(userId);
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    output.Add(entry);
                }
            }

            return output;
        }

        #endregion

    }

}
