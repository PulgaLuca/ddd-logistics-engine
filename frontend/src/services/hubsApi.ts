import type { CreateHubRequest, HubDto } from '../types/api'
import { request } from './httpClient'

// Le funzioni di questa feature conoscono gli endpoint Hub e delegano il trasporto HTTP al client comune.
export const hubsApi = {
  getAll: () => request<HubDto[]>('/hubs'),
  create: (hub: CreateHubRequest) => request<HubDto>('/hubs', {
    method: 'POST',
    body: JSON.stringify(hub),
  }),
}
