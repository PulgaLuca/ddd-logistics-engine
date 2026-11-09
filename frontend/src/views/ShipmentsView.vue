<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { shipmentsApi } from '../services/shipmentsApi'
import type { ShipmentDto } from '../types/api'

const shipments = ref<ShipmentDto[]>([])
const trackingNumber = ref('')
const search = ref('')
const error = ref('')
const loading = ref(false)
const saving = ref(false)
const visibleShipments = computed(() => shipments.value.filter(item => item.trackingNumber.toLowerCase().includes(search.value.toLowerCase())))

async function loadShipments() {
  loading.value = true
  error.value = ''
  try { shipments.value = await shipmentsApi.getAll() }
  catch (cause) { error.value = cause instanceof Error ? cause.message : 'Errore nel caricamento delle spedizioni.' }
  finally { loading.value = false }
}

async function createShipment() {
  saving.value = true
  error.value = ''
  try { await shipmentsApi.create({ trackingNumber: trackingNumber.value.trim() }); trackingNumber.value = ''; await loadShipments() }
  catch (cause) { error.value = cause instanceof Error ? cause.message : 'Errore nella creazione della spedizione.' }
  finally { saving.value = false }
}

// La regola di transizione vive nel Domain: la UI inoltra lo stato richiesto e visualizza l'eventuale rifiuto.
async function changeStatus(shipment: ShipmentDto, event: Event) {
  const status = (event.target as HTMLSelectElement).value
  try { await shipmentsApi.updateStatus(shipment.id, status); await loadShipments() }
  catch (cause) { error.value = cause instanceof Error ? cause.message : 'Transizione di stato non valida.'; await loadShipments() }
}

async function deleteShipment(shipment: ShipmentDto) {
  if (!window.confirm(`Eliminare la spedizione “${shipment.trackingNumber}”?`)) return
  try { await shipmentsApi.delete(shipment.id); await loadShipments() }
  catch (cause) { error.value = cause instanceof Error ? cause.message : 'Errore durante l’eliminazione.' }
}

onMounted(loadShipments)
</script>

<template>
  <section class="page-heading"><div><p class="eyebrow">MOVIMENTAZIONE · TRACKING</p><h1>Spedizioni</h1><p>Monitora le spedizioni e aggiorna il loro ciclo di vita.</p></div><button class="button button-outline" @click="loadShipments">↻ Aggiorna</button></section>
  <p v-if="error" class="alert" role="alert">{{ error }}</p>
  <div class="grid">
    <section class="panel"><div class="panel-heading"><div><p class="eyebrow">TRACKING BOARD</p><h2>Spedizioni <span class="count-badge">{{ shipments.length }}</span></h2></div></div><input v-model="search" class="search-input" placeholder="Cerca tracking number…" aria-label="Cerca spedizione" /><p v-if="loading" class="muted">Caricamento…</p><p v-else-if="visibleShipments.length === 0" class="empty-state">{{ shipments.length ? 'Nessun risultato per questa ricerca.' : 'Nessuna spedizione presente.' }}</p>
      <ul v-else class="record-list shipment-list"><li v-for="shipment in visibleShipments" :key="shipment.id"><span class="shipment-mark">↗</span><span class="record-main"><strong>{{ shipment.trackingNumber }}</strong><small>{{ shipment.id.slice(0, 8) }}</small></span><select class="status-select" :value="shipment.status" :aria-label="`Stato ${shipment.trackingNumber}`" @change="changeStatus(shipment, $event)"><option>Created</option><option>InTransit</option><option>Delivered</option><option>Cancelled</option></select><button class="icon-button danger" :aria-label="`Elimina ${shipment.trackingNumber}`" @click="deleteShipment(shipment)">×</button></li></ul>
    </section>
    <section class="panel"><p class="eyebrow">NUOVO TRACKING</p><h2>Registra una spedizione</h2><p class="panel-description">Le nuove spedizioni iniziano nello stato Created.</p><form class="form" @submit.prevent="createShipment"><label>Numero di tracciamento<input v-model="trackingNumber" required maxlength="100" placeholder="TRK-2026-001" /></label><button class="button" :disabled="saving">{{ saving ? 'Salvataggio…' : 'Crea spedizione' }}</button></form></section>
  </div>
</template>
