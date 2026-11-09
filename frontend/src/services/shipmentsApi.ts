import type { CreateShipmentRequest, ShipmentDto } from '../types/api'
import { request } from './httpClient'

// Le funzioni di questa feature mantengono isolati gli URL e i payload relativi alle spedizioni.
export const shipmentsApi = {
  getAll: () => request<ShipmentDto[]>('/shipments'),
  create: (shipment: CreateShipmentRequest) => request<ShipmentDto>('/shipments', {
    method: 'POST',
    body: JSON.stringify(shipment),
  }),
  updateStatus: (id: string, status: ShipmentDto['status']) => request<void>(`/shipments/${id}/status`, {
    method: 'PUT', body: JSON.stringify({ status }),
  }),
  delete: (id: string) => request<void>(`/shipments/${id}`, { method: 'DELETE' }),
}
