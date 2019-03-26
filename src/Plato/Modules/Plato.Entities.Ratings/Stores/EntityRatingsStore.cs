﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Ratings.Models;
using Plato.Entities.Ratings.Repositories;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Ratings.Stores
{

    public class EntityRatingsStore : IEntityRatingsStore<EntityRating>
    {

        private readonly IEntityRatingsRepository<EntityRating> _entityRatingRepository;
        private readonly ILogger<EntityRatingsStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public EntityRatingsStore(
            IEntityRatingsRepository<EntityRating> entityRatingRepository,
            ILogger<EntityRatingsStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _entityRatingRepository = entityRatingRepository;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EntityRating> CreateAsync(EntityRating model)
        {
            var result = await _entityRatingRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<EntityRating> UpdateAsync(EntityRating model)
        {
            var result = await _entityRatingRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<bool> DeleteAsync(EntityRating model)
        {
            var success = await _entityRatingRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted rating with id {1}", model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<EntityRating> GetByIdAsync(int id)
        {
            return await _entityRatingRepository.SelectByIdAsync(id);
        }

        public IQuery<EntityRating> QueryAsync()
        {
            var query = new EntityRatingsQuery(this);
            return _dbQuery.ConfigureQuery<EntityRating>(query); ;
        }

        public async Task<IPagedResults<EntityRating>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityRatingRepository.SelectAsync(args));
        }
        
        public async Task<IEnumerable<EntityRating>> SelectEntityRatingsByEntityId(int entityId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityRatingRepository.SelectEntityRatingsByEntityId(entityId));
        }

        public async Task<IEnumerable<EntityRating>> SelectEntityRatingsByUserIdAndEntityId(int userId, int entityId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), userId, entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityRatingRepository.SelectEntityRatingsByUserIdAndEntityId(userId, entityId));
        }

        #endregion
        
    }
}



