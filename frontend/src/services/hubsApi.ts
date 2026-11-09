import type { CreateHubRequest, HubCollectionsDto, HubConnectionDto, HubDto, HubGroupDto, HubPathDto } from '../types/api'
import { request } from './httpClient'

// Le funzioni di questa feature conoscono gli endpoint Hub e delegano il trasporto HTTP al client comune.
export const hubsApi = {
  getAll: () => request<HubDto[]>('/hubs'),
  create: (hub: CreateHubRequest) => request<HubDto>('/hubs', {
    method: 'POST',
    body: JSON.stringify(hub),
  }),
  update: (id: string, hub: CreateHubRequest) => request<void>(`/hubs/${id}`, {
    method: 'PUT', body: JSON.stringify(hub),
  }),
  delete: (id: string) => request<void>(`/hubs/${id}`, { method: 'DELETE' }),
  getConnections: () => request<HubConnectionDto[]>('/hubs/connections'),
  connect: (connection: HubConnectionDto) => request<HubConnectionDto>('/hubs/connections', {
    method: 'POST', body: JSON.stringify(connection),
  }),
  shortestPath: (originId: string, destinationId: string) => request<HubPathDto>(
    `/hubs/algorithms/shortest-path?originId=${originId}&destinationId=${destinationId}`,
  ),
  simplePaths: (originId: string, destinationId: string, maximumPaths = 5) => request<HubPathDto[]>(
    `/hubs/algorithms/simple-paths?originId=${originId}&destinationId=${destinationId}&maximumPaths=${maximumPaths}`,
  ),
  traversal: (originId: string, traversal: 'BreadthFirst' | 'DepthFirst') => request<HubDto[]>(
    `/hubs/algorithms/traversal?originId=${originId}&traversal=${traversal}`,
  ),
  sorted: (property: 'Id' | 'Name' | 'Zone') => request<HubDto[]>(`/hubs/algorithms/sort?property=${property}`),
  grouped: () => request<HubGroupDto[]>('/hubs/algorithms/groups-by-zone'),
  collections: () => request<HubCollectionsDto>('/hubs/algorithms/collections'),
  binaryTree: () => request<HubDto[]>('/hubs/algorithms/binary-tree'),
  trie: (prefix: string) => request<HubDto[]>(`/hubs/algorithms/trie?prefix=${encodeURIComponent(prefix)}`),
}
