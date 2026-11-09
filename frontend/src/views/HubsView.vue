<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { hubsApi } from '../services/hubsApi'
import type { HubDto } from '../types/api'

const hubs = ref<HubDto[]>([])
const name = ref('')
const zone = ref('')
const error = ref('')
const loading = ref(false)
const saving = ref(false)
const search = ref('')
const editingId = ref<string | null>(null)
const visibleHubs = computed(() => hubs.value.filter(hub => `${hub.name} ${hub.zone}`.toLocaleLowerCase().includes(search.value.toLocaleLowerCase())))

async function loadHubs() {
  loading.value = true
  error.value = ''
  try { hubs.value = await hubsApi.getAll() }
  catch (cause) { error.value = cause instanceof Error ? cause.message : 'Errore nel caricamento degli hub.' }
  finally { loading.value = false }
}

async function createHub() {
  saving.value = true
  error.value = ''
  try {
    await hubsApi.create({ name: name.value.trim(), zone: zone.value.trim() })
    name.value = ''
    zone.value = ''
    await loadHubs()
  } catch (cause) {
    error.value = cause instanceof Error ? cause.message : 'Errore nella creazione dell’hub.'
  } finally { saving.value = false }
}

// Aggiorna ed elimina passano sempre dal servizio HTTP; il componente non costruisce URL direttamente.
function editHub(hub: HubDto) { editingId.value = hub.id; name.value = hub.name; zone.value = hub.zone }
function cancelEdit() { editingId.value = null; name.value = ''; zone.value = '' }
async function saveHub() {
  if (!editingId.value) return createHub()
  saving.value = true
  error.value = ''
  try { await hubsApi.update(editingId.value, { name: name.value.trim(), zone: zone.value.trim() }); cancelEdit(); await loadHubs() }
  catch (cause) { error.value = cause instanceof Error ? cause.message : 'Errore durante la modifica dell’hub.' }
  finally { saving.value = false }
}
async function deleteHub(hub: HubDto) {
  if (!window.confirm(`Eliminare l’hub “${hub.name}”?`)) return
  try { await hubsApi.delete(hub.id); await loadHubs() }
  catch (cause) { error.value = cause instanceof Error ? cause.message : 'Errore durante l’eliminazione dell’hub.' }
}

onMounted(loadHubs)
</script>

<template>
  <section class="page-heading"><div><p class="eyebrow">ANAGRAFICA · NODI LOGISTICI</p><h1>Hub</h1><p>Gestisci gli hub che compongono la rete logistica.</p></div><button class="button button-outline" @click="loadHubs">↻ Aggiorna</button></section>
  <p v-if="error" class="alert" role="alert">{{ error }}</p>
  <div class="grid">
    <section class="panel"><div class="panel-heading"><div><p class="eyebrow">DIRECTORY</p><h2>Hub registrati <span class="count-badge">{{ hubs.length }}</span></h2></div></div><input v-model="search" class="search-input" placeholder="Cerca per nome o zona…" aria-label="Cerca hub" /><p v-if="loading" class="muted">Caricamento…</p><p v-else-if="visibleHubs.length === 0" class="empty-state">{{ hubs.length ? 'Nessun risultato per questa ricerca.' : 'Nessun hub presente. Crea il primo hub dal modulo.' }}</p>
      <ul v-else class="record-list"><li v-for="hub in visibleHubs" :key="hub.id"><span class="hub-avatar">{{ hub.name.charAt(0).toUpperCase() }}</span><span class="record-main"><strong>{{ hub.name }}</strong><small>{{ hub.zone }} · {{ hub.id.slice(0, 8) }}</small></span><span class="row-actions"><button class="icon-button" :aria-label="`Modifica ${hub.name}`" @click="editHub(hub)">✎</button><button class="icon-button danger" :aria-label="`Elimina ${hub.name}`" @click="deleteHub(hub)">×</button></span></li></ul>
    </section>
    <section class="panel"><p class="eyebrow">{{ editingId ? 'MODIFICA RECORD' : 'NUOVA ANAGRAFICA' }}</p><h2>{{ editingId ? 'Aggiorna hub' : 'Aggiungi un hub' }}</h2><p class="panel-description">I dati vengono validati e salvati dall’API.</p><form class="form" @submit.prevent="saveHub"><label>Nome<input v-model="name" required maxlength="100" placeholder="Milano Nord" /></label><label>Zona<input v-model="zone" required maxlength="100" placeholder="Lombardia" /></label><button class="button" :disabled="saving">{{ saving ? 'Salvataggio…' : editingId ? 'Salva modifiche' : 'Crea hub' }}</button><button v-if="editingId" type="button" class="text-action" @click="cancelEdit">Annulla modifica</button></form></section>
  </div>
</template>
