// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $(".ajax-pagination-container").on("click", ".page-link", function (e) {
        e.preventDefault();
        const url = $(this).attr("href");

        if (url) {
            $(".ajax-pagination-container").load(url);
        }
    });

});