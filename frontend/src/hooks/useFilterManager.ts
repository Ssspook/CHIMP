import { SetStateAction, useState } from 'react';

export type FilterManager = {
  file: File | null;
  initialFileBase64: string | null;
  fetchedFileBase64: string | null;
  imageBlob: Blob | null;
};

export const useFilterManager = (): {
  clearFilterState: () => void;
  filterMeta: FilterManager;
  setFilterMeta: (value: SetStateAction<FilterManager>) => void;
} => {
  const [filterMeta, setFilterMeta] = useState<FilterManager>({
    initialFileBase64: null,
    fetchedFileBase64: null,
    file: null,
    imageBlob: null,
  });

  const clearFilterState = (): void => {
    setFilterMeta({
      initialFileBase64: null,
      fetchedFileBase64: null,
      imageBlob: null,
      file: null,
    });
  };

  return {
    clearFilterState,
    filterMeta,
    setFilterMeta,
  };
};
