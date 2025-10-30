import { CssBaseline } from '@mui/material'
import { ThemeProvider, createTheme } from '@mui/material/styles'
import { ReactNode, useMemo } from 'react'

interface AppThemeProps {
  children: ReactNode
}

export const AppTheme = (props: AppThemeProps) => {
  const { children } = props
  const theme = useMemo(
    () =>
      createTheme({
        colorSchemes: {
          dark: true,
          light: true
        }
      }),
    []
  )

  return (
    <ThemeProvider theme={theme} disableTransitionOnChange defaultMode='dark'>
      <CssBaseline />
      {children}
    </ThemeProvider>
  )
}
