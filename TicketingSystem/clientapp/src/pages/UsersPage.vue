<script setup lang="ts">
import type { QTable } from 'quasar';
import { useUsersApi, type GetAllUsersUser } from 'src/data/useUsersApi';
import { computed, onMounted, ref, type ComponentInstance } from 'vue';

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
    sortable: true,
    align: 'left',
  },
];

const selectedRows = ref<GetAllUsersUser[]>([]);
const rows = ref<GetAllUsersUser[]>([]);
const selected = computed(() => selectedRows.value[0]);
const hasSelection = computed(() => selected.value !== undefined);

const { getAllUsers } = useUsersApi();

onMounted(async () => {
  const users = await getAllUsers();

  rows.value = users.users;
});
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
          <q-btn
            :to="`/users/${selected?.id}`"
            :disable="!hasSelection"
            icon="edit"
            flat
            class="q-m-4"
            @click="() => {}"
          />
        </div>
      </template>
    </q-table>
  </q-page>
</template>
