<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import { hubsApi } from '../services/hubsApi'
import { shipmentsApi } from '../services/shipmentsApi'
import type { HubDto, ShipmentDto } from '../types/api'

const hubs = ref<HubDto[]>([])
const shipments = ref<ShipmentDto[]>([])
const error = ref('')

// La dashboard riassume i dati delle due risorse principali usando gli stessi client delle rispettive pagine.
onMounted(async () => {
  try { [hubs.value, shipments.value] = await Promise.all([hubsApi.getAll(), shipmentsApi.getAll()]) }
  catch (cause) { error.value = cause instanceof Error ? cause.message : 'Impossibile caricare il riepilogo.' }
})
</script>

<template>
  <section class="welcome-banner"><div><p class="eyebrow light">LOGISTICS CONTROL CENTER</p><h1>La tua rete, a colpo d’occhio.</h1><p>Gestisci hub e spedizioni, poi esplora i percorsi con gli algoritmi del dominio.</p><RouterLink to="/network" class="button button-light">Esplora la rete <span>→</span></RouterLink></div><div class="banner-orbit" aria-hidden="true"><span class="orbit orbit-one"></span><span class="orbit orbit-two"></span><span class="orbit-node node-a">H</span><span class="orbit-node node-b">H</span><span class="orbit-node node-c">H</span><span class="orbit-center">⌘</span></div></section>
  <p v-if="error" class="alert" role="alert">{{ error }}</p>
  <section class="metric-grid"><article class="metric-card"><span class="metric-icon green">⌖</span><div><p>Hub registrati</p><strong>{{ hubs.length }}</strong></div><RouterLink to="/hubs" aria-label="Apri hub">↗</RouterLink></article><article class="metric-card"><span class="metric-icon blue">↗</span><div><p>Spedizioni</p><strong>{{ shipments.length }}</strong></div><RouterLink to="/shipments" aria-label="Apri spedizioni">↗</RouterLink></article><article class="metric-card"><span class="metric-icon violet">⌘</span><div><p>Collegamenti rete</p><strong>Live</strong></div><RouterLink to="/network" aria-label="Apri rete">↗</RouterLink></article></section>
  <section class="section-heading"><div><p class="eyebrow">INIZIA DA QUI</p><h2>Il tuo workspace</h2></div><span class="section-note">API connessa tramite HTTPS</span></section>
  <section class="quick-grid"><RouterLink to="/hubs" class="quick-card"><span class="quick-icon green">⌖</span><div><h3>Gestione hub</h3><p>Crea, consulta e organizza i nodi della rete.</p></div><span class="quick-arrow">→</span></RouterLink><RouterLink to="/shipments" class="quick-card"><span class="quick-icon blue">↗</span><div><h3>Spedizioni</h3><p>Consulta le spedizioni e monitora il loro stato.</p></div><span class="quick-arrow">→</span></RouterLink><RouterLink to="/network" class="quick-card"><span class="quick-icon violet">⌘</span><div><h3>Laboratorio algoritmi</h3><p>Visualizza grafi, visite e cammini minimi.</p></div><span class="quick-arrow">→</span></RouterLink></section>
  <aside class="info-banner"><span class="info-symbol">i</span><p><strong>Nota sull’autorizzazione:</strong> al momento gli endpoint Hub e Shipment sono pubblici. La schermata non usa i ruoli per limitare l’accesso; applicare l’autorizzazione a tali endpoint nel backend sarà necessario prima di introdurre permessi UI effettivi.</p></aside>
</template>
