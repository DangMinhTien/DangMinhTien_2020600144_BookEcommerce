using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Service.Abstract;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Book_Ecommerce.Domain.MySettings;

namespace Book_Ecommerce.Service
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly IConfiguration _configuration;

        public CloudinaryService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<CloudinaryModel> UploadAsync(IFormFile file)
        {
            var cloudinary = new Cloudinary(new Account(
                cloud: _configuration.GetSection("Cloudinary:CloudName").Value,
                apiKey: _configuration.GetSection("Cloudinary:ApiKey").Value,
                apiSecret: _configuration.GetSection("Cloudinary:ApiSecret").Value
            ));

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = MyAppSetting.FOLDER_NAME_CLOUDINARY
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return new CloudinaryModel
            {
                FileName = uploadResult.PublicId,
                Url = uploadResult.Url.ToString(),
            };
        }
        public async Task DeleteAsync(string fileName)
        {
            var cloudinary = new Cloudinary(new Account(
                cloud: _configuration.GetSection("Cloudinary:CloudName").Value,
                apiKey: _configuration.GetSection("Cloudinary:ApiKey").Value,
                apiSecret: _configuration.GetSection("Cloudinary:ApiSecret").Value
            ));
            var deleteParams = new DelResParams()
            {
                PublicIds = new List<string> { fileName },
                Type = "upload"
            };
            var result = await cloudinary.DeleteResourcesAsync(deleteParams);
        }
        public async Task DeleteRangeAsync(List<string> fileNames)
        {
            var cloudinary = new Cloudinary(new Account(
                cloud: _configuration.GetSection("Cloudinary:CloudName").Value,
                apiKey: _configuration.GetSection("Cloudinary:ApiKey").Value,
                apiSecret: _configuration.GetSection("Cloudinary:ApiSecret").Value
            ));
            var deleteParams = new DelResParams()
            {
                PublicIds = fileNames,
                Type = "upload"
            };
            var result = await cloudinary.DeleteResourcesAsync(deleteParams);
        }
    }
}
