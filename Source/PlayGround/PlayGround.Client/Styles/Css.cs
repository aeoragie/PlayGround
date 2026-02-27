namespace PlayGround.Client.Styles
{
    /// <summary>
    /// Tailwind CSS 클래스 상수
    /// </summary>
    public static class Css
    {
        // 하위 호환 별칭 (Auth 하위로 이동된 클래스)
        public static class Layout
        {
            public const string PageCenter        = Auth.PageCenter;
            public const string SplitCard         = Auth.SplitCard;
            public const string DarkSidePanel     = Auth.DarkSidePanel;
            public const string ContentPanel      = Auth.ContentPanel;
            public const string DecorCircleTop    = Auth.DecorCircleTop;
            public const string DecorCircleBottom = Auth.DecorCircleBottom;
            public const string PanelLogo         = Auth.PanelLogo;
            public const string PanelContent      = Auth.PanelContent;
            public const string PanelFooter       = Auth.PanelFooter;
        }

        public static class Steps
        {
            public const string Container = Auth.Steps.Container;

            public static string Dot(int step, int current) =>
                Auth.Steps.Dot(step, current);
        }

        public static class Tabs
        {
            public const string Container = Auth.Tabs.Container;

            public static string Tab(bool isActive) =>
                Auth.Tabs.Tab(isActive);
        }

        public static class Card
        {
            public static string Selectable(bool isSelected) =>
                Auth.RoleCard.Container(isSelected);

            public static string CardIcon(bool isSelected) =>
                Auth.RoleCard.Icon(isSelected);

            public static string CardName(bool isSelected) =>
                Auth.RoleCard.Name(isSelected);

            public static string CardDesc(bool isSelected) =>
                Auth.RoleCard.Desc(isSelected);
        }

        // ────────────────────────────────────────────
        //  공통 타이포그래피
        // ────────────────────────────────────────────

        public static class Typography
        {
            public const string PageTitle    = "text-xl font-bold text-navy mb-1";
            public const string PageSubtitle = "text-[13px] text-text-secondary";
            public const string SectionLabel = "section-label";
            public const string SectionTitle = "section-title";
            public const string SectionDesc  = "section-desc";
            public const string BrandTitle   = "text-[26px] font-bold text-white leading-[1.4] mb-3";
            public const string BodyOnDark   = "text-sm text-white/55 leading-[1.7] mb-9";
            public const string FooterNote   = "text-center mt-[18px] text-[13px] text-text-secondary";
        }

        // ────────────────────────────────────────────
        //  공통 버튼
        // ────────────────────────────────────────────

        public static class Button
        {
            public const string Primary     = "btn-auth-submit";
            public const string Navy        = "btn-auth-navy";
            public const string Ghost       = "btn-ghost";
            public const string Sm          = "btn-sm";
            public const string SmAccent    = "btn-sm-accent";
            public const string HeroPrimary = "btn-hero-primary";
            public const string HeroSecondary = "btn-hero-secondary";
            public const string Final       = "btn-final";
            public const string Social      = "flex items-center gap-3 w-full px-4 py-[11px] rounded-btn text-[13px] font-semibold " +
                                              "border border-border bg-surface text-text-primary cursor-pointer " +
                                              "hover:border-slate-light hover:bg-surface-alt transition-all";
        }

        // ────────────────────────────────────────────
        //  공통 뱃지
        // ────────────────────────────────────────────

        public static class Badge
        {
            public const string Navy   = "badge-navy";
            public const string Trust  = "badge-trust";
            public const string Accent = "badge-accent";
            public const string Violet = "badge-violet";
            public const string Subtle = "inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-[10px] font-semibold bg-navy-subtle text-navy";

            public static string Of(string variant) => $"badge-{variant}";
        }

        // ────────────────────────────────────────────
        //  공통 폼 요소
        // ────────────────────────────────────────────

        public static class Form
        {
            public const string FieldGroup = "mb-3.5";
            public const string Label      = "block text-xs font-semibold text-slate mb-1.5";
            public const string Input      = "auth-input";
            public const string Select     = "auth-select";
            public const string Row        = "flex items-center justify-between mb-[18px]";
            public const string CheckLabel = "flex items-center gap-1.5 text-xs text-text-secondary cursor-pointer";
        }

        // ────────────────────────────────────────────
        //  공통 카드 / 패널
        // ────────────────────────────────────────────

        public static class Panel
        {
            public const string Default = "panel";
            public const string Kpi     = "kpi-card";
            public const string Header  = "flex items-center justify-between mb-3.5";
            public const string Title   = "text-[15px] font-bold text-navy";
            public const string Link    = "text-xs font-semibold text-primary hover:text-primary-dark cursor-pointer transition-colors";
        }

        // ────────────────────────────────────────────
        //  공통 알림 메시지
        // ────────────────────────────────────────────

        public static class Alert
        {
            public const string Error   = "px-3.5 py-2.5 rounded-btn text-xs font-semibold mb-4 bg-error-subtle text-error border border-red-200";
            public const string Success = "px-3.5 py-2.5 rounded-btn text-xs font-semibold mb-4 bg-green-50 text-success border border-green-200";
            public const string Warning = "px-3.5 py-2.5 rounded-btn text-xs font-semibold mb-4 bg-amber-50 text-warning border border-amber-200";
        }

        // ────────────────────────────────────────────
        //  공통 구분선
        // ────────────────────────────────────────────

        public static class Divider
        {
            public const string Container = "flex items-center gap-3.5 mb-5";
            public const string Line      = "flex-1 h-px bg-border";
            public const string Text      = "text-[11px] font-semibold text-text-light tracking-wide";
        }

        // ────────────────────────────────────────────
        //  공통 링크
        // ────────────────────────────────────────────

        public static class Link
        {
            public const string Navy    = "text-navy font-bold no-underline hover:text-primary transition-colors";
            public const string Accent  = "text-primary font-semibold text-xs hover:text-primary-dark cursor-pointer transition-colors";
            public const string Skip    = "block text-center text-xs text-text-light mt-2.5 cursor-pointer hover:text-text-secondary transition-colors";
        }

        // ────────────────────────────────────────────
        //  공통 그리드
        // ────────────────────────────────────────────

        public static class Grid
        {
            public const string Col2 = "grid grid-cols-2 gap-4";
            public const string Col3 = "grid grid-cols-3 gap-4";
            public const string Col4 = "grid grid-cols-4 gap-4";
        }

        // ────────────────────────────────────────────
        //  Auth 페이지 레이아웃
        // ────────────────────────────────────────────

        public static class Auth
        {
            public const string PageSubtitle      = "text-[13px] text-text-secondary";
            public const string PageCenter        = "min-h-screen bg-bg flex items-center justify-center p-6";
            public const string SplitCard         = "flex w-full max-w-[1000px] min-h-[640px] bg-surface rounded-lg " +
                                                    "border border-border overflow-hidden shadow-auth " +
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

            public static class Tabs
            {
                public const string Container = "flex bg-border-light rounded-btn p-[3px] mb-6";

                public static string Tab(bool isActive) => isActive
                    ? "flex-1 py-2 px-4 rounded-[6px] text-[13px] font-semibold text-center no-underline text-navy bg-surface shadow-sm transition-all"
                    : "flex-1 py-2 px-4 rounded-[6px] text-[13px] font-semibold text-center no-underline text-text-light hover:text-text-secondary cursor-pointer transition-all";
            }

            public static class RoleCard
            {
                private const string Base = "py-3.5 px-2.5 rounded-btn border-[1.5px] text-center cursor-pointer transition-all";

                public static string Container(bool isSelected) => isSelected
                    ? $"{Base} border-navy bg-navy-subtle"
                    : $"{Base} border-border bg-surface hover:border-navy hover:bg-navy-subtle";

                private const string IconBase = "w-8 h-8 rounded-lg flex items-center justify-center mx-auto mb-1.5 text-sm";

                public static string Icon(bool isSelected) => isSelected
                    ? $"{IconBase} bg-navy text-white"
                    : $"{IconBase} bg-border-light text-text-light";

                public static string Name(bool isSelected) => isSelected
                    ? "text-[11px] font-bold text-navy"
                    : "text-[11px] font-semibold text-text-secondary";

                public static string Desc(bool isSelected) => isSelected
                    ? "text-[10px] text-slate-light mt-0.5 leading-tight"
                    : "text-[10px] text-text-light mt-0.5 leading-tight";
            }

            public static class Steps
            {
                public const string Container = "flex items-center justify-center gap-2 mb-6";

                public static string Dot(int step, int current) =>
                    step < current  ? "w-2 h-2 rounded-full bg-trust transition-all" :
                    step == current ? "w-6 h-2 rounded bg-navy transition-all" :
                                      "w-2 h-2 rounded-full bg-border transition-all";
            }
        }

        // ────────────────────────────────────────────
        //  Landing 페이지
        // ────────────────────────────────────────────

        public static class Landing
        {
            public const string Section     = "py-20 px-10 max-md:py-12 max-md:px-5";
            public const string SectionAlt  = "py-20 px-10 bg-surface-alt max-md:py-12 max-md:px-5";
            public const string Inner       = "max-w-[1120px] mx-auto";

            public static class Gnb
            {
                public const string Bar       = "fixed top-0 left-0 right-0 h-[60px] flex items-center px-10 z-50 transition-all";
                public const string Scrolled  = "bg-white/95 backdrop-blur-[12px] shadow-[0_1px_8px_rgba(0,0,0,0.06)]";
                public const string Logo      = "text-lg font-bold text-navy";
                public const string NavLink   = "text-[13px] font-semibold text-slate hover:text-navy transition-colors cursor-pointer";
                public const string Cta       = "px-5 py-2 rounded-btn text-[13px] font-bold bg-primary text-white " +
                                                "hover:bg-primary-dark transition-colors cursor-pointer";
            }

            public static class Hero
            {
                public const string Container  = "pt-[140px] pb-20 px-10 text-center max-md:pt-24 max-md:pb-12 max-md:px-5";
                public const string Badge      = "inline-flex items-center gap-2 px-3.5 py-1.5 rounded-full bg-navy-subtle text-navy text-[13px] font-semibold mb-5";
                public const string BadgeDot   = "w-2 h-2 rounded-full bg-trust";
                public const string Title      = "text-[44px] font-bold text-navy leading-tight tracking-[-0.5px] mb-4 max-md:text-[28px]";
                public const string Desc       = "text-[16px] text-text-secondary leading-relaxed max-w-[520px] mx-auto mb-10";
                public const string Actions    = "flex items-center justify-center gap-3 mb-12 max-md:flex-col";
            }

            public static class Search
            {
                public const string Bar    = "flex items-center border-[1.5px] border-border rounded-card bg-surface overflow-hidden";
                public const string Select = "px-4 py-2.5 text-[13px] bg-transparent border-r border-border-light outline-none text-text-primary appearance-none cursor-pointer";
                public const string Input  = "flex-1 px-4 py-2.5 text-[13px] bg-transparent border-none outline-none text-text-primary placeholder:text-text-light";
                public const string Btn    = "px-7 py-2.5 bg-navy text-white text-[13px] font-bold border-none cursor-pointer hover:bg-navy-light transition-colors";
            }

            public static class SportChip
            {
                private const string Base = "px-4 py-1.5 rounded-full text-[13px] font-semibold border-[1.5px] cursor-pointer transition-all";

                public static string Of(bool isActive) => isActive
                    ? $"{Base} border-navy bg-navy text-white"
                    : $"{Base} border-border bg-surface text-text-secondary hover:border-navy hover:text-navy";
            }

            public static class TrustStats
            {
                public const string Container = "flex items-center justify-center gap-10 mt-8";
                public const string Divider   = "w-px h-8 bg-border";
                public const string Number    = "text-2xl font-bold text-navy";
                public const string Label     = "text-[12px] text-text-secondary mt-0.5";
            }

            public static class TeamCard
            {
                public const string Container   = "bg-surface rounded-card border border-border p-5 cursor-pointer " +
                                                  "hover:border-navy hover:shadow-card hover:-translate-y-0.5 transition-all";
                public const string Header      = "flex items-center gap-3 mb-3";
                public const string Avatar      = "w-10 h-10 rounded-lg flex items-center justify-center text-white font-bold text-sm flex-shrink-0";
                public const string Name        = "text-sm font-bold text-navy";
                public const string Badge       = "inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-[10px] font-semibold bg-trust-subtle text-trust";
                public const string Desc        = "text-[13px] text-text-secondary leading-relaxed mb-3";
                public const string MetaRow     = "flex items-center gap-3 text-[11px] text-text-light";
                public const string Rating      = "text-primary font-bold";
            }

            public static class ShowcaseCard
            {
                public const string Container  = "min-w-[280px] bg-surface rounded-card border border-border p-5 snap-start " +
                                                 "hover:border-navy hover:shadow-card transition-all";
                public const string Avatar     = "w-12 h-12 rounded-full bg-navy flex items-center justify-center text-white font-bold text-lg mb-3";
                public const string Name       = "text-sm font-bold text-navy";
                public const string Position   = "text-[11px] text-text-secondary mt-0.5";
                public const string MetricGrid = "grid grid-cols-3 gap-2 mt-3";
                public const string Metric     = "text-center py-1.5 rounded-[6px] bg-bg";
                public const string MetricVal  = "text-sm font-bold text-navy";
                public const string MetricLbl  = "text-[10px] text-text-light mt-0.5";
            }

            public static class ReviewCard
            {
                public const string Container  = "bg-surface rounded-card border border-border p-5 " +
                                                 "hover:shadow-sm transition-all";
                public const string Header     = "flex items-center gap-3 mb-3";
                public const string Avatar     = "w-8 h-8 rounded-full bg-navy-subtle flex items-center justify-center text-navy text-xs font-semibold";
                public const string Name       = "text-[13px] font-semibold text-text-primary";
                public const string Verified   = "text-[10px] text-trust";
                public const string Stars      = "flex gap-0.5 mb-2";
                public const string Body       = "text-[13px] text-text-secondary leading-relaxed";
            }

            public static class ValueCard
            {
                public const string Container  = "bg-surface rounded-card border border-border py-7 px-6 " +
                                                 "hover:border-navy hover:-translate-y-0.5 transition-all";
                public const string Icon       = "w-11 h-11 rounded-[10px] flex items-center justify-center mb-4";
                public const string Title      = "text-[15px] font-bold text-navy mb-2";
                public const string Desc       = "text-[13px] text-text-secondary leading-relaxed mb-4";
                public const string Feature    = "flex items-center gap-2 text-[12px] text-slate mb-1.5";
                public const string FeatureDot = "w-1 h-1 rounded-full flex-shrink-0";
                public const string LinkText   = "text-primary text-[13px] font-semibold mt-3 inline-flex items-center gap-1 " +
                                                 "hover:gap-2 transition-all cursor-pointer";
            }

            public static class NumbersBar
            {
                public const string Container = "grid grid-cols-4 gap-4 bg-surface-alt rounded-card py-12 px-10 " +
                                                "max-md:grid-cols-2";
                public const string Value     = "text-[32px] font-bold text-navy";
                public const string Label     = "text-[13px] text-text-secondary mt-1";
            }

            public static class FinalCta
            {
                public const string Container = "bg-navy py-[72px] px-10 text-center";
                public const string Title     = "text-[28px] font-bold text-white mb-4";
                public const string Desc      = "text-[15px] text-white/60 mb-8 max-w-[480px] mx-auto";
            }

            public static class Footer
            {
                public const string Container  = "border-t border-border py-10 px-10";
                public const string Inner      = "max-w-[1120px] mx-auto grid grid-cols-3 gap-10 max-md:grid-cols-1";
                public const string Logo       = "text-[15px] font-bold text-navy mb-2";
                public const string Desc       = "text-[12px] text-text-light leading-relaxed";
                public const string GroupTitle  = "text-[12px] font-bold text-navy mb-3";
                public const string GroupLink   = "text-[12px] text-text-secondary hover:text-navy cursor-pointer transition-colors";
                public const string Bottom     = "max-w-[1120px] mx-auto mt-8 pt-5 border-t border-border-light " +
                                                 "flex items-center justify-between text-[11px] text-text-light";
            }
        }

        // ────────────────────────────────────────────
        //  Player Dashboard 페이지
        // ────────────────────────────────────────────

        public static class Dashboard
        {
            public const string Container = "max-w-[1080px] mx-auto px-8 py-6 pb-[60px]";

            public static class Gnb
            {
                public const string Bar       = "sticky top-0 z-50 h-14 flex items-center px-8 bg-white/95 backdrop-blur-[12px] border-b border-border";
                public const string Logo      = "text-[17px] font-bold text-navy";
                public const string NavLink   = "text-[13px] font-semibold text-text-secondary hover:text-navy transition-colors cursor-pointer";

                public static string NavLinkActive(bool isActive) => isActive
                    ? "text-[13px] font-semibold text-navy border-b-2 border-navy pb-[17px]"
                    : "text-[13px] font-semibold text-text-secondary hover:text-navy transition-colors cursor-pointer";
            }

            public static class ProfileHero
            {
                public const string Container   = "flex items-start gap-6 mb-7 max-md:flex-col max-md:items-center";
                public const string Avatar      = "w-28 h-28 rounded-full bg-navy flex items-center justify-center text-white text-[40px] font-bold " +
                                                  "shadow-[0_4px_12px_rgba(30,58,95,0.2)] relative group cursor-pointer";
                public const string AvatarOverlay = "absolute inset-0 rounded-full bg-black/45 flex flex-col items-center justify-center " +
                                                    "opacity-0 group-hover:opacity-100 transition-opacity";
                public const string Info        = "flex-1";
                public const string Name        = "text-2xl font-bold text-navy mb-1";
                public const string Sub         = "text-[13px] text-text-secondary mb-3";
                public const string BadgeRow    = "flex flex-wrap gap-1.5 mb-4";
                public const string Actions     = "flex items-center gap-2";
                public const string StatRow     = "flex items-center gap-3 mt-4";
                public const string StatPill    = "flex items-center gap-3 px-5 py-2.5 rounded-btn border border-border bg-surface";
                public const string StatLabel   = "text-[11px] text-text-light";
                public const string StatValue   = "text-xl font-bold text-navy";
            }

            public static class Tabs
            {
                public const string Container = "flex border-b border-border mb-5 overflow-x-auto";

                public static string Tab(bool isActive) => isActive
                    ? "px-5 py-3 text-[13px] font-semibold text-navy border-b-2 border-navy cursor-pointer whitespace-nowrap"
                    : "px-5 py-3 text-[13px] font-semibold text-text-light hover:text-text-secondary cursor-pointer transition-colors whitespace-nowrap";
            }

            public static class Timeline
            {
                public const string Container  = "relative pl-6 border-l-2 border-border";
                public const string Item       = "relative pb-5";
                public const string Dot        = "absolute -left-[29px] top-0.5 w-2.5 h-2.5 rounded-full border-2 border-navy bg-surface";
                public const string DotCurrent = "absolute -left-[29px] top-0.5 w-2.5 h-2.5 rounded-full border-2 border-navy bg-navy";
                public const string Period     = "text-[11px] text-text-light mb-0.5";
                public const string Title      = "text-[13px] font-semibold text-navy";
                public const string Sub        = "text-[12px] text-text-secondary";
            }

            public static class MatchCard
            {
                public const string Container = "flex items-center gap-3.5 p-3 bg-bg rounded-btn";
                public const string Date      = "text-[11px] text-text-light w-[60px] flex-shrink-0";
                public const string Teams     = "flex-1 text-[13px] text-text-primary";
                public const string Score     = "text-lg font-bold text-navy w-[60px] text-center flex-shrink-0";
                public const string PerfMain  = "text-[13px] font-bold text-primary";
                public const string PerfSub   = "text-[11px] text-text-light";
            }

            public static class Portfolio
            {
                public const string Grid      = "grid grid-cols-3 gap-3 max-md:grid-cols-2";
                public const string Card      = "rounded-btn border border-border overflow-hidden cursor-pointer " +
                                                "hover:shadow-card hover:-translate-y-0.5 transition-all";
                public const string Featured  = "rounded-btn border border-primary overflow-hidden cursor-pointer " +
                                                "hover:shadow-card hover:-translate-y-0.5 transition-all";
                public const string Thumb     = "h-[120px] bg-navy-subtle relative flex items-center justify-center";
                public const string PlayBtn   = "w-10 h-10 rounded-full bg-[rgba(30,58,95,0.8)] flex items-center justify-center text-white";
                public const string VideoTag  = "absolute top-2 right-2 px-2 py-0.5 rounded text-[10px] font-semibold bg-black/50 text-white";
                public const string Info      = "p-3";
                public const string Title     = "text-[13px] font-semibold text-text-primary";
                public const string Meta      = "text-[11px] text-text-light mt-1";
            }

            public static class StatBar
            {
                public const string Row       = "flex items-center gap-3";
                public const string Label     = "w-20 text-[12px] text-text-secondary flex-shrink-0";
                public const string BgBar     = "stat-bar-bg";
                public const string FillBar   = "stat-bar";
                public const string Value     = "w-8 text-right text-[12px] font-bold text-navy flex-shrink-0";
            }

            public static class Evaluation
            {
                public const string Grade       = "w-14 h-14 rounded-full bg-navy flex items-center justify-center text-white text-2xl font-bold";
                public const string Stars       = "text-warning tracking-[1px]";
                public const string SectionBody = "border-l-[3px] border-border bg-bg rounded-btn p-4 text-[13px] text-text-secondary leading-relaxed";
                public const string Writer      = "text-[11px] text-text-light";

                public static string HistoryGrade(string grade) => grade switch
                {
                    "A" => "w-8 h-8 rounded-full bg-trust text-white text-xs font-bold flex items-center justify-center",
                    "B" => "w-8 h-8 rounded-full bg-navy text-white text-xs font-bold flex items-center justify-center",
                    "C" => "w-8 h-8 rounded-full bg-warning text-white text-xs font-bold flex items-center justify-center",
                    _   => "w-8 h-8 rounded-full bg-slate text-white text-xs font-bold flex items-center justify-center",
                };
            }

            public static class SeasonChip
            {
                private const string Base = "px-3.5 py-1.5 rounded-full text-[12px] font-semibold cursor-pointer transition-all";

                public static string Of(bool isActive) => isActive
                    ? $"{Base} bg-navy text-white"
                    : $"{Base} bg-border-light text-text-secondary hover:bg-navy-subtle hover:text-navy";
            }

            public static class Honor
            {
                public const string Item = "flex items-center gap-3 py-3 border-b border-border-light last:border-0";
                public const string Icon = "w-9 h-9 rounded-full flex items-center justify-center text-sm flex-shrink-0";
                public const string Name = "text-[13px] font-semibold text-text-primary";
                public const string Desc = "text-[11px] text-text-light";
            }

            public const string PrivateBanner = "px-4 py-3 rounded-btn bg-[#FEF3C7] border border-warning text-[13px] text-[#92400E]";
        }

        // ────────────────────────────────────────────
        //  다크 패널 아이콘 (Auth 좌측)
        // ────────────────────────────────────────────

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

        // ────────────────────────────────────────────
        //  다크 패널 리스트 (Auth 좌측)
        // ────────────────────────────────────────────

        public static class List
        {
            public const string Container = "flex flex-col gap-4";
            public const string IconItem  = "flex items-center gap-3 text-[13px] font-medium text-white/70";
        }
    }
}
