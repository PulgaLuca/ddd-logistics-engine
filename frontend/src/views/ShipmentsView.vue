<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { shipmentsApi } from '../services/shipmentsApi'
import type { ShipmentDto } from '../types/api'

const shipments = ref<ShipmentDto[]>([])
const trackingNumber = ref('')
const error = ref('')
const loading = ref(false)
const saving = ref(false)

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
  try {
    await shipmentsApi.create({ trackingNumber: trackingNumber.value.trim() })
    trackingNumber.value = ''
    await loadShipments()
  } catch (cause) {
    error.value = cause instanceof Error ? cause.message : 'Errore nella creazione della spedizione.'
  } finally { saving.value = false }
}

onMounted(loadShipments)
</script>

<template>
  <section class="page-heading"><div><p class="eyebrow">Movimentazione</p><h1>Spedizioni</h1><p>Elenco e creazione delle spedizioni tramite API.</p></div><button class="button secondary" @click="loadShipments">Aggiorna</button></section>
  <p v-if="error" class="alert" role="alert">{{ error }}</p>
  <div class="grid">
    <section class="panel"><h2>Elenco spedizioni</h2><p v-if="loading" class="muted">Caricamento…</p><p v-else-if="shipments.length === 0" class="muted">Nessuna spedizione presente.</p>
      <ul v-else class="record-list"><li v-for="shipment in shipments" :key="shipment.id"><span><strong>{{ shipment.trackingNumber }}</strong><small>{{ shipment.status }}</small></span><code>{{ shipment.id.slice(0, 8) }}</code></li></ul>
    </section>
    <section class="panel"><h2>Nuova spedizione</h2><form class="form" @submit.prevent="createShipment"><label>Numero di tracciamento<input v-model="trackingNumber" required maxlength="100" placeholder="TRK-2026-001" /></label><button class="button" :disabled="saving">{{ saving ? 'Salvataggio…' : 'Crea spedizione' }}</button></form></section>
  </div>
</template>
