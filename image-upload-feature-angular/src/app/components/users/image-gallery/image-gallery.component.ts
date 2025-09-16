import { Component, Input, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ImageService } from '../../../services/image.service';
import { firstValueFrom } from 'rxjs';
import { LightboxModule } from 'ngx-lightbox';

@Component({
  selector: 'app-image-gallery',
  standalone: true,
  imports: [CommonModule, FormsModule, LightboxModule],
  templateUrl: './image-gallery.component.html',
  styleUrls: ['./image-gallery.component.scss']
})
export class ImageGalleryComponent implements OnInit {
  @Input() userName!: string; 
  @Input() entityType = 'customers';
  @Input() entityId: number = 1;
  @Input() maxImages = 10;

  images: any[] = [];
  pendingImages: any[] = [];
  isLoading = true;
  isUploading = false;
  selectedImageIndex: number | null = null;
  error = '';
  successMessage = '';
  dragOver = false;
  deletingImageIds = new Set<number>();

  @ViewChild('fileInput') fileInputRef!: ElementRef<HTMLInputElement>;

  constructor(private imageService: ImageService) {}

  ngOnInit(): void {
    this.loadImages(true);
  }

  get allImages(): any[] {
    return [...this.images, ...this.pendingImages];
  }

  get selectedImage() {
    return this.selectedImageIndex !== null ? this.allImages[this.selectedImageIndex] : null;
  }

  async loadImages(showLoading: boolean) {
    if (showLoading) this.isLoading = true;
    try {
      this.images = await firstValueFrom(this.imageService.getImages(this.entityType, this.entityId));
      this.error = '';
    } catch (err) {
      this.error = 'Failed to load images';
      console.error(err);
    } finally {
      this.isLoading = false;
    }
  }

  handleFileSelect(files: FileList | null) {
    if (!files) return;
    const fileArray = Array.from(files);
    const remainingSlots = this.maxImages - this.allImages.length;

    if (fileArray.length > remainingSlots) {
      const filesToProcess = fileArray.slice(0, remainingSlots);
      const filesExceedingLimit = fileArray.slice(remainingSlots);
      const fileNames = filesExceedingLimit.map(f => f.name).join(', ');
      this.error = `The following files could not be uploaded because the limit of ${this.maxImages} images would be exceeded: ${fileNames}`;
      if (filesToProcess.length > 0) {
        setTimeout(() => {
        this.error = ''
          this.processFiles(filesToProcess);
        }, 3000);
      }
    } else {
      this.processFiles(fileArray);
      this.error = '';
    }
  }

  async processFiles(files: File[]) {
    try {
      const imagePromises = files.map(file => new Promise<any>((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve({
          tempId: Date.now() + Math.random(),
          base64Data: reader.result,
          fileName: file.name,
          contentType: file.type,
          isNew: true
        });
        reader.onerror = reject;
        reader.readAsDataURL(file);
      }));
      const imageData = await Promise.all(imagePromises);
      this.pendingImages.push(...imageData);
      this.uploadNewImages(imageData);
    } catch (err) {
      console.error(err);
      this.error = 'Failed to process images';
    }
  }

  async uploadNewImages(newImages: any[]) {
    this.isUploading = true;
    this.successMessage = '';
    try {
      const result = await firstValueFrom(this.imageService.uploadImages(this.entityType, this.entityId, newImages));
      if (result.success) {
        await this.loadImages(false);
        const uploadedTempIds = newImages.map(img => img.tempId);
        this.pendingImages = this.pendingImages.filter(img => !uploadedTempIds.includes(img.tempId));
        this.successMessage = `Successfully uploaded ${newImages.length} image${newImages.length > 1 ? 's' : ''}`;
        setTimeout(() => (this.successMessage = ''), 3000);
      } else {
        this.error = result.message || 'Upload failed';
      }
    } catch (err) {
      console.error(err);
      this.error = 'Failed to upload images';
    } finally {
      this.isUploading = false;
    }
  }

  async deleteImage(image: any) {
    const isPending = image.isNew && !image.id;
    if (isPending) {
      this.pendingImages = this.pendingImages.filter(img => img.tempId !== image.tempId);
      return;
    }
    this.deletingImageIds.add(image.id);
    this.error = '';
    this.successMessage = '';
    try {
      const success = await this.imageService.deleteImage(this.entityType, this.entityId, image.id);
      if (success) {
        this.images = this.images.filter(img => img.id !== image.id);
        this.selectedImageIndex = null;
        this.successMessage = 'Image successfully deleted';
        setTimeout(() => (this.successMessage = ''), 3000);
      } else this.error = 'Failed to delete image';
    } catch (err) {
      console.error(err);
      this.error = 'Failed to delete image';
    } finally {
      this.deletingImageIds.delete(image.id);
    }
  }

  handleDragOver(event: DragEvent) {
    event.preventDefault();
    this.dragOver = true;
  }

  handleDragLeave(event: DragEvent) {
    event.preventDefault();
    this.dragOver = false;
  }

  handleDrop(event: DragEvent) {
    event.preventDefault();
    this.dragOver = false;
    this.handleFileSelect(event.dataTransfer?.files || null);
  }

  openFileDialog() {
    this.fileInputRef.nativeElement.click();
  }

  nextImage() {
    if (this.selectedImageIndex !== null && this.selectedImageIndex < this.allImages.length - 1) {
      this.selectedImageIndex++;
    }
  }

  prevImage() {
    if (this.selectedImageIndex !== null && this.selectedImageIndex > 0) {
      this.selectedImageIndex--;
    }
  }

  openGallery(index: number) {
    this.selectedImageIndex = index;
  }

  closeGallery() {
    this.selectedImageIndex = null;
  }

  // Optional: handle ESC key to close gallery
  @HostListener('document:keydown.escape', ['$event'])
  handleEscape(event: KeyboardEvent) {
    if (this.selectedImageIndex !== null) {
      this.closeGallery();
    }
  }

  @HostListener('document:keydown.arrowright', ['$event'])
  handleArrowRight(event: KeyboardEvent) {
    if (this.selectedImageIndex !== null) {
      this.nextImage();
    }
  }

  @HostListener('document:keydown.arrowleft', ['$event'])
  handleArrowLeft(event: KeyboardEvent) {
    if (this.selectedImageIndex !== null) {
      this.prevImage();
    }
  }
}