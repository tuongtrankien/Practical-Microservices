using OrchestratorService.Domain.Entities;

namespace OrchestratorService.Application.Interfaces;

public interface IOrderSagaRepository
{
    Task<OrderSagaState?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrderSagaState?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderSagaState>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderSagaState>> GetByStatusAsync(SagaStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(OrderSagaState sagaState, CancellationToken cancellationToken = default);
    Task UpdateAsync(OrderSagaState sagaState, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
