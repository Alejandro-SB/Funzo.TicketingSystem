<script setup lang="ts">
import { useDialogPluginComponent } from 'quasar';
import { computed, ref } from 'vue';

defineEmits([
  // REQUIRED; need to specify some events that your
  // component will emit through useDialogPluginComponent()
  ...useDialogPluginComponent.emits,
]);
const subject = ref('');
const body = ref('');

const { dialogRef, onDialogHide, onDialogOK, onDialogCancel } = useDialogPluginComponent();

const isValidTicket = computed(
  () => subject.value.trim().length > 0 && body.value.trim().length > 50,
);
</script>

<template>
  <q-dialog ref="dialogRef" @hide="onDialogHide">
    <q-card class="q-dialog-plugin">
      <q-card-section>
        <div class="text-h6">Create new ticket</div>
      </q-card-section>
      <q-card-section>
        <div class="q-gutter-y-md column" style="max-width: 300px">
          <q-input v-model="subject" filled dense label="Subject" />
          <q-input v-model="body" filled type="textarea" label="Problem description" />
        </div>
      </q-card-section>
      <q-card-actions align="right">
        <q-btn
          color="primary"
          label="OK"
          @click="onDialogOK({ subject, body })"
          :disabled="!isValidTicket"
        />
        <q-btn color="primary" label="Cancel" @click="onDialogCancel" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>
