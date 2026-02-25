using PlayGround.Domain.Enums.Soccer;

namespace PlayGround.Application.Interfaces
{
    /// <summary>
    /// 선수 리포지토리 인터페이스 (Persistence에서 구현)
    /// </summary>
    public interface IPlayerRepository
    {
        Task<PlayerDetailResult?> GetByIdAsync(int playerId);
        Task<IReadOnlyList<PlayerSummaryResult>> SearchAsync(SoccerPosition? position, int? minAge, int? maxAge, int page, int size);
        Task<int> GetTotalCountAsync(SoccerPosition? position, int? minAge, int? maxAge);
    }

    /// <summary>
    /// 선수 상세 조회 결과
    /// </summary>
    public class PlayerDetailResult
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public SoccerPosition Position { get; set; }
        public int Age { get; set; }
        public string? TeamName { get; set; }
    }

    /// <summary>
    /// 선수 목록 조회 결과
    /// </summary>
    public class PlayerSummaryResult
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public SoccerPosition Position { get; set; }
        public int Age { get; set; }
    }
}
