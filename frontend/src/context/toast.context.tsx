import { createContext, Dispatch, FC, ReactNode, SetStateAction, useState } from 'react';
import { IToastMessage } from 'app/common';
import { AlertColor } from '@mui/material';

interface IContext {
  toast: IToastMessage,
  setToast: Dispatch<SetStateAction<IToastMessage>>,
  onToastClose: () => void,
  showToast(variant: AlertColor, message?: string)
}

export const ToastMessageContext = createContext({} as IContext);

export const ToastMessageProvider: FC<{
  children: ReactNode;
}> = ({ children }) => {
  const [toast, setToast] = useState<IToastMessage>({
    open: false,
    vertical: 'bottom',
    horizontal: 'left',
    message: '',
    variant: null,
  });

  const onToastClose = (): void => {
    setToast((prev) => ({ ...prev, open: false, message: '', variant: null }));
  };

  const showToast = (variant: AlertColor, message?: string): void => {
    setToast((prev) => ({
      ...prev,
      open: true,
      message: message || 'Что-то пошло не так',
      variant: variant,
    }));
  };

  return (
    <ToastMessageContext.Provider value={{ toast, setToast, onToastClose, showToast }}>
      {children}
    </ToastMessageContext.Provider>
  );
};
