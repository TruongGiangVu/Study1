using GenApi.Dtos;
using GenApi.Models;

namespace GenApi.Repositories;

public class AnimeRepository
{
    public Anime GetAnimeById(string id)
    {
        if (id == "1")
            throw new NotFoundException($"Anime id {id} không tồn tại");

        return new Anime
        {
            Id = id,
            Name = "Test Anime",
            Description = "Desc của test anime",
            Episodes = 5,
            IsRelease = false,
        };
    }

    public Anime? CreateAnime(CreateAnimeDto entity)
    {
        return new Anime
        {
            Id = Guid.NewGuid().ToString(),
            Name = entity.Name,
            Description = entity.Description,
            Episodes = entity.Episodes ?? 0,
            IsRelease = entity.IsRelease,
        };
    }
}
