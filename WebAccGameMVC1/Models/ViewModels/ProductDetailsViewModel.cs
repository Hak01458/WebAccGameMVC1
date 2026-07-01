using System.Collections.Generic;
using WebAccGameMVC.Models;


namespace WebAccGameMVC1.Models.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; } = null!;
        public List<Product> SimilarProducts { get; set; } = new();
    }
}
