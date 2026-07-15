// Wrapper attorno a vis-network per disegnare grafi/alberi.
// Il genitore passa nodi/archi già in formato vis (compresi i colori per
// l'evidenziazione del percorso).
import { ref, onMounted, onBeforeUnmount, watch } from 'vue';

export default {
  name: 'GraphPanel',
  props: {
    nodes: { default: () => [] },
    edges: { default: () => [] },
    hierarchical: { default: false },
    directed: { default: false }
  },
  setup(props) {
    const elRef = ref(null);
    let network = null;

    const buildOptions = () => ({
      nodes: {
        shape: 'dot',
        size: 16,
        borderWidth: 2,
        color: { background: '#c7d2fe', border: '#4f46e5' },
        font: { size: 14, color: '#0f172a' }
      },
      edges: {
        arrows: props.directed ? 'to' : '',
        color: { color: '#94a3b8', highlight: '#4f46e5' },
        font: { align: 'top', size: 12, color: '#475569', strokeWidth: 3 },
        smooth: false
      },
      physics: { enabled: !props.hierarchical, stabilization: true },
      layout: props.hierarchical
        ? { hierarchical: { direction: 'UD', sortMethod: 'directed', levelSeparation: 90, nodeSpacing: 150 } }
        : {},
      interaction: { hover: true, tooltipDelay: 120 }
    });

    const render = () => {
      if (!elRef.value) {
        return;
      }
      if (!window.vis) {
        elRef.value.innerHTML = '<div class="p-3 text-danger small">Libreria di visualizzazione non caricata (vis-network).</div>';
        return;
      }
      const data = {
        nodes: new window.vis.DataSet(props.nodes),
        edges: new window.vis.DataSet(props.edges)
      };
      if (network) {
        network.destroy();
      }
      network = new window.vis.Network(elRef.value, data, buildOptions());
    };

    onMounted(render);
    watch(() => [props.nodes, props.edges, props.hierarchical], render, { deep: true });
    onBeforeUnmount(() => {
      if (network) {
        network.destroy();
      }
    });

    return { elRef };
  },
  template: '<div ref="elRef" class="graph-canvas"></div>'
};
