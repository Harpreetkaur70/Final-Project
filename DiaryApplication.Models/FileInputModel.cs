using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryApplication.Models
{
    public class FileInputModel
    {
        public IFormFile FileToUpload { get; set; }
    }
}
