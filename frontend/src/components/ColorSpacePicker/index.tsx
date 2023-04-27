import { ColorSpaceEnum } from '@/utils/enums/color-spaces.enum';
import { FormControl, FormControlLabel, FormLabel, Radio, RadioGroup } from '@mui/material';
import { ChangeEvent, FC } from 'react';

type ColorSpacePickerProps = {
  handleChange: (event: ChangeEvent<HTMLInputElement>, value: string) => void;
  variant: 'input' | 'output';
};

export const ColorSpacePicker: FC<ColorSpacePickerProps> = (props) => {
  const { handleChange, variant = 'input' } = props;

  return (
    <FormControl>
      <FormLabel id="demo-row-radio-buttons-group-label">Цветовое пространство {
        variant === 'input' ? 'входного изображения' : 'выходного изображения'
      }</FormLabel>
      <RadioGroup row aria-labelledby="demo-row-radio-buttons-group-label"
        name="row-radio-buttons-group" onChange={handleChange}>
        <FormControlLabel value={ColorSpaceEnum.RGB} control={<Radio />} label="RGB" />
        <FormControlLabel value={ColorSpaceEnum.HSL} control={<Radio />} label="HSL" />
        <FormControlLabel value={ColorSpaceEnum.HSV} control={<Radio />} label="HSV" />
        <FormControlLabel value={ColorSpaceEnum.YCbCr601} control={<Radio />} label="YCbCr.601" />
        <FormControlLabel value={ColorSpaceEnum.YCbCr709} control={<Radio />} label="YCbCr.709" />
        <FormControlLabel value={ColorSpaceEnum.YCoCg} control={<Radio />} label="YCoCg" />
        <FormControlLabel value={ColorSpaceEnum.CMY} control={<Radio />} label="CMY" />
      </RadioGroup>
    </FormControl>
  );
};
