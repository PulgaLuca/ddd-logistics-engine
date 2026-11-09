<script setup lang="ts">
import cytoscape, { type Core, type ElementDefinition } from 'cytoscape'
import { onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { hubsApi } from '../services/hubsApi'
import type { HubCollectionsDto, HubConnectionDto, HubDto, HubGroupDto, HubPathDto } from '../types/api'

const hubs = ref<HubDto[]>([])
const connections = ref<HubConnectionDto[]>([])
const originId = ref('')
const destinationId = ref('')
const traversal = ref<'BreadthFirst' | 'DepthFirst'>('BreadthFirst')
const edgeDistance = ref(1)
const prefix = ref('')
const sortProperty = ref<'Id' | 'Name' | 'Zone'>('Name')
const activeResult = ref<HubDto[] | HubGroupDto[] | HubCollectionsDto | null>(null)
const resultTitle = ref('')
const pathResult = ref<HubPathDto | null>(null)
const alternativePaths = ref<HubPathDto[]>([])
const error = ref('')
const notice = ref('')
const busy = ref(false)
const graphHost = ref<HTMLDivElement | null>(null)
let graph: Core | undefined


// Il grafico deriva dai DTO caricati dall'API; il layout Cytoscape posiziona automaticamente i nodi e gli archi.
function renderGraph() {
  if (!graph || !graphHost.value) return
  const elements: ElementDefinition[] = [
    ...hubs.value.map(hub => ({ data: { id: hub.id, label: hub.name, zone: hub.zone } })),
    ...connections.value.map((edge, index) => ({ data: { id: `edge-${index}`, source: edge.firstHubId, target: edge.secondHubId, label: String(edge.distance) } })),
  ]
  graph.elements().remove()
  graph.add(elements)
  graph.layout({ name: 'cose', animate: true, animationDuration: 500, padding: 36 }).run()
}

async function loadNetwork() {
  busy.value = true
  error.value = ''
  notice.value = ''
  try {
    // I nodi alimentano i menu e restano utili anche se il repository degli archi non è raggiungibile.
    const [hubResult, connectionResult] = await Promise.allSettled([
      hubsApi.getAll(),
      hubsApi.getConnections(),
    ])
    if (hubResult.status === 'rejected') throw hubResult.reason

    hubs.value = hubResult.value
    if (!hubs.value.some(hub => hub.id === originId.value)) {
      originId.value = hubs.value[0]?.id ?? ''
    }
    if (!hubs.value.some(hub => hub.id === destinationId.value) || destinationId.value === originId.value) {
      destinationId.value = hubs.value.find(hub => hub.id !== originId.value)?.id ?? ''
    }

    if (connectionResult.status === 'fulfilled') {
      connections.value = connectionResult.value
    } else {
      connections.value = []
      notice.value = 'Gli hub sono caricati, ma non è stato possibile recuperare i collegamenti. Puoi ancora selezionare gli hub.'
    }
    renderGraph()
  } catch (cause) { error.value = cause instanceof Error ? cause.message : 'Impossibile caricare la rete.' }
  finally { busy.value = false }
}

function highlight(hubIds: string[]) {
  if (!graph) return
  graph.elements().removeClass('highlighted')
  hubIds.forEach((id, index) => {
    graph?.getElementById(id).addClass('highlighted')
    if (index > 0) graph?.edges().filter(edge =>
      (edge.source().id() === hubIds[index - 1] && edge.target().id() === id) ||
      (edge.target().id() === hubIds[index - 1] && edge.source().id() === id),
    ).addClass('highlighted')
  })
  graph.fit(graph.elements(), 52)
}

async function run(action: () => Promise<void>) {
  busy.value = true
  error.value = ''
  notice.value = ''
  pathResult.value = null
  alternativePaths.value = []
  try { await action() }
  catch (cause) { error.value = cause instanceof Error ? cause.message : 'L’operazione non è riuscita.' }
  finally { busy.value = false }
}

function requireEndpoints() {
  if (!originId.value || !destinationId.value) throw new Error('Crea almeno due hub per usare questo algoritmo.')
}

function showHubList(title: string, data: HubDto[]) {
  resultTitle.value = title
  activeResult.value = data
  highlight(data.map(hub => hub.id))
}

async function shortestPath() { await run(async () => {
  requireEndpoints()
  pathResult.value = await hubsApi.shortestPath(originId.value, destinationId.value)
  highlight(pathResult.value.hubs.map(hub => hub.id))
}) }

async function findSimplePaths() { await run(async () => {
  requireEndpoints()
  alternativePaths.value = await hubsApi.simplePaths(originId.value, destinationId.value, 5)
  if (alternativePaths.value[0]) highlight(alternativePaths.value[0].hubs.map(hub => hub.id))
}) }

async function runTraversal() { await run(async () => showHubList(
  traversal.value === 'BreadthFirst' ? 'Visita BFS · ampiezza' : 'Visita DFS · profondità',
  await hubsApi.traversal(originId.value, traversal.value),
)) }

async function runSort() { await run(async () => showHubList(`Ordinamento per ${sortProperty.value}`, await hubsApi.sorted(sortProperty.value))) }
async function runTree() { await run(async () => showHubList('Visita in-order dell’albero binario', await hubsApi.binaryTree())) }
async function runTrie() { await run(async () => showHubList(`Trie · prefisso “${prefix.value}”`, await hubsApi.trie(prefix.value))) }
async function runGroups() { await run(async () => { resultTitle.value = 'Raggruppamento per zona'; activeResult.value = await hubsApi.grouped() }) }
async function runCollections() { await run(async () => { resultTitle.value = 'Collezioni generiche'; activeResult.value = await hubsApi.collections() }) }

async function connectHubs() { await run(async () => {
  requireEndpoints()
  await hubsApi.connect({ firstHubId: originId.value, secondHubId: destinationId.value, distance: edgeDistance.value })
  notice.value = 'Collegamento creato. Il grafo è stato aggiornato.'
  activeResult.value = null
  await loadNetwork()
}) }

onMounted(() => {
  graph = cytoscape({
    container: graphHost.value,
    elements: [],
    style: [
      { selector: 'node', style: { 'background-color': '#1d8a6a', label: 'data(label)', color: '#172b3b', 'font-size': 11, 'font-weight': 600, 'text-valign': 'bottom', 'text-margin-y': 8, width: 34, height: 34, 'border-width': 3, 'border-color': '#d8f1e8' } },
      { selector: 'edge', style: { width: 2, 'line-color': '#c7d5d3', 'target-arrow-color': '#c7d5d3', 'target-arrow-shape': 'triangle', 'curve-style': 'bezier', label: 'data(label)', 'font-size': 9, color: '#70817f', 'text-background-color': '#fff', 'text-background-opacity': 1, 'text-background-padding': '3px' } },
      { selector: '.highlighted', style: { 'background-color': '#e88e45', 'line-color': '#e88e45', 'target-arrow-color': '#e88e45', 'border-color': '#ffead4', width: 4, 'z-index': 10 } },
    ],
  })
  void loadNetwork()
})
watch([hubs, connections], renderGraph)
onBeforeUnmount(() => graph?.destroy())
</script>

<template>
  <section class="page-heading"><div><p class="eyebrow">DOMAIN SERVICES · INTERACTIVE LAB</p><h1>Rete degli hub</h1><p>Esplora nodi, collegamenti e algoritmi direttamente sul grafo.</p></div><button class="button button-outline" :disabled="busy" @click="loadNetwork">↻ Aggiorna rete</button></section>
  <aside class="info-banner compact"><span class="info-symbol">i</span><p>Le API degli hub sono pubbliche al momento. Gli algoritmi in questa pagina non sono protetti da ruolo o token: la UI non simula autorizzazioni non applicate dal backend.</p></aside>
  <p v-if="error" class="alert" role="alert">{{ error }}</p><p v-if="notice" class="success-banner">{{ notice }}</p>

  <section class="network-layout">
    <article class="panel graph-panel"><div class="panel-heading"><div><p class="eyebrow">VISUALIZZAZIONE LIVE</p><h2>Mappa dei collegamenti</h2></div><span class="count-badge">{{ hubs.length }} nodi · {{ connections.length }} archi</span></div>
      <div ref="graphHost" class="graph-canvas"></div><div v-if="!hubs.length" class="graph-empty">Aggiungi almeno un hub per iniziare a costruire la rete.</div>
      <div class="graph-legend"><span><i class="legend-node"></i> Hub</span><span><i class="legend-edge"></i> Distanza del collegamento</span><span><i class="legend-path"></i> Percorso selezionato</span></div>
    </article>
    <article class="panel control-panel"><p class="eyebrow">OPERAZIONI SUL GRAFO</p><h2>Collegamenti e percorsi</h2>
      <label class="field-label">Hub di partenza<select v-model="originId"><option disabled value="">{{ hubs.length ? 'Seleziona hub' : 'Nessun hub disponibile' }}</option><option v-for="hub in hubs" :key="hub.id" :value="hub.id">{{ hub.name }} · {{ hub.zone }}</option></select></label>
      <label class="field-label">Hub di destinazione<select v-model="destinationId"><option disabled value="">{{ hubs.length ? 'Seleziona hub' : 'Nessun hub disponibile' }}</option><option v-for="hub in hubs" :key="hub.id" :value="hub.id">{{ hub.name }} · {{ hub.zone }}</option></select></label>
      <div class="button-row"><button class="button" :disabled="busy || hubs.length < 2" @click="shortestPath">Percorso minimo <span class="button-caption">DIJKSTRA</span></button><button class="button button-secondary" :disabled="busy || hubs.length < 2" @click="findSimplePaths">Cammini semplici <span class="button-caption">BACKTRACKING</span></button></div>
      <div class="divider"></div><div class="inline-fields"><label class="field-label">Distanza<input v-model.number="edgeDistance" type="number" min="0.01" step="0.01" /></label><button class="button button-outline connect-button" :disabled="busy || hubs.length < 2" @click="connectHubs">+ Crea collegamento</button></div>
      <div class="divider"></div><label class="field-label">Visita del grafo<select v-model="traversal"><option value="BreadthFirst">BFS · ampiezza</option><option value="DepthFirst">DFS · profondità</option></select></label><button class="text-action" :disabled="busy || !originId" @click="runTraversal">Esegui visita <span>→</span></button>
    </article>
  </section>

  <section class="section-heading"><div><p class="eyebrow">ESPLORA LE STRUTTURE</p><h2>Altri domain services</h2></div><span class="section-note">Gli algoritmi sono eseguiti dal backend</span></section>
  <section class="algorithm-grid">
    <article class="panel algorithm-card"><span class="algorithm-icon violet">▤</span><h3>Queue, Stack, HashMap e Set</h3><p>Confronta FIFO, LIFO, indice per ID e zone distinte.</p><button class="text-action" :disabled="busy" @click="runCollections">Visualizza collezioni →</button></article>
    <article class="panel algorithm-card"><span class="algorithm-icon blue">⌘</span><h3>Albero binario</h3><p>Visita in-order degli hub indicizzati per ID.</p><button class="text-action" :disabled="busy" @click="runTree">Esegui visita →</button></article>
    <article class="panel algorithm-card"><span class="algorithm-icon green">⌕</span><h3>Trie · ricerca per prefisso</h3><p>Cerca hub in base all’inizio del nome.</p><div class="inline-search"><input v-model="prefix" placeholder="Es. Mil" aria-label="Prefisso da cercare" /><button class="button" :disabled="busy" @click="runTrie">Cerca</button></div></article>
    <article class="panel algorithm-card"><span class="algorithm-icon amber">↕</span><h3>Ordinamento</h3><p>Ordina per identificativo, nome o zona.</p><div class="inline-search"><select v-model="sortProperty" aria-label="Proprietà per ordinamento"><option value="Name">Nome</option><option value="Zone">Zona</option><option value="Id">ID</option></select><button class="button" :disabled="busy" @click="runSort">Ordina</button></div></article>
    <article class="panel algorithm-card"><span class="algorithm-icon rose">▦</span><h3>Gruppi per zona</h3><p>Raccoglie gli hub che condividono la stessa zona.</p><button class="text-action" :disabled="busy" @click="runGroups">Raggruppa →</button></article>
  </section>

  <section v-if="pathResult || alternativePaths.length || activeResult" class="panel result-panel">
    <div class="panel-heading"><div><p class="eyebrow">RISULTATO API</p><h2>{{ pathResult ? 'Percorso minimo' : alternativePaths.length ? 'Cammini semplici' : resultTitle }}</h2></div><button class="close-result" aria-label="Chiudi risultato" @click="pathResult=null; alternativePaths=[]; activeResult=null">×</button></div>
    <div v-if="pathResult" class="path-result"><div class="path-track"><template v-for="(hub, index) in pathResult.hubs" :key="hub.id"><span class="path-chip">{{ hub.name }}</span><span v-if="index < pathResult.hubs.length - 1" class="path-arrow">→</span></template></div><span class="distance-total">Distanza totale <strong>{{ pathResult.totalDistance }}</strong></span></div>
    <div v-else-if="alternativePaths.length" class="result-list"><button v-for="(path, index) in alternativePaths" :key="index" class="path-row" @click="highlight(path.hubs.map(hub => hub.id))"><span class="result-index">{{ index + 1 }}</span><span>{{ path.hubs.map(hub => hub.name).join(' → ') }}</span><strong>{{ path.totalDistance }}</strong></button><p v-if="!alternativePaths.length" class="muted">Nessun cammino trovato tra questi hub.</p></div>
    <div v-else-if="Array.isArray(activeResult) && activeResult.length && 'zone' in activeResult[0]" class="result-groups"><div v-for="group in activeResult as HubGroupDto[]" :key="group.zone"><strong>{{ group.zone }}</strong><span>{{ group.hubs.map(hub => hub.name).join(', ') }}</span></div></div>
    <div v-else-if="Array.isArray(activeResult)" class="result-list"><div v-for="(hub, index) in activeResult as HubDto[]" :key="hub.id" class="result-row"><span class="result-index">{{ index + 1 }}</span><span>{{ hub.name }}</span><small>{{ hub.zone }}</small></div><p v-if="!activeResult.length" class="muted">Nessun risultato.</p></div>
    <div v-else-if="activeResult" class="collection-results"><div><h3>Queue · FIFO</h3><p>{{ activeResult.queueOrder.map(hub => hub.name).join(' → ') || '—' }}</p></div><div><h3>Stack · LIFO</h3><p>{{ activeResult.stackOrder.map(hub => hub.name).join(' → ') || '—' }}</p></div><div><h3>HashMap · ID indicizzati</h3><p>{{ Object.keys(activeResult.idHashMap).length }} elementi</p></div><div><h3>Set · zone uniche</h3><p>{{ activeResult.zones.join(', ') || '—' }}</p></div></div>
  </section>
</template>
