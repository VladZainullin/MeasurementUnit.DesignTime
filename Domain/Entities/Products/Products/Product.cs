﻿// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

using Domain.Entities.Products.Products.Parameters;

namespace Domain.Entities.Products.Products;

public sealed class Product
{
    private string _title = default!;
    private string _description = default!;
    
    private Product()
    {
    }
    
    public Product(CreateProductParameters parameters) : this()
    {
        _title = parameters.Title;
        _description = parameters.Description;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Title => _title;

    public void SetTitle(SetProductTitleParameters parameters)
    {
        _title = parameters.Title;
    }

    public string Description => _description;
    
    public void SetDescription(SetProductDescriptionParameters parameters)
    {
        _description = parameters.Description;
    }
}