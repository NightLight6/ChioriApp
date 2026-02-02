using System.Collections.Generic;
using ChioriApp.Models;

namespace ChioriApp.Services
{
    public static class BasketService
    {
        private static readonly List<BasketItem> _items = new();

        public static IReadOnlyList<BasketItem> Items => _items.AsReadOnly();

        public static void AddItem(BasketItem item)
        {
            var existing = _items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                _items.Add(item);
            }
        }

        public static void RemoveItem(int productId)
        {
            _items.RemoveAll(i => i.ProductId == productId);
        }

        public static void Clear()
        {
            _items.Clear();
        }

        public static decimal GetTotal()
        {
            return _items.Sum(i => i.Price * i.Quantity);
        }
    }
}