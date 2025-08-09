<script setup lang="ts">
import { QTable, useQuasar } from 'quasar';
import CreateNewTicketDialog from 'src/components/CreateNewTicketDialog.vue';
import { useAuth } from 'src/composables/useAuth';
import type { GetAllTicketsTicket, CreateTicketError } from 'src/data/useTicketsApi';
import { useTicketsApi } from 'src/data/useTicketsApi';
import { handleUnion } from 'src/types/utils';
import type { ComponentInstance } from 'vue';
import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';

type ColumnType = ComponentInstance<typeof QTable>['$props']['columns'];

const selectedRows = ref([]);
const rows = ref<GetAllTicketsTicket[]>([]);

const { getAllTickets, create } = useTicketsApi();
const { isRegistered } = useAuth();
const router = useRouter();
const $q = useQuasar();
const onDelete = () => {};
const onCreateNew = () => {
  $q.dialog({
    component: CreateNewTicketDialog,
    title: 'Create new ticket',
    componentProps: {},
    // eslint-disable-next-line @typescript-eslint/no-misused-promises
  }).onOk(async ({ subject, body }) => {
    const response = await create(subject, body);

    if (response.isOk) {
      $q.notify({
        type: 'positive',
        message: 'Ticket created',
      });

      await router.push(`/tickets/${response.ok}`);
    } else {
      const message = handleUnion<CreateTicketError, string>(
        {
          InvalidTicketBody: (e) => e.reason,
          InvalidTicketSubject: (e) => e.reason,
          UserNotFound: () => 'User not found',
        },
        response.err,
      );

      $q.notify({
        type: 'negative',
        message,
      });
    }
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
  if (!isRegistered.value) {
    await router.push('/');
    return;
  }

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
