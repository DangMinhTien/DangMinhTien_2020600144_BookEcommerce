// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Toast Start
$(document).ready(function () {
    $('.m-toast-box').on('click', '.m-toast-close', function () {
        var toast = $(this).closest('.m-toast-message')
        toast.addClass('hidden')
        setTimeout(function () {
            toast.remove()
        }, 2000)
    })
    $('.m-toast-message').show(function () {
        var toast = $(this)
        setTimeout(function () {
            toast.addClass('hidden')
            setTimeout(function () {
                toast.remove()
            }, 2000)
        }, 5000)
    })
    // Toast End
    // Img Product Start
    $('.img-product').each(function (index, element) {
        var width = $(element).width()
        $(element).height(width * 1.2)
    })
    $(window).resize(function () {
        $('.img-product').each(function (index, element) {
            var width = $(element).width()
            $(element).height(width * 1.2)
        })
        console.log('Đã xảy ra sự kiện thay đổi kích thước màn hình');
    });
    // Img Product End
    // Product Quantity Start
    
    // Product Quantity End
})