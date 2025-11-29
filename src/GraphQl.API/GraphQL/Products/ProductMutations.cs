using GraphQl.Application.Features.Products.CreateProduct;
using GraphQl.Application.Features.Products.UpdateProduct;
using GraphQl.Application.Features.Products.DeleteProduct;
using MediatR;

namespace GraphQl.API.GraphQL.Products;

public class ProductMutations
{
    public async Task<CreateProductDto> CreateProduct(
        CreateProductInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            input.Name,
            input.Description,
            input.Price,
            input.Stock
        );

        return await mediator.Send(command, cancellationToken);
    }

    public async Task<UpdateProductDto> UpdateProduct(
        string id,
        UpdateProductInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            input.Name,
            input.Description,
            input.Price,
            input.Stock
        );

        return await mediator.Send(command, cancellationToken);
    }

    public async Task<DeleteProductDto> DeleteProduct(
        string id,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);
        return await mediator.Send(command, cancellationToken);
    }
}

public record CreateProductInput(
    string Name,
    string Description,
    decimal Price,
    int Stock
);

public record UpdateProductInput(
    string Name,
    string Description,
    decimal Price,
    int Stock
);
