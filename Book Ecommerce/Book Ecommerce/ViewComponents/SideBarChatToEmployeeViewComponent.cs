using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.ViewModels.AuthorViewModel;
using Book_Ecommerce.Domain.ViewModels.ChatViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.ViewComponents
{
    public class SideBarChatToEmployeeViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public SideBarChatToEmployeeViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync(string? searchEmployee = null, string? employeeActive = "",
            string customerId = "")
        {
            var query = _unitOfWork.EmployeeRepository.Table().AsQueryable();
            if(!string.IsNullOrEmpty(searchEmployee) )
            {
                query = query.Where(e => e.FullName.Contains(searchEmployee));
            }
            var employees = await query.ToListAsync();
            var lstChat = new List<ChatVM>();
            foreach (var employee in employees)
            {
                var message = await _unitOfWork.MessageRepository.Table()
                    .OrderByDescending(m => m.SendDate)
                    .FirstOrDefaultAsync(m => m.CustomerId == customerId && m.EmployeeId == employee.EmployeeId);
                lstChat.Add(new ChatVM()
                {
                    CustomerId = customerId,
                    EmployeeId = employee.EmployeeId,
                    Employee = employee,
                    IsActive = employee.EmployeeId == employeeActive,
                    LastMessage = message != null ? message.Content : ""
                });
            }
            return View(lstChat);
        }
    }
}
