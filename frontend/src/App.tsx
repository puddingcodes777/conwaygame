import { FC, Suspense } from 'react'
import { Flip, ToastContainer } from 'react-toastify'

import { CircularProgress, Stack } from '@mui/material'
import ConwayGame from './component/ConwayGame'

const App: FC = () => {
  return (
    <>
      <Stack component='main' justifyContent='center' alignItems='center' minHeight='calc(100vh - 530px)'>
        <Suspense fallback={<CircularProgress />}>
          <ConwayGame />
        </Suspense>
      </Stack>
      <ToastContainer style={{ zIndex: 10000, fontSize: '16px' }} theme='colored' autoClose={2000} transition={Flip} />
    </>
  )
}

export default App
