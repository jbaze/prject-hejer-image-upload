import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ImageGalleryDialogComponent } from '../image-gallery-dialog/image-gallery-dialog.component';

@Component({
  selector: 'app-customers-table',
  imports: [CommonModule],
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.scss'
})
export class CustomersComponent {
  @Input() customers: any[] = [];

  constructor(
    private dialog: MatDialog
  ){}

  openGallery(entityId: number, name: string) {
    this.dialog.open(ImageGalleryDialogComponent, {
      width: '90vw',
      data: { entityId, entityType: 'customers', name: name }
    });
  } 
}
