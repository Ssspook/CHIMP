import React from 'react';
import ReactDOM from 'react-dom/client';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import App from './App';
import { GradientDithering, Labs, Png } from '@/routes';

import './styles/globals.scss';
import { ToastMessageProvider } from '@/context/toast.context';

const router = createBrowserRouter([
  {
    path: '/',
    element: <App/>,
    children: [
      {
        path: '/labs',
        element: <Labs/>,
      },
      {
        path: '/labs-gradient-dithering',
        element: <GradientDithering/>,
      },
      {
        path: '/labs-png',
        element: <Png/>,
      },
    ],
  },
]);

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <ToastMessageProvider>
      <RouterProvider router={router}/>
    </ToastMessageProvider>
  </React.StrictMode>,
);
