import { AxiosResponse } from 'axios';
import { api } from '@/api/config';

export class PngService {
  static async uploadPngImage(
    image: File,
  ): Promise<AxiosResponse> {
    const formData = new FormData();
    formData.append('image', image);

    return api.post('png/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
      responseType: 'arraybuffer',
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
}
