// Mostra qualunque risposta dell'API come campi leggibili (oggetti -> tabella
// campo/valore, liste -> tabella con intestazioni), ricorsivamente. Niente JSON.
import { humanizeKey, formatValue, isPlainObject } from '../format.js';

export default {
  name: 'DataView',
  props: {
    data: { default: null }
  },
  computed: {
    kind() {
      if (this.data === null || this.data === undefined || this.data === '') {
        return 'empty';
      }
      if (Array.isArray(this.data)) {
        return 'array';
      }
      if (isPlainObject(this.data)) {
        return 'object';
      }
      return 'scalar';
    },
    entries() {
      return isPlainObject(this.data) ? Object.entries(this.data) : [];
    },
    isObjectArray() {
      return Array.isArray(this.data) && this.data.length > 0 && this.data.every(isPlainObject);
    },
    columns() {
      if (!this.isObjectArray) {
        return [];
      }
      const set = new Set();
      this.data.forEach((row) => Object.keys(row).forEach((key) => set.add(key)));
      return [...set];
    }
  },
  methods: {
    humanizeKey,
    formatValue,
    isComposite(value) {
      return value !== null && typeof value === 'object';
    }
  },
  template: `
    <span v-if="kind === 'empty'" class="text-muted">Nessun dato.</span>

    <span v-else-if="kind === 'scalar'">{{ formatValue(data) }}</span>

    <table v-else-if="kind === 'object'" class="table table-sm field-table mb-0">
      <tbody>
        <tr v-for="[key, value] in entries" :key="key">
          <th>{{ humanizeKey(key) }}</th>
          <td>
            <data-view v-if="isComposite(value)" :data="value" />
            <span v-else>{{ formatValue(value) }}</span>
          </td>
        </tr>
      </tbody>
    </table>

    <div v-else-if="kind === 'array' && isObjectArray" class="table-responsive">
      <table class="table table-sm field-table mb-0">
        <thead>
          <tr><th v-for="col in columns" :key="col">{{ humanizeKey(col) }}</th></tr>
        </thead>
        <tbody>
          <tr v-for="(row, i) in data" :key="i">
            <td v-for="col in columns" :key="col">
              <data-view v-if="isComposite(row[col])" :data="row[col]" />
              <span v-else>{{ formatValue(row[col]) }}</span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <ul v-else-if="kind === 'array'" class="mb-0 small">
      <li v-for="(value, i) in data" :key="i">
        <data-view v-if="isComposite(value)" :data="value" />
        <span v-else>{{ formatValue(value) }}</span>
      </li>
    </ul>

    <span v-else class="text-muted">—</span>
  `
};
