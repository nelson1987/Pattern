namespace ContaMe.Domain.Movimentacao.Inclusao
{
    public class MovimentacaoInclusaoCommandResponse
    {
        public bool IsSucess { get; set; }
        public object Data { get; set; }
        public StatusCodeEnum StatusCode { get; set; }
    }
    public enum StatusCodeEnum
    {
        Sucess = 200,
        BadRequest = 400,
        InternalServerError = 500
    }
}
