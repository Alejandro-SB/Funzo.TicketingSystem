<script setup lang="ts">
import { useQuasar } from 'quasar';
import { useAuth } from 'src/composables/useAuth';
import type { CreateUserError } from 'src/data/useUsersApi';
import { useUsersApi } from 'src/data/useUsersApi';
import { handleUnion } from 'src/types/utils';
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';

const { createUser, getUser } = useUsersApi();
const { user } = useAuth();

const username = ref('');
const displayName = ref('');

const $q = useQuasar();
const router = useRouter();

const onSubmit = async () => {
  const response = await createUser(username.value, displayName.value);

  if (response.isOk) {
    $q.notify({
      type: 'positive',
      message: 'User created',
    });

    const getUserResponse = await getUser(response.ok);

    if (getUserResponse.hasValue) {
      user.value = getUserResponse.value;
      await router.replace('/tickets');
    } else {
      $q.notify({
        type: 'negative',
        message: 'Unable to find user',
      });
    }
  } else {
    const responseMessage = handleUnion<CreateUserError, string>(
      {
        DisplayNameNotValid: (e) => e.reason,
        UsernameAlreadyExists: () => 'Username already exists',
        UsernameNotValid: (e) => e.reason,
      },
      response.err,
    );

    $q.notify({
      type: 'negative',
      message: responseMessage,
    });
  }
};

onMounted(async () => {
  if (user.value !== null) {
    await router.replace('/tickets');
  }
});
</script>

<template>
  <q-page class="row items-center justify-evenly">
    <q-card class="q-dialog-plugin">
      <q-card-section>
        <div class="text-h6">Create User</div>
      </q-card-section>
      <q-card-section>
        <div class="q-gutter-y-md column" style="max-width: 300px">
          <q-input v-model="username" filled dense label="Username" />
          <q-input v-model="displayName" filled dense label="Display name" />
        </div>
      </q-card-section>
      <q-card-actions align="right">
        <q-btn color="primary" label="Create" @click="onSubmit" />
      </q-card-actions>
    </q-card>
  </q-page>
</template>
