declare module 'app/common' {
  import { AlertColor, SnackbarOrigin } from '@mui/material';

  export interface IToastMessage extends SnackbarOrigin {
    open: boolean;
    message?: string;
    variant?: AlertColor | null;
  }

  type Coordinates = [number, number];
}
