using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ChioriApp.Models;

namespace ChioriApp.Validators
{
    public class OrderValidator
    {
        public List<ValidationResult> Validate(Order order)
        {
            var context = new ValidationContext(order);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(order, context, results, true);
            return results;
        }
    }
}