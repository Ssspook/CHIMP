import { SyntheticEvent, useState } from 'react';

export const useTabManager = (): any => {
  const [tab, setTab] = useState<number>(0);
  const onTabsChange = (event: SyntheticEvent, value: number): void => {
    setTab(value);
  };
  
  return {
    tab,
    onTabsChange,
  };
};
