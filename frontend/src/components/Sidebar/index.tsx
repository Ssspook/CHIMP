import { Box, Stack } from '@mui/material';
import clsx from 'clsx';
import { FC } from 'react';
import { NavLink } from 'react-router-dom';
import styles from './Sidebar.module.scss';

export const Sidebar: FC = () => {
  return (
    <div className={styles.bar}>
      <h4 className={styles.bar__heading}>Главная</h4>
      <Stack direction={'column'} spacing={2}>
        <NavLink to={'/'} end
          className={({ isActive }) => clsx(styles.bar__item, isActive && styles.bar__item_active)}>
          Тыкни меня
        </NavLink>
      </Stack>
      <h4 className={styles.bar__heading}>Все лабы</h4>
      <Stack direction={'column'} spacing={2}>
        <NavLink to={'/labs'}
          className={({ isActive }) => clsx(styles.bar__item, isActive && styles.bar__item_active)}>
          <Box>Pnm</Box>
        </NavLink>
        <NavLink to={'/labs-gradient-dithering'}
          className={({ isActive }) => clsx(styles.bar__item, isActive && styles.bar__item_active)}>
          <Box>Дизеринг на градиенте</Box>
        </NavLink>
      </Stack>
    </div>
  );
};

