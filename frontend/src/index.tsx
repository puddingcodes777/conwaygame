import { StyledEngineProvider } from '@mui/material'
import { createRoot } from 'react-dom/client'
import App from './App'
import './index.css'
import { AppTheme } from './theme'

const root = createRoot(document.getElementById('root') as HTMLElement)

root.render(
  <StyledEngineProvider>
    <AppTheme>
      <App />
    </AppTheme>
  </StyledEngineProvider>
)
