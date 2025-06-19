using GenApi.Dtos;
using GenApi.Models;
using GenApi.Repositories;
using GenApi.Services;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

using OpenSearch.Client;

namespace GenApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AnimeController : ControllerBase
{
    private readonly ILogger<AnimeController> _logger;
    private readonly AnimeService _service;
    private readonly IAnimeRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IOpenSearchClient _openSearchClient;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public AnimeController(ILogger<AnimeController> logger,
                AnimeService service,
                IAnimeRepository repository,
                IPublishEndpoint publishEndpoint,
                IOpenSearchClient openSearchClient,
                ISendEndpointProvider sendEndpointProvider)
    {
        _logger = logger;
        _service = service;
        _repository = repository;
        _publishEndpoint = publishEndpoint;
        _openSearchClient = openSearchClient;
        _sendEndpointProvider = sendEndpointProvider;
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

    [HttpGet]
    [ProducesResponseType(typeof(ResponseDto<List<Anime>>), StatusCodes.Status200OK)]
    public IActionResult GetAnime(string? name)
    {
        _logger.LogInformation("{method} name={id}", nameof(GetAnime), name);
        List<Anime>? data = _repository.GetAnime(name ?? string.Empty);
        var response = ResponseDto<List<Anime>>.Success(data);
        _logger.LogInformation("{method} response: {response}", nameof(GetAnime), response.ToJsonString());
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseDto<Anime>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAnime(CreateAnimeDto? input)
    {
        _logger.LogInformation("{method} input={input}", nameof(CreateAnime), input.ToJsonString());

        ResponseDto<Anime> response = _service.CreateAnime(input);


        if (response.Payload is not null)
        {
            // nếu Create thành công, đẩy vào rabbit MQ theo kiểu fanout
            await _publishEndpoint.Publish(response.Payload);

            // đẩy tới 1 queue name cụ thể
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:anime-queue"));
            await endpoint.Send(response.Payload);

            IndexResponse opensSearchResponse = await _openSearchClient.IndexDocumentAsync(response.Payload);
            _logger.LogInformation("{method} OpenSearch isValid={isValid} id={id}", nameof(CreateAnime), opensSearchResponse.IsValid, opensSearchResponse.Id);
        }


        _logger.LogInformation("{method} response: {response}", nameof(CreateAnime), response.ToJsonString());
        return Ok(response);
    }

    [HttpGet("open-search/{id}")]
    public async Task<IActionResult> GetOpenSearchAnimeById(string id)
    {
        var response = await _openSearchClient.GetAsync<Anime>(id);

        if (!response.Found)
            return NotFound();

        return Ok(response.Source);
    }

    [HttpGet("open-search")]
    public async Task<IActionResult> SearchOpenSearch(string name)
    {
        var searchResponse = await _openSearchClient.SearchAsync<Anime>(s => s
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Name)
                    .Query(query: name)
                )
            )
        );

        return Ok(searchResponse.Documents);
    }
}
