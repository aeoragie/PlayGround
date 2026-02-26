namespace Crawler.KFA.Models
{
    /// <summary>
    /// 리그/대회 정보
    /// </summary>
    public class KfaMatch
    {
        /// <summary>대회 고유 ID (hex string)</summary>
        public string Idx { get; set; } = string.Empty;

        /// <summary>대회명</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>대회 구분명 (초등, 중등, 고등, K리그, 시도대회 등)</summary>
        public string MgcNm { get; set; } = string.Empty;

        /// <summary>스타일 (리그, 대회)</summary>
        public string StyleNm { get; set; } = string.Empty;

        /// <summary>일정 (yyyy-MM-dd ~ yyyy-MM-dd)</summary>
        public string MatchDate { get; set; } = string.Empty;

        /// <summary>시작일</summary>
        public string StartDate { get; set; } = string.Empty;

        /// <summary>종료일</summary>
        public string EndDate { get; set; } = string.Empty;

        /// <summary>장소</summary>
        public string PlayingArea { get; set; } = string.Empty;

        /// <summary>섹션 수</summary>
        public string SectCnt { get; set; } = string.Empty;
    }

    /// <summary>
    /// 참가팀 정보
    /// </summary>
    public class KfaTeam
    {
        /// <summary>팀 고유 ID (hex string)</summary>
        public string TeamId { get; set; } = string.Empty;

        /// <summary>팀명</summary>
        public string TeamName { get; set; } = string.Empty;

        /// <summary>엠블럼 경로</summary>
        public string Emblem { get; set; } = string.Empty;

        /// <summary>가상팀 여부</summary>
        public string VirtualTeamYn { get; set; } = string.Empty;

        /// <summary>소속 대회 IDX</summary>
        public string MatchIdx { get; set; } = string.Empty;

        /// <summary>대회 구분명 (초등, 중등, 고등 등)</summary>
        public string MgcNm { get; set; } = string.Empty;
    }

    /// <summary>
    /// 선수 정보
    /// </summary>
    public class KfaPlayer
    {
        /// <summary>선수명</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>소속팀 ID</summary>
        public string TeamId { get; set; } = string.Empty;

        /// <summary>소속팀명</summary>
        public string TeamName { get; set; } = string.Empty;

        /// <summary>포지션 (GK, DF, MF, FW)</summary>
        public string Position { get; set; } = string.Empty;

        /// <summary>등번호</summary>
        public string EntryNo { get; set; } = string.Empty;

        /// <summary>사진 경로</summary>
        public string PhotoPath { get; set; } = string.Empty;

        /// <summary>사진 파일명</summary>
        public string Photo { get; set; } = string.Empty;

        /// <summary>생년월일</summary>
        public string Birth { get; set; } = string.Empty;

        /// <summary>신장 (cm)</summary>
        public string Height { get; set; } = string.Empty;

        /// <summary>체중 (kg)</summary>
        public string Weight { get; set; } = string.Empty;

        /// <summary>종료 여부</summary>
        public bool IsEnded { get; set; }

        /// <summary>대회 구분명</summary>
        public string MgcNm { get; set; } = string.Empty;
    }

    /// <summary>
    /// 경기 결과 정보
    /// </summary>
    public class KfaMatchResult
    {
        /// <summary>개별 경기 ID (IDX)</summary>
        public string SingleIdx { get; set; } = string.Empty;

        /// <summary>소속 대회 IDX</summary>
        public string MatchIdx { get; set; } = string.Empty;

        /// <summary>경기 번호</summary>
        public string MatchNumber { get; set; } = string.Empty;

        /// <summary>조 (A/B 등)</summary>
        public string MatchGroup { get; set; } = string.Empty;

        /// <summary>경기 시간</summary>
        public string Time { get; set; } = string.Empty;

        /// <summary>경기장</summary>
        public string MatchArea { get; set; } = string.Empty;

        /// <summary>경기 일자</summary>
        public string MatchDate { get; set; } = string.Empty;

        /// <summary>홈팀명</summary>
        public string HomeTeam { get; set; } = string.Empty;

        /// <summary>어웨이팀명</summary>
        public string AwayTeam { get; set; } = string.Empty;

        /// <summary>홈 최종 스코어</summary>
        public string HomeScore { get; set; } = string.Empty;

        /// <summary>어웨이 최종 스코어</summary>
        public string AwayScore { get; set; } = string.Empty;

        /// <summary>홈 PK 스코어</summary>
        public string HomePkScore { get; set; } = string.Empty;

        /// <summary>어웨이 PK 스코어</summary>
        public string AwayPkScore { get; set; } = string.Empty;

        /// <summary>스코어 텍스트</summary>
        public string ScoreText { get; set; } = string.Empty;

        /// <summary>대회 구분명</summary>
        public string MgcNm { get; set; } = string.Empty;
    }

    /// <summary>
    /// 경기 상세 정보 (Nexacro SEARCH00.do 응답)
    /// </summary>
    public class KfaMatchDetail
    {
        /// <summary>개별 경기 ID</summary>
        public string SingleIdx { get; set; } = string.Empty;

        /// <summary>소속 대회 IDX</summary>
        public string MatchIdx { get; set; } = string.Empty;

        /// <summary>경기 번호</summary>
        public string MatchNumber { get; set; } = string.Empty;

        /// <summary>경기 일시</summary>
        public string MatchDate { get; set; } = string.Empty;

        /// <summary>경기장</summary>
        public string MatchArea { get; set; } = string.Empty;

        /// <summary>대회명</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>홈팀명</summary>
        public string HomeTeam { get; set; } = string.Empty;

        /// <summary>어웨이팀명</summary>
        public string AwayTeam { get; set; } = string.Empty;

        /// <summary>홈 최종 스코어</summary>
        public string HomeScoreFinal { get; set; } = string.Empty;

        /// <summary>어웨이 최종 스코어</summary>
        public string AwayScoreFinal { get; set; } = string.Empty;

        /// <summary>홈 전반 스코어</summary>
        public string HomeScoreFirstHalf { get; set; } = string.Empty;

        /// <summary>어웨이 전반 스코어</summary>
        public string AwayScoreFirstHalf { get; set; } = string.Empty;

        /// <summary>홈 후반 스코어</summary>
        public string HomeScoreSecondHalf { get; set; } = string.Empty;

        /// <summary>어웨이 후반 스코어</summary>
        public string AwayScoreSecondHalf { get; set; } = string.Empty;

        /// <summary>홈 PK 스코어</summary>
        public string HomeScorePk { get; set; } = string.Empty;

        /// <summary>어웨이 PK 스코어</summary>
        public string AwayScorePk { get; set; } = string.Empty;

        /// <summary>주심</summary>
        public string RefereeMain { get; set; } = string.Empty;

        /// <summary>날씨</summary>
        public string Weather { get; set; } = string.Empty;

        /// <summary>관중 수</summary>
        public string Viewers { get; set; } = string.Empty;

        /// <summary>경기 시간 (분)</summary>
        public string PlayingTime { get; set; } = string.Empty;

        /// <summary>총 경기 시간 텍스트</summary>
        public string TotalTime { get; set; } = string.Empty;

        /// <summary>홈팀 감독</summary>
        public string CoachHome { get; set; } = string.Empty;

        /// <summary>어웨이팀 감독</summary>
        public string CoachAway { get; set; } = string.Empty;

        /// <summary>홈 경고 수</summary>
        public string HomeYellowCount { get; set; } = string.Empty;

        /// <summary>어웨이 경고 수</summary>
        public string AwayYellowCount { get; set; } = string.Empty;

        /// <summary>홈 퇴장 수</summary>
        public string HomeRedCount { get; set; } = string.Empty;

        /// <summary>어웨이 퇴장 수</summary>
        public string AwayRedCount { get; set; } = string.Empty;

        /// <summary>대회 구분명</summary>
        public string MgcNm { get; set; } = string.Empty;

        /// <summary>타임라인 이벤트 (득점, 경고, 교체 등)</summary>
        public List<KfaMatchEvent> Events { get; set; } = [];

        /// <summary>홈팀 선발 라인업</summary>
        public List<KfaLineupPlayer> HomeStarters { get; set; } = [];

        /// <summary>홈팀 교체/후보</summary>
        public List<KfaLineupPlayer> HomeSubstitutes { get; set; } = [];

        /// <summary>어웨이팀 선발 라인업</summary>
        public List<KfaLineupPlayer> AwayStarters { get; set; } = [];

        /// <summary>어웨이팀 교체/후보</summary>
        public List<KfaLineupPlayer> AwaySubstitutes { get; set; } = [];
    }

    /// <summary>
    /// 경기 이벤트 (dsTimelineInfo)
    /// </summary>
    public class KfaMatchEvent
    {
        /// <summary>선수명</summary>
        public string PlayerName { get; set; } = string.Empty;

        /// <summary>이벤트 유형명 (득점, 경고, 교체IN, 교체OUT)</summary>
        public string EventType { get; set; } = string.Empty;

        /// <summary>이벤트 코드 (11=득점, 31=경고, 51=교체IN, 52=교체OUT)</summary>
        public string EventCode { get; set; } = string.Empty;

        /// <summary>이벤트 시간 (분)</summary>
        public string Time { get; set; } = string.Empty;

        /// <summary>등번호</summary>
        public string EntryNo { get; set; } = string.Empty;

        /// <summary>홈/어웨이 (H/A)</summary>
        public string Side { get; set; } = string.Empty;

        /// <summary>PK 여부</summary>
        public string IsPk { get; set; } = string.Empty;
    }

    /// <summary>
    /// 라인업 선수 정보
    /// </summary>
    public class KfaLineupPlayer
    {
        /// <summary>선수명</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>포지션 (GK, DF, MF, FW)</summary>
        public string Position { get; set; } = string.Empty;

        /// <summary>등번호</summary>
        public string EntryNo { get; set; } = string.Empty;

        /// <summary>출전 시간 (분)</summary>
        public string PlayTime { get; set; } = string.Empty;

        /// <summary>선발/후보 (S/R)</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>주장 여부 (Y/N)</summary>
        public string IsCaptain { get; set; } = string.Empty;

        /// <summary>득점 시간</summary>
        public string GoalTime { get; set; } = string.Empty;

        /// <summary>경고 시간</summary>
        public string YellowTime { get; set; } = string.Empty;

        /// <summary>퇴장 시간</summary>
        public string RedTime { get; set; } = string.Empty;

        /// <summary>도움 시간</summary>
        public string AssistTime { get; set; } = string.Empty;
    }

    /// <summary>
    /// 크롤링 결과 통계
    /// </summary>
    public class CrawlStats
    {
        public int MatchCount { get; set; }
        public int MatchResultCount { get; set; }
        public int MatchDetailCount { get; set; }
        public int TeamCount { get; set; }
        public int PlayerCount { get; set; }
        public TimeSpan Elapsed { get; set; }
    }

    /// <summary>
    /// 크롤링 대상 등급 설정
    /// </summary>
    public static class GradeFilter
    {
        /// <summary>
        /// MGC_IDX 코드 → 등급명 매핑
        /// 리그: 1=초등, 2=중등, 3=고등, 91=초등(저), 92=중등(저), 93=고등(저)
        /// 대회: 51=초등, 52=중등, 53=고등
        /// </summary>
        public static readonly Dictionary<string, string[]> GradeCodes = new()
        {
            ["초등"] = ["1", "51", "91"],
            ["중등"] = ["2", "52", "92"],
            ["고등"] = ["3", "53", "93"]
        };

        /// <summary>
        /// 전체 등급 코드 목록
        /// </summary>
        public static string[] GetAllCodes()
        {
            return GradeCodes.Values.SelectMany(v => v).ToArray();
        }

        /// <summary>
        /// 지정된 등급의 코드 목록
        /// </summary>
        public static string[] GetCodes(params string[] grades)
        {
            return grades
                .Where(g => GradeCodes.ContainsKey(g))
                .SelectMany(g => GradeCodes[g])
                .ToArray();
        }
    }
}
