using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Book_Ecommerce.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task SendMessage(string chatName, string customerId, string employeeId, string content, string sendBy)
        {
            try
            {
                if (string.IsNullOrEmpty(content))
                {
                    await Clients.Caller.SendAsync("Notification", false, "Bạn cần phải nhập nội dung trước khi gửi", "content is null or empty");
                    return;
                }
                var message = new Book_Ecommerce.Domain.Entities.Messsages
                {
                    MessageId = Guid.NewGuid().ToString(),
                    EmployeeId = employeeId,
                    CustomerId = customerId,
                    SendBy = sendBy,
                    Content = content,
                    SendDate = DateTime.Now,
                };
                await _unitOfWork.MessageRepository.AddAsync(message);
                await _unitOfWork.SaveChangesAsync();
                var messageSend = new
                {
                    sendDate = message.SendDate.ToString("HH:mm - dd/MM/yyyy"),
                    content = message.Content,
                    sendBy = message.SendBy,
                };
                await Clients.Group(chatName).SendAsync("ReceiveMessage", messageSend);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Notification", false, "Không gửi được đánh giá do lỗi hệ thống", ex.Message);
                return;
            }
        }

        

        // Thêm client vào nhóm sản phẩm cụ thể
        public async Task JoinChat(string chatName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
        }

        // Xóa client khỏi nhóm sản phẩm cụ thể
        public async Task LeaveChat(string chatName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatName);
        }
    }
}
