using GenApi.Dtos;
using GenApi.Models;

namespace GenApi.Repositories;

public interface IAnimeRepository
{
    Anime? GetAnimeById(string id);
    List<Anime>? GetAnime(string name);
    Anime? CreateAnime(CreateAnimeDto entity);
}
