﻿using System;
using PlatoCore.Abstractions.Extensions;

namespace PlatoCore.Layout.Views.Abstractions
{

    public interface IPositionedView : IView
    {

        ViewPosition Position { get; }

        IPositionedView Zone(string zone);

        IPositionedView Order(int order);

    }

    public class PositionedView : View, IPositionedView
    {

        private string _zone;
        private int _order;

        public PositionedView()
        {
            _zone = LayoutZones.ContentZoneName;
            _order = 1;
        }

        public PositionedView(string viewName, object model) : 
            base(viewName, model)
        {
            _zone = LayoutZones.ContentZoneName;
            _order = 1;
        }

        public ViewPosition Position => new ViewPosition(_zone, _order);
        
        public IPositionedView Zone(string zone)
        {

            // We already expect a zone
            if (string.IsNullOrEmpty(zone))
            {
                throw new Exception(
                    $"No zone has been specified for the view {ViewName}.");
            }

            // Is the zone supported?            
            if (!LayoutZones.SupportedZones.Contains(zone, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    $"The zone name '{zone}' is not supported. Supported zones include {String.Join(", ", LayoutZones.SupportedZones)}. Please update the zone name within your view provider and try again.");
            }

            _zone = zone;
            return this;
        }

        public IPositionedView Order(int order)
        {
            _order = order;
            return this;
        }

    }

}
