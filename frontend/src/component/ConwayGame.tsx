import { ChangeEvent, useEffect, useState } from 'react'
import { toast } from 'react-toastify'

import {
  Delete as DeleteIcon,
  History as HistoryIcon,
  KeyboardDoubleArrowRight as KeyboardDoubleArrowRightIcon,
  NavigateNext as NavigateNextIcon,
  Save as SaveIcon,
  Science as ScienceIcon
} from '@mui/icons-material'
import { Box, Button, CircularProgress, IconButton, List, ListItem, Stack, TextField, Typography } from '@mui/material'

import { BOARD_HEIGHT, BOARD_WIDTH, CENTER_X, CENTER_Y } from '@/constants'
import { deleteBoard, getAll, getCurrentBoard, getNextBoard, getNextNBoard, getStable, uploadBoard } from '@/services'
import { Cell, IBoard } from '@/types'

const ConwayGame = () => {
  const [liveCells, setLiveCells] = useState<Cell[]>([])
  const [boardId, setBoardId] = useState<number>()
  const [generationNum, setGenerationNum] = useState<number>(10)
  const [maxStepNum, setMaxStepNum] = useState<number>(100)
  const [boardList, setBoardList] = useState<IBoard[]>()
  const [loading, setLoading] = useState<boolean>(false)
  const [actionLoading, setActionLoading] = useState<string>('')

  const changeGenerationNum = (event: ChangeEvent<HTMLInputElement>) => {
    setGenerationNum(parseInt(event.target.value))
  }

  const changeMaxStepNum = (event: ChangeEvent<HTMLInputElement>) => {
    setMaxStepNum(parseInt(event.target.value))
  }

  // Toggle live cell on click
  const toggleCell = (x: number, y: number) => {
    const exists = liveCells.some(c => c.x === x && c.y === y)
    if (exists) {
      // Remove cell
      setLiveCells(liveCells.filter(c => !(c.x === x && c.y === y)))
    } else {
      // Add cell
      setLiveCells([...liveCells, { x: x, y: y }])
    }
  }

  const uploadBoardState = async () => {
    try {
      setActionLoading('create')
      const data = { liveCells: liveCells }
      const response = await uploadBoard(data)
      if (response) {
        toast.success('Upload Board Successfully')
        setBoardId(response.boardId)
      }
    } catch (err: unknown) {
      if (err instanceof Error) {
        toast(err.message, { type: 'error' })
      } else {
        toast('An unknown error occurred', { type: 'error' })
      }
    } finally {
      setActionLoading('')
      getBoardList()
    }
  }

  const getBoardList = async () => {
    try {
      setLoading(true)
      const response = await getAll()
      if (response) {
        setBoardList(response)
      }
    } catch (err: unknown) {
      if (err instanceof Error) {
        toast(err.message, { type: 'error' })
      } else {
        toast('An unknown error occurred', { type: 'error' })
      }
    } finally {
      setLoading(false)
    }
  }

  const getNextState = async () => {
    try {
      setActionLoading('next')
      if (!boardId) {
        toast.error('Must upload a new state')
        return
      }
      const response = await getNextBoard(boardId)
      setLiveCells(response.liveCells)
    } catch (err: unknown) {
      if (err instanceof Error) {
        toast(err.message, { type: 'error' })
      } else {
        toast('An unknown error occurred', { type: 'error' })
      }
    } finally {
      setActionLoading('')
    }
  }

  const getCurrentState = async (boardId: number) => {
    try {
      setLoading(true)
      const response = await getCurrentBoard(boardId)
      setLiveCells(response.liveCells)
      setBoardId(boardId)
    } catch (err: unknown) {
      if (err instanceof Error) {
        toast(err.message, { type: 'error' })
      } else {
        toast('An unknown error occurred', { type: 'error' })
      }
    } finally {
      setLoading(false)
    }
  }

  const getNextNState = async () => {
    try {
      setActionLoading('next n')
      if (!boardId) {
        toast.error('Must upload a new state')
        return
      }
      const response = await getNextNBoard(boardId, generationNum)
      setLiveCells(response.liveCells)
    } catch (err: unknown) {
      if (err instanceof Error) {
        toast(err.message, { type: 'error' })
      } else {
        toast('An unknown error occurred', { type: 'error' })
      }
    } finally {
      setActionLoading('')
    }
  }

  const getFinalState = async () => {
    try {
      setActionLoading('final')
      const data = {
        liveCells: liveCells,
        maxStepNum: maxStepNum
      }
      const response = await getStable(data)
      setLiveCells(response.liveCells)
    } catch (err: unknown) {
      if (err instanceof Error) {
        toast(err.message, { type: 'error' })
      } else {
        toast('An unknown error occurred', { type: 'error' })
      }
    } finally {
      setActionLoading('')
    }
  }

  const deleteState = async (boardId: number) => {
    try {
      const response = await deleteBoard(boardId)
      if (response) toast.success('Successfully Deleted')
      getBoardList()
    } catch (err: unknown) {
      if (err instanceof Error) {
        toast(err.message, { type: 'error' })
      } else {
        toast('An unknown error occurred', { type: 'error' })
      }
    }
  }

  const reset = () => {
    setLiveCells([])
    setBoardId(0)
  }

  // Check if cell is live
  const isLive = (x: number, y: number) => liveCells.some(c => c.x === x && c.y === y)

  useEffect(() => {
    getBoardList()
  }, [])

  return (
    <Stack direction={{ md: 'row', xs: 'column' }} sx={{ p: 4 }} gap={5}>
      {/* <Stack gap={2}>
        <Typography variant='h5' color='white'>
          Saved Boards
        </Typography>
        <List sx={{ m: 0, p: 0 }}>
          {loading ? (
            <Stack alignItems='center'>
              <CircularProgress />
            </Stack>
          ) : (
            boardList &&
            boardList.map(item => (
              <ListItem
                key={item.id}
                sx={{
                  borderRadius: 2,
                  cursor: 'pointer',
                  '&:hover': { backgroundColor: '#434343' },
                  background: item.id === boardId ? '#434343' : 'transparent'
                }}
                onClick={() => getCurrentState(item.id)}
              >
                <Stack direction='row' gap='2' alignItems='center'>
                  <Typography variant='h6' color='white'>
                    Board {item.id}
                  </Typography>
                  <IconButton color='primary' onClick={() => deleteState(item.id)}>
                    <DeleteIcon />
                  </IconButton>
                </Stack>
              </ListItem>
            ))
          )}
        </List>
      </Stack>
      <Stack direction='column' gap={2}>
        <Typography variant='h5' color='white'>
          Live Cell Grid ({BOARD_WIDTH} x {BOARD_HEIGHT})
        </Typography>
        <Stack direction='row' gap={2}>
          <Button
            loading={actionLoading === 'create'}
            disabled={!!actionLoading}
            variant='contained'
            color='primary'
            startIcon={<SaveIcon />}
            onClick={uploadBoardState}
          >
            Upload
          </Button>
          <Button
            loading={actionLoading === 'next'}
            disabled={!!actionLoading}
            variant='contained'
            color='primary'
            startIcon={<NavigateNextIcon />}
            onClick={getNextState}
          >
            Next
          </Button>
          <Button
            loading={actionLoading === 'next n'}
            disabled={!!actionLoading}
            variant='contained'
            color='primary'
            startIcon={<KeyboardDoubleArrowRightIcon />}
            onClick={getNextNState}
          >
            Next N
          </Button>
          <Button
            loading={actionLoading === 'final'}
            disabled={!!actionLoading}
            variant='contained'
            color='primary'
            startIcon={<ScienceIcon />}
            onClick={getFinalState}
          >
            Simulate
          </Button>
          <Button
            disabled={!!actionLoading}
            variant='contained'
            color='primary'
            startIcon={<HistoryIcon />}
            onClick={reset}
          >
            Reset
          </Button>
        </Stack>
        <Stack direction='row' gap={2}>
          <TextField
            label='Next Step Count'
            value={generationNum}
            onChange={changeGenerationNum}
            type='number'
            placeholder='Pls input N'
            size='small'
          />
          <TextField
            label='Simulation Limit'
            value={maxStepNum}
            onChange={changeMaxStepNum}
            type='number'
            placeholder='Pls input the limit'
            size='small'
          />
        </Stack>

        <Box
          sx={{
            display: 'grid',
            gridTemplateColumns: `repeat(${BOARD_WIDTH}, 20px)`,
            gridTemplateRows: `repeat(${BOARD_HEIGHT}, 20px)`,
            gap: 0.5
          }}
        >
          {[...Array(BOARD_HEIGHT)].map((_, y) =>
            [...Array(BOARD_WIDTH)].map((_, x) => {
              const live = isLive(x, y)
              const isCenter = x === CENTER_X && y === CENTER_Y
              return (
                <Box
                  key={`${x}-${y}`}
                  onClick={() => toggleCell(x, y)}
                  sx={{
                    cursor: 'pointer',
                    backgroundColor: live ? '#e24744' : '#fff',
                    color: live ? '#fff' : '#000',
                    textAlign: 'center',
                    fontWeight: isCenter ? 'bold' : 'normal',
                    userSelect: 'none'
                  }}
                />
              )
            })
          )}
        </Box>
      </Stack> */}
    </Stack>
  )
}

export default ConwayGame
