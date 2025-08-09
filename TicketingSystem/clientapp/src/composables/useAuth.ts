import { StorageSerializers, useSessionStorage } from '@vueuse/core';
import type { GetUserResponse } from 'src/data/useUsersApi';
import { computed } from 'vue';

const AuthKey = 'Funzo.Auth';

export const useAuth = () => {
  const user = useSessionStorage<GetUserResponse>(AuthKey, null, {
    serializer: StorageSerializers.object,
  });
  const isRegistered = computed(() => user.value !== null);

  return {
    isRegistered,
    user,
  };
};
