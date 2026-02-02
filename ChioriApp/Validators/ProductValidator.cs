using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ChioriApp.Models;

namespace ChioriApp.Validators
{
    public class ProductValidator
    {
        public List<ValidationResult> Validate(Product product)
        {
            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(product, context, results, true);
            return results;
        }
    }
}