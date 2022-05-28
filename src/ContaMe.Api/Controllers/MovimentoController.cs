using ContaMe.Domain.Movimentacao.Inclusao;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Mapster;

namespace ContaMe.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimentoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MovimentoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [TimestampFilter]
        [HttpPost]
        public async Task<IActionResult> Buscar([FromBody] MovimentacaoInclusaoCommand command,
            [FromHeader] string partnerId,
            [FromServices] ILogger<MovimentoController> log)
        {
            log.LogInformation("[Entrada] Metodo Buscar");
            var mapeamento = command.Adapt<MovimentacaoInclusaoCommandDTO>();
            var result = await _mediator.Send(command);

            if (result.IsSucess)
                return StatusCode((int)result.StatusCode, result.Data);
            log.LogInformation("[Saída] Erro: Metodo Buscar");
            return StatusCode((int)result.StatusCode, new { erros = result.Data });
        }
    }

    public class TimestampFilterAttribute : Attribute, IActionFilter, IAsyncActionFilter
    {
        //public void OnActionException(ActionExceptionContext context)
        //{
        //    context.ActionDescriptor.RouteValues["timestamp"] = DateTime.Now.
        //    ToString();
        //}
        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.ActionDescriptor.RouteValues["timestamp"] = DateTime.Now.
            ToString();
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var ts = DateTime.Parse(context.ActionDescriptor.
            RouteValues["timestamp"])
            .AddHours(1)
            .ToString();
            context.HttpContext.Response.Headers["X-EXPIRY-TIMESTAMP"] = ts;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext
        context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.ContainsKey("partnerId"))
            {
                //    context.ActionArguments.
            }
            this.OnActionExecuting(context);
            var resultContext = await next();
            this.OnActionExecuted(resultContext);
        }
    }
}
