# La SPA non ha build step (import map + moduli ES nativi): basta servirla
# come contenuto statico. Il build context è la ROOT del repository.
FROM nginx:1.27-alpine

COPY frontend/nginx.conf /etc/nginx/conf.d/default.conf

COPY frontend/index.html /usr/share/nginx/html/index.html
COPY frontend/config.runtime.js /usr/share/nginx/html/config.runtime.js
COPY frontend/css /usr/share/nginx/html/css
COPY frontend/js /usr/share/nginx/html/js

# nginx esegue all'avvio gli script in /docker-entrypoint.d: qui viene
# generato config.runtime.js con l'indirizzo dell'API.
COPY frontend/docker-entrypoint.d/10-api-base.sh /docker-entrypoint.d/10-api-base.sh
RUN chmod +x /docker-entrypoint.d/10-api-base.sh

EXPOSE 80
