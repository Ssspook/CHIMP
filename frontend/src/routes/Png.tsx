import { Box, Button, Container, Stack, TextField } from '@mui/material';
import { ChangeEvent, FC, useContext, useEffect, useRef, useState } from 'react';
import { blobToBase64 } from '@/utils/converters';
import { errors } from '@/utils/errors';
import { useFileManager } from '@/hooks/useFileManager';
import { ToastMessageContext } from '@/context/toast.context';
import { PngService } from '@/api/services/png.service';

export const Png: FC = () => {
  const { meta, setMeta } = useFileManager();
  const { showToast } = useContext(ToastMessageContext);
  const [gamma, setGamma] = useState<string>();

  const onPngImageUpload = async (event: ChangeEvent<HTMLInputElement>): Promise<void> => {
    if (!event.target.files) {
      showToast('warning', 'Загрузите файл');
      return;
    }

    try {
      const [file] = event.target.files;
      const { data } = await PngService.uploadPngImage(file);
      const blob = new Blob([data], { type: 'image/png' });
      const base64 = await blobToBase64(blob);
      setMeta((prev) => ({ ...prev, imageBlob: blob, fetchedFileBase64: base64, file }));
    } catch (e) {
      showToast('error', errors[(e as any).code] || 'Произошла неизвестная ошибка');
    }
  };

  const onGammaAssign = async (): Promise<void> => {
    if (!gamma || !meta.imageBlob) {
      showToast('warning', 'Введите гамму или загрузите изображение');
      return;
    }

    try {
      const { data } = await PngService.assignGamma(meta.imageBlob, gamma);
      const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
      const base64 = await blobToBase64(blob);
      setMeta((prev) => ({ ...prev, imageBlob: blob, fetchedFileBase64: base64 }));
    } catch (e) {
      showToast('error', errors[(e as any).code] || 'Произвестная ошибка');
    }
  };

  const onGammaRecalculate = async (): Promise<void> => {
    if (!gamma || !meta.imageBlob) {
      showToast('warning', 'Введите гамму или загрузите изображение');
      return;
    }

    try {
      const { data } = await PngService.recalculateGamma(meta.imageBlob, gamma);
      const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
      const base64 = await blobToBase64(blob);
      setMeta((prev) => ({ ...prev, imageBlob: blob, fetchedFileBase64: base64 }));
    } catch (e) {
      showToast('error', errors[(e as any).code] || 'Произвестная ошибка');
    }
  };

  console.log(meta.fetchedFileBase64);

  return (
    <Container maxWidth={'xl'}>
      <h2>Просмотр PNG</h2>
      <Box sx={{ mt: '2rem' }}>
        <Button variant="contained" component="label">
          Загрузить файл
          <input hidden accept="image/x-portable-anymap" type="file" onChange={onPngImageUpload}/>
        </Button>
      </Box>
      <Box sx={{ mt: '2rem' }}>
        <TextField label="Введите гамму" variant="standard" inputProps={{ type: 'number' }}
          value={gamma} onChange={(e) => setGamma(e.target.value)}/>
        <Stack direction="row" spacing={2} sx={{ mt: '1rem' }}>
          <Button variant="outlined" onClick={onGammaAssign}>Назначить</Button>
          <Button variant="outlined" onClick={onGammaRecalculate}>Пересчитать</Button>
        </Stack>
      </Box>
      {
        meta.fetchedFileBase64 &&
        <Box sx={{ mt: '2rem' }}>
          <img src={meta.fetchedFileBase64} alt={'png image'}/>
        </Box>
      }
    </Container>
  );
};
