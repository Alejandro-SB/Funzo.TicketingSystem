<script setup lang="ts">
import { useQuasar } from 'quasar';
import { useAuth } from 'src/composables/useAuth';
import type {
  AddCommentToTicketError,
  EscalateTicketError,
  GetTicketResponse,
  SolveTicketError,
} from 'src/data/useTicketsApi';
import { useTicketsApi } from 'src/data/useTicketsApi';
import { handleUnion } from 'src/types/utils';
import { formatDate, toParam } from 'src/utils';
import { onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const {
  params: { id },
} = useRoute();
const router = useRouter();
const comment = ref('');
const { user } = useAuth();

const $q = useQuasar();

const ticket = ref<GetTicketResponse>(null!); // Not null because if null we navigate to not-found page
const ticketId = ref(0);

const { getTicket, addComment, escalate, solve } = useTicketsApi();

onMounted(async () => {
  const parsedTicketId = parseFloat(toParam(id) || '0');
  if (parsedTicketId > 0) {
    ticketId.value = parsedTicketId;
  } else {
    await router.replace({
      name: 'not-found',
    });

    return;
  }

  await getTicketInfo();
});

const getTicketInfo = async () => {
  const ticketResponse = await getTicket(ticketId.value);

  if (ticketResponse.hasValue) {
    ticket.value = ticketResponse.value;
  } else {
    await router.replace({ name: 'not-found' });
  }
};

const add = async () => {
  if (comment.value.trim().length === 0) {
    return;
  }

  const addCommentResult = await addComment(ticketId.value, comment.value);

  if (addCommentResult.isOk) {
    $q.notify({
      type: 'positive',
      message: 'comment added',
    });
    await getTicketInfo();

    comment.value = '';
  } else {
    const { err } = addCommentResult;

    const message = handleUnion<AddCommentToTicketError, string>(
      {
        InvalidCommentText: () => 'Comment is not valid',
        TicketAlreadySolved: (e) => `Ticket was already solved at ${formatDate(e.resolutionDate)}`,
        TicketNotFound: () => 'Ticket was not found. Check the request.',
        UserNotFound: () => 'User not found',
      },
      err,
    );

    $q.notify({
      type: 'negative',
      message,
    });
  }
};

const escalateTicket = async () => {
  const response = await escalate(ticket.value.id);

  if (response.isOk) {
    $q.notify({
      message: 'Ticket escalated to level 2 support',
      type: 'positive',
    });

    ticket.value.isEscalated = true;

    return;
  }

  const message = handleUnion<EscalateTicketError, string>(
    {
      TicketAlreadyEscalated: () => 'This ticket has already been escalated',
      TicketAlreadySolved: (e) =>
        `This ticket was solve at ${formatDate(e.resolutionDate)} and cannot be escalated`,
      TicketNotFound: () => 'Ticket was not found. Contact support',
    },
    response.err,
  );

  $q.notify({
    message,
    type: 'negative',
  });
};

const solveTicket = async () => {
  const response = await solve(ticket.value.id);

  if (response.isOk) {
    $q.notify({
      message: 'Ticket marked as solved',
      type: 'positive',
    });

    ticket.value.resolutionDate = new Date().toISOString();
    return;
  }

  const message = handleUnion<SolveTicketError, string>(
    {
      TicketAlreadySolved: (e) =>
        `This ticket was solve at ${formatDate(e.resolutionDate)} and cannot be escalated`,
      TicketNotFound: () => 'Ticket was not found. Contact support',
    },
    response.err,
  );

  $q.notify({
    message,
    type: 'negative',
  });
};
</script>
<template>
  <q-page padding v-if="ticket">
    <h2>Subject: {{ ticket.subject }}</h2>
    <h4 v-if="ticket.isEscalated && ticket.resolutionDate === null" class="bg-negative">
      High priority
    </h4>
    <h4 v-if="ticket.resolutionDate !== null" class="bg-positive">Solved</h4>
    <div class="q-pa-md row justify-center">
      <div style="width: 100%; max-width: 400px">
        <q-chat-message
          v-for="(message, i) in ticket.comments"
          :key="i"
          :text="[message.text]"
          :stamp="message.displayName"
          :sent="message.userId === user.id"
        >
        </q-chat-message>
      </div>
    </div>
    <q-input type="textarea" v-model="comment" label="Add Comment" @keydown.enter="add">
      <template #after>
        <q-btn @click="add" label="Add" flat :disable="comment.trim().length === 0" />
      </template>
    </q-input>
    <div class="row q-gutter-x-md justify-end">
      <q-btn
        label="Escalate"
        :disable="ticket.isEscalated"
        @click="escalateTicket"
        color="accent"
      />
      <q-btn
        label="Solve"
        :disable="ticket.resolutionDate !== null"
        @click="solveTicket"
        color="accent"
      />
    </div>
  </q-page>
</template>
