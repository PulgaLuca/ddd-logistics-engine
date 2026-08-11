#!/bin/sh
# Genera la configurazione runtime della SPA a partire dalle variabili
# d'ambiente del container: la stessa immagine può così puntare all'API di
# sviluppo, di staging o di produzione senza essere ricostruita.
set -e

API_BASE_URL="${API_BASE_URL:-}"

cat > /usr/share/nginx/html/config.runtime.js <<EOF
// File generato all'avvio del container: non modificare a mano.
window.__API_BASE__ = "${API_BASE_URL}";
EOF

echo "config.runtime.js generato con API_BASE_URL='${API_BASE_URL}'"
