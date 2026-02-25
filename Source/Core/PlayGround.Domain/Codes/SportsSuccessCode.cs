using PlayGround.Shared.Result;

namespace PlayGround.Domain.Codes;

/// <summary>
/// 스포츠 도메인 특화 성공 코드
/// </summary>
public static class SportsSuccessCode
{
    public static readonly SuccessCode MatchCreated = SuccessCode.Register(DetailCodeRange.Success.Sports.Min, "MatchCreated", "Match successfully created.");
    public static readonly SuccessCode MatchStarted = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 1, "MatchStarted", "Match successfully started.");
    public static readonly SuccessCode MatchEnded = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 2, "MatchEnded", "Match successfully ended.");
    public static readonly SuccessCode MatchPaused = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 3, "MatchPaused", "Match successfully paused.");
    public static readonly SuccessCode MatchResumed = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 4, "MatchResumed", "Match successfully resumed.");
    public static readonly SuccessCode MatchCancelled = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 5, "MatchCancelled", "Match successfully cancelled.");
    public static readonly SuccessCode ScoreUpdated = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 6, "ScoreUpdated", "Score successfully updated.");
    public static readonly SuccessCode ScoreSubmitted = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 7, "ScoreSubmitted", "Score successfully submitted.");
    public static readonly SuccessCode PlayerJoined = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 8, "PlayerJoined", "Player successfully joined the team.");
    public static readonly SuccessCode PlayerLeft = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 9, "PlayerLeft", "Player successfully left the team.");
    public static readonly SuccessCode PlayerSubstituted = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 10, "PlayerSubstituted", "Player successfully substituted.");
    public static readonly SuccessCode TeamCreated = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 11, "TeamCreated", "Team successfully created.");
    public static readonly SuccessCode TeamUpdated = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 12, "TeamUpdated", "Team successfully updated.");
    public static readonly SuccessCode SeasonCreated = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 13, "SeasonCreated", "Season successfully created.");
    public static readonly SuccessCode SeasonStarted = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 14, "SeasonStarted", "Season successfully started.");
    public static readonly SuccessCode SeasonEnded = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 15, "SeasonEnded", "Season successfully ended.");
    public static readonly SuccessCode TournamentCreated = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 16, "TournamentCreated", "Tournament successfully created.");
    public static readonly SuccessCode TournamentStarted = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 17, "TournamentStarted", "Tournament successfully started.");
    public static readonly SuccessCode TournamentEnded = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 18, "TournamentEnded", "Tournament successfully ended.");
    public static readonly SuccessCode LeagueCreated = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 19, "LeagueCreated", "League successfully created.");
    public static readonly SuccessCode StandingsUpdated = SuccessCode.Register(DetailCodeRange.Success.Sports.Min + 20, "StandingsUpdated", "League standings successfully updated.");
}
