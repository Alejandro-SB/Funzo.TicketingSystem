import { useSessionStorage } from '@vueuse/core';
import { computed } from 'vue';

const AuthKey = 'Funzo.Auth';

export const useAuth = () => {
  const userId = useSessionStorage(AuthKey, 0);
  const isRegistered = computed(() => userId.value > 0);

  return {
    isRegistered,
    userId,
  };
};
