using PlayGround.Shared.Result;

namespace PlayGround.Domain.Codes;

/// <summary>
/// 스포츠 도메인 특화 정보 코드
/// </summary>
public static class SportsInformationCode
{
    public static readonly InformationCode MatchCreated = InformationCode.Register(DetailCodeRange.Information.Sports.Min, "MatchCreated", "Match successfully created.");
    public static readonly InformationCode MatchStarted = InformationCode.Register(DetailCodeRange.Information.Sports.Min + 1, "MatchStarted", "Match has started.");
    public static readonly InformationCode MatchEnded = InformationCode.Register(DetailCodeRange.Information.Sports.Min + 2, "MatchEnded", "Match has ended.");
    public static readonly InformationCode MatchPaused = InformationCode.Register(DetailCodeRange.Information.Sports.Min + 3, "MatchPaused", "Match has been paused.");
    public static readonly InformationCode MatchResumed = InformationCode.Register(DetailCodeRange.Information.Sports.Min + 4, "MatchResumed", "Match has been resumed.");
    public static readonly InformationCode ScoreUpdated = InformationCode.Register(DetailCodeRange.Information.Sports.Min + 5, "ScoreUpdated", "Score successfully updated.");
    public static readonly InformationCode PlayerJoined = InformationCode.Register(DetailCodeRange.Information.Sports.Min + 6, "PlayerJoined", "Player successfully joined the team.");
    public static readonly InformationCode PlayerLeft = InformationCode.Register(DetailCodeRange.Information.Sports.Min + 7, "PlayerLeft", "Player has left the team.");
    public static readonly InformationCode TeamCreated = InformationCode.Register(DetailCodeRange.Information.Sports.Min + 8, "TeamCreated", "Team successfully created.");
    public static readonly InformationCode SeasonStarted = InformationCode.Register(DetailCodeRange.Information.Sports.Min + 9, "SeasonStarted", "Season has started.");
    public static readonly InformationCode SeasonEnded = InformationCode.Register(DetailCodeRange.Information.Sports.Min + 10, "SeasonEnded", "Season has ended.");
    public static readonly InformationCode TournamentCreated = InformationCode.Register(DetailCodeRange.Information.Sports.Min + 11, "TournamentCreated", "Tournament successfully created.");
}
