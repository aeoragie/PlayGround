/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './**/*.razor',
    './**/*.html',
    './**/*.cs',
  ],
  theme: {
    extend: {
      colors: {
        // PlayGround 디자인 시스템
        primary: {
          DEFAULT: '#FF6B35',
          light: '#FF8F5E',
          dark: '#E55A2B',
          subtle: '#FFF7F3',
        },
        secondary: {
          DEFAULT: '#2EC4B6',
          dark: '#1FA89C',
          subtle: '#F0FAF9',
        },
        navy: {
          DEFAULT: '#1E3A5F',
          light: '#2A4A73',
          dark: '#152D4A',
          subtle: '#F0F4F8',
        },
        trust: {
          DEFAULT: '#0D9488',
          light: '#14B8A6',
          subtle: '#F0FDFA',
        },
        violet: {
          DEFAULT: '#6D28D9',
          subtle: '#F5F3FF',
        },
        slate: {
          DEFAULT: '#475569',
          light: '#64748B',
        },
        success: '#16A34A',
        warning: '#D97706',
        error: {
          DEFAULT: '#DC2626',
          subtle: '#FEF2F2',
        },
        surface: {
          DEFAULT: '#FFFFFF',
          alt: '#F8FAFC',
        },
        bg: '#FAFAFA',
        border: {
          DEFAULT: '#E5E7EB',
          light: '#F3F4F6',
        },
        'text-primary': '#1F2937',
        'text-secondary': '#6B7280',
        'text-light': '#9CA3AF',
      },
      fontFamily: {
        sans: ['Plus Jakarta Sans', 'Pretendard', '-apple-system', 'sans-serif'],
      },
      borderRadius: {
        'card': '12px',
        'btn': '8px',
        'input': '8px',
        'lg': '16px',
      },
      boxShadow: {
        'card': '0 4px 16px rgba(0,0,0,0.07)',
        'sm': '0 1px 3px rgba(0,0,0,0.05)',
        'lg': '0 8px 32px rgba(0,0,0,0.08)',
        'auth': '0 4px 24px rgba(0,0,0,0.07)',
        'accent': '0 2px 8px rgba(255,107,53,0.2)',
        'accent-hover': '0 4px 12px rgba(255,107,53,0.3)',
        'focus': '0 0 0 3px rgba(30,58,95,0.08)',
      },
    },
  },
  plugins: [],
}
