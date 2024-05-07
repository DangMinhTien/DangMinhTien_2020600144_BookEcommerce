﻿using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.UserViewModel;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public UserService(IUnitOfWork unitOfWork,
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IQueryable<AppUser> Table()
        {
            return _unitOfWork.UserRepository.Table();
        }
        public async Task<(IdentityResult, AppUser)> RegisterCustomerAccountAsync(RegisterVM registerVM)
        {
            var codeNumber = _unitOfWork.CustomerRepository.Table().Count() > 0 ? 
                _unitOfWork.CustomerRepository.Table().Max(c => c.CodeNumber) + 1 : 1000;
            var customer = new Customer
            {
                CustomerId = Guid.NewGuid().ToString(),
                FullName = registerVM.FullName,
                CodeNumber = codeNumber,
                CustomerCode = "KH" + DateTime.Now.Year.ToString() + codeNumber,
                Gender = registerVM.Gender,
                Address = registerVM.Address,
                DateOfBirth = registerVM.DateOfBirth,
            };
            await _unitOfWork.CustomerRepository.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            var user = new AppUser
            {
                UserName = registerVM.Email,
                Email = registerVM.Email,
                PhoneNumber = registerVM.PhoneNumber,
                CustomerId = customer.CustomerId
            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            return (result, user);
        }
        public async Task<AppUser?> GetSingleByConditionAsync(Expression<Func<AppUser, bool>> expression)
        {
            return await _unitOfWork.UserRepository.GetSingleByConditionAsync(expression);
        }
    }
}