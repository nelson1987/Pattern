using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ContaMe.Domain.Movimentacao.Inclusao
{
    public class MovimentacaoInclusaoCommandNotification : INotificationHandler<MovimentacaoInclusaoCommand>
    {
        public Task Handle(MovimentacaoInclusaoCommand notification, CancellationToken cancellationToken)
        {
            return Task.FromResult("Movimentacao Inclusao");
        }
    }
}
