﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Stores.Abstractions;

namespace Plato.Categories.Stores
{
    public interface IEntityCategoryStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByEntityIdAsync(int entityId);

        Task<TModel> GetByEntityIdAndCategoryIdAsync(int entityId, int categoryId);
        
        Task<bool> DeleteByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAndCategoryIdAsync(int entityId, int categoryId);

    }

}
