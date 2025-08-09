<script setup lang="ts">
import type { QTable } from 'quasar';
import type { GetAllUsersUser } from 'src/data/useUsersApi';
import { computed, ref, type ComponentInstance } from 'vue';

type ColumnType = ComponentInstance<typeof QTable>['$props']['columns'];
const columns: ColumnType = [
  {
    name: 'id',
    field: 'id',
    required: true,
    label: 'Id',
    sortable: true,
    align: 'left',
  },
  {
    name: 'username',
    field: 'username',
    required: true,
    label: 'Username',
    format: (val: string) => `${val.substring(0, 50)}...`,
    sortable: true,
    align: 'left',
  },
  {
    name: 'displayName',
    field: 'displayName',
    required: true,
    label: 'Display Name',
    format: (val: string) => `${val.substring(0, 50)}...`,
    sortable: true,
    align: 'left',
  },
  {
    name: 'userComments',
    field: 'userComments',
    required: true,
    label: 'User Comments',
    format: (val: string) => `${val.substring(0, 50)}...`,
    sortable: true,
    align: 'left',
  },
];

const selectedRows = ref([]);
const rows = ref<GetAllUsersUser[]>([]);
const selected = computed(() => selectedRows.value[0]);
const hasSelection = computed(() => selected.value !== undefined);

const onEdit = () => {};
const onDelete = () => {};
</script>

<template>
  <q-page padding>
    <q-table
      title="Tickets"
      :rows="rows"
      :columns="columns"
      row-key="id"
      selection="single"
      v-model:selected="selectedRows"
    >
      <template #top-right>
        <div class="q-gutter-x-md row justify-end">
          <q-btn @click="onDelete" icon="delete" class="q-m-4" flat :disabled="!hasSelection" />
          <q-btn @click="onEdit" icon="edit" flat :disabled="!hasSelection" />
        </div>
      </template>
    </q-table>
  </q-page>
</template>
