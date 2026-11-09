import { request } from './httpClient'
import type { AuthUser } from './auth'

export interface UpdateProfileRequest {
  firstName: string
  lastName: string
  professionalRole: string
}

export const getMyProfile = () => request<AuthUser>('/users/me')
export const updateMyProfile = (profile: UpdateProfileRequest) =>
  request<AuthUser>('/users/me', { method: 'PUT', body: JSON.stringify(profile) })
