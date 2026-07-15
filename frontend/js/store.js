// Stato di autenticazione reattivo (token + ruolo), persistito nel browser.
import { reactive } from 'vue';

const KEY = 'le.auth';

function load() {
  const raw = localStorage.getItem(KEY);
  return raw ? JSON.parse(raw) : null;
}

export const auth = reactive({ user: load() });

const MANAGE_DATA_ROLES = ['Administrator', 'Manager'];
const OPERATE_ROLES = ['Administrator', 'Manager', 'Operator'];

export function setAuth(user) {
  auth.user = user;
  localStorage.setItem(KEY, JSON.stringify(user));
}

export function clearAuth() {
  auth.user = null;
  localStorage.removeItem(KEY);
}

export function isLoggedIn() {
  const user = auth.user;
  if (!user?.accessToken) {
    return false;
  }
  if (user.expiresAtUtc && new Date(user.expiresAtUtc).getTime() < Date.now()) {
    clearAuth();
    return false;
  }
  return true;
}

export function role() {
  return auth.user?.role ?? null;
}

export function canManageData() {
  return MANAGE_DATA_ROLES.includes(role());
}

export function canOperate() {
  return OPERATE_ROLES.includes(role());
}
