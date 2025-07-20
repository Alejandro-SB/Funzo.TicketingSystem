<template>
  <q-page padding>
    <q-table title="Treats" :rows="rows" :columns="columns" row-key="name" />
  </q-page>
</template>

<script setup lang="ts">
import { get } from 'src/data/api';
import { onMounted, ref } from 'vue';

const columns = [
  {
    name: 'subject',
    field: 'subject',
    required: true,
    label: 'Subject',
    sortable: true,
    align: 'left',
  },
  {
    name: 'body',
    field: 'body',
    required: true,
    label: 'Body',
    format: (val: string) => `${val.substring(0, 50)}`,
    sortable: true,
    align: 'left',
  },
];

const rows = ref([]);

onMounted(() => {
  get('/api/tickets')
    .then((r) => (rows.value = r.tickets))
    .catch((r) => console.log(r));
});
</script>
