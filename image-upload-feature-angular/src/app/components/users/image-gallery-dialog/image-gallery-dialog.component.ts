import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ImageGalleryComponent } from '../image-gallery/image-gallery.component';

@Component({
  selector: 'app-image-gallery-dialog',
  template: `<div class="dialog-container">
      <!-- Close Button -->
      <button class="close-btn" (click)="dialogRef.close()" aria-label="Close">
        <!-- Custom X SVG -->
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <line x1="18" y1="6" x2="6" y2="18"></line>
          <line x1="6" y1="6" x2="18" y2="18"></line>
        </svg>
      </button>

      <!-- Image Gallery Component -->
      <app-image-gallery [entityType]="data.entityType" [userName]="data.name" [entityId]="data.entityId"></app-image-gallery>
    </div>`,
  standalone: true,
    styles: [`
    .dialog-container {
      position: relative;
      display: inline-block; /* Make width fit content */
      padding: 16px;
      background: white;
      border-radius: 8px;
    }

    .close-btn {
      position: absolute;
      top: 8px;
      right: 8px;
      background: transparent;
      border: none;
      cursor: pointer;
      padding: 4px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #333;
    }

    .close-btn svg {
      width: 24px;
      height: 24px;
    }
  `],
  imports: [ImageGalleryComponent]
})
export class ImageGalleryDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<ImageGalleryDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { entityType: string, entityId: number, name: string }
  ) {}
}