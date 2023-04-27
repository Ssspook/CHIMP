import { PnmService } from '@/api/services/pnm.service';
import { TabPanel } from '@/components';
import { blobToBase64 } from '@/utils/converters';
import { errors } from '@/utils/errors';
import {
  Alert,
  Box,
  Button,
  Checkbox,
  Container,
  FormControlLabel,
  FormGroup,
  FormLabel,
  Modal,
  Radio,
  RadioGroup,
  Slider,
  Snackbar,
  Stack,
  Tab,
  Tabs,
  TextField,
  Typography,
} from '@mui/material';
import { ChangeEvent, FC, useCallback, useContext, useEffect, useRef, useState, MouseEvent } from 'react';
import { ToastMessageContext } from '@/context/toast.context';
import { ColorSpacePicker } from '@/components/ColorSpacePicker';
import { useFileManager } from '@/hooks/useFileManager';
import { useTabManager } from '@/hooks/useTabManager';
import { ColorSpaceEnum } from '@/utils/enums/color-spaces.enum';
import { ColorSpaceTumbler } from '@/components/ColorSpaceTumbler';
import { ColorChannelsEnum, RgbChannelsEnum } from '@/utils/enums/color-channels.enum';
import { Coordinates } from 'app/common';
import { DitheringAlgo } from '@/utils/enums/dithering-algo.enum';
import { Bar } from 'react-chartjs-2';
import 'chart.js/auto';
import { FiltersEnum } from '@/utils/enums/filters.enum';
import { FilterService } from '@/api/services/filter.service';
import { useFilterManager } from '@/hooks/useFilterManager';

const modalStyle = {
  position: 'absolute',
  top: '50%',
  left: '50%',
  transform: 'translate(-50%, -50%)',
  width: 500,
  bgcolor: 'background.paper',
  boxShadow: 12,
  borderRadius: 4,
  p: 4,
};

