import { FC, ReactNode } from 'react';
import { Box } from '@mui/material';

type TabPanelProps = {
  children?: ReactNode;
  index: number;
  value: number;
};

export const TabPanel: FC<TabPanelProps> = (props) => {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
      {...other}>
      {value === index && (
        <Box sx={{ p: 3 }}>
          {children}
        </Box>
      )}
    </div>
  );
};
