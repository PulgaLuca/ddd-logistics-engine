// Helper di formattazione per mostrare i dati come campi leggibili (non JSON).

const LABELS = {
  id: 'ID',
  trackingNumber: 'Tracking number',
  orderNumber: 'Order number',
  customerName: 'Cliente',
  weightInKilograms: 'Peso (kg)',
  priceAmount: 'Prezzo',
  priceCurrency: 'Valuta',
  isExpress: 'Express',
  status: 'Stato',
  createdOnUtc: 'Creato il',
  deliveredOnUtc: 'Consegnato il',
  placedOnUtc: 'Registrato il',
  assignedHubId: 'Hub assegnato',
  originCity: 'Città di origine',
  destinationCity: 'Città di destinazione',
  hubCode: 'Codice hub',
  parentZoneId: 'Zona padre',
  distanceInKilometers: 'Distanza (km)',
  trafficFactor: 'Fattore traffico',
  totalCost: 'Costo totale',
  hopCount: 'Numero di salti',
  canReach: 'Raggiungibile',
  canUndo: 'Undo disponibile',
  canRedo: 'Redo disponibile'
};

export function humanizeKey(key) {
  if (LABELS[key]) {
    return LABELS[key];
  }
  return key
    .replace(/([A-Z])/g, ' $1')
    .replace(/^./, (c) => c.toUpperCase())
    .trim();
}

export function formatValue(value) {
  if (value === null || value === undefined || value === '') {
    return '—';
  }
  if (typeof value === 'boolean') {
    return value ? 'Sì' : 'No';
  }
  if (typeof value === 'string' && /^\d{4}-\d{2}-\d{2}T/.test(value)) {
    const date = new Date(value);
    if (!Number.isNaN(date.getTime())) {
      return date.toLocaleString('it-IT');
    }
  }
  return String(value);
}

export function isPlainObject(value) {
  return value !== null && typeof value === 'object' && !Array.isArray(value);
}
