using Application.Contracts.Features.MeasurementUnits.Commands.CreateMeasurementUnit;
using Domain.MeasurementUnitPositions;
using Domain.MeasurementUnits;
using Domain.MeasurementUnits.Parameters;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Contracts;

namespace Application.Features.MeasurementUnits.Commands.CreateMeasurementUnit;

file sealed class CreateMeasurementUnitHandler(IDbContext context, TimeProvider timeProvider)
    : IRequestHandler<CreateMeasurementUnitCommand, CreateMeasurementUnitResponseDto>
{
    public async Task<CreateMeasurementUnitResponseDto> Handle(
        CreateMeasurementUnitCommand request,
        CancellationToken cancellationToken)
    {
        var existingMeasurementUnit = await GetMeasurementUnitAsync(request, cancellationToken);

        var measurementUnitPositionsFromRequest = 
            await GetMeasurementUnitPositionsFromRequestAsync(request, cancellationToken);
        
        if (!ReferenceEquals(existingMeasurementUnit, default))
        {
            if (existingMeasurementUnit.IsRemove)
            {
                existingMeasurementUnit.Restore();
            
                existingMeasurementUnit.RemoveMeasurementUnitPositions(new RemoveMeasurementUnitPositionsFromMeasurementUnitParameters
                {
                    TimeProvider = timeProvider,
                    MeasurementUnitPositions = measurementUnitPositionsFromRequest
                        .Except(existingMeasurementUnit.MeasurementUnitPositions)
                });
            }
            
            existingMeasurementUnit.AddMeasurementUnitPositions(new AddMeasurementUnitPositionToMeasurementUnitParameters
            {
                MeasurementUnitPositions = measurementUnitPositionsFromRequest
            });
            
            return new CreateMeasurementUnitResponseDto
            {
                Id = existingMeasurementUnit.Id
            };
        }

        var measurementUnit = new MeasurementUnit(new CreateMeasurementUnitParameters
        {
            Title = request.BodyDto.Title,
        });
        measurementUnit.AddMeasurementUnitPositions(new AddMeasurementUnitPositionToMeasurementUnitParameters
        {
            MeasurementUnitPositions = measurementUnitPositionsFromRequest
        });
        
        context.MeasurementUnits.Add(measurementUnit);
        await context.SaveChangesAsync(cancellationToken);

        return new CreateMeasurementUnitResponseDto
        {
            Id = measurementUnit.Id
        };
    }

    private Task<List<MeasurementUnitPosition>> GetMeasurementUnitPositionsFromRequestAsync(CreateMeasurementUnitCommand request, CancellationToken cancellationToken)
    {
        return context.MeasurementUnitPositions
            .AsTracking()
            .Where(mup => request.BodyDto.MeasurementUnitPositions
                .Select(m => m.Id)
                .Contains(mup.Id))
            .ToListAsync(cancellationToken);
    }

    private Task<MeasurementUnit?> GetMeasurementUnitAsync(CreateMeasurementUnitCommand request, CancellationToken cancellationToken)
    {
        return context.MeasurementUnits
            .Include(static mu => mu.MeasurementUnitPositions)
            .AsTracking()
            .SingleOrDefaultAsync(mu => mu.Title == request.BodyDto.Title, cancellationToken);
    }
}