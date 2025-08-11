<script setup lang="ts">
import { useQuasar } from 'quasar';
import { useAuth } from 'src/composables/useAuth';
import type { AddCommentToTicketError, GetTicketResponse } from 'src/data/useTicketsApi';
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

const ticket = ref<GetTicketResponse>();
const ticketId = ref(0);

const { getTicket, addComment } = useTicketsApi();

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
  const addCommentResult = await addComment(ticketId.value, comment.value);

  if (addCommentResult.isOk) {
    $q.notify({
      type: 'positive',
      message: 'comment added',
    });
    await getTicketInfo();
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
</script>
<template>
  <q-page padding v-if="ticket">
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
    <q-input type="textarea" v-model="comment" label="Add Comment">
      <template #after>
        <q-btn @click="add" label="Add" flat :disable="comment.trim().length === 0" />
      </template>
    </q-input>
  </q-page>
</template>
