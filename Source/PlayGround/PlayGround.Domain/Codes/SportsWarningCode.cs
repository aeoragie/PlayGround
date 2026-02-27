using PlayGround.Shared.Result;

namespace PlayGround.Domain.Codes;

/// <summary>
/// 스포츠 도메인 특화 경고 코드
/// </summary>
public static class SportsWarningCode
{
    public static readonly WarningCode MatchDataIncomplete = WarningCode.Register(DetailCodeRange.Warning.Sports.Min, "MatchDataIncomplete", "Match data is incomplete.");
    public static readonly WarningCode PlayerEligibilityIssue = WarningCode.Register(DetailCodeRange.Warning.Sports.Min + 1, "PlayerEligibilityIssue", "Player eligibility issue detected.");
    public static readonly WarningCode ScoreDiscrepancy = WarningCode.Register(DetailCodeRange.Warning.Sports.Min + 2, "ScoreDiscrepancy", "Score discrepancy detected.");
    public static readonly WarningCode MatchTimeAdjustment = WarningCode.Register(DetailCodeRange.Warning.Sports.Min + 3, "MatchTimeAdjustment", "Match time was adjusted.");
    public static readonly WarningCode TeamRosterIncomplete = WarningCode.Register(DetailCodeRange.Warning.Sports.Min + 4, "TeamRosterIncomplete", "Team roster is incomplete.");
    public static readonly WarningCode SeasonOverlap = WarningCode.Register(DetailCodeRange.Warning.Sports.Min + 5, "SeasonOverlap", "Season dates overlap with another season.");
    public static readonly WarningCode StatisticsOutdated = WarningCode.Register(DetailCodeRange.Warning.Sports.Min + 6, "StatisticsOutdated", "Player or team statistics may be outdated.");
    public static readonly WarningCode RuleViolationMinor = WarningCode.Register(DetailCodeRange.Warning.Sports.Min + 7, "RuleViolationMinor", "Minor rule violation detected.");
    public static readonly WarningCode EquipmentIssue = WarningCode.Register(DetailCodeRange.Warning.Sports.Min + 8, "EquipmentIssue", "Equipment issue reported.");
    public static readonly WarningCode WeatherCondition = WarningCode.Register(DetailCodeRange.Warning.Sports.Min + 9, "WeatherCondition", "Adverse weather condition may affect play.");
}
