using PlayGround.Shared.Result;

namespace PlayGround.Domain.Codes;

/// <summary>
/// 스포츠 도메인 특화 에러 코드
/// </summary>
public static class SportsErrorCode
{
    public static readonly ErrorCode MatchAlreadyStarted = ErrorCode.Register(DetailCodeRange.Error.Sports.Min, "MatchAlreadyStarted", "The match has already started.");
    public static readonly ErrorCode MatchNotActive = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 1, "MatchNotActive", "The match is not currently active.");
    public static readonly ErrorCode PlayerNotEligible = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 2, "PlayerNotEligible", "The player is not eligible for this operation.");
    public static readonly ErrorCode TeamNotFound = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 3, "TeamNotFound", "The specified team was not found.");
    public static readonly ErrorCode SeasonEnded = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 4, "SeasonEnded", "The season has already ended.");
    public static readonly ErrorCode InvalidScore = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 5, "InvalidScore", "The score is invalid for this sport.");
    public static readonly ErrorCode MaxPlayersReached = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 6, "MaxPlayersReached", "Maximum number of players reached.");
    public static readonly ErrorCode PlayerAlreadyInTeam = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 7, "PlayerAlreadyInTeam", "The player is already in a team.");
    public static readonly ErrorCode PlayerNotInTeam = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 8, "PlayerNotInTeam", "The player is not in the specified team.");
    public static readonly ErrorCode MatchNotFinished = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 9, "MatchNotFinished", "The match has not finished yet.");
    public static readonly ErrorCode InvalidMatchPeriod = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 10, "InvalidMatchPeriod", "Invalid match period for this sport.");
    public static readonly ErrorCode PlayerInjured = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 11, "PlayerInjured", "The player is injured and cannot participate.");
    public static readonly ErrorCode PlayerSuspended = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 12, "PlayerSuspended", "The player is suspended.");
    public static readonly ErrorCode TeamInactive = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 13, "TeamInactive", "The team is inactive.");
    public static readonly ErrorCode VenueUnavailable = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 14, "VenueUnavailable", "The venue is unavailable.");
    public static readonly ErrorCode WeatherCancellation = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 15, "WeatherCancellation", "The event was cancelled due to weather conditions.");
    public static readonly ErrorCode EquipmentFailure = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 16, "EquipmentFailure", "Equipment failure occurred.");
    public static readonly ErrorCode OfficialUnavailable = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 17, "OfficialUnavailable", "No official is available for the match.");
    public static readonly ErrorCode TournamentFull = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 18, "TournamentFull", "The tournament is full.");
    public static readonly ErrorCode RegistrationClosed = ErrorCode.Register(DetailCodeRange.Error.Sports.Min + 19, "RegistrationClosed", "Registration period has closed.");
}
