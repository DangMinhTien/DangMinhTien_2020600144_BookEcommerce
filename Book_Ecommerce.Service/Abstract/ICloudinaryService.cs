﻿using Book_Ecommerce.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface ICloudinaryService
    {
        Task<CloudinaryModel> UploadAsync(IFormFile file);
    }
}
