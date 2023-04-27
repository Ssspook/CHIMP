import { ColorSpaceEnum } from '@/utils/enums/color-spaces.enum';
import { AxiosResponse } from 'axios';
import { api } from '../config';
import { ColorChannelsEnum, RgbChannelsEnum } from '@/utils/enums/color-channels.enum';
import { Coordinates } from 'app/common';
import { DitheringAlgo } from '@/utils/enums/dithering-algo.enum';

export class PnmService {
  static async getInitialImage(image: File): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('images/initial', formData, {
      responseType: 'arraybuffer',
      headers: {
        'Content-Type': 'multipart/form-data',
        'Accept': 'application/octet-stream',
      },
    });
  }

  static async getHistogram(image: Blob, channel: RgbChannelsEnum): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('histogram/channel', formData, {
      params: {
        channel,
      },
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  }

  static async previewPnmImageWithColorSpace(
    image: File,
    bitrate: number,
    ditheringAlgorithm: DitheringAlgo,
    inputConvertStrategy: ColorSpaceEnum,
    outputConvertStrategy: ColorSpaceEnum,
    channel: ColorChannelsEnum | null,
    hasAutoCorrection: boolean,
    correctionPercentage: number,
  ): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('images/read/space', formData, {
      responseType: 'arraybuffer',
      params: {
        inputConvertStrategy,
        outputConvertStrategy,
        bitrate,
        ditheringAlgorithm,
        ...channel && { channel: +channel },
        hasAutoCorrection,
        ...(hasAutoCorrection && { correctionPercentage }),
      },
      headers: {
        'Content-Type': 'multipart/form-data',
        'Accept': 'application/octet-stream',
      },
    });
  }

  static async uploadPnmImageWithColorSpace(
    image: File,
    fileName: string,
    inputConvertStrategy: ColorSpaceEnum,
    outputConvertStrategy: ColorSpaceEnum,
  ): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('images/upload/space', formData, {
      responseType: 'arraybuffer',
      params: {
        fileName,
        inputConvertStrategy,
        outputConvertStrategy,
      },
      headers: {
        'Content-Type': 'multipart/form-data',
        'Accept': 'application/octet-stream',
      },
    });
  }

  static async assignGamma(image: Blob, gamma: string): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('images/assign_gamma', formData, {
      responseType: 'arraybuffer',
      params: {
        gamma,
      },
      headers: {
        'Content-Type': 'multipart/form-data',
        'Accept': 'application/octet-stream',
      },
    });
  }

  static async recalculateGamma(image: Blob, gamma: string): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('images/recalculate_gamma', formData, {
      responseType: 'arraybuffer',
      params: {
        gamma,
      },
      headers: {
        'Content-Type': 'multipart/form-data',
        'Accept': 'application/octet-stream',
      },
    });
  }

  static async drawLine(
    image: Blob,
    startPoint: Coordinates,
    endPoint: Coordinates,
    color: string,
    lineWidth: number,
    transparency: number,
  ): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('images/draw_line', formData, {
      responseType: 'arraybuffer',
      params: {
        x1: Math.round(startPoint[0]),
        y1: Math.round(startPoint[1]),
        x2: Math.round(endPoint[0]),
        y2: Math.round(endPoint[1]),
        color,
        lineWidth,
        transparency,
      },
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  }

  static async applyGradient(): Promise<AxiosResponse> {
    return api.post('images/apply_gradient', {}, {
      responseType: 'arraybuffer',
    });
  }

  static async previewGradientImage(
    image: File,
    bitrate: number,
    ditheringAlgorithm: DitheringAlgo,
  ): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('images/read_gradient', formData, {
      responseType: 'arraybuffer',
      params: {
        bitrate,
        ditheringAlgorithm,
      },
      headers: {
        'Content-Type': 'multipart/form-data',
        'Accept': 'application/octet-stream',
      },
    });
  }
}
