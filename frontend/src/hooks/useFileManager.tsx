import { SetStateAction, useState } from 'react';

export type FileManager = {
  file: File | null;
  initialFileBase64: string | null;
  fetchedFileBase64: string | null;
  imageBlob: Blob | null;
  fileNameUploaded: string | null;
  initialGradientBlob: Blob | null;
  initialGradientBase64: string | null;
  finalGradientBase64: string | null;
};

export const useFileManager = (): {
  clearFileState: () => void;
  meta: FileManager;
  setMeta: (value: SetStateAction<FileManager>) => void;
} => {
  const [meta, setMeta] = useState<FileManager>({
    initialFileBase64: null,
    fetchedFileBase64: null,
    file: null,
    imageBlob: null,
    fileNameUploaded: null,
    initialGradientBlob: null,
    initialGradientBase64: null,
    finalGradientBase64: null,
  });

  const clearFileState = (): void => {
    setMeta({
      initialFileBase64: null,
      fetchedFileBase64: null,
      imageBlob: null,
      file: null,
      fileNameUploaded: null,
      initialGradientBlob: null,
      initialGradientBase64: null,
      finalGradientBase64: null,
    });
  };

  return {
    clearFileState,
    meta,
    setMeta,
  };
};
