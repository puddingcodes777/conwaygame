// import { API_ENDPOINTS } from '@/configs/endpoint'
// import apiRequest from '@/lib/axios'
// import { IBoard, IBoardResponse, IUpload, IUploadResponse } from '@/types'

// export const uploadBoard = async (data: IUpload): Promise<IUploadResponse> => {
//   return apiRequest({
//     method: 'POST',
//     url: API_ENDPOINTS.UPLOAD,
//     data,
//     errorMessage: 'Upload Board Failed'
//   })
// }

// export const getCurrentBoard = async (boardId: number): Promise<IBoardResponse> => {
//   return apiRequest({
//     method: 'GET',
//     url: API_ENDPOINTS.GET_CURRENT(boardId),
//     errorMessage: 'Get Current Board Failed'
//   })
// }

// export const getNextBoard = async (boardId: number): Promise<IBoardResponse> => {
//   return apiRequest({
//     method: 'GET',
//     url: API_ENDPOINTS.GET_NEXT(boardId),
//     errorMessage: 'Get Next Board Failed'
//   })
// }

// export const getNextNBoard = async (boardId: number, generationNum: number): Promise<IBoardResponse> => {
//   return apiRequest({
//     method: 'GET',
//     url: API_ENDPOINTS.GET_N_NEXT(boardId, generationNum),
//     errorMessage: 'Get N Board State Ahead Failed'
//   })
// }

// export const getStable = async (data: IUpload): Promise<IBoardResponse> => {
//   return apiRequest({
//     method: 'POST',
//     data,
//     url: API_ENDPOINTS.GET_STABLE,
//     errorMessage: 'Get Final State Failed'
//   })
// }

// export const getAll = async (): Promise<IBoard[]> => {
//   return apiRequest({
//     method: 'GET',
//     url: API_ENDPOINTS.GETALL,
//     errorMessage: 'Get All Failed'
//   })
// }

// export const deleteBoard = async (id: number) => {
//   return apiRequest({
//     method: 'DELETE',
//     url: API_ENDPOINTS.DELETE(id),
//     errorMessage: 'Delete Board Failed'
//   })
// }
