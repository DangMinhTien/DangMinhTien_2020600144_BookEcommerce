using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.ViewModels.ChatViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.ViewComponents
{
    public class SideBarChatToCustomerViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public SideBarChatToCustomerViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync(string? searchCustomer = null, string? customerActive = "",
            string employeeId = "")
        {
            var query = _unitOfWork.CustomerRepository.Table().AsQueryable();
            if (!string.IsNullOrEmpty(searchCustomer))
            {
                query = query.Where(c => c.FullName.Contains(searchCustomer));
            }
            var customers = await query.ToListAsync();
            var lstChat = new List<ChatVM>();
            foreach (var customer in customers)
            {
                var message = await _unitOfWork.MessageRepository.Table()
                    .OrderByDescending(m => m.SendDate)
                    .FirstOrDefaultAsync(m => m.CustomerId == customer.CustomerId && m.EmployeeId == employeeId);
                lstChat.Add(new ChatVM()
                {
                    CustomerId = customer.CustomerId,
                    EmployeeId = employeeId,
                    Customer = customer,
                    IsActive = customer.CustomerId == customerActive,
                    LastMessage = message != null ? message.Content : ""
                });
            }
            return View(lstChat);
        }
    }
}
