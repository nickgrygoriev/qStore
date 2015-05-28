using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qStore.Models
{
    public class ProductInfo
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageContentType { get; set; }
        public string ImageBase64String { get; set; }
        public double Price { get; set; }
    }
}