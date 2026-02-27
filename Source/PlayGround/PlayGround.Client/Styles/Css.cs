namespace PlayGround.Client.Styles
{
    /// <summary>
    /// Tailwind CSS 클래스 상수
    /// </summary>
    public static class Css
    {
        /// <summary>
        /// 알림 메시지 배너
        /// </summary>
        public static class Alert
        {
            public const string Error = "px-3.5 py-2.5 rounded-btn text-xs font-semibold mb-4 bg-red-50 text-red-600 border border-red-200";
            public const string Success = "px-3.5 py-2.5 rounded-btn text-xs font-semibold mb-4 bg-green-50 text-green-600 border border-green-200";
        }

        /// <summary>
        /// 폼 요소
        /// </summary>
        public static class Form
        {
            public const string Label = "block text-xs font-semibold text-slate mb-1.5";
            public const string FieldGroup = "mb-3.5";
        }

        /// <summary>
        /// 인증 페이지 공통
        /// </summary>
        public static class Auth
        {
            public const string PageTitle = "text-xl font-bold text-navy mb-1";
            public const string PageSubtitle = "text-[13px] text-text-secondary";
            public const string FooterText = "text-center mt-[18px] text-[13px] text-text-secondary";
            public const string Link = "text-navy font-bold no-underline hover:text-primary";
            public const string SkipLink = "block text-center text-xs text-text-light mt-2.5 cursor-pointer hover:text-text-secondary transition-colors";
        }

        /// <summary>
        /// 구분선 (소셜 / 이메일 구분용)
        /// </summary>
        public static class Divider
        {
            public const string Container = "flex items-center gap-3.5 mb-5";
            public const string Line = "flex-1 h-px bg-border";
            public const string Text = "text-[11px] font-semibold text-text-light tracking-wide";
        }

        /// <summary>
        /// 뱃지 (태그/라벨 형태)
        /// </summary>
        public static class Badge
        {
            public const string Optional = "inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-[10px] font-semibold bg-navy-subtle text-navy";
        }
    }
}
