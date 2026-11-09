// DTOs: rappresentano i dati JSON scambiati con l'API, non sono le entità di dominio.
export interface HubDto {
  id: string
  name: string
  zone: string
}

export interface CreateHubRequest {
  name: string
  zone: string
}

export interface ShipmentDto {
  id: string
  trackingNumber: string
  status: string
}

export interface CreateShipmentRequest {
  trackingNumber: string
}
