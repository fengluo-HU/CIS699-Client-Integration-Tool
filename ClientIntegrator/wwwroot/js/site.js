// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function ($, window) {
    "use strict";
    $(document).ready(function () {
        // If login
        if ($("#logoutButton").length > 0) {
            window.addEventListener("storage", logout);
            var idleTimer = null;
            // Track user actions(15 minutes)
            $("*").bind(
                "mousemove click mouseup mousedown keydown keypress keyup submit change mouseenter scroll resize dblclick",
                function () {
                    clearTimeout(idleTimer);
                    idleTimer = setTimeout(function () {
                        $("#logoutButton").trigger("click");
                        var eventValue = "logout" + Math.random();
                        localStorage.setItem("logout-event", eventValue);
                    }, 10000);
                });
            $("body").trigger("mousemove");
        } else {
            window.removeEventListener("storage", logout);
        }
        // Trigger logout button
        function logout(event) {
            if (event.key === "logout-event") {
                $("#logoutButton").trigger("click");
            }
        };

        $('.orgLink.dropdown-item').on('click', function (e) {
            e.preventDefault(); // stop the standard link behaviour
            $.ajax({
                url: $(this).attr('href'),
                type: 'Post',
                cache: false,
                headers: {
                    RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
                }
            }).done(function (result) {
                window.location.href = window.location.href.split("?")[0];
            });
        });
    });
})(jQuery, window);
