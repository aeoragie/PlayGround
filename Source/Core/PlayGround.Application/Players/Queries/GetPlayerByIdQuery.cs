using System.Diagnostics;
using PlayGround.Application.Interfaces;
using PlayGround.Shared.Result;

namespace PlayGround.Application.Players.Queries
{
    /// <summary>
    /// 선수 상세 조회 유즈케이스
    /// </summary>
    public class GetPlayerByIdQuery
    {
        private readonly IPlayerRepository Repository;

        public GetPlayerByIdQuery(IPlayerRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result<PlayerDetailResult>> ExecuteAsync(int playerId)
        {
            Debug.Assert(playerId > 0, "Player ID must be greater than zero");

            if (playerId <= 0)
            {
                return Result<PlayerDetailResult>.Error(ErrorCode.InvalidInput, "Player ID must be greater than zero");
            }

            var player = await Repository.GetByIdAsync(playerId);
            if (player == null)
            {
                return Result<PlayerDetailResult>.Error(ErrorCode.NotFound, $"Player not found: {playerId}");
            }

            return Result<PlayerDetailResult>.Success(player);
        }
    }
}
