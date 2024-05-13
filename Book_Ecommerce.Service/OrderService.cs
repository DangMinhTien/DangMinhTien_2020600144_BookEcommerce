using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Book_Ecommerce.Domain.ViewModels.CustomerViewModel;
using Microsoft.EntityFrameworkCore;
using Book_Ecommerce.Domain.ViewModels.OrderViewModel;
using PayPal.v1.Payments;
using System.Linq.Expressions;

namespace Book_Ecommerce.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUrlHelper _urlHelper;
        private List<PageSizeModel> lstPageSize;

        public OrderService(IUnitOfWork unitOfWork,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor action)
        {
            _unitOfWork = unitOfWork;
            _urlHelper = urlHelperFactory.GetUrlHelper(action.ActionContext ?? new ActionContext());
            lstPageSize = new List<PageSizeModel>
            {
                new PageSizeModel
                {
                    Size = MyAppSetting.PAGE_SIZE,
                },
                new PageSizeModel
                {
                    Size = 30,
                },
                new PageSizeModel
                {
                    Size = 50,
                }
            };
        }
        public async Task<(IEnumerable<OrderVM>, PagingModel, IEnumerable<PageSizeModel>)>
           GetToViewManageAsync(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var query = _unitOfWork.OrderRepository.Table()
                                                    .Include(o => o.Customer)
                                                    .Include(o => o.OrderDetails)
                                                    .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.OrderCode.Contains(search));
            }
            query = query.OrderBy(c => c.CodeNumber);
            #region bắt đầu phân trang
            var totalItem = query.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page > totalPage)
                page = totalPage;
            if (page < 1)
                page = 1;
            var orders = await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync();
            var orderVMs = orders.Select(o => new OrderVM
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                DateCreated = o.DateCreated,
                FullName = o.FullName,
                PhoneNumber = o.PhoneNumber,
                Address = o.Address,
                Status = o.Status,
                TransportFee = o.TransportFee,
                PaymentType = o.PaymentType,
                Note = o.Note,
                DateDelivery = o.DateDelivery,
                Customer = o.Customer,
                SumQuantity = o.OrderDetails.Sum(od  => od.Quantity),
                Amount = o.OrderDetails.Sum(od => od.Quantity * od.Price)
            }).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => string.IsNullOrEmpty(search) ?
                                        _urlHelper.Action("Index", "Orders", new { area = "Admin", page = p, pagesize = pagesize }) ?? "" :
                                        _urlHelper.Action("Index", "Orders", new { area = "Admin", page = p, search = search, pagesize = pagesize }) ?? ""
            };
            #endregion kết thúc phân trang
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = string.IsNullOrEmpty(search) ?
                _urlHelper.Action("Index", "Orders", new { area = "Admin", page = page, pagesize = psItem.Size }) ?? "" :
                                        _urlHelper.Action("Index", "Orders", new { area = "Admin", page = page, search = search, pagesize = psItem.Size }) ?? "";
            }
            return (orderVMs, pagingModel, lstPageSize);
        }
        public async Task<(IEnumerable<OrderVM>, PagingModel, IEnumerable<PageSizeModel>)>
           GetToViewMyOrderAsync(string customerId ,string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var query = _unitOfWork.OrderRepository.Table()
                                                    .Include(o => o.Customer)
                                                    .Include(o => o.OrderDetails)
                                                    .Where(o => o.CustomerId == customerId)
                                                    .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.OrderCode.Contains(search));
            }
            query = query.OrderByDescending(c => c.CodeNumber);
            #region bắt đầu phân trang
            var totalItem = query.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page > totalPage)
                page = totalPage;
            if (page < 1)
                page = 1;
            var orders = await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync();
            var orderVMs = orders.Select(o => new OrderVM
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                DateCreated = o.DateCreated,
                FullName = o.FullName,
                PhoneNumber = o.PhoneNumber,
                Address = o.Address,
                Status = o.Status,
                TransportFee = o.TransportFee,
                PaymentType = o.PaymentType,
                Note = o.Note,
                DateDelivery = o.DateDelivery,
                Customer = o.Customer,
                SumQuantity = o.OrderDetails.Sum(od => od.Quantity),
                Amount = o.OrderDetails.Sum(od => od.Quantity * od.Price)
            }).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => string.IsNullOrEmpty(search) ?
                                        _urlHelper.Action("Index", "MyOrders", new {page = p, pagesize = pagesize }) ?? "" :
                                        _urlHelper.Action("Index", "MyOrders", new {page = p, search = search, pagesize = pagesize }) ?? ""
            };
            #endregion kết thúc phân trang
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = string.IsNullOrEmpty(search) ?
                _urlHelper.Action("Index", "MyOrders", new { page = page, pagesize = psItem.Size }) ?? "" :
                                        _urlHelper.Action("Index", "MyOrders", new { page = page, search = search, pagesize = psItem.Size }) ?? "";
            }
            return (orderVMs, pagingModel, lstPageSize);
        }
        public IQueryable<Book_Ecommerce.Domain.Entities.Order> Table()
        {
            return _unitOfWork.OrderRepository.Table();
        }
        public async Task<Book_Ecommerce.Domain.Entities.Order?> 
            GetSingleByConditionAsync(Expression<Func<Book_Ecommerce.Domain.Entities.Order, bool>> expression)
        {
            return await _unitOfWork.OrderRepository.GetSingleByConditionAsync(expression);
        }
        public async Task UpdateAsync(Book_Ecommerce.Domain.Entities.Order order)
        {
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveAsync(Book_Ecommerce.Domain.Entities.Order order)
        {
            _unitOfWork.OrderRepository.Remove(order);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
