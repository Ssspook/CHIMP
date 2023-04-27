import { CmyChannelsEnum, HslChannelsEnum, HsvChannelsEnum, RgbChannelsEnum, YCbCr601ChannelsEnum, YCbCr709ChannelsEnum, YCoCgChannelsEnum } from '@/utils/enums/color-channels.enum';
import { ColorSpaceEnum } from '@/utils/enums/color-spaces.enum';
import { FormControlLabel, Radio, RadioGroup } from '@mui/material';
import { ChangeEvent, FC } from 'react';

type ColorSpaceTumblerProps = {
  colorSpace: ColorSpaceEnum;
  onChange:(event: ChangeEvent<HTMLInputElement>, value: string) => void;
};

export const ColorSpaceTumbler: FC<ColorSpaceTumblerProps> = (props) => {
  const { colorSpace, onChange } = props;

  return (
    <>
      {
        +colorSpace === ColorSpaceEnum.RGB && (
          <RadioGroup row aria-labelledby="demo-row-radio-buttons-group-label"
            name="row-radio-buttons-group" onChange={onChange}>
            <FormControlLabel value={RgbChannelsEnum.R} control={<Radio />} label="R" />
            <FormControlLabel value={RgbChannelsEnum.G} control={<Radio />} label="G" />
            <FormControlLabel value={RgbChannelsEnum.B} control={<Radio />} label="B" />
          </RadioGroup>
        )
      }
      {
        +colorSpace === ColorSpaceEnum.HSV && (
          <RadioGroup row aria-labelledby="demo-row-radio-buttons-group-label"
            name="row-radio-buttons-group" onChange={onChange}>
            <FormControlLabel value={HslChannelsEnum.H} control={<Radio />} label="H" />
            <FormControlLabel value={HslChannelsEnum.S} control={<Radio />} label="S" />
            <FormControlLabel value={HslChannelsEnum.L} control={<Radio />} label="V" />
          </RadioGroup>
        )
      }
      {
        +colorSpace === ColorSpaceEnum.HSL && (
          <RadioGroup row aria-labelledby="demo-row-radio-buttons-group-label"
            name="row-radio-buttons-group" onChange={onChange}>
            <FormControlLabel value={HsvChannelsEnum.H} control={<Radio />} label="H" />
            <FormControlLabel value={HsvChannelsEnum.S} control={<Radio />} label="S" />
            <FormControlLabel value={HsvChannelsEnum.V} control={<Radio />} label="L" />
          </RadioGroup>
        )
      }
      {
        +colorSpace === ColorSpaceEnum.YCbCr601 && (
          <RadioGroup row aria-labelledby="demo-row-radio-buttons-group-label"
            name="row-radio-buttons-group" onChange={onChange}>
            <FormControlLabel value={YCbCr601ChannelsEnum.Y} control={<Radio />} label="Y" />
            <FormControlLabel value={YCbCr601ChannelsEnum.Cb} control={<Radio />} label="Cb" />
            <FormControlLabel value={YCbCr601ChannelsEnum.Cr} control={<Radio />} label="Cr" />
          </RadioGroup>
        )
      }
      {
        +colorSpace === ColorSpaceEnum.YCbCr709 && (
          <RadioGroup row aria-labelledby="demo-row-radio-buttons-group-label"
            name="row-radio-buttons-group" onChange={onChange}>
            <FormControlLabel value={YCbCr709ChannelsEnum.Y} control={<Radio />} label="Y" />
            <FormControlLabel value={YCbCr709ChannelsEnum.Cb} control={<Radio />} label="Cb" />
            <FormControlLabel value={YCbCr709ChannelsEnum.Cr} control={<Radio />} label="Cr" />
          </RadioGroup>
        )
      }
      {
        +colorSpace === ColorSpaceEnum.YCoCg && (
          <RadioGroup row aria-labelledby="demo-row-radio-buttons-group-label"
            name="row-radio-buttons-group" onChange={onChange}>
            <FormControlLabel value={YCoCgChannelsEnum.Y} control={<Radio />} label="Y" />
            <FormControlLabel value={YCoCgChannelsEnum.Co} control={<Radio />} label="Co" />
            <FormControlLabel value={YCoCgChannelsEnum.Cg} control={<Radio />} label="Cg" />
          </RadioGroup>
        )
      }
      {
        +colorSpace === ColorSpaceEnum.CMY && (
          <RadioGroup row aria-labelledby="demo-row-radio-buttons-group-label"
            name="row-radio-buttons-group" onChange={onChange}>
            <FormControlLabel value={CmyChannelsEnum.C} control={<Radio />} label="C" />
            <FormControlLabel value={CmyChannelsEnum.M} control={<Radio />} label="M" />
            <FormControlLabel value={CmyChannelsEnum.Y} control={<Radio />} label="Y" />
          </RadioGroup>
        )
      }
    </>
  );
};
