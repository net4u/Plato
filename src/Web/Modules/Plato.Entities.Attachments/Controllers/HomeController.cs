﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Layout.ModelBinding;
using Plato.Entities.Attachments.ViewModels;

namespace Plato.Entities.Attachments.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        public HomeController()
        { 
        }

        // -----------
        // Index
        // -----------

        public Task<IActionResult> Index(EntityAttachmentOptions opts)
        {

            if (opts == null)
            {
                opts = new EntityAttachmentOptions();
            }

            // We always need a guid
            if (string.IsNullOrEmpty(opts.Guid))
            {
                throw new ArgumentNullException(nameof(opts.Guid));
            }

            // Return view
            return Task.FromResult((IActionResult)View(opts));


        }

        // -----------
        // Preview
        // -----------

        public Task<IActionResult> Preview(EntityAttachmentOptions opts)
        {

            if (opts == null)
            {
                opts = new EntityAttachmentOptions();
            }

            // We always need a guid
            if (string.IsNullOrEmpty(opts.Guid))
            {
                throw new ArgumentNullException(nameof(opts.Guid));
            }

            // Return view
            return Task.FromResult((IActionResult) View(opts));

        }

    }

}
