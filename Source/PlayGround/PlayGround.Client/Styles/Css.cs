namespace PlayGround.Client.Styles
{
    /// <summary>
    /// Tailwind CSS 클래스 상수
    /// </summary>
    public static class Css
    {
        /// <summary>
        /// 텍스트 스타일
        /// </summary>
        public static class Typography
        {
            public const string PageTitle    = "text-xl font-bold text-navy mb-1";
            public const string PageSubtitle = "text-[13px] text-text-secondary";
            public const string SectionTitle = "text-[26px] font-bold text-white leading-[1.4] mb-3";
            public const string BodyOnDark   = "text-sm text-white/55 leading-[1.7] mb-9";
            public const string FooterNote   = "text-center mt-[18px] text-[13px] text-text-secondary";
        }

        /// <summary>
        /// 페이지 레이아웃 구조
        /// </summary>
        public static class Layout
        {
            public const string PageCenter        = "min-h-screen bg-[#FAFAFA] flex items-center justify-center p-6";
            public const string SplitCard         = "flex w-full max-w-[1000px] min-h-[640px] bg-surface rounded-2xl " +
                                                    "border border-border overflow-hidden shadow-[0_4px_24px_rgba(0,0,0,0.07)] " +
                                                    "max-md:flex-col max-md:max-w-[480px] max-md:min-h-0";
            public const string DarkSidePanel     = "w-[420px] flex-shrink-0 relative overflow-hidden " +
                                                    "bg-[linear-gradient(165deg,#1E3A5F_0%,#152D4A_100%)] " +
                                                    "px-10 py-11 flex flex-col justify-between " +
                                                    "max-md:w-full max-md:px-6 max-md:py-7";
            public const string ContentPanel      = "flex-1 px-10 py-11 flex flex-col justify-center overflow-y-auto " +
                                                    "max-md:px-6 max-md:py-7";
            public const string DecorCircleTop    = "absolute -top-[60px] -right-[60px] w-[200px] h-[200px] rounded-full bg-white/[0.03]";
            public const string DecorCircleBottom = "absolute -bottom-[80px] -left-10 w-[240px] h-[240px] rounded-full bg-white/[0.02]";
            public const string PanelLogo         = "text-xl font-bold text-white relative z-10";
            public const string PanelContent      = "flex-1 flex flex-col justify-center relative z-10 max-md:hidden";
            public const string PanelFooter       = "text-[11px] text-white/30 relative z-10";
        }

        /// <summary>
        /// 버튼
        /// </summary>
        public static class Button
        {
            public const string Primary = "btn-auth-submit";
            public const string Navy    = "btn-auth-navy";
            public const string Social  = "flex items-center gap-3 w-full px-4 py-[11px] rounded-btn text-[13px] font-semibold " +
                                          "border border-border bg-surface text-text-primary cursor-pointer " +
                                          "hover:border-slate-light hover:bg-[#FAFAFA] transition-all";
        }

        /// <summary>
        /// 폼 요소
        /// </summary>
        public static class Form
        {
            public const string FieldGroup = "mb-3.5";
            public const string Label      = "block text-xs font-semibold text-slate mb-1.5";
            public const string Row        = "flex items-center justify-between mb-[18px]";
            public const string CheckLabel = "flex items-center gap-1.5 text-xs text-text-secondary cursor-pointer";
        }

        /// <summary>
        /// 알림 메시지 배너
        /// </summary>
        public static class Alert
        {
            public const string Error   = "px-3.5 py-2.5 rounded-btn text-xs font-semibold mb-4 bg-red-50 text-red-600 border border-red-200";
            public const string Success = "px-3.5 py-2.5 rounded-btn text-xs font-semibold mb-4 bg-green-50 text-green-600 border border-green-200";
        }

        /// <summary>
        /// 구분선
        /// </summary>
        public static class Divider
        {
            public const string Container = "flex items-center gap-3.5 mb-5";
            public const string Line      = "flex-1 h-px bg-border";
            public const string Text      = "text-[11px] font-semibold text-text-light tracking-wide";
        }

        /// <summary>
        /// 뱃지
        /// </summary>
        public static class Badge
        {
            public const string Subtle = "inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-[10px] font-semibold bg-navy-subtle text-navy";
        }

        /// <summary>
        /// 링크
        /// </summary>
        public static class Link
        {
            public const string Navy = "text-navy font-bold no-underline hover:text-primary";
            public const string Skip = "block text-center text-xs text-text-light mt-2.5 cursor-pointer hover:text-text-secondary transition-colors";
        }

        /// <summary>
        /// 아이콘 박스 (다크 패널용)
        /// </summary>
        public static class Icon
        {
            public static string RoundedBox(string bgColorClass) =>
                $"w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0 {bgColorClass}";

            public static string Svg(string strokeColorClass) =>
                $"w-4 h-4 {strokeColorClass}";

            public const string BgNavy   = "bg-[rgba(96,165,250,0.15)]";
            public const string BgTeal   = "bg-[rgba(13,148,136,0.15)]";
            public const string BgOrange = "bg-[rgba(255,107,53,0.15)]";
            public const string BgViolet = "bg-[rgba(109,40,217,0.15)]";

            public const string StrokeNavy   = "stroke-[#60A5FA]";
            public const string StrokeTeal   = "stroke-[#2DD4BF]";
            public const string StrokeOrange = "stroke-[#FF8F5E]";
            public const string StrokeViolet = "stroke-[#A78BFA]";
        }

        /// <summary>
        /// 기능 목록 (다크 패널용)
        /// </summary>
        public static class List
        {
            public const string Container = "flex flex-col gap-4";
            public const string IconItem  = "flex items-center gap-3 text-[13px] font-medium text-white/70";
        }

        /// <summary>
        /// 탭
        /// </summary>
        public static class Tabs
        {
            public const string Container = "flex bg-border-light rounded-btn p-[3px] mb-6";

            public static string Tab(bool isActive) => isActive
                ? "flex-1 py-2 px-4 rounded-[6px] text-[13px] font-semibold text-center no-underline text-navy bg-surface shadow-[0_1px_3px_rgba(0,0,0,0.06)] transition-all"
                : "flex-1 py-2 px-4 rounded-[6px] text-[13px] font-semibold text-center no-underline text-text-light hover:text-text-secondary cursor-pointer transition-all";
        }

        /// <summary>
        /// 선택형 카드
        /// </summary>
        public static class Card
        {
            private const string SelectableBase = "py-3.5 px-2.5 rounded-btn border-[1.5px] text-center cursor-pointer transition-all";

            public static string Selectable(bool isSelected) => isSelected
                ? $"{SelectableBase} border-navy bg-navy-subtle"
                : $"{SelectableBase} border-border bg-surface hover:border-navy hover:bg-navy-subtle";

            private const string IconBase = "w-8 h-8 rounded-lg flex items-center justify-center mx-auto mb-1.5 text-sm";

            public static string CardIcon(bool isSelected) => isSelected
                ? $"{IconBase} bg-navy text-white"
                : $"{IconBase} bg-border-light text-text-light";

            public static string CardName(bool isSelected) => isSelected
                ? "text-[11px] font-bold text-navy"
                : "text-[11px] font-semibold text-text-secondary";

            public static string CardDesc(bool isSelected) => isSelected
                ? "text-[10px] text-slate-light mt-0.5 leading-tight"
                : "text-[10px] text-text-light mt-0.5 leading-tight";
        }

        /// <summary>
        /// 단계 표시기
        /// </summary>
        public static class Steps
        {
            public const string Container = "flex items-center justify-center gap-2 mb-6";

            public static string Dot(int step, int current) =>
                step < current  ? "w-2 h-2 rounded-full bg-trust transition-all" :
                step == current ? "w-6 h-2 rounded bg-navy transition-all" :
                                  "w-2 h-2 rounded-full bg-border transition-all";
        }
    }
}
