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
        public string MatchIdx { get; set; } = string.Empty;
        public string MatchNumber { get; set; } = string.Empty;
        public string MatchDate { get; set; } = string.Empty;
        public string MatchTime { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public string HomeScore { get; set; } = string.Empty;
        public string AwayScore { get; set; } = string.Empty;
    }

    /// <summary>
    /// 크롤링 결과 통계
    /// </summary>
    public class CrawlStats
    {
        public int MatchCount { get; set; }
        public int TeamCount { get; set; }
        public int PlayerCount { get; set; }
        public int MatchResultCount { get; set; }
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
