import { ref } from 'vue';

type ToastType = 'success' | 'error' | 'info' | 'warning';
interface Toast {
  message: string;
  type: ToastType;
}

const toasts = ref<Toast[]>([]);

export const useToast = () => {
  const addToast = (message: string, type: ToastType = 'info') => {
    toasts.value.push({ message, type });
    setTimeout(() => {
      toasts.value.shift();
    }, 5000);
  };

  const removeToast = (index: number) => {
    toasts.value.splice(index, 1);
  };

  return {
    toasts,
    addToast,
    removeToast,
  };
};

// Global toast function for error handler
export const globalAddToast = (message: string, type: 'success' | 'error' | 'info' = 'info') => {
  toasts.value.push({ message, type });
  setTimeout(() => {
    toasts.value.shift();
  }, 5000);
};