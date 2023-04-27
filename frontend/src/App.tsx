import { Box, Stack } from '@mui/material';
import { FC } from 'react';
import { Sidebar } from '@/components';
import { Outlet, useLocation } from 'react-router-dom';
import { Container } from '@mui/system';
import styles from './App.module.scss';
import { LabWrapper } from './components/layout/LabWrapper';

const App: FC = () => {
  const { pathname } = useLocation();
  const isLab = pathname.includes('lab');

  return (
    <Stack direction={'row'}> 
      <Sidebar/>
      {
        isLab ? 
          <LabWrapper>
            <Outlet/> 
          </LabWrapper> : 
          <Container maxWidth={'xl'}>
            <Box className={styles.main}>
              <h1>Главная страница <mark>команды #7</mark></h1>
            </Box>
          </Container>
      }
    </Stack>
  );
};

export default App;
