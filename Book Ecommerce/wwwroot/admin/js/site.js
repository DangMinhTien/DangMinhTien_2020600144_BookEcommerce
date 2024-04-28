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
    // Product Quantity Start

    // Product Quantity End
    
    $(window).resize(function () {
        $('.preview-image').each(function (index, element) {
            var width = $(element).width()
            $(element).height(width * 1.2)
        })
        console.log('Đã xảy ra sự kiện thay đổi kích thước màn hình');
    });
})