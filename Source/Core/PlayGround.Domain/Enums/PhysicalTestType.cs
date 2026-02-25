namespace PlayGround.Domain.Enums;

/// <summary>
/// 피지컬 측정 항목
/// </summary>
public enum PhysicalTestType
{
    Sprint10m = 1,
    Sprint30m = 2,
    Sprint50m = 3,
    VerticalJump = 10,
    StandingLongJump = 11,
    Agility = 20,       // T-test
    Endurance = 30,     // YYIR (Yo-Yo 간헐적 회복 테스트)
    Flexibility = 40,
    BodyFat = 50,
}
