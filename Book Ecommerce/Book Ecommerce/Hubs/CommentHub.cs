using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.ViewModels;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Book_Ecommerce.Hubs
{
    public class CommentHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public CommentHub(IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // Gửi bình luận tới tất cả các client thuộc nhóm sản phẩm cụ thể
        public async Task SendComment(string productId, string vote, string message)
        {
            try
            {
                var user = await _userManager.GetUserAsync(Context.User);
                if (user == null)
                {
                    await Clients.Caller.SendAsync("Notification", false, "Yêu cầu đăng nhập để bình luận", "Required login");
                    return;
                }
                var customer = await _unitOfWork.CustomerRepository
                    .GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    await Clients.Caller.SendAsync("Notification", false, "Yêu cầu đăng nhập bằng tài khoản khách hàng để bình luận", "Required login account customer");
                    return;
                }
                var product = await _unitOfWork.ProductRepository
                    .GetSingleByConditionAsync(p => p.ProductId == productId);
                if (product == null)
                {
                    await Clients.Caller.SendAsync("Notification", false, "Không gửi được đánh giá do không tìm thấy được sản phẩm", "product is not find");
                    return;
                }
                var comment = new Comment
                {
                    CommentId = Guid.NewGuid().ToString(),
                    Vote = int.Parse(vote),
                    Message = message,
                    DateCreated = DateTime.Now,
                    CustomerId = customer.CustomerId,
                    ProductId = product.ProductId,
                };
                await _unitOfWork.CommentRepository.AddAsync(comment);
                await _unitOfWork.SaveChangesAsync();
                var commentSend = new
                {
                    commentId = comment.CommentId,
                    vote = comment.Vote,
                    message = comment.Message,
                    customerName = customer.FullName,
                    dateCreated = comment.DateCreated.ToString("dd/MM/yyyy - HH:mm:ss"),
                };
                var sumCommentInProduct = _unitOfWork.CommentRepository.Table().Count(c => c.ProductId == product.ProductId);
                await Clients.Caller.SendAsync("Notification", true, "Gửi đánh giá thành công", "send comment success");
                await Clients.Group(productId).SendAsync("ReceiveComment", commentSend, sumCommentInProduct);
            }
            catch(Exception ex)
            {
                await Clients.Caller.SendAsync("Notification", false, "Không gửi được đánh giá do lỗi hệ thống", ex.Message);
                return;
            }
        }

        // Thêm client vào nhóm sản phẩm cụ thể
        public async Task JoinProductGroup(string productId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, productId);
        }

        // Xóa client khỏi nhóm sản phẩm cụ thể
        public async Task LeaveProductGroup(string productId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, productId);
        }
    }
}
