using Friday.BuildingBlocks.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public abstract class AdminControllerBase : Controller
{
    protected async Task CommitAsync(CancellationToken cancellationToken)
    {
        IUnitOfWork unitOfWork = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
