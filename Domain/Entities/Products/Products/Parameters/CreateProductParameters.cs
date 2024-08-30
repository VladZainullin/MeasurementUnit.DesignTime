﻿// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Domain.Entities.Products.Products.Parameters;

public sealed class CreateProductParameters
{
    public required string Title { get; init; }

    public required string Description { get; init; }
}