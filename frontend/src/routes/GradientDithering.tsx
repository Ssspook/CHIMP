import { FC, useContext, useState } from 'react';
import { Box, Button, Container, FormControlLabel, FormLabel, Radio, RadioGroup, Slider, Stack } from '@mui/material';
import { PnmService } from '@/api/services/pnm.service';
import { blobToBase64 } from '@/utils/converters';
import { errors } from '@/utils/errors';
import { useFileManager } from '@/hooks/useFileManager';
import { ToastMessageContext } from '@/context/toast.context';
import { DitheringAlgo } from '@/utils/enums/dithering-algo.enum';

export const GradientDithering: FC = () => {
  const [dithering, setDithering] = useState<DitheringAlgo>(DitheringAlgo.NONE);
  const [bitrate, setBitrate] = useState<number>(8);
  const { meta, setMeta } = useFileManager();
  const { showToast } = useContext(ToastMessageContext);

  const onGradientApply = async (): Promise<void> => {
    try {
      const { data } = await PnmService.applyGradient();
      const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
      const base64 = await blobToBase64(blob);
      setMeta((prev) => ({ ...prev, initialGradientBase64: base64, initialGradientBlob: blob }));
    } catch (e) {
      showToast('error', errors[(e as any).code] || 'Произвестная ошибка');
    }
  };

  const onGradientDitheringApply = async (): Promise<void> => {
    try {
      if (!meta.initialGradientBlob) {
        showToast('warning', 'Загрузите изображение');
        return;
      }

      const { data } = await PnmService.previewGradientImage(
        new File([meta.initialGradientBlob], 'gradient'),
        bitrate,
        dithering,
      );
      const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
      const base64 = await blobToBase64(blob);
      setMeta((prev) => ({ ...prev, finalGradientBase64: base64 }));
    } catch (e) {
      showToast('error', errors[(e as any).code] || 'Произвестная ошибка');
    }
  };

  return (
    <Container maxWidth={'xl'}>
      <>
        <FormLabel>Выбор алгоритма дизеринга</FormLabel>
        <RadioGroup row name="algorithms-group" defaultValue={DitheringAlgo.NONE}
          onChange={(_, value) => setDithering(value as unknown as DitheringAlgo)}>
          <FormControlLabel value={DitheringAlgo.NONE}
            control={<Radio/>}
            label={'None'}/>
          <FormControlLabel value={DitheringAlgo.FLOYD_STEINBERG}
            control={<Radio/>}
            label={'Floyd-Steinberg'}/>
          <FormControlLabel value={DitheringAlgo.ATKINSON}
            control={<Radio/>}
            label={'Atkinson'}/>
          <FormControlLabel value={DitheringAlgo.ORDERED}
            control={<Radio/>}
            label={'Ordered'}/>
          <FormControlLabel value={DitheringAlgo.RANDOM}
            control={<Radio/>}
            label={'Random'}/>
        </RadioGroup>
      </>
      <>
        <FormLabel>Выбор битности</FormLabel>
        <Slider defaultValue={8}
          onChange={(_, value) => setBitrate(value as number)}
          aria-label="Default"
          valueLabelDisplay="auto"
          min={1}
          max={8}/>
      </>
      <Stack sx={{ mt: '2rem' }} direction={'row'} spacing={2}>
        <Button variant="outlined" onClick={onGradientApply}>Отобразить градиент</Button>
        <Button variant="outlined" onClick={onGradientDitheringApply}>Применить дизеринг к градиенту</Button>
      </Stack>
      {
        meta.initialGradientBase64 && <Box sx={{ mt: '2rem' }}>
          <img src={meta.initialGradientBase64} alt="gradient"/>
        </Box>
      }
      {
        meta.finalGradientBase64 && <Box sx={{ mt: '2rem' }}>
          <img src={meta.finalGradientBase64} alt="gradient"/>
        </Box>
      }
    </Container>
  );
};
