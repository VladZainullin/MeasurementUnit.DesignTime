using Domain.MeasurementUnitPositions;

namespace Domain.MeasurementUnits.Parameters;

public readonly struct CreateMeasurementUnitParameters
{
    public required string Title { get; init; }
}