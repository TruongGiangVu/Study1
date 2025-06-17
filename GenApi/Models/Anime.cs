namespace GenApi.Models;

public class Anime
{
    public string Id { get; set; } = string.Empty;

    /// <summary> Tên bộ anime </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary> Diễn giải anime </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary> Đã phát hành hay chưa </summary>
    public bool IsRelease { get; set; } = false;

    /// <summary> Số tập phim </summary>
    public int Episodes { get; set; } = 0;

    /// <summary> Ngày phát hành </summary>
    public DateTime ReleaseDate { get; set; } = DateTime.Now;
}
