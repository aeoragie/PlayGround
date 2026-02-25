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
          subtle: '#FFF5EF',
        },
        secondary: {
          DEFAULT: '#2EC4B6',
          dark: '#1FA89C',
          subtle: '#F0FAF9',
        },
        surface: '#FFFFFF',
        bg: '#F8F6F3',
        border: '#E8E3DD',
        'border-light': '#F0EBE5',
        'text-primary': '#1A1D21',
        'text-secondary': '#5F666D',
        'text-light': '#A0A7AE',
      },
      fontFamily: {
        sans: ['Plus Jakarta Sans', 'Noto Sans KR', 'sans-serif'],
      },
      borderRadius: {
        'card': '14px',
        'btn': '8px',
        'input': '10px',
      },
    },
  },
  plugins: [],
}
