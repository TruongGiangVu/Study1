using GenApi.Dtos;
using GenApi.Models;
using GenApi.Services;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

namespace GenApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AnimeController : ControllerBase
{
    private readonly ILogger<AnimeController> _logger;
    private readonly AnimeService _service;
    private readonly IPublishEndpoint _publishEndpoint;

    public AnimeController(ILogger<AnimeController> logger, AnimeService service, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _service = service;
        _publishEndpoint = publishEndpoint;
    }
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("Home controller is running");
    }
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDto<Anime>), StatusCodes.Status200OK)]
    public IActionResult GetAnimeById(string? id)
    {
        _logger.LogInformation("{method} id={id}", nameof(GetAnimeById), id);
        Anime? data = _service.GetAnimeById(id);
        var response = ResponseDto<Anime>.Success(data);
        // _logger.LogInformation("{method} response:{@response}", nameof(GetAnimeById), response);
        _logger.LogInformation("{method} response: {response}", nameof(GetAnimeById), response.ToJsonString());
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseDto<Anime>), StatusCodes.Status200OK)]
    public IActionResult CreateAnime(CreateAnimeDto? input)
    {
        _logger.LogInformation("{method} input={input}", nameof(CreateAnime), input.ToJsonString());

        ResponseDto<Anime> response = _service.CreateAnime(input);

        // nếu Create thành công, đẩy vào rabbit MQ 
        if (response.Payload is not null)
            _publishEndpoint.Publish(response.Payload);

        _logger.LogInformation("{method} response: {response}", nameof(CreateAnime), response.ToJsonString());
        return Ok(response);
    }
}