export const Labs: FC = () => {
  const [inputColorSpace, setInputColorSpace] = useState<ColorSpaceEnum | null>(null);
  const [outputColorSpace, setOutputColorSpace] = useState<ColorSpaceEnum | null>(null);

  const [channel, setChannel] = useState<ColorChannelsEnum | null>(null);
  const [gamma, setGamma] = useState<string>();
  const [transparency, setTransparency] = useState<number>(0);
  const [lineWidth, setLineWidth] = useState<number>(1);

  const [dithering, setDithering] = useState<DitheringAlgo>(DitheringAlgo.NONE);
  const [bitrate, setBitrate] = useState<number>(8);

  const [hasAutoCorrection, setHasAutoCorrection] = useState<boolean>(false);
  const [autoCorrectionPercentage, setAutoCorrectionPercentage] = useState<number>(0.5);
  const [correctionChannel, setCorrectionChannel] = useState<RgbChannelsEnum | null>(null);
  const [initialHistogramData, setInitialHistogramData] = useState<number[][]>([]);
  const [modifiedHistogramData, setModifiedHistogramData] = useState<number[][]>([]);

  const [filter, setFilter] = useState<FiltersEnum>(FiltersEnum.None);

  const [threshold, setThreshold] = useState<number>(0);
  const [medianRadius, setMedianRadius] = useState<number>(0);
  const [gaussianRadius, setGaussianRadius] = useState<number>(0);
  const [boxBlurRadius, setBoxBlurRadius] = useState<number>(0);
  const [sharpening, setSharpening] = useState<number>(0);

  const [modalOpen, setModalOpen] = useState<boolean>(false);

  const coordinates = useRef<Coordinates[]>([]);
  const colorPicker = useRef<HTMLInputElement | null>(null);
  const canvas = useRef<HTMLCanvasElement | null>(null);

  const { clearFileState, meta, setMeta } = useFileManager();
  const { filterMeta, setFilterMeta } = useFilterManager();
  const { tab, onTabsChange } = useTabManager();
  const { toast, onToastClose, showToast } = useContext(ToastMessageContext);

  useEffect(() => {
    const loadImage = async (): Promise<void> => {
      if (!canvas.current) return;
      const ctx = canvas.current.getContext('2d');

      if (ctx && meta.fetchedFileBase64) {
        const img = new Image();

        img.onload = () => {
          if (canvas.current) {
            canvas.current.width = img.width;
            canvas.current.height = img.height;
          }
          ctx.drawImage(img, 0, 0, img.width, img.height);
        };

        img.src = meta.fetchedFileBase64;
      }
    };

    loadImage();
  }, [meta]);

  useEffect(() => {
    const getHistogram = async (): Promise<void> => {
      if (!hasAutoCorrection || !meta.file || !meta.imageBlob) return;

      if (correctionChannel !== null) {
        const { data } = await PnmService.getHistogram(meta.file, correctionChannel);
        setInitialHistogramData([data]);

        const { data: modifiedData } = await PnmService.getHistogram(meta.imageBlob, correctionChannel);
        setModifiedHistogramData([modifiedData]);
      } else {
        for (const [channel] of Object.entries(RgbChannelsEnum).slice(0, 3)) {
          const { data } = await PnmService.getHistogram(meta.file, channel as unknown as RgbChannelsEnum);
          setInitialHistogramData(prev => [...prev, data]);

          const { data: modifiedData } = await PnmService.getHistogram(meta.imageBlob, channel as unknown as RgbChannelsEnum);
          setModifiedHistogramData(prev => [...prev, modifiedData]);
        }
      }
    };

    getHistogram();
  }, [correctionChannel, hasAutoCorrection, meta.file, meta.imageBlob]);

  const onColorSpacedPreviewImage = useCallback(async (event: ChangeEvent<HTMLInputElement>): Promise<void> => {
    if (!event.target.files || !inputColorSpace || !outputColorSpace) {
      showToast('warning', 'Загрузите файл, либо выберите цветовое пространство');
      return;
    }

    try {
      const [file] = event.target.files;
      const { data } = await PnmService.previewPnmImageWithColorSpace(
        file,
        bitrate,
        dithering,
        inputColorSpace,
        outputColorSpace,
        channel,
        hasAutoCorrection,
        autoCorrectionPercentage,
      );
      const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
      const base64 = await blobToBase64(blob);

      const { data: initialDataImage } = await PnmService.getInitialImage(file);
      const initialBlob = new Blob([new Uint8Array(initialDataImage)], { type: 'image/x-portable-anymap' });
      const initialFileBase64 = await blobToBase64(initialBlob);
      setMeta((prev) => ({ ...prev, imageBlob: blob, fetchedFileBase64: base64, file, initialFileBase64 }));
    } catch (e) {
      showToast('error', errors[(e as any).code] || 'Произошла неизвестная ошибка');
    }
  }, [inputColorSpace, outputColorSpace, outputColorSpace, channel, bitrate, dithering, hasAutoCorrection, autoCorrectionPercentage]);

  const onColorSpacedImageUpload = useCallback(async (event: ChangeEvent<HTMLInputElement>) => {
    if (!event.target.files || !meta.fileNameUploaded || !inputColorSpace || !outputColorSpace) {
      showToast('warning', 'Пожалуйста, напишите название файла и загрузите его');
      return;
    }

    try {
      const [file] = event.target.files;
      const { data } = await PnmService.uploadPnmImageWithColorSpace(file, meta.fileNameUploaded, inputColorSpace, outputColorSpace);
      const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
      const base64 = await blobToBase64(blob);
      setMeta((prev) => ({ ...prev, imageBlob: blob, fetchedFileBase64: base64 }));
    } catch (e) {
      showToast('error', errors[(e as any).code] || 'Произошла неизвестная ошибка');
    }
  }, [meta.fileNameUploaded, inputColorSpace, outputColorSpace]);

  const onGammaAssign = async (): Promise<void> => {
    if (!gamma || !meta.imageBlob) {
      showToast('warning', 'Введите гамму или загрузите изображение');
      return;
    }

    try {
      const { data } = await PnmService.assignGamma(meta.imageBlob, gamma);
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
      const { data } = await PnmService.recalculateGamma(meta.imageBlob, gamma);
      const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
      const base64 = await blobToBase64(blob);
      setMeta((prev) => ({ ...prev, imageBlob: blob, fetchedFileBase64: base64 }));
    } catch (e) {
      showToast('error', errors[(e as any).code] || 'Произвестная ошибка');
    }
  };

  const onCanvasClick = (e: MouseEvent<HTMLCanvasElement>): void => {
    if (!canvas.current || coordinates.current.length === 2) {
      setModalOpen(true);
      return;
    }

    const rect = canvas.current.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    const ctx = canvas.current.getContext('2d');
    if (!ctx) return;
    ctx.fillStyle = 'red';
    ctx.imageSmoothingEnabled = true;
    ctx.imageSmoothingQuality = 'high';
    ctx.fillRect(x, y, 10, 10);

    coordinates.current.push([x, y]);
  };


  const onLineDraw = async (): Promise<void> => {
    if (!coordinates.current.length || !meta.imageBlob) {
      showToast('warning', 'Выберите точки или загрузите изображение');
      return;
    }

    try {
      const { data } = await PnmService.drawLine(
        meta.imageBlob,
        coordinates.current[0],
        coordinates.current[1],
        colorPicker.current?.value || '#000000',
        lineWidth,
        transparency,
      );
      const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
      coordinates.current = [];
      const base64 = await blobToBase64(blob);
      setMeta((prev) => ({ ...prev, imageBlob: blob, fetchedFileBase64: base64 }));
      setModalOpen(false);
    } catch (e) {
      showToast('error', errors[(e as any).code] || 'Произвестная ошибка');
    }
  };

  const handleFilterApply = async (): Promise<void> => {
    if (!meta.file) {
      showToast('warning', 'Загрузите изображение');
      return;
    }

    try {
      switch (+filter) {
        case FiltersEnum.Threshold: {
          const { data } = await FilterService.applyThreshold(meta.file, threshold);
          const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
          const base64 = await blobToBase64(blob);
          setFilterMeta((prev) => ({
            ...prev,
            file: meta.file,
            imageBlob: blob,
            initialFileBase64: meta.initialFileBase64,
            fetchedFileBase64: base64,
          }));
          break;
        }
        case FiltersEnum.OtsuThreshold: {
          const { data } = await FilterService.applyOtsuThreshold(meta.file);
          const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
          const base64 = await blobToBase64(blob);
          setFilterMeta((prev) => ({
            ...prev,
            file: meta.file,
            imageBlob: blob,
            initialFileBase64: meta.initialFileBase64,
            fetchedFileBase64: base64,
          }));
          break;
        }
        case FiltersEnum.Median: {
          const { data } = await FilterService.applyMedianFilter(meta.file, medianRadius);
          const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
          const base64 = await blobToBase64(blob);
          setFilterMeta((prev) => ({
            ...prev,
            file: meta.file,
            imageBlob: blob,
            initialFileBase64: meta.initialFileBase64,
            fetchedFileBase64: base64,
          }));
          break;
        }
        case FiltersEnum.Gaussian: {
          const { data } = await FilterService.applyGaussianFilter(meta.file, gaussianRadius);
          const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
          const base64 = await blobToBase64(blob);
          setFilterMeta((prev) => ({
            ...prev,
            file: meta.file,
            imageBlob: blob,
            initialFileBase64: meta.initialFileBase64,
            fetchedFileBase64: base64,
          }));
          break;
        }
        case FiltersEnum.BoxBlur: {
          const { data } = await FilterService.applyLinearFilter(meta.file, boxBlurRadius);
          const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
          const base64 = await blobToBase64(blob);
          setFilterMeta((prev) => ({
            ...prev,
            file: meta.file,
            imageBlob: blob,
            initialFileBase64: meta.initialFileBase64,
            fetchedFileBase64: base64,
          }));
          break;
        }
        case FiltersEnum.Sobel: {
          const { data } = await FilterService.applySobelFilter(meta.file);
          const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
          const base64 = await blobToBase64(blob);
          setFilterMeta((prev) => ({
            ...prev,
            file: meta.file,
            imageBlob: blob,
            initialFileBase64: meta.initialFileBase64,
            fetchedFileBase64: base64,
          }));
          break;
        }
        case FiltersEnum.ContrastAdaptiveSharpening: {
          const { data } = await FilterService.applyContrast(meta.file, sharpening);
          const blob = new Blob([new Uint8Array(data)], { type: 'image/x-portable-anymap' });
          const base64 = await blobToBase64(blob);
          setFilterMeta((prev) => ({
            ...prev,
            file: meta.file,
            imageBlob: blob,
            initialFileBase64: meta.initialFileBase64,
            fetchedFileBase64: base64,
          }));
          break;
        }
        case FiltersEnum.None:
        default:
          break;
      }
    } catch (e) {
      showToast('error', errors[(e as any).code] || 'Произвестная ошибка');
    }
  };

  return (
    <Container maxWidth={'xl'}>
      <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
        <Tabs value={tab} onChange={onTabsChange}>
          <Tab label="Просмотр PNM изображений"/>
          <Tab label="Сохранение PNM изображений"/>
        </Tabs>
      </Box>

      <TabPanel value={tab} index={0}>
        <Stack spacing={2} direction={'column'}>
          <Button variant="contained" component="label">
            Загрузить файл
            <input hidden accept="image/x-portable-anymap" type="file" onChange={onColorSpacedPreviewImage}/>
          </Button>

          <>
            <ColorSpacePicker variant={'input'}
              handleChange={(_, value) => setInputColorSpace(value as unknown as ColorSpaceEnum)}/>
            <ColorSpacePicker variant={'output'}
              handleChange={(_, value) => setOutputColorSpace(value as unknown as ColorSpaceEnum)}/>

            {
              outputColorSpace && (
                <ColorSpaceTumbler colorSpace={outputColorSpace}
                  onChange={(_, value) => setChannel(value as unknown as ColorChannelsEnum)}/>
              )
            }

            {
              +(outputColorSpace ?? 0) === ColorSpaceEnum.RGB &&
              <>
                <TextField label="Введите гамму" variant="standard" inputProps={{ type: 'number' }}
                  value={gamma} onChange={(e) => setGamma(e.target.value)}/>
                <Stack direction="row" spacing={2}>
                  <Button variant="outlined" onClick={onGammaAssign}>Назначить</Button>
                  <Button variant="outlined" onClick={onGammaRecalculate}>Пересчитать</Button>
                </Stack>
              </>
            }

            {
              meta.fetchedFileBase64 && (
                <Box>
                  <canvas ref={canvas} onClick={onCanvasClick} />
                </Box>
              )
            }
          </>

          <>
            <Box sx={{ mt: 4 }}>
              <Typography variant={'h5'} sx={{ mb: 2 }}>
                Дизеринг
              </Typography>
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
            </Box>

            <FormLabel>Выбор битности</FormLabel>
            <Slider defaultValue={8}
              onChange={(_, value) => setBitrate(value as number)}
              aria-label="Default"
              valueLabelDisplay="auto"
              min={1}
              max={8}/>
          </>

          <>
            <Box sx={{ mt: '2rem' }}>
              <Typography variant={'h5'} sx={{ mb: 2 }}>
                Автокоррекция
              </Typography>

              <FormGroup>
                <FormControlLabel
                  control={<Checkbox onChange={(_, v) => setHasAutoCorrection(v)} checked={hasAutoCorrection}/>}
                  label="Применять автокоррекцию"/>
                {
                  hasAutoCorrection && <Stack spacing={2} direction={'column'}>
                    <Slider defaultValue={0.5}
                      aria-label="Default"
                      valueLabelDisplay="auto"
                      min={0}
                      step={0.01}
                      onChange={(e: any) => setAutoCorrectionPercentage(e.target?.value as number)}
                      max={0.5}/>
                    <RadioGroup row aria-labelledby="demo-row-radio-buttons-group-label"
                      name="row-radio-buttons-group"
                      onChange={(_, e) => setCorrectionChannel(e as unknown as RgbChannelsEnum)}>
                      <FormControlLabel value={RgbChannelsEnum.R} control={<Radio/>} label="R"/>
                      <FormControlLabel value={RgbChannelsEnum.G} control={<Radio/>} label="G"/>
                      <FormControlLabel value={RgbChannelsEnum.B} control={<Radio/>} label="B"/>
                    </RadioGroup>
                  </Stack>
                }
              </FormGroup>
              {
                meta.fetchedFileBase64 && meta.initialFileBase64 && initialHistogramData.length > 0 && modifiedHistogramData.length > 0 &&
                <Box sx={{ mt: '2rem' }}>
                  <h2>Исходное изображение</h2>
                  <img src={meta.initialFileBase64} alt="preview"/>
                  {
                    initialHistogramData.map((data, i) => {
                      const dataset = {
                        labels: data.map((_, i) => i),
                        datasets: [
                          {
                            label: correctionChannel || (i === 0 ? 'R' : i === 1 ? 'G' : 'B'),
                            data: data,
                            backgroundColor: 'rgba(99,120,255,0.5)',
                          },
                        ],
                      };

                      // @ts-ignore
                      return <Bar key={i} id={`chart-${i}`} data={dataset}/>;
                    })
                  }

                  <h2>Модифицированное изображение</h2>
                  <img src={meta.fetchedFileBase64} alt="preview"/>
                  {
                    modifiedHistogramData.map((data, i) => {
                      const dataset = {
                        labels: data.map((_, i) => i),
                        datasets: [
                          {
                            label: correctionChannel || (i === 0 ? 'R' : i === 1 ? 'G' : 'B'),
                            data: data,
                            backgroundColor: 'rgba(99,120,255,0.5)',
                          },
                        ],
                      };

                      // @ts-ignore
                      return <Bar key={i} id={`chart-${i}`} data={dataset}/>;
                    })
                  }
                </Box>
              }
              {
                meta.fetchedFileBase64 && colorPicker.current &&
                <Box>
                  <Button variant="outlined" href={meta.fetchedFileBase64} onClick={clearFileState}
                    download={meta.fileNameUploaded}>
                    Скачать картинку
                  </Button>
                </Box>
              }
            </Box>
          </>

          <>
            <Box sx={{ mt: '2rem' }}>
              <Typography id="modal-modal-title" variant="h5">
                Применение фильтров
              </Typography>
              <RadioGroup row name="filters-group" defaultValue={FiltersEnum.None}
                onChange={(_, value) => setFilter(value as unknown as FiltersEnum)}>
                <FormControlLabel value={FiltersEnum.None}
                  control={<Radio/>}
                  label={'None'}/>
                <FormControlLabel value={FiltersEnum.Threshold}
                  control={<Radio/>}
                  label={'Пороговая фильтрация'}/>
                <FormControlLabel value={FiltersEnum.OtsuThreshold}
                  control={<Radio/>}
                  label={'Оцу'}/>
                <FormControlLabel value={FiltersEnum.Median}
                  control={<Radio/>}
                  label={'Медианный фильтр'}/>
                <FormControlLabel value={FiltersEnum.Gaussian}
                  control={<Radio/>}
                  label={'Фильтр Гаусса'}/>
                <FormControlLabel value={FiltersEnum.BoxBlur}
                  control={<Radio/>}
                  label={'Линейный усредняющий фильтр'}/>
                <FormControlLabel value={FiltersEnum.Sobel}
                  control={<Radio/>}
                  label={'Фильтр Собеля'}/>
                <FormControlLabel value={FiltersEnum.ContrastAdaptiveSharpening}
                  control={<Radio/>}
                  label={'Contrast Adaptive Sharpening'}/>
              </RadioGroup>
              <>
                <>
                  {
                    +filter === FiltersEnum.Threshold &&
                    <Box sx={{ mt: 2 }}>
                      <FormLabel>Порог</FormLabel>
                      <Slider defaultValue={0}
                        aria-label="Default"
                        onChange={(_, value) => setThreshold(value as number)}
                        valueLabelDisplay="auto"
                        min={0}
                        max={255}/>
                      <Button onClick={handleFilterApply}>Применить фильтр</Button>
                    </Box>
                  }
                  {
                    +filter === FiltersEnum.Threshold && filterMeta.initialFileBase64 && filterMeta.fetchedFileBase64 && (
                      <Box>
                        <h2>Исходное изображение</h2>
                        <img src={filterMeta.initialFileBase64} alt="inital image"/>
                        <h2>Изображение с фильтром</h2>
                        <img src={filterMeta.fetchedFileBase64} alt={'image'}/>
                      </Box>
                    )
                  }
                </>

                <>
                  {
                    +filter === FiltersEnum.OtsuThreshold &&
                    <Box sx={{ mt: 2 }}>
                      <Button onClick={handleFilterApply}>Применить фильтр</Button>
                    </Box>

                  }
                  {
                    +filter === FiltersEnum.OtsuThreshold && filterMeta.initialFileBase64 && filterMeta.fetchedFileBase64 && (
                      <Box>
                        <h2>Исходное изображение</h2>
                        <img src={filterMeta.initialFileBase64} alt="inital image"/>
                        <h2>Изображение с фильтром</h2>
                        <img src={filterMeta.fetchedFileBase64} alt={'image'}/>
                      </Box>
                    )
                  }
                </>

                <>
                  {
                    +filter === FiltersEnum.Median &&
                    <Box sx={{ mt: 2 }}>
                      <TextField label="Радиус ядра" variant="standard" inputProps={{ type: 'number' }}
                        value={gamma} onChange={(e) => setMedianRadius(e.target.value as unknown as number)}/>
                      <Button onClick={handleFilterApply}>Применить фильтр</Button>
                    </Box>
                  }
                  {
                    +filter === FiltersEnum.Median && filterMeta.initialFileBase64 && filterMeta.fetchedFileBase64 && (
                      <Box>
                        <h2>Исходное изображение</h2>
                        <img src={filterMeta.initialFileBase64} alt="inital image"/>
                        <h2>Изображение с фильтром</h2>
                        <img src={filterMeta.fetchedFileBase64} alt={'image'}/>
                      </Box>
                    )
                  }
                </>

                <>
                  {
                    +filter === FiltersEnum.Gaussian &&
                    <Box sx={{ mt: 2 }}>
                      <TextField label="Сигма" variant="standard" inputProps={{ type: 'number' }}
                        value={gaussianRadius} onChange={(e) => setGaussianRadius(e.target.value as unknown as number)}/>
                      <Button onClick={handleFilterApply}>Применить фильтр</Button>
                    </Box>
                  }
                  {
                    +filter === FiltersEnum.Gaussian && filterMeta.initialFileBase64 && filterMeta.fetchedFileBase64 && (
                      <Box>
                        <h2>Исходное изображение</h2>
                        <img src={filterMeta.initialFileBase64} alt="inital image"/>
                        <h2>Изображение с фильтром</h2>
                        <img src={filterMeta.fetchedFileBase64} alt={'image'}/>
                      </Box>
                    )
                  }
                </>

                <>
                  {
                    +filter === FiltersEnum.BoxBlur &&
                    <Box sx={{ mt: 2 }}>
                      <TextField label="Радиус ядра" variant="standard" inputProps={{ type: 'number' }}
                        value={boxBlurRadius} onChange={(e) => setBoxBlurRadius(e.target.value as unknown as number)}/>
                      <Button onClick={handleFilterApply}>Применить фильтр</Button>
                    </Box>
                  }
                  {
                    +filter === FiltersEnum.BoxBlur && filterMeta.initialFileBase64 && filterMeta.fetchedFileBase64 && (
                      <Box>
                        <h2>Исходное изображение</h2>
                        <img src={filterMeta.initialFileBase64} alt="inital image"/>
                        <h2>Изображение с фильтром</h2>
                        <img src={filterMeta.fetchedFileBase64} alt={'image'}/>
                      </Box>
                    )
                  }
                </>

                <>
                  {
                    +filter === FiltersEnum.Sobel &&
                    <Box sx={{ mt: 2 }}>
                      <Button onClick={handleFilterApply}>Применить фильтр</Button>
                    </Box>

                  }
                  {
                    +filter === FiltersEnum.Sobel && filterMeta.initialFileBase64 && filterMeta.fetchedFileBase64 && (
                      <Box>
                        <h2>Исходное изображение</h2>
                        <img src={filterMeta.initialFileBase64} alt="inital image"/>
                        <h2>Изображение с фильтром</h2>
                        <img src={filterMeta.fetchedFileBase64} alt={'image'}/>
                      </Box>
                    )
                  }
                </>

                <>
                  {
                    +filter === FiltersEnum.ContrastAdaptiveSharpening &&
                    <Box sx={{ mt: 2 }}>
                      <FormLabel>Contrast Adaptive Sharpening</FormLabel>
                      <Slider defaultValue={0}
                        step={0.01}
                        aria-label="Default"
                        onChange={(_, value) => setSharpening(value as number)}
                        valueLabelDisplay="auto"
                        min={0}
                        max={1}/>
                      <Button onClick={handleFilterApply}>Применить фильтр</Button>
                    </Box>
                  }
                  {
                    +filter === FiltersEnum.ContrastAdaptiveSharpening && filterMeta.initialFileBase64 && filterMeta.fetchedFileBase64 && (
                      <Box>
                        <h2>Исходное изображение</h2>
                        <img src={filterMeta.initialFileBase64} alt="inital image"/>
                        <h2>Изображение с фильтром</h2>
                        <img src={filterMeta.fetchedFileBase64} alt={'image'}/>
                      </Box>
                    )
                  }
                </>
              </>
            </Box>
          </>
        </Stack>
      </TabPanel>

      <TabPanel value={tab} index={1}>
        <Stack spacing={2} direction="column" alignItems={'baseline'}>
          <TextField label="Название файла" required variant="standard" value={meta.fileNameUploaded ?? ''}
            sx={{ width: '30%' }}
            onChange={(e) => setMeta(prev => ({ ...prev, fileNameUploaded: e.target.value }))}/>
          <ColorSpacePicker variant={'input'}
            handleChange={(_, value) => setInputColorSpace(value as unknown as ColorSpaceEnum)}/>
          <ColorSpacePicker variant={'output'}
            handleChange={(_, value) => setOutputColorSpace(value as unknown as ColorSpaceEnum)}/>
          {
            !meta.fetchedFileBase64 &&
            <Button variant="contained" component="label">
              Загрузить файл
              <input hidden accept="image/x-portable-anymap" type="file" onChange={onColorSpacedImageUpload}/>
            </Button>
          }
          {
            meta.fetchedFileBase64 &&
            <Button variant="outlined" href={meta.fetchedFileBase64} onClick={clearFileState}
              download={meta.fileNameUploaded}>
              Скачать картинку
            </Button>
          }
        </Stack>
      </TabPanel>

      <Snackbar open={toast.open}
        autoHideDuration={3000}
        onClose={onToastClose}
        anchorOrigin={{ vertical: toast.vertical, horizontal: toast.horizontal }}>
        <Alert onClose={onToastClose} severity={toast.variant ?? 'error'} sx={{ width: '100%' }}>{toast.message}</Alert>
      </Snackbar>

      <>
        {
          coordinates.current.length === 2 && <Modal
            open={modalOpen}
            onClose={() => setModalOpen(false)}
            aria-labelledby="modal-modal-title"
            aria-describedby="modal-modal-description">
            <Box sx={modalStyle}>
              <Typography id="modal-modal-title" variant="h6" component="h2">
                Отрисовка растровой линии
              </Typography>
              <Typography id="modal-modal-description" sx={{ mt: 2 }}>
                Нажмите на кнопку ниже, чтобы получить новую картинку
                из координат &#91;{[...coordinates.current[0]].join(', ')}&#93;
                и &#91;{[...coordinates.current[1]].join(', ')}&#93;
              </Typography>
              <Box sx={{ margin: '1rem 0' }}>
                <Typography variant="overline" sx={{ marginRight: '1rem' }}>Цвет линии</Typography>
                <input type={'color'} ref={colorPicker}/>
              </Box>
              <Box sx={{ margin: '1rem 0' }}>
                <TextField label="Ширина линии"
                  variant="standard"
                  inputProps={{ type: 'number' }}
                  onChange={(e: any) => setLineWidth(e.target.value as number)}/>
              </Box>
              <Box sx={{ margin: '1rem 0' }}>
                <Typography variant="body1">Прозрачность</Typography>
                <Slider defaultValue={50}
                  aria-label="Default"
                  valueLabelDisplay="auto"
                  min={0}
                  onChange={(e: any) => setTransparency(e.target?.value as number)}
                  max={100}/>
              </Box>
              <Button variant="outlined" sx={{ mt: '1rem' }} onClick={onLineDraw}>Отрисовать</Button>
            </Box>
          </Modal>
        }
      </>

    </Container>
  );
};
