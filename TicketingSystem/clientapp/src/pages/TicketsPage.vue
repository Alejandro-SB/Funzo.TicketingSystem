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

const selectedRows = ref<GetAllTicketsTicket[]>([]);
const rows = ref<GetAllTicketsTicket[]>([]);

const { getAllTickets, create } = useTicketsApi();
const { isRegistered } = useAuth();
const router = useRouter();
const $q = useQuasar();
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
    name: 'id',
    field: 'id',
    required: true,
    label: 'Id',
    sortable: true,
    align: 'left',
  },
  {
    name: 'subject',
    field: 'subject',
    required: true,
    label: 'Subject',
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
          <q-btn
            :to="`/tickets/${selected?.id}`"
            :disable="!hasSelection"
            flat
            class="q-m-4"
            icon="edit"
          />
          <q-btn @click="onCreateNew" icon="add" flat />
        </div>
      </template>
    </q-table>
  </q-page>
</template>
