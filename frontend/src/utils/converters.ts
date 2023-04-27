export const blobToBase64 = (blob: Blob): Promise<string> => {
  return new Promise((resolve) => {
    const reader = new FileReader();
    // @ts-ignore
    reader.onloadend = () => resolve(reader.result);
    reader.readAsDataURL(blob);
  });
};
