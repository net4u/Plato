﻿// <reference path="/wwwroot/js/app.js" />

if (typeof $().modal === 'undefined') {
    throw new Error("BootStrap 4.1.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

$(function (win, doc, $) {

    "use strict";

    // --------

    var app = win.$.Plato,
        featureId = "Plato.Articles.Drafts";

    // --------

    var drafts = {
        init: function () {
            app.logger.logInfo(featureId + " initializing");
            this.bind();
            app.logger.logInfo(featureId + " initialized");
        },
        bind: function () {
            
        }
    };

    // --------

    app.ready(function () {
        drafts.init();
    });
    
}(window, document, jQuery));
