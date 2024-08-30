﻿using Domain.Entities.Products.Categories.Parameters;

namespace Domain.Entities.Products.Categories;

public sealed class Category
{
    private string _title = default!;
    
    private Category()
    {
    }

    public Category(CreateCategoryParameters parameters) : this()
    {
        SetTitle(new SetCategoryTitleParameters
        {
            Title = parameters.Title
        });
    }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Title => _title;
    
    private void SetTitle(SetCategoryTitleParameters parameters)
    {
        _title = parameters.Title;
    }
}