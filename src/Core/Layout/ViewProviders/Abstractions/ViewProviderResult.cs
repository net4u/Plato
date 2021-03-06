﻿using System.Collections.Generic;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.ViewProviders.Abstractions
{

    public class ViewProviderResult : IViewProviderResult
    {

        public IEnumerable<IView> Views { get; private set; }

        public ViewProviderResult(params IView[] views)
        {
            Views = views ?? ((IEnumerable<IView>) new List<IView>());
        }

        public ViewProviderResult(params ILayoutZoneView[] views)
        {
            Views = views ?? ((IEnumerable<IView>) new List<IView>());
        }

    }

}
