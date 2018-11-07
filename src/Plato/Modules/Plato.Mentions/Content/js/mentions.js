﻿
if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("$.Plato.Context Required");
}

$(function (win, doc, $) {

    'use strict';

    // keyBinder
    var keyBinder = function () {

        var dataKey = "keyBinder",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "keyup", // use keyup to ensure text to test against expressions has been entered
            keys: [
                {
                    match: /(^|\s|\()(@([a-z0-9\-_/]*))$/i,
                    search: function ($input) {},
                    bind: function ($input) {}
                },
                {
                    match: /(^|\s|\()(#([a-z0-9\-_/]*))$/i,
                    search: function ($input) { },
                    bind: function ($input) { }
                 }
            ]
        };

        var methods = {
            init: function ($caller, methodName) {

                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                this.bind($caller);
            },
            bind: function($caller) {

                var key = null,
                    keys = $caller.data(dataKey).keys,
                    event = $caller.data(dataKey).event;

                $caller.bind(event, function(e) {
                        for (var i = 0; i < keys.length; i++) {
                            key = keys[i];
                            var match = false,
                                selection = methods.getSelection($(this)),
                                search = key.search 
                                    ? key.search($(this), selection).value
                                    : $(this).val();
                          
                            if (search) {
                                if (key.match) {
                                    match = key.match.test(search);
                                }
                            }
                            if (match) {
                                if (key.bind) {
                                    key.bind($(this), search, e);
                                }
                            } else {
                                if (key.unbind) {
                                    key.unbind($(this), e);
                                }
                                
                            }
                        }
                    });

            },
            unbind: function ($caller) {
                $caller.unbind($caller.data(dataKey).event);
            },
            getSelection: function ($caller) {

                var e = $caller[0];

                return (

                    ('selectionStart' in e && function () {
                            var l = e.selectionEnd - e.selectionStart;
                            return {
                                start: e.selectionStart,
                                end: e.selectionEnd,
                                length: l,
                                text: e.value.substr(e.selectionStart, l)
                            };
                        }) ||

                        /* browser not supported */
                        function () {
                            return null;
                        }

                )();

            }
        }

        return {
            init: function () {

                var options = {};
                var methodName = null;
                for (var i = 0; i < arguments.length; ++i) {
                    var a = arguments[i];
                    if (a) {
                        switch (a.constructor) {
                        case Object:
                            $.extend(options, a);
                            break;
                        case String:
                            methodName = a;
                            break;
                        case Boolean:
                            break;
                        case Number:
                            break;
                        case Function:
                            break;
                        }
                    }
                }

                if (this.length > 0) {
                    // $(selector).keyBinder
                    return this.each(function () {
                        if (!$(this).data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $(this).data(dataIdKey, id);
                            $(this).data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                        }
                        methods.init($(this), methodName);
                    });
                } else {
                    // $().keyBinder
                    if (methodName) {
                        if (methods[methodName]) {
                            var $caller = $("body");
                            $caller.data(dataKey, $.extend({}, defaults, options));
                            methods[methodName].apply(this, [$caller]);
                        } else {
                            alert(methodName + " is not a valid method!");
                        }
                    }
                }

            }

        }

    }();

    // textFieldMirror
    var textFieldMirror = function () {

        var dataKey = "textFieldMirror",
            dataIdKey = dataKey + "Id";

        var defaults = {
            start: 0,
            ready: function($caller) {

            }
        };

        var methods = {
            init: function ($caller, methodName) {

                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                this.bind($caller);
            },
            bind: function ($caller) {
                
                var start = $caller.data(dataKey).start,
                    prefix = $caller.val().substring(0, start),
                    suffix = $caller.val().substring(start, $caller.val().length - 1),
                    marker = '<span class="text-field-mirror-marker position-relative">@</span>',
                    markerHtml = prefix + marker + suffix,
                    html = markerHtml.replace(/\n/gi, '<br/>');

                var $mirror = methods.getOrCreate($caller);
                if ($mirror) {

                    // Populate & show mirror
                    $mirror.html(html).show();

                    // Ensure mirror is always same height as called
                    $mirror.css({
                        "height": $caller.outerHeight()
                    });

                    // Ensure mirror is always scrolled to same position as calller
                    $mirror[0].scrollTop = $caller.scrollTop();

                    // Marker added raise ready event
                    if ($caller.data(dataKey).ready) {
                        $caller.data(dataKey).ready($mirror);
                    }

                }
                
            },
            hide: function ($caller) {
                var $mirror = this.getOrCreate($caller);
                if ($mirror) {
                    $mirror.hide();
                }
            },
            getOrCreate: function($caller) {
                var id = $caller.data(dataIdKey) + "Mirror",
                    $mirror = $("#" + id);
                if ($mirror.length === 0) {
                    $mirror = $('<div>',
                            {
                                "id": id,
                                "class": "form-control text-field-mirror"
                            })
                        .css({
                            "height": $caller.outerHeight()
                        });
                    $caller.before($mirror);
                }
                return $mirror;
            }
        }

        return {
            init: function () {

                var options = {};
                var methodName = null;
                for (var i = 0; i < arguments.length; ++i) {
                    var a = arguments[i];
                    if (a) {
                        switch (a.constructor) {
                            case Object:
                                $.extend(options, a);
                                break;
                            case String:
                                methodName = a;
                                break;
                            case Boolean:
                                break;
                            case Number:
                                break;
                            case Function:
                                break;
                        }
                    }
                }

                if (this.length > 0) {
                    // $(selector).textFieldMirror
                    return this.each(function () {
                        if (!$(this).data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $(this).data(dataIdKey, id);
                            $(this).data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                        }
                        methods.init($(this), methodName);
                    });
                } else {
                    // $().textFieldMirror
                    if (methodName) {
                        if (methods[methodName]) {
                            var $caller = $("body");
                            $caller.data(dataKey, $.extend({}, defaults, options));
                            methods[methodName].apply(this, [$caller]);
                        } else {
                            alert(methodName + " is not a valid method!");
                        }
                    }
                }

            }

        }

    }();
    
    // suggester
    var suggester = function () {

        var dataKey = "suggester",
            dataIdKey = dataKey + "Id";

        var defaults = {};

        var methods = {
            init: function ($caller, methodName) {

                if (methodName) {
                    if (this[methodName]) {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                this.bind($caller);

            },
            bind: function ($caller) {
               
                // Track @mention pattern
                $caller.keyBinder($caller.data(dataKey));

                // Handle arrow keys when menu is active
                $caller.bind("keydown",
                    function (e) {
                        var $menu = methods.getOrCreateMenu($(this));
                        if ($menu) {
                            if ($menu.is(":visible")) {
                                var pageSize = $menu.data("pagedList").pageSize,
                                    itemCss = $menu.data("pagedList").itemCss,
                                    itemSelection = $menu.data("pagedList").itemSelection,
                                    newIndex = -1;

                                if (itemSelection.enable) {
                                    switch (e.which) {
                                        case 13: // carriage return
                                            e.preventDefault();
                                            e.stopPropagation();
                                            // find active and click
                                            $menu.find("." + itemCss).each(function() {
                                                if ($(this).hasClass(itemSelection.css)) {
                                                    $(this).click();
                                                }
                                            });
                                            break;
                                        case 38: // up
                                            e.preventDefault();
                                            e.stopPropagation();
                                            newIndex = itemSelection.index - 1;
                                            if (newIndex < 0) {
                                                newIndex = 0;
                                            }
                                            break;
                                        case 40: // down
                                            e.preventDefault();
                                            e.stopPropagation();
                                            newIndex = itemSelection.index + 1;
                                            if (newIndex > (pageSize - 1)) {
                                                newIndex = (pageSize - 1);
                                            }
                                            break;
                                    }
                                    if (newIndex >= 0) {
                                        $menu.pagedList({
                                            itemSelection: $.extend(itemSelection,
                                                {
                                                    index: newIndex
                                                })
                                        }, "setItemIndex");
                                    }
                                  
                                }

                            
                            }
                        }
                    });
                
                // Wrap a relative wrapper around the input to
                // correctly position the absolutely positioned mention menu
                $caller.wrap($('<div class="position-relative"></div>'));

                // Hide menu on click & scroll
                $caller.bind("click scroll",
                    function() {
                        var $menu = methods.getOrCreateMenu($(this));
                        if ($menu) {
                            $menu.hide();
                        }
                    });

            },
            unbind: function($caller) {
                $caller.keyBinder("unbind");
            },
            show: function ($caller) {
                
                // Ensure our input has focus
                $caller.focus();

                // Get cursor selection
                var cursor = this.getSelection($caller);

                // Invoke text field mirror to correctly position menu
                $caller.textFieldMirror({
                    start: cursor.start,
                    ready: function ($mirror) {

                        // Get position from mirrored marker
                        var $marker = $mirror.find(".text-field-mirror-marker"),
                            position = $marker.position(),
                            left = Math.floor(position.left),
                            top = Math.floor(position.top + 26);

                        // Build & position menu
                        var $menu = methods.getOrCreateMenu($caller);
                        $menu.css({
                            "left": left + "px",
                            "top": top + "px"
                        }).show();

                        // Hide mirror after positioning menu
                        $caller.textFieldMirror("hide");

                        // Invoke paged list
                        $menu.pagedList($caller.data(dataKey));

                    }
                });
                
            },
            hide: function ($caller) {
                var $menu = this.getOrCreateMenu($caller);
                $menu.hide();
            },
            getOrCreateMenu: function ($caller) {
                var id = $caller.attr("id") + "SuggesterDropDown",
                    $menu = $("#" + id);
                if ($menu.length === 0) {
                    $menu = $("<div>",
                        {
                            "id": id,
                            "class": "dropdown-menu col-6",
                            "role": "menu"
                        });
                    $caller.after($menu);
                }
                return $menu;
            },
            getSelection: function ($caller) {

                var e = $caller[0];

                return (

                    ('selectionStart' in e && function () {
                            var l = e.selectionEnd - e.selectionStart;
                            return {
                                start: e.selectionStart,
                                end: e.selectionEnd,
                                length: l,
                                text: e.value.substr(e.selectionStart, l)
                            };
                        }) ||

                        /* browser not supported */
                        function () {
                            return null;
                        }

                )();

            },
            setSelection: function ($caller, start, end) {

                var e = $caller[0];

                return (

                    ('selectionStart' in e && function () {
                            e.selectionStart = start;
                            e.selectionEnd = end;
                            return;
                        }) ||

                        /* browser not supported */
                        function () {
                            return null;
                        }

                )();

            },
            replaceSelection: function ($caller, text) {

                var e = $caller[0];

                return (

                    ('selectionStart' in e && function () {
                            e.value = e.value.substr(0, e.selectionStart) + text + e.value.substr(e.selectionEnd, e.value.length);
                            // Set cursor to the last replacement end
                            e.selectionStart = e.value.length;
                            return this;
                        }) ||

                        /* browser not supported */
                        function () {
                            e.value += text;
                            return jQuery(e);
                        }

                )();
            },
        }

        return {
            init: function () {

                var options = {};
                var methodName = null;
                for (var i = 0; i < arguments.length; ++i) {
                    var a = arguments[i];
                    if (a) {
                        switch (a.constructor) {
                            case Object:
                                $.extend(options, a);
                                break;
                            case String:
                                methodName = a;
                                break;
                            case Boolean:
                                break;
                            case Number:
                                break;
                            case Function:
                                break;
                        }
                    }
                }

                if (this.length > 0) {
                    // $(selector).suggester
                    return this.each(function () {
                        if (!$(this).data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $(this).data(dataIdKey, id);
                            $(this).data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                        }
                        methods.init($(this), methodName);
                    });
                } else {
                    // $().suggester
                    if (methodName) {
                        if (methods[methodName]) {
                            var $caller = $("body");
                            $caller.data(dataKey, $.extend({}, defaults, options));
                            methods[methodName].apply(this, [$caller]);
                        } else {
                            alert(methodName + " is not a valid method!");
                        }
                    }
                }

            }

        }

    }();

    // mentions
    var mentions = function () {

        var dataKey = "mentions",
            dataIdKey = dataKey + "Id";

        var defaults = {};

        var methods = {
            init: function($caller, methodName) {

                if (methodName) {
                    if (this[methodName]) {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                this.bind($caller);

            },
            bind: function($caller) {

                $caller.suggester($.extend($caller.data(dataKey)),
                    {
                        // keyBinder options
                        event: "keyup",
                        keys: [
                            {
                                event: "keyup",
                                match: /(^|\s|\()(@([a-z0-9\-_/]*))$/i,
                                search: function($input, selection) {

                                    // The result of the search method is tested
                                    // against the match regular expiression within keyBinder
                                    // If a match is found the bind method is called 
                                    // otherwise unbind method is called
                                    // This code executes on everykey press so should be optimized

                                    var chars = $input.val().split(''),
                                        value = "",
                                        marker = "@",
                                        markerIndex = -1,
                                        i = 0;

                                    // Search backwards from caret for marker, until 
                                    // terminators & attempt to get marker index
                                    for (i = selection.start - 1; i >= 0; i--) {
                                        if (chars[i] === marker) {
                                            markerIndex = i;
                                            break;
                                        } else {
                                            if (chars[i] === "\n" || chars[i] === " ") {
                                                break;
                                            }
                                        }
                                    }

                                    // If we have a marker search forward 
                                    // from marker until terminator to get value
                                    if (markerIndex >= 0) {
                                        for (i = markerIndex; i <= chars.length; i++) {
                                            if (chars[i] === "\n" || chars[i] === " ") {
                                                break;
                                            }
                                            value += chars[i];
                                        }
                                    }

                                    return {
                                        markerIndex: markerIndex,
                                        value: value
                                    };

                                },
                                bind: function ($input, search, e) {
                                
                                    switch (e.which) {
                                        case 13: // carriage return
                                            return;
                                        case 38: // up
                                            return;
                                        case 40: // down
                                            return;
                                        default:

                                                // Remove any @ prefix from search string
                                                if (search.substring(0, 1) === "@") {
                                                    search = search.substring(1, search.length);
                                                }
                                                
                                                $caller.suggester({ // pagedList options
                                                        pageSize: 5,
                                                        itemSelection: {
                                                            enable: true,
                                                            index: 0,
                                                            css: "active"
                                                        },
                                                        valueField: "keywords",
                                                        config: {
                                                            method: "GET",
                                                            url:
                                                                'api/users/get?page={page}&size={pageSize}&keywords=' +
                                                                    encodeURIComponent(search),
                                                            data: {
                                                                sort: "LastLoginDate",
                                                                order: "Desc"
                                                            }
                                                        },
                                                        itemCss: "dropdown-item",
                                                        itemTemplate:
                                                            '<a class="{itemCss}" href="{url}"><span class="avatar avatar-sm mr-2"><span style="background-image: url(/users/photo/{id});"></span></span>{displayName}<span class="float-right">@{userName}</span></a>',
                                                        parseItemTemplate: function(html, result) {

                                                            if (result.id) {
                                                                html = html.replace(/\{id}/g, result.id);
                                                            } else {
                                                                html = html.replace(/\{id}/g, "0");
                                                            }

                                                            if (result.displayName) {
                                                                html = html.replace(/\{displayName}/g, result.displayName);
                                                            } else {
                                                                html = html.replace(/\{displayName}/g, "(no username)");
                                                            }
                                                            if (result.userName) {
                                                                html = html.replace(/\{userName}/g, result.userName);
                                                            } else {
                                                                html = html.replace(/\{userName}/g, "(no username)");
                                                            }

                                                            if (result.email) {
                                                                html = html.replace(/\{email}/g, result.email);
                                                            } else {
                                                                html = html.replace(/\{email}/g, "");
                                                            }
                                                            if (result.agent_url) {
                                                                html = html.replace(/\{url}/g, result.url);
                                                            } else {
                                                                html = html.replace(/\{url}/g, "#");
                                                            }
                                                            return html;

                                                        },
                                                        onItemClick: function($self, result, e) {

                                                            e.preventDefault();
                                                            //$menu.hide();
                                                            $caller.focus();

                                                            var sel = methods.getSelection($caller),
                                                                value = result.userName + " ",
                                                                cursor = (markerIndex + 1) + value.length;

                                                            // Select everything from marker
                                                            methods.setSelection($caller, markerIndex + 1, sel.start);

                                                            // Replace selection with username
                                                            methods.replaceSelection($caller, value);

                                                            // Place cursor at end of new @mention 
                                                            methods.setSelection($caller, cursor, cursor);

                                                        }
                                                    },
                                                    "show");

                                            break;
                                        }
                                    
                                },
                                unbind: function ($input, selection, e) {
                                    $caller.suggester("hide");
                                }
                            }
                        ]
                    });
            }
        }

        return {
            init: function () {

                var options = {};
                var methodName = null;
                for (var i = 0; i < arguments.length; ++i) {
                    var a = arguments[i];
                    if (a) {
                        switch (a.constructor) {
                            case Object:
                                $.extend(options, a);
                                break;
                            case String:
                                methodName = a;
                                break;
                            case Boolean:
                                break;
                            case Number:
                                break;
                            case Function:
                                break;
                        }
                    }
                }

                if (this.length > 0) {
                    // $(selector).mentions
                    return this.each(function () {
                        if (!$(this).data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $(this).data(dataIdKey, id);
                            $(this).data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                        }
                        methods.init($(this), methodName);
                    });
                } else {
                    // $().mentions
                    if (methodName) {
                        if (methods[methodName]) {
                            var $caller = $("body");
                            $caller.data(dataKey, $.extend({}, defaults, options));
                            methods[methodName].apply(this, [$caller]);
                        } else {
                            alert(methodName + " is not a valid method!");
                        }
                    }
                }

            }

        }

    }();
    
    $.fn.extend({
        keyBinder: keyBinder.init,
        textFieldMirror: textFieldMirror.init,
        suggester: suggester.init,
        mentions: mentions.init
    });

    $(doc).ready(function () {
        
        $('[data-provide="mentions"]').mentions();

        $('.md-textarea').mentions();

    });

}(window, document, jQuery));