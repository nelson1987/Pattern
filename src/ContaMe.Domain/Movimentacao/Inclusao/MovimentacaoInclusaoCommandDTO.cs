using MediatR;

namespace ContaMe.Domain.Movimentacao.Inclusao
{
    public class MovimentacaoInclusaoCommandDTO
    {
        public string PartnerId { get; set; }
        public void SetPartnerId(string partnerId) => this.PartnerId = partnerId;
    }
    public class MovimentacaoInclusaoMapper 
    { 
    }
}
