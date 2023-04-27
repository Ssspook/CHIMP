export enum RgbChannelsEnum {
  R = 1,
  G,
  B,
}

export enum HslChannelsEnum {
  H = 1,
  S,
  L,
}

export enum HsvChannelsEnum {
  H = 1,
  S,
  V,
}

export enum YCbCr601ChannelsEnum {
  Y = 1,
  Cb,
  Cr,
}

export enum YCbCr709ChannelsEnum {
  Y = 1,
  Cb,
  Cr,
}

export enum YCoCgChannelsEnum {
  Y = 1,
  Co,
  Cg,
}

export enum CmyChannelsEnum {
  C = 1,
  M,
  Y,
}

export type ColorChannelsEnum = RgbChannelsEnum | HslChannelsEnum | HsvChannelsEnum | YCbCr601ChannelsEnum | YCbCr709ChannelsEnum | YCoCgChannelsEnum | CmyChannelsEnum;
