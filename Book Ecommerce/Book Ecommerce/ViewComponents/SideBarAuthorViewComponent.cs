﻿using Book_Ecommerce.Domain.ViewModels.AuthorViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Book_Ecommerce.Data;
using Book_Ecommerce.Service.Abstract;
using Book_Ecommerce.Service;

namespace Book_Ecommerce.ViewComponents
{
    public class SideBarAuthorViewComponent : ViewComponent
    {
        private readonly IAuthorService _authorService;

        public SideBarAuthorViewComponent(IAuthorService authorService)
        {
            _authorService = authorService;
        }
        public async Task<IViewComponentResult> InvokeAsync(string? AuthorId)
        {
            var authors = await _authorService.GetToViewComponentAsync();
            var result = authors.Select(a => new AuthorVM
            {
                AuthorId = a.AuthorId,
                AuthorName = a.AuthorName,
                AuthorCode = a.AuthorCode,
                AuthorSlug = a.AuthorSlug,
                SumProduct = a.AuthorProducts != null ? a.AuthorProducts.Count() : 0,
                IsActive = a.AuthorId == AuthorId ? true : false,
                Information = a.Information
            }).ToList();
            return View(result);
        }
    }
}
