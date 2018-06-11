﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Plato.Abstractions.Settings;

using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Data;
using Plato.Internal.Data.Abstractions;

using Plato.Internal.Models.Roles;
using Plato.Internal.Stores.Settings;
using Plato.Internal.Stores.Roles;
using Plato.Internal.Stores.Users;
using Plato.Internal.Models.Users;

using Plato.Internal.Stores.Files;
using Plato.Internal.Stores.Query;

namespace Plato.Internal.Stores.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddStores(
         this IServiceCollection services)
        {
            
         
            services.AddScoped<IDbQuery, DbQuery>();

            // files

            services.AddScoped<IFileStore, FileStore>();

            // settings

            services.AddScoped<ISiteSettingsStore, SiteSettingsStore>();
            
            // roles

            services.AddScoped<IPlatoRoleStore, PlatoRoleStore>();
         
            // users

            services.AddScoped<IPlatoUserStore<User>, PlatoUserStore>();
            services.AddScoped<IUserPhotoStore<UserPhoto>, UserPhotoStore>();
            services.AddScoped<IUserBannerStore<UserBanner>, UserBannerStore>();

            return services;


        }

    }
}
