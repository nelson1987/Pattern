using Mapster;
using MediatR;

namespace ContaMe.Domain.Movimentacao.Inclusao
{
    [AdaptTo("[name]DTO"), GenerateMapper]
    public class MovimentacaoInclusaoCommand : IRequest<MovimentacaoInclusaoCommandResponse>, INotification
    {
        /// <summary>
        /// Id do Parceiro
        /// </summary>
        /// <value>
        /// ParceiroId
        /// </value>
        public string PartnerId { get; set; }
        public void SetPartnerId(string partnerId) => this.PartnerId = partnerId;
    }
}
