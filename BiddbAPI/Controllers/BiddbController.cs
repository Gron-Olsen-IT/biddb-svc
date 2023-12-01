namespace BiddbAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using BiddbAPI.Services;


[ApiController]
[Route("[controller]")]
public class BiddbController : ControllerBase
{
    private readonly IRepo _repo;

    private readonly ILogger<BiddbController> _logger;

    public BiddbController(ILogger<BiddbController> logger, IRepo repo)
    {
        _repo = repo;
        _logger = logger;
    }



    
}
