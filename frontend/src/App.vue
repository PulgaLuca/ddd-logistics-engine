<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <div class="shell">
    <header class="topbar">
      <a class="brand" href="/">Logistics<span>Engine</span></a>
      <nav aria-label="Sezioni">
        <RouterLink to="/hubs">Hub</RouterLink>
        <RouterLink to="/shipments">Spedizioni</RouterLink>
      </nav>
    </header>
    <main class="content"><RouterView /></main>
    <footer>Frontend dimostrativo · API v1</footer>
  </div>
</template>
