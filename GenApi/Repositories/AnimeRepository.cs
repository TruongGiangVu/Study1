
using GenApi.Dtos;
using GenApi.Models;

namespace GenApi.Repositories;

public class AnimeRepository : IAnimeRepository
{
    private readonly AppDbContext _db;

    public AnimeRepository(AppDbContext db)
    {
        _db = db;
    }
    public Anime? GetAnimeById(string id)
    {
        return _db.Anime.Where(p => p.Id == id).FirstOrDefault();
    }

    public Anime? CreateAnime(CreateAnimeDto entity)
    {
        var newAnime = new Anime
        {
            Id = Guid.NewGuid().ToString(),
            Name = entity.Name,
            Description = entity.Description,
            Episodes = entity.Episodes ?? 0,
            IsRelease = entity.IsRelease,
            // ReleaseDate = entity.ReleaseDate ?? DateTime.Now,
            ReleaseDate = DateTime.SpecifyKind(entity.ReleaseDate ?? DateTime.Now, DateTimeKind.Utc),
        };
        _db.Anime.Add(newAnime);
        _db.SaveChanges();
        return newAnime;
    }

    public List<Anime>? GetAnime(string name)
    {
        return _db.Anime.Where(p => p.Name.Contains(name)).ToList();
    }
}
