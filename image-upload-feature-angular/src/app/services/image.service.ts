import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';

export interface GalleryImage {
  id?: number;
  tempId?: number;
  base64Data: string;
  fileName: string;
  contentType: string;
  isNew?: boolean;
  createdAt?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ImageService {
  private API_BASE_URL = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getImages(entityType: string, entityId: number): Observable<GalleryImage[]> {
    return this.http.get<any>(`${this.API_BASE_URL}/${entityType}/${entityId}/images`).pipe(
      map(res => {
        if (res.success && res.data && res.data.images) {
          return res.data.images.map((img: any) => ({
            id: img.id,
            base64Data: img.base64Data,
            fileName: img.fileName,
            contentType: img.contentType,
            createdAt: img.uploadedAt
          }));
        }
        return [];
      })
    );
  }

  uploadImages(entityType: string, entityId: number, images: GalleryImage[]): Observable<any> {
    return this.http.post<any>(`${this.API_BASE_URL}/${entityType}/${entityId}/images/bulk`, {
      images: images.map(img => ({
        base64Data: img.base64Data,
        fileName: img.fileName,
        contentType: img.contentType
      }))
    });
  }

  deleteImage(entityType: string, entityId: number, imageId: number): Observable<any> {
    return this.http.delete<any>(`${this.API_BASE_URL}/${entityType}/${entityId}/images/${imageId}`);
  }
}