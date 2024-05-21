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
using Book_Ecommerce.Domain.Helpers;
using System.Globalization;
using Book_Ecommerce.Domain.Entities;
using static System.Net.Mime.MediaTypeNames;

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
        public async Task<dynamic?> GetOrderDetailToView(string orderId)
        {
            var order = await _unitOfWork.OrderRepository.Table().Include(o => o.OrderDetails)
                                                 .ThenInclude(od => od.Product)
                                                 .ThenInclude(p => p.Images)
                                                 .Include(o => o.Customer)
                                                 .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
                return null;
            CultureInfo cultureInfo = new CultureInfo("vi-VN");
            var result = new
            {
                orderId = order.OrderId,
                orderCode = order.OrderCode,
                dateCreated = order.DateCreated.ToString("dd/MM/yyyy"),
                status = Generation.GenerationStatusOrderString(order.Status),
                note = order.Note,
                customerName = order.Customer.FullName,
                fullName = order.FullName,
                phoneNumber = order.PhoneNumber,
                address = order.Address,
                dateDelivery = order.DateDelivery.ToString("dd/MM/yyyy"),
                paymentType = order.PaymentType,
                transportFee = string.Format(cultureInfo, "{0:C0}", order.TransportFee),
                totalAmount = string.Format(cultureInfo, "{0:C0}", order.OrderDetails.Sum(od => od.Quantity * od.Price) + order.TransportFee),
                orderDetails = order.OrderDetails.Select(od => new
                {
                    productName = od.Product.ProductName,
                    urlImage = od.Product.Images.FirstOrDefault()?.Url ?? "",
                    quantity = od.Quantity,
                    price = string.Format(cultureInfo, "{0:C0}", od.Price),
                    amount = string.Format(cultureInfo, "{0:C0}", od.Price * od.Quantity)
                }).ToList(),
            };
            return result;
        }
        public async Task<string?> GenerateOrderToHtml(string orderId)
        {
            var order = await _unitOfWork.OrderRepository.Table().Include(o => o.OrderDetails)
                                                 .ThenInclude(od => od.Product)
                                                 .ThenInclude(p => p.Images)
                                                 .Include(o => o.Customer)
                                                 .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
                return null;
            CultureInfo cultureInfo = new CultureInfo("vi-VN");
            string tbody = "";
            int stt = 1;
            foreach (var item in order.OrderDetails)
            {
                tbody += @$"
                    <tr>
                        <td style=""text-align: center;"">{stt++}</td>
                        <td>{item.Product.ProductName}</td>
                        <td>
                            <img style=""width: 100px; height: 120px; object-fit: cover;"" src=""{item.Product.Images.FirstOrDefault()?.Url}
                            """" />
                        </td>
                        <td style=""text-align: center;"">{item.Quantity}</td>
                        <td style=""text-align: right;"">{string.Format(cultureInfo, "{0:C0}", item.Price)}</td>
                        <td style=""text-align: right;"">{string.Format(cultureInfo, "{0:C0}", item.Price * item.Quantity)}</td>
                    </tr>    
                ";
            }
            string orderHtml = @$"
                    <h2 style=""text-align: center; "">Hóa đơn mua hàng</h2>
                    <div style=""display: flex; width: 80%; margin: auto;"">
                        <div style=""flex-grow: 1;"">
                            <p>Mã hóa đơn: {order.OrderCode}</p>
                            <p>Ngày tạo: {order.DateCreated.ToString("dd/MM/yyyy")}</p>
                            <p>Trạng thái: {Generation.GenerationStatusOrderString(order.Status)}</p>
                            <p>Ghi chú: {order.Note}</p>
                        </div>
                        <div style=""flex-grow: 1;"">
                            <p>Người đặt: {order.Customer.FullName}</p>
                            <p>Người nhận: {order.FullName}</p>
                            <p>Điện thoại: {order.PhoneNumber}</p>
                            <p>Địa chỉ: {order.Address}</p>
                        </div>
                        <div style=""flex-grow: 1;"">
                            <p>Ngày nhận: {order.DateDelivery.ToString("dd/MM/yyyy")}</p>
                            <p>PT thanh toán: {order.PaymentType}</p>
                            <p>Phí vận truyển: {string.Format(cultureInfo, "{0:C0}", order.TransportFee)}</p>
                            <p>Tổng tiền: {string.Format(cultureInfo, "{0:C0}", order.OrderDetails.Sum(od => od.Quantity * od.Price) + order.TransportFee)}</p>
                        </div>
                    </div>
                    <table style=""width: 80%; margin: auto;"" border=""1"">
                        <thead>
                            <tr>
                                <th>STT</th>
                                <th>Sản phẩm</th>
                                <th>Hình ảnh</th>
                                <th>Số lượng</th>
                                <th>Đơn giá</th>
                                <th>Thành tiền</th>
                            </tr>
                        </thead>
                        <tbody>
                            {tbody}
                        </tbody>
                    </table>
                ";
            return orderHtml;
        }
        public IQueryable<Book_Ecommerce.Domain.Entities.Order> Table()
        {
            return _unitOfWork.OrderRepository.Table();
        }
        public IQueryable<OrderDetail> TableOrderDetail()
        {
            return _unitOfWork.OrderDetailRepository.Table();
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
