<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { getMyProfile, updateMyProfile } from '../services/profileApi'
import { updateSessionUser } from '../services/auth'

const profile = reactive({ firstName: '', lastName: '', professionalRole: '' })
const accountRole = ref('')
const loading = ref(true)
const saving = ref(false)
const error = ref('')
const success = ref('')

onMounted(async () => {
  try {
    const user = await getMyProfile()
    profile.firstName = user.firstName ?? ''
    profile.lastName = user.lastName ?? ''
    profile.professionalRole = user.professionalRole ?? ''
    accountRole.value = user.role
  } catch (cause) {
    error.value = cause instanceof Error ? cause.message : 'Impossibile caricare il profilo.'
  } finally { loading.value = false }
})

async function saveProfile() {
  error.value = ''
  success.value = ''
  saving.value = true
  try {
    const user = await updateMyProfile({ ...profile })
    profile.firstName = user.firstName ?? ''
    profile.lastName = user.lastName ?? ''
    profile.professionalRole = user.professionalRole ?? ''
    accountRole.value = user.role
    updateSessionUser(user)
    success.value = 'Profilo aggiornato.'
  } catch (cause) {
    error.value = cause instanceof Error ? cause.message : 'Impossibile salvare il profilo.'
  } finally { saving.value = false }
}
</script>

<template>
  <section>
    <div class="page-heading">
      <div><p class="eyebrow">ACCOUNT</p><h1>Il mio profilo</h1><p>Gestisci le informazioni personali del tuo account.</p></div>
    </div>
    <div class="profile-card panel">
      <div v-if="loading" class="empty-state">Caricamento profilo…</div>
      <form v-else class="form profile-form" @submit.prevent="saveProfile">
        <div v-if="error" class="alert" role="alert">{{ error }}</div>
        <div v-if="success" class="success-banner" role="status">{{ success }}</div>
        <label>Nome<input v-model.trim="profile.firstName" autocomplete="given-name" maxlength="80" required placeholder="Il tuo nome" /></label>
        <label>Cognome<input v-model.trim="profile.lastName" autocomplete="family-name" maxlength="80" required placeholder="Il tuo cognome" /></label>
        <label>Ruolo professionale<input v-model.trim="profile.professionalRole" autocomplete="organization-title" maxlength="80" placeholder="Es. Responsabile logistica" /></label>
        <div class="account-role"><span>Ruolo account</span><strong>{{ accountRole }}</strong><small>Gestisce i permessi applicativi e non è modificabile da questa pagina.</small></div>
        <div><button class="button" type="submit" :disabled="saving">{{ saving ? 'Salvataggio…' : 'Salva modifiche' }}</button></div>
      </form>
    </div>
  </section>
</template>
