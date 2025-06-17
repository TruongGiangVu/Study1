using FluentValidation.Results;

using GenApi.Dtos;
using GenApi.Models;
using GenApi.Repositories;

namespace GenApi.Services;

public class AnimeService
{
    private readonly AnimeRepository _repository;

    public AnimeService(AnimeRepository repository)
    {
        _repository = repository;
    }
    public Anime? GetAnimeById(string? id)
    {
        if (string.IsNullOrEmpty(id))
            throw new RequiredException($"Anime id {id} không để trống");

        if (id == "aa")
            throw new ValidationException(details: [$"Anime id {id} không hợp lệ"]);

        return _repository.GetAnimeById(id);
    }

    public ResponseDto<Anime> CreateAnime(CreateAnimeDto? dto)
    {
        if (dto is null)
            throw new RequiredException($"Anime thông tin không bị null");

        CreateAnimeValidator validator = new();
        ValidationResult validationResult= validator.Validate(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(details: validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        Anime? data = _repository.CreateAnime(dto);

        if(data is null)
            throw new DatabaseException($"Tạo Anime bị lỗi");

        return ResponseDto<Anime>.Success(data);
    }
}
