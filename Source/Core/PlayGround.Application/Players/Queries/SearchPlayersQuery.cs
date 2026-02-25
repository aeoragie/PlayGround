using System.Diagnostics;
using PlayGround.Application.Interfaces;
using PlayGround.Domain.Enums.Soccer;
using PlayGround.Shared.DTOs;
using PlayGround.Shared.Result;

namespace PlayGround.Application.Players.Queries
{
    /// <summary>
    /// 선수 검색 유즈케이스
    /// </summary>
    public class SearchPlayersQuery
    {
        private readonly IPlayerRepository Repository;

        private const int DefaultPage = 1;
        private const int DefaultSize = 20;
        private const int MaxSize = 100;

        public SearchPlayersQuery(IPlayerRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result<PagedList<PlayerSummaryResult>>> ExecuteAsync(
            SoccerPosition? position,
            int? minAge,
            int? maxAge,
            int page = DefaultPage,
            int size = DefaultSize)
        {
            if (page < 1)
            {
                return Result<PagedList<PlayerSummaryResult>>.Error(ErrorCode.InvalidPagination, "Page must be greater than 0");
            }

            if (size < 1 || size > MaxSize)
            {
                return Result<PagedList<PlayerSummaryResult>>.Error(ErrorCode.InvalidPagination, $"Size must be between 1 and {MaxSize}");
            }

            if (minAge.HasValue && maxAge.HasValue && minAge > maxAge)
            {
                return Result<PagedList<PlayerSummaryResult>>.Error(ErrorCode.OutOfRange, "Min age cannot be greater than max age");
            }

            var players = await Repository.SearchAsync(position, minAge, maxAge, page, size);
            var totalCount = await Repository.GetTotalCountAsync(position, minAge, maxAge);

            Debug.Assert(players != null, "Repository should return empty list, not null");

            var pagedList = new PagedList<PlayerSummaryResult>(players.ToList(), totalCount, page, size);
            return Result<PagedList<PlayerSummaryResult>>.Success(pagedList);
        }
    }
}
