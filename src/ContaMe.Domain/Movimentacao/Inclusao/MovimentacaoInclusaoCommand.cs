using MediatR;

namespace ContaMe.Domain.Movimentacao.Inclusao
{
    public class MovimentacaoInclusaoCommand : IRequest<MovimentacaoInclusaoCommandResponse>, INotification
    {
    }
}
