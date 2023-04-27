import { AxiosResponse } from 'axios';
import { api } from '@/api/config';

export class FilterService {
  static async applyThreshold(
    image: File,
    threshold: number,
  ): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('filters/threshold', formData, {
      params: {
        threshold,
      },
      headers: {
        'Content-Type': 'multipart/form-data',
        'Accept': 'application/octet-stream',
      },
      responseType: 'arraybuffer',
    });
  }

  static async applyOtsuThreshold(
    image: File,
  ): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('filters/otsu', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        'Accept': 'application/octet-stream',
      },
      responseType: 'arraybuffer',
    });
  }

  static async applyMedianFilter(
    image: File,
    radius: number,
  ): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('filters/median', formData, {
      params: {
        radius,
      },
      headers: {
        'Content-Type': 'multipart/form-data',
        'Accept': 'application/octet-stream',
      },
      responseType: 'arraybuffer',
    });
  }

  static async applyGaussianFilter(
    image: File,
    sigma: number,
  ): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('filters/gaussian', formData, {
      params: {
        sigma,
      },
      headers: {
        'Content-Type': 'multipart/form-data',
        'Accept': 'application/octet-stream',
      },
      responseType: 'arraybuffer',
    });
  }

  static async applyLinearFilter(
    image: File,
    radius: number,
  ): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('filters/linear', formData, {
      params: {
        radius,
      },
      headers: {
        'Content-Type': 'multipart/form-data',
        'Accept': 'application/octet-stream',
      },
      responseType: 'arraybuffer',
    });
  }

  static async applySobelFilter(
    image: File,
  ): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('filters/sobel', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        'Accept': 'application/octet-stream',
      },
      responseType: 'arraybuffer',
    });
  }

  static async applyContrast(
    image: File,
    sharpening: number,
  ): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('filters/contrast', formData, {
      params: {
        sharpening,
      },
      headers: {
        'Content-Type': 'multipart/form-data',
        'Accept': 'application/octet-stream',
      },
      responseType: 'arraybuffer',
    });
  }
}
