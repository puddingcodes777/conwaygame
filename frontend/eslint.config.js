const eslint = require('@eslint/js')
const tslint = require('typescript-eslint')
const react = require('eslint-plugin-react')
const ts = require('@typescript-eslint/eslint-plugin')
const parser = require('@typescript-eslint/parser')
const importPlugin = require('eslint-plugin-import')
const globals = require('globals')

module.exports = [
  eslint.configs.recommended,
  ...tslint.configs.recommended,
  {
    files: ['src/**/*.{ts,tsx}'],
    ignores: ['**/*.config.js'],
    plugins: {
      ts,
      react,
      import: importPlugin
    },
    languageOptions: {
      parser,
      parserOptions: {
        ecmaFeatures: {
          jsx: true
        },
        ecmaVersion: 12,
        sourceType: 'module',
        globals: {
          ...globals.browser
        }
      }
    },
    rules: {
      'react/display-name': 'error',
      '@typescript-eslint/no-unused-vars': 'error',
      '@typescript-eslint/ban-ts-comment': 'off',
      '@typescript-eslint/no-explicit-any': 'error',
      '@typescript-eslint/no-non-null-assertion': 'off',
      'lines-around-comment': [
        'error',
        {
          beforeLineComment: true,
          beforeBlockComment: true,
          allowBlockStart: true,
          allowClassStart: true,
          allowObjectStart: true,
          allowArrayStart: true
        }
      ],
      'import/newline-after-import': [
        'error',
        {
          count: 1
        }
      ]
    }
  }
]
