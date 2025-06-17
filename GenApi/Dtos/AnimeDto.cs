using FluentValidation;

namespace GenApi.Dtos;

public class AnimeDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsRelease { get; set; } = false;
    public int? Episodes { get; set; } = 0;
    public DateTime? ReleaseDate { get; set; } = DateTime.Now;
}

public class CreateAnimeDto : AnimeDto
{
}

public class UpdateAnimeDto : AnimeDto
{
    public string Id { get; set; } = string.Empty;
}

public class AnimeValidator : AbstractValidator<AnimeDto>
{
    public AnimeValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(4);
        RuleFor(x => x.Episodes).NotNull().GreaterThan(0);
    }
}

public class CreateAnimeValidator : AbstractValidator<AnimeDto>
{
    public CreateAnimeValidator()
    {
        Include(new AnimeValidator());
    }
}

public class UpdateAnimeValidator : AbstractValidator<UpdateAnimeDto>
{
    public UpdateAnimeValidator()
    {
        Include(new AnimeValidator());
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}
