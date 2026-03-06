using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Nwind;

public class Invoice
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class NwindProduct
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class NwindDataService
{
    private static readonly List<Invoice> Invoices = new()
    {
        new() { ProductId = 1, ProductName = "Chai", Country = "USA", City = "New York", Address = "1st Ave" },
        new() { ProductId = 2, ProductName = "Chang", Country = "USA", City = "Boston", Address = "2nd Ave" },
        new() { ProductId = 3, ProductName = "Aniseed Syrup", Country = "UK", City = "London", Address = "Baker Street" }
    };

    private static readonly List<NwindProduct> Products = new()
    {
        new() { ProductId = 1, Name = "Chai" },
        new() { ProductId = 2, Name = "Chang" },
        new() { ProductId = 3, Name = "Aniseed Syrup" }
    };

    public Task<IEnumerable<Invoice>> GetInvoicesAsync()
        => Task.FromResult(Invoices.AsEnumerable());

    public Task<IEnumerable<NwindProduct>> GetProductsAsync()
        => Task.FromResult(Products.AsEnumerable());
}

