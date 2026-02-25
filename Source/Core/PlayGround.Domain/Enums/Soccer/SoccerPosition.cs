namespace PlayGround.Domain.Enums.Soccer;

/// <summary>
/// 축구 포지션
/// </summary>
public enum SoccerPosition
{
    // 골키퍼
    GK = 1,

    // 수비수
    CB = 10,
    LB = 11,
    RB = 12,
    LWB = 13,
    RWB = 14,

    // 미드필더
    CDM = 20,
    CM = 21,
    CAM = 22,
    LM = 23,
    RM = 24,

    // 공격수
    LW = 30,
    RW = 31,
    CF = 32,
    ST = 33,
}
