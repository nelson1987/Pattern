using MediatR;

namespace ContaMe.Domain.Movimentacao.Inclusao
{
    public class MovimentacaoInclusaoCommand : IRequest<MovimentacaoInclusaoCommandResponse>, INotification
    {
        public string PartnerId { get; set; }
        public void SetPartnerId(string partnerId) => this.PartnerId = partnerId;
    }
}
