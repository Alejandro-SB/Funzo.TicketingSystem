<script setup lang="ts">
import { QTable } from 'quasar';
import { useUsersApi, type UserComment } from 'src/data/useUsersApi';
import { toParam } from 'src/utils';
import { onMounted, ref, type ComponentInstance } from 'vue';
import { useRoute, useRouter } from 'vue-router';

type ColumnType = ComponentInstance<typeof QTable>['$props']['columns'];
const columns: ColumnType = [
  {
    name: 'ticketId',
    field: 'ticketId',
    required: true,
    label: 'Ticket id',
    sortable: true,
    align: 'left',
  },
  {
    name: 'text',
    field: 'text',
    required: true,
    label: 'Text',
    format: (val: string) => `${val.substring(0, 50)}...`,
    sortable: true,
    align: 'left',
  },
  {
    name: 'date',
    field: 'date',
    required: true,
    label: 'Date',
    // Not the best way, but easier
    format: (val: string) => new Intl.DateTimeFormat().format(new Date(val)),
    sortable: true,
    align: 'left',
  },
  {
    name: 'actions',
    field: 'actions',
    required: true,
    label: 'Actions',
    format: (val) => val,
    sortable: true,
    align: 'left',
  },
];

const { getUserComments } = useUsersApi();
const route = useRoute();
const router = useRouter();
const rows = ref<UserComment[]>([]);

onMounted(async () => {
  const { id } = route.params;

  const parsedUserId = parseFloat(toParam(id) ?? '0');

  if (parsedUserId <= 0) {
    await router.replace({ name: 'not-found' });
    return;
  }

  const commentsResponse = await getUserComments(parsedUserId);

  rows.value = commentsResponse.comments;
});
</script>

<template>
  <q-page padding>
    <q-table title="Comments" :rows="rows" :columns="columns" row-key="id">
      <template v-slot:body-cell-actions="props">
        <q-td :props="props">
          <q-btn
            :to="`/tickets/${props.row.ticketId}`"
            color="accent"
            label="See ticket"
            flat
          ></q-btn>
        </q-td>
      </template>
    </q-table>
  </q-page>
</template>
