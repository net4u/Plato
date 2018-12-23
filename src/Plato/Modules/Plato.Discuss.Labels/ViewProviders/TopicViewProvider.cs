﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Plato.Discuss.Labels.Models;
using Plato.Entities.Stores;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Labels.Models;
using Plato.Labels.Stores;
using Plato.Discuss.Labels.ViewModels;
using Plato.Discuss.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Labels.Services;

namespace Plato.Discuss.Labels.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        private const string LabelHtmlName = "label";

        private readonly ILabelStore<Label> _labelStore;
        private readonly IEntityLabelManager<EntityLabel> _entityLabelManager;
        private readonly IEntityLabelStore<EntityLabel> _entityLabelStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly IStringLocalizer T;
        private readonly IFeatureFacade _featureFacade;

        private readonly HttpRequest _request;

        public TopicViewProvider(
            IContextFacade contextFacade,
            ILabelStore<Label> labelStore, 
            IEntityStore<Topic> entityStore,
            IHttpContextAccessor httpContextAccessor,
            IEntityLabelStore<EntityLabel> entityLabelStore,
            IStringLocalizer<TopicViewProvider> stringLocalize,
            IFeatureFacade featureFacade, 
            IEntityLabelManager<EntityLabel> entityLabelManager)
        {
            _contextFacade = contextFacade;
            _labelStore = labelStore;
            _entityStore = entityStore;
            _entityLabelStore = entityLabelStore;
            _request = httpContextAccessor.HttpContext.Request;
            T = stringLocalize;
            _featureFacade = featureFacade;
            _entityLabelManager = entityLabelManager;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(Topic viewModel, IViewProviderContext updater)
        {

            // Ensure we explictly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Labels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            // Get top 10 labels
            var labels = await _labelStore.QueryAsync()
                .Take(1, 10)
                .Select<LabelQueryParams>(q =>
                {
                    q.FeatureId.Equals(feature.Id);
                })
                .OrderBy("TotalEntities", OrderBy.Desc)
                .ToList();

            return Views(View<LabelsViewModel>("Topic.Labels.Index.Sidebar", model =>
                {
                    model.Labels = labels?.Data;
                    return model;
                }).Zone("sidebar").Order(2)
            );
            

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Topic viewModel, IViewProviderContext updater)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Labels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }
            
            // Get entity labels
            var labels = await _labelStore.QueryAsync()
                .Take(1, 10)
                .Select<LabelQueryParams>(q =>
                {
                    q.EntityId.Equals(viewModel.Id);
                })
                .OrderBy("Name", OrderBy.Desc)
                .ToList();
            
            return Views(
                View<LabelsViewModel>("Topic.Labels.Display.Sidebar", model =>
                {
                    model.Labels = labels?.Data;
                    return model;
                }).Zone("sidebar").Order(3)
            );

        }
        
        public override async Task<IViewProviderResult> BuildEditAsync(Topic topic, IViewProviderContext updater)
        {

            var entityLabels = await GetEntityLabelsByEntityIdAsync(topic.Id);
            var viewModel = new EditTopicLabelsViewModel()
            {
                HtmlName = LabelHtmlName,
                SelectedLabels = entityLabels.Select(l => l.LabelId).ToArray()
            };
            
            return Views(
                View<EditTopicLabelsViewModel>("Topic.Labels.Edit.Sidebar", model => viewModel).Zone("sidebar")
                    .Order(2)
            );

        }

        //public override Task ComposeTypeAsync(Topic model, IUpdateModel updater)
        //{
        //    return base.ComposeTypeAsync(model, updater);
        //}

        public override Task<bool> ValidateModelAsync(Topic topic, IUpdateModel updater)
        {
            // ensure labels are optional
            return Task.FromResult(true);
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Topic topic, IViewProviderContext context)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(topic.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(topic, context);
            }

            // Validate model
            if (await ValidateModelAsync(topic, context.Updater))
            {

                // Get selected labels
                //var labelsToAdd = GetLabelsToAdd();
                var labelsToAdd = await GetLabelsToAddAsync();
                
                // Build labels to remove
                var labelsToRemove = new List<EntityLabel>();
                foreach (var entityLabel in await GetEntityLabelsByEntityIdAsync(topic.Id))
                {
                    // Entry already exists remove from labels to add
                    if (labelsToAdd.Contains(entityLabel.LabelId))
                    {
                        labelsToAdd.Remove(entityLabel.LabelId);
                    }
                    else
                    {
                        // Entry does NOT exist in labels to add ensure it's removed
                        labelsToRemove.Add(entityLabel);
                    }
                }

                // Remove entity labels
                foreach (var entityLabel in labelsToRemove)
                {
                    var result = await _entityLabelManager.DeleteAsync(entityLabel);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

                // Add new entity labels
                foreach (var labelId in labelsToAdd)
                {
                    var result = await _entityLabelManager.CreateAsync(new EntityLabel()
                    {
                        EntityId = topic.Id,
                        LabelId = labelId
                    });
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

            }

            return await BuildEditAsync(topic, context);

        }
        
        #endregion

        #region "Private Methods"
         
        async Task<List<int>> GetLabelsToAddAsync()
        {
            // Build selected channels
            var labelsToAdd = new List<int>();
            foreach (var key in _request.Form.Keys)
            {
                if (key.Equals(LabelHtmlName))
                {
                    var value = _request.Form[key];
                    if (!String.IsNullOrEmpty(value))
                    {
                        var items = JsonConvert.DeserializeObject<IEnumerable<LabelApiResult>>(value);
                        foreach (var item in items)
                        {
                            if (item.Id > 0)
                            {
                                var label = await _labelStore.GetByIdAsync(item.Id);
                                if (label != null)
                                {
                                    labelsToAdd.Add(label.Id);
                                }
                            }
                        }
                    }
                 
                }
            }

            return labelsToAdd;
        }
        
        async Task<IEnumerable<EntityLabel>> GetEntityLabelsByEntityIdAsync(int entityId)
        {

            if (entityId == 0)
            {
                // return empty collection for new topics
                return new List<EntityLabel>();
            }

            return await _entityLabelStore.GetByEntityId(entityId) ?? new List<EntityLabel>();

        }

        #endregion



    }
}
