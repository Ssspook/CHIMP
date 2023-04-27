import { Box } from '@mui/system';
import { FC, ReactNode } from 'react';

type LabWrapperProps = {
  children: ReactNode;
};

export const LabWrapper: FC<LabWrapperProps> = ({ children }) => {
  return (
    <Box sx={{ p: 4 }}>
      {children}
    </Box>
  );
};
