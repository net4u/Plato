﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Localization.Locales
{

    public class LocaleProvider : ILocaleProvider
    {

        private const string LocaleFolderName = "Locales";

        private static IEnumerable<ComposedLocaleDescriptor> _composedLocaleDescriptors;
        private static IEnumerable<LocaleDescriptor> _localeDescriptors;
        
        private readonly ILocaleCompositionStrategy _compositionStrategy;
        private readonly IModuleManager _moduleManager;
        private readonly ILocaleLocator _localeLocator;

        public LocaleProvider(
            ILocaleCompositionStrategy compositionStrategy,
            IModuleManager moduleManager,
            ILocaleLocator localeLocator)
        {
            _compositionStrategy = compositionStrategy;
            _moduleManager = moduleManager;
            _localeLocator = localeLocator;
        }

        public async Task<IEnumerable<ComposedLocaleDescriptor>> GetLocalesAsync()
        {

            // Ensure local descriptors are only composed once 
            if (_composedLocaleDescriptors == null)
            {

                // Compose locales
                var output = new List<ComposedLocaleDescriptor>();
                foreach (var localeDescriptor in await GetAvailableLocaleDescriptors())
                {
                    output.Add(await _compositionStrategy.ComposeLocaleDescriptorAsync(localeDescriptor));
                }

                _composedLocaleDescriptors = output;

            }

            return _composedLocaleDescriptors;

        }

        async Task<IEnumerable<LocaleDescriptor>> GetAvailableLocaleDescriptors()
        {

            // Build paths to locale descriptors
            var paths = await GetLocaleDescriptorPathsToSearch();

            // Load descriptors or reuse if already loaded
            return _localeDescriptors ?? (_localeDescriptors = await _localeLocator.LocateLocalesAsync(paths));

        }

        async Task<string[]> GetLocaleDescriptorPathsToSearch()
        {

            // Initialize with root "Locales" folder
            var output = new List<string>()
            {
                LocaleFolderName
            };

            // Append module locations to paths
            var moduleDescriptors = await _moduleManager.LoadModulesAsync();
            foreach (var module in moduleDescriptors)
            {
                var modulePath = module.Descriptor.VirtualPathToModule;
                if (!modulePath.EndsWith("\\"))
                {
                    modulePath += "\\";
                }
                output.Add(modulePath + LocaleFolderName);
            }

            return output.ToArray();

        }

    }

}