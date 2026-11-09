<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'

const navigation = [
  { to: '/overview', label: 'Panoramica', icon: '◫' },
  { to: '/hubs', label: 'Hub', icon: '⌖' },
  { to: '/shipments', label: 'Spedizioni', icon: '↗' },
  { to: '/network', label: 'Rete & algoritmi', icon: '⌘' },
]
</script>

<template>
  <div class="app-frame">
    <aside class="sidebar">
      <RouterLink class="brand" to="/overview"><span class="brand-mark">L</span><span>logistics<span class="brand-light">engine</span></span></RouterLink>
      <p class="nav-caption">WORKSPACE</p>
      <nav class="side-nav" aria-label="Navigazione principale">
        <RouterLink v-for="item in navigation" :key="item.to" :to="item.to" class="nav-link"><span class="nav-icon">{{ item.icon }}</span>{{ item.label }}</RouterLink>
      </nav>
      <div class="sidebar-bottom"><span class="status-dot"></span><div><strong>API v1</strong><small>Ambiente di sviluppo</small></div><span class="api-pill">DEV</span></div>
    </aside>
    <div class="main-column">
      <header class="topbar"><div><span class="breadcrumb">LogisticsEngine</span><span class="breadcrumb-separator">/</span><span class="breadcrumb current">Workspace</span></div><a class="api-link" href="https://localhost:7259/swagger" target="_blank" rel="noreferrer">Apri Swagger <span>↗</span></a></header>
      <main class="content"><RouterView /></main>
      <footer class="app-footer">Demo didattica · Vue 3 · API versionata</footer>
    </div>
  </div>
</template>
