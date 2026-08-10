using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Dashboard.Models;
using Restaurant.Application.Features.Dashboard.Queries;

namespace Restaurant.Web.Controllers
{
    public class DashboardController : BaseController
    {
        public DashboardController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<DashboardDto> GetDashboard(
            CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(
                new GetDashboardQuery(),
                cancellationToken);
        }
    }
}