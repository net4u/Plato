﻿using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class ChannelViewProvider : BaseViewProvider<ChannelIndexViewModel>
    {

        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly ICategoryManager<Category> _categoryManager;

        public ChannelViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<Category> categoryStore,
            ICategoryManager<Category> categoryManager)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _categoryManager = categoryManager;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(ChannelIndexViewModel indexViewModel, IUpdateModel updater)
        {

            // Ensure we explictly set the featureId
            var feature = await _contextFacade.GetFeatureByModuleIdAsync("Plato.Discuss.Channels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);

            Category category = null;
            if (indexViewModel.FilterOpts.ChannelId > 0)
            {
                category = await _categoryStore.GetByIdAsync(indexViewModel.FilterOpts.ChannelId);
            }


            return Views(
                View<Category>("Home.Index.Header", model => category).Zone("header").Order(1),
                View<Category>("Home.Index.Tools", model => category).Zone("tools").Order(1),
                View<ChannelIndexViewModel>("Home.Index.Content", model => indexViewModel).Zone("content").Order(1),
                View<ChannelsViewModel>("Discuss.Index.Sidebar", model =>
                {
                    model.Channels = categories;
                    return model;
                }).Zone("sidebar").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(ChannelIndexViewModel indexViewModel, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildEditAsync(ChannelIndexViewModel category, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(ChannelIndexViewModel category, IUpdateModel updater)
        {

            return Task.FromResult(default(IViewProviderResult));


        }

        #endregion

        #region "Private Methods"

    
        #endregion

    }
}
