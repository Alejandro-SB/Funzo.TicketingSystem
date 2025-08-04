<script setup lang="ts">
import { QTable, useQuasar } from 'quasar';
import CreateNewTicketDialog from 'src/components/CreateNewTicketDialog.vue';
import type { GetAllTicketsTicket } from 'src/data/useTicketsApi';
import { useTicketsApi } from 'src/data/useTicketsApi';
import type { ComponentInstance } from 'vue';
import { computed, onMounted, ref } from 'vue';

type ColumnType = ComponentInstance<typeof QTable>['$props']['columns'];

const selectedRows = ref([]);
const rows = ref<GetAllTicketsTicket[]>([]);

const { getAllTickets, create } = useTicketsApi();

const $q = useQuasar();
const onDelete = () => {};
const onCreateNew = () => {
  $q.dialog({
    component: CreateNewTicketDialog,
    title: 'Create new ticket',
    componentProps: {},
  }).onOk(({ subject, body }) => {
    create(subject, body).catch(() => {});
  });
};

const columns: ColumnType = [
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
    format: (val: string) => `${val.substring(0, 50)}...`,
    sortable: true,
    align: 'left',
  },
];

onMounted(async () => {
  const { tickets } = await getAllTickets();
  rows.value = tickets;
});

const selected = computed(() => selectedRows.value[0]);
const hasSelection = computed(() => selected.value !== undefined);
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
          <q-btn @click="onDelete" icon="edit" flat :disabled="!hasSelection" />
          <q-btn @click="onCreateNew" icon="add" flat />
        </div>
      </template>
    </q-table>
  </q-page>
</template>
