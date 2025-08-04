<script setup lang="ts">
import { useQuasar } from 'quasar';
import { useAuth } from 'src/composables/useAuth';
import type { CreateUserError } from 'src/data/useUsersApi';
import { useUsersApi } from 'src/data/useUsersApi';
import type { UnionToFunc } from 'src/types/utils';
import { ref } from 'vue';

const { createUser } = useUsersApi();
const { userId } = useAuth();

const username = ref('');
const displayName = ref('');

const $q = useQuasar();

const onSubmit = async () => {
  const response = await createUser(username.value, displayName.value);

  if (response.isOk) {
    userId.value = response.ok;
  } else {
    const responseProcessor: UnionToFunc<CreateUserError> = {
      DisplayNameNotValid: (err) => err.reason,
      UsernameAlreadyExists: () => 'Username already exists',
      UsernameNotValid: (err) => err.reason,
    };

    const message = responseProcessor[response.err]
    }
  }
};

// const isValidForm = computed(
//   () => username.value.trim().length > 0 && displayName.value.trim().length > 0,
// );
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
