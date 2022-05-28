using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ContaMe.Domain.Movimentacao.Inclusao
{
    public class MovimentacaoInclusaoCommandHandler : IRequestHandler<MovimentacaoInclusaoCommand, MovimentacaoInclusaoCommandResponse>
    {
        public Task<MovimentacaoInclusaoCommandResponse> Handle(MovimentacaoInclusaoCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new MovimentacaoInclusaoCommandResponse() { });
        }
    }
}
