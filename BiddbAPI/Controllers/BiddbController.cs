using Microsoft.AspNetCore.Mvc;

namespace BiddbAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class BiddbController : ControllerBase
{

    private readonly ILogger<BiddbController> _logger;

    public BiddbController(ILogger<BiddbController> logger)
    {
        _logger = logger;
    }

    
}
